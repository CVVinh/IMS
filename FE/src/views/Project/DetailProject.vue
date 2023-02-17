<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable :value="data" :paginator="true" class="p-datatable-customers" :rows="10" dataKey="id"
                :rowHover="true" v-model:filters="filters" filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="['firstName', 'idGroup']" responsiveLayout="scroll">
                <template #header>
                    <div class="flex justify-content-center align-items-center">
                        <h5 class="" style="color: #fff;">Member</h5>
                        <div class="header-content">
                            <span class="header-buttons">
                                <Add @click="openBasic" label="Add member" style="margin-right: 5px;" v-if="isPM()" />
                                <Button label="Back" class="p-button-sm" icon="pi pi-arrow-left"
                                    @click="backToProject()" />
                            </span>
                            <span class="search-box">
                                <div class="p-input-icon-left">
                                    <i class="pi pi-search" />
                                    <InputText class="p-inputtext-sm" v-model="filters['global'].value"
                                        placeholder="Keyword Search" />
                                </div>
                            </span>
                        </div>
                        <Dialog header="Assign members to project" :visible="displayBasic"
                            :breakpoints="{ '960px': '75vw', '640px': '90vw' }" :style="{ width: '30vw' }"
                            :modal="true">
                            <MultiSelect v-model="selectedMember" :options="data1" optionLabel="firstName"
                                optionValue="id" filter="true" placeholder="Unassign" />
                            <template #footer>
                                <Button label="No" icon="pi pi-times" @click="closeBasic" class="p-button-text" />
                                <Button label="Yes" icon="pi pi-check" @click="submit" autofocus />
                            </template>
                        </Dialog>
                    </div>
                </template>
                <template #empty>
                    No users found.
                </template>
                <Column field="id" header="#" style="min-width: 5rem">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="fullName" header="Full name" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.firstName }}
                    </template>
                </Column>
                <Column field="IdGroup" header="Group" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ data.idGroup }}
                    </template>
                </Column>
                <Column field="" header="" style="min-width: 18rem; text-align: center;">
                    <template #body="{ data }">
                        <Delete @click="confirmDelete(data.id, this.$route.params.id)" />
                    </template>
                </Column>
            </DataTable>
        </div>

    </LayoutDefaultDynamic>
</template>

<script>

