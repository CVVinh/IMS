<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                :paginator="true"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :rowHover="true"
                v-model:filters="filters"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="['firstName', 'idGroup']"
                responsiveLayout="scroll"
            >
                <template #header>
                    <div class="flex justify-content-center align-items-center">
                        <h5 class="" style="color: #fff">Thành viên</h5>
                        <div class="header-content">
                            <span class="header-buttons">
                                <Add
                                    @click="openBasic"
                                    label="Thêm thành viên "
                                    style="margin-right: 5px"
                                    v-if="isPM()"
                                />
                                <Button
                                    label="Quay về"
                                    class="p-button-sm"
                                    icon="pi pi-arrow-left"
                                    @click="backToProject()"
                                />
                            </span>
                            <span class="search-box">
                                <div class="p-input-icon-left">
                                    <i class="pi pi-search" />
                                    <InputText
                                        class="p-inputtext-sm"
                                        v-model="filters['global'].value"
                                        placeholder="Tìm kiếm"
                                    />
                                </div>
                            </span>
                        </div>
                        <Dialog
                            header="Thêm thành viên vào dự án "
                            :visible="displayBasic"
                            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
                            :style="{ width: '30vw' }"
                            :modal="true"
                        >
                            <MultiSelect
                                v-model="selectedMember"
                                :options="data1"
                                optionLabel="firstName"
                                optionValue="id"
                                filter="true"
                                placeholder="Chọn thành viên"
                            />
                            <template #footer>
                                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
                                <Button label="Quay về" icon="pi pi-times" @click="closeBasic" class="p-button-text" />                             
                            </template>
                        </Dialog>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>
                <Column field="id" header="#" style="min-width: 5rem">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="fullName" header="Họ" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.firstName }}
                    </template>
                </Column>
                <Column field="IdGroup" header="Nhóm" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ data.idGroup }}
                    </template>
                </Column>
                <Column field="" header="" style="min-width: 18rem; text-align: center">
                    <template #body="{ data }">
                        <Delete @click="deleteMember(data.id, this.$route.params.id)" />
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
    import { FilterMatchMode } from 'primevue/api'
    import { UserRoleHelper } from '@/helper/user-role.helper'
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
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
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
                this.getUsersForSelect(this.$route.params.id)
            this.getData()
        },
        methods: {
            async isPM() {
                if (await UserRoleHelper.isPm()) return true
                else return false
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
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteUser(id)
                    },
                    reject: () => {
                        return
                    },
                })
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
                    alert('Xóa thành công')
                })
            },
            openBasic() {
                this.displayBasic = true
            },
            getData() {
                HTTP.get('Users/getAllUsersByIdProject/' + this.$route.params.id).then((res) => {
                    if (res.data._success) {
                        this.data = res.data._Data
                        this.data.forEach((e) => {
                            e.idGroup = this.getRole(e.idGroup)
                            e.firstName = this.mergeString(e.lastName, e.firstName)
                        })
                    } else {
                        this.$message({
                            title: res.data._message,
                            type: 'error',
                            message: res.data._message,
                        })
                    }
                })
            },
            getRole(index) {
                if (index == 1) return 'Admin'
                else if (index == 2) return 'Director'
                else if (index == 3) return 'PM'
                else if (index == 4) return 'Leader'
                else if (index == 5) return 'Accountant'
                else if (index == 6) return 'HR'
                else if (index == 7) return 'Users'
            },
            submit() {
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)
                this.selectedMember.forEach((id) => {
                    HTTP.post('memberProject/addMemberAtProject', {
                        idProject: this.$route.params.id,
                        member: id,
                        createUser: decode.Id,
                    }).then((res) => {
                        if (res.data) {
                            this.closeBasic()
                            this.getData()
                        }
                    })
                })
                this.showSuccess('Thêm thành công')
                this.closeBasic()
            },
            closeBasic() {
                this.displayBasic = false
            },
            async getUsersForSelect(idProject) {
                await HTTP.get('Users/getUsersOutsideProject/' + idProject).then((res) => {
                    if (res.data) {
                        this.data1 = res.data._Data
                        this.data1.forEach((e) => {
                            e.firstName = e.lastName + ' ' + e.firstName
                        })
                    }
                })
            },
            deleteMember(idMember, idProject) {
                HTTP.delete('memberProject/deleteMemberInProject/' + idMember + '/' + idProject).then((res) => {
                    if (res.data._success == true) {
                        this.showSuccess('Xóa thành công')
                        this.getData()
                    } else {
                        this.showError('Xóa lỗi ')
                    }
                })
            },
            
            showSuccess(mess) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: mess, life: 3000 })
            },
            showError(mess) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: mess, life: 3000 })
            },
            mergeString(str1, str2) {
                return str1 + ' ' + str2
            },
        },
        components: { Add, Delete },
    }
</script>
<style lang="scss" scoped>
    ::v-deep(.p-paginator) {
        .p-paginator-current {
            margin-left: auto;
        }
    }

    ::v-deep(.p-progressbar) {
        height: 0.5rem;
        background-color: #d8dadc;

        .p-progressbar-value {
            background-color: #607d8b;
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
            background-color: #607d8b;
        }

        .p-paginator {
            padding: 1rem;
        }

        .p-datatable-thead > tr > th {
            text-align: left;
        }

        .p-datatable-tbody > tr > td {
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