import { HTTP } from '@/http-common'
import Add from '../../components/buttons/Add.vue'
import Delete from '../../components/buttons/Delete.vue'
import jwt_decode from 'jwt-decode'
import { FilterMatchMode } from 'primevue/api';
import { UserRoleHelper } from '@/helper/user-role.helper';
export default {
    data() {
        return {
            data: [],
            data1: [],
            displayBasic: false,
            userInfo: [],
            idArr: [],
            usercodeArr: [],
            search: [],
            selectedMember: [],
            filters: {
                'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            selectedCustomers: null,
        }
    },
    mounted() {

        HTTP.get('Users/getInfo').then((res) => {
            if (res.data) {
                this.userInfo = res.data
            }
        }),
            this.getUsersForSelect(this.$route.params.id);
        this.getData();
    },
    methods: {
        async isPM() {
            if (await UserRoleHelper.isPm()) return true;
            else return false;
        },
        backToProject() {
            this.$router.push({ name: 'project' })
        },
        toAddMember(id) {
            this.$router.push({ name: 'addmember', params: { id: id } })
        },
        toEditUserPage(id) {
            this.$router.push({ name: 'edituser', params: { id: id } })
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: 'Do you want to delete this user?',
                header: 'Delete Confirmation',
                icon: 'pi pi-info-circle',
                acceptClass: 'p-button-danger',
                accept: () => {
                    this.deleteUser(id)
                },
                reject: () => {
                    return
                }
            });
        },
        deleteUser(userId) {
            let API_URL = 'Users/deleteUser/' + userId
            HTTP.put(API_URL).then((res) => {
                window.location.reload()
            })
        },
        getUsercode(userId) {
            if (userId && userId !== 0) {
                for (var user of this.userInfo) {
                    if (user.id === userId) {
                        return user.userCode
                    }
                }
            }
        },
        deleteMember(memberId) {
            HTTP.put('memberProject/deleteMemberById/' + memberId).then((res) => {
                alert('Delete user successfully');
            })
        },
        openBasic() {
            this.displayBasic = true;
        },
        getData() {
            HTTP.get('Users/getAllUsersByIdProject/' + this.$route.params.id).then((res) => {
                if (res.data._success) {
                    this.data = res.data._Data;
                    this.data.forEach(e => {
                        e.idGroup = this.getRole(e.idGroup);
                        e.firstName = this.mergeString(e.lastName, e.firstName);
                    })
                } else {
                    this.$message({
                        title: res.data._message,
                        type: 'error',
                        message: res.data._message,
                    });
                }

            })
        },
        getRole(index) {
            if (index == 1) return 'Admin';
            else if (index == 2) return 'Director';
            else if (index == 3) return 'PM';
            else if (index == 4) return 'Leader';
            else if (index == 5) return 'Accountant';
            else if (index == 6) return 'HR';
            else if (index == 7) return 'Users';
        },
        submit() {
            var token = localStorage.getItem('token');
            var decode = jwt_decode(token);
            this.selectedMember.forEach(id => {
                HTTP.post('memberProject/addMemberAtProject', {
                    idProject: this.$route.params.id,
                    member: id,
                    createUser: decode.Id,
                }).then((res) => {
                    if (res.data) {
                        this.closeBasic();
                        this.getData();
                    }
                });
            });
            this.showSuccess('Add member successfully');
            this.closeBasic();
        },
        closeBasic() {
            this.displayBasic = false;
        },
        async getUsersForSelect(idProject) {
            await HTTP.get('Users/getUsersOutsideProject/' + idProject).then((res) => {
                if (res.data) {
                    this.data1 = res.data._Data;
                    this.data1.forEach(e => {
                        e.firstName = e.lastName + ' ' + e.firstName;
                    })
                }
            })
        },
        deleteMember(idMember, idProject) {
            HTTP.delete('memberProject/deleteMemberInProject/' + idMember + '/' + idProject).then((res) => {

                if (res.data._success == true) {
                    this.showSuccess('Delete member successfully');
                    this.getData();
                } else {
                    this.showError('Delete member failed');
                }
            })
        },
        confirmDelete(idMember, idProject) {
            this.$confirm.require({
                message: 'Do you want to delete this member?',
                header: 'Delete Confirmation',
                icon: 'pi pi-exclamation-triangle',
                acceptClass: 'p-button-danger',
                accept: () => {
                    this.deleteMember(idMember, idProject)
                },
                reject: () => {
                    return
                }
            });
        },
        showSuccess(mess) {
            this.$toast.add({ severity: 'success', summary: 'Success Message', detail: mess, life: 3000 });
        },
        showError(mess) {
            this.$toast.add({ severity: 'error', summary: 'Error Message', detail: mess, life: 3000 });
        },
        mergeString(str1, str2) {
            return str1 + ' ' + str2;
        }
    },
    components: { Add, Delete }
}

</script>
<style lang="scss" scoped>
::v-deep(.p-paginator) {
    .p-paginator-current {
        margin-left: auto;
    }
}

::v-deep(.p-progressbar) {
    height: .5rem;
    background-color: #D8DADC;

    .p-progressbar-value {
        background-color: #607D8B;
    }
}

::v-deep(.p-datepicker) {
    min-width: 25rem;

    td {
        font-weight: 400;
    }
}

::v-deep(.p-datatable.p-datatable-customers) {
    .p-datatable-header {
        padding: 1rem;
        text-align: left;
        font-size: 1.5rem;
        background-color: #607D8B;
    }

    .p-paginator {
        padding: 1rem;
    }

    .p-datatable-thead>tr>th {
        text-align: left;
    }

    .p-datatable-tbody>tr>td {
        cursor: auto;
    }

    .p-dropdown-label:not(.p-placeholder) {
        text-transform: uppercase;
    }
}

.header-content {
    display: flex;
    justify-content: space-between;
    height: 40px;
}

.search-box {
    height: 40px;
    margin: 0;
}

.div-back-button {
    width: 100%;
    display: flex;
    justify-content: flex-end;
}
</style>
