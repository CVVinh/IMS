<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable :value="data" :paginator="true" ref="dt" class="p-datatable-customers" sortField="dateCreate"
                :sortOrder="-1" :rows="10" dataKey="id" :rowHover="true" v-model:filters="filters"
                v-model:selection="selectedRemote" filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="['id', 'date', 'start', 'end', 'realTime', 'status', 'description', 'leadCreate', 'dateCreate', 'updateUser', 'dateUpdate', 'note', 'user', 'idProject']"
                responsiveLayout="scroll">
                <!-- Header -->
                <template #header>
                    <h5 class="" style="color: white;">REMOTES LIST</h5>
                    <div class="flex align-items-center">
                        <div>

                            <Export @click="exportCSV($event)" label='Export' />
                            <Button label="Add Remote" icon="pi pi-plus" @click="add()" v-bind:hidden='cantAdd' />
                            <Button label="Accept" icon="pi pi-check-square" @click="acceptMulti()" :hidden='!rolePM' />
                        </div>

                        <div class="filter-pro">
                            <h5 style="display:inline; margin-right: 10px;  color: antiquewhite;">Status:</h5>
                            <Dropdown class="p-column-filter" v-model="filters['status'].value" :options="statuses"
                                optionLabel="text" optionValue="num" placeholder="Select a status" />
                            <Button type="button" icon="pi pi-filter-slash" class="p-button-outlined"
                                @click="clearFilter()" style="background-color: antiquewhite;" />
                            <MultiSelect :modelValue="selectedColumns" :options="columns" optionLabel="header"
                                @update:modelValue="onToggle" placeholder="Select Columns" style="width: 20em; " />
                            <div class="p-input-icon-left" style="display:inline">
                                <i class="pi pi-search" />
                                <InputText v-model="filters['global'].value" placeholder="Keyword Search" />
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty>
                    No Remote found.
                </template>
                <Column header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="date" header="Remote Date" sortable dataType="date" style="min-width: 7.5rem">
                    <template #body="{ data }">
                        {{ formatDate(data.date) }}
                    </template>
                </Column>
                <Column field="user" header="Remoter" sortable style="min-width: 4rem">
                    <template #body="{ data }">
                        {{ username[data.user] }}
                    </template>
                </Column>
                <Column field="leadCreate" header="Lead" sortable style="min-width: 6rem">
                    <template #body="{ data }">
                        {{ username[data.leadCreate] }}
                    </template>
                </Column>
                <Column field="dateCreate" header="Create Date" sortable dataType="date" style="min-width: 9.5rem">
                    <template #body="{ data }">
                        {{ formatDate(data.dateCreate) }}
                    </template>
                </Column>
                <Column field="realTime" header="Remote Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.realTime }}
                    </template>
                </Column>
                <Column field="start" header="Start Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.start }}
                    </template>
                </Column>
                <Column field="end" header="End Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.end }}
                    </template>
                </Column>
                <Column field="description" header="Description" sortable :filterMenuStyle="{ 'width': '14rem' }"
                    style="min-width: 10rem; width: 10rem;">
                    <template #body="{ data }">
                        {{ data.description }}
                    </template>
                </Column>
                <Column field="updateUser" header="Approved by" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ username[data.updateUser] }}
                    </template>
                </Column>
                <!--
                <Column field="dateUpdate" header="Approve Date" sortable dataType="date" style="min-width: 11rem">
                    <template #body="{ data }">
                        {{ formatDate(data.dateUpdate) }}
                    </template>
                </Column> -->

                <!-- <Column field="note" header="Reason" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.note }}
                    </template>
                </Column> -->
                <Column sortable v-for="(col, index) of selectedColumns" :field="col.field" :header="col.header"
                    :key="col.field + '_' + index"></Column>
                <!-- <Column field="idProject" header="Project" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ proName[data.idProject] }}
                    </template>
                </Column> -->
                <Column field="" header="Action" style="min-width: 10rem;">
                    <template #body="{ data }">
                        <Edit v-if="canAccept && data.status == 0 && checkPM(data.idProject)" icon="pi pi-check"
                            @click="accept(true, data.id, data.leadCreate)" />
                        <Delete v-if="canAccept && data.status == 0 && checkPM(data.idProject)" icon="pi pi-times"
                            @click="accept(false, data.id, data.leadCreate)" />
                        <Edit @click="EditData(data.id)" :disabled='canEdit(data.status, token, data.leadCreate)'>
                        </Edit>
                        <Delete @click="confirmDelete(data.id, token)"
                            :disabled='canEdit(data.status, token, data.leadCreate)'></Delete>
                    </template>
                </Column>
                <Column field="status" header="Status" sortable :filterMenuStyle="{ 'width': '14rem' }"
                    style="min-width: 6rem;">
                    <template #body="{ data }">
                        <span :class="'badge ' + color(data.status)">{{ statuses[data.status].text }}</span>
                    </template>
                </Column>

            </DataTable>

        </div>
        <Dialog header="Access is denied!" :visible="displayDialog1"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }" :style="{ width: '30vw' }" :modal="true"
            :closable="false">
            <p>You do not have permission to access this page!</p>
            <medium>You will be redirected to the homepage in <strong>{{ num }}</strong> seconds!</medium>
            <template #footer>
                <Button label="Yes" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <Dialog header="Please enter reason!" :visible="displayDialog2"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }" :style="{ width: '30vw' }" :modal="true">
            <Textarea v-model="reason" style="margin: auto; width: 100%;height: 100%;"></Textarea>
            <medium v-if="entered" class="p-error">
                Reason is required!
            </medium>
            <template #footer>
                <Button label="No" icon="pi pi-times" @click="closeBasic" class="p-button-text" />
                <Button label="Yes" icon="pi pi-check" @click="onSubmit(data.leadCreate)" autofocus />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>

<script>

import Edit from '@/components/buttons/Edit.vue'
import Export from '@/components/buttons/Export.vue'
import router from '@/router/index'
import { HTTP } from '@/http-common.js'
import { FilterMatchMode, FilterOperator } from 'primevue/api';
import Delete from '@/components/buttons/Delete.vue'
import jwtDecode from 'jwt-decode'
import { UserRoleHelper } from '@/helper/user-role.helper'
export default {
    name: 'Remotes',
    data() {
        return {
            mail: {
                to: '',
                subject: '',
                body: ''
            },
            selectedOT: null,
            data: null,
            filters: {
                'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
                'status': { value: null, matchMode: FilterMatchMode.EQUALS }

            },
            selectedColumns: null,
            columns: null,
            project: [],
            proName: [],
            proPM: [],
            user: [],
            username: [],
            cantAdd: false,
            token: null,
            rolePM: true,
            displayDialog1: false,
            displayDialog2: false,
            num: 5,
            timeout: null,
            canAccept: true,
            entered: false,
            reason: '',
            id: '',
            lead: '',
            statuses: [
                { num: 0, text: 'Waiting' },
                { num: 1, text: 'Accepted' },
                { num: 2, text: 'Denied' },
                { num: 3, text: 'Deleted' }
            ]
        };
    },
    mounted() {
        if (!UserRoleHelper.isLeader())
            this.cantAdd = true;
        if (!UserRoleHelper.isPm()) {
            this.canAccept = false
            this.rolePM = false;
        }
        if (UserRoleHelper.isAdmin()) {
            this.cantAdd = false
            this.canAccept = true
            this.rolePM = true
        }
        if (!UserRoleHelper.isLeader() && !UserRoleHelper.isPm() && !UserRoleHelper.isAdmin()) {
            this.countTime()
            this.displayDialog1 = true
        }
        else this.getAllRemote();
        this.columns = [
            { field: 'dateUpdate', header: 'Approve Date' },
            { field: 'note', header: 'Note' },
            { field: 'idProject', header: 'Project', },
        ];
        HTTP.get("Project/getAllProject").then(res => {
            if (res.status == 200)
                res.data.forEach(element => {
                    if (element.isDeleted != true && element.isFinished != true) {
                        this.project.push(element);
                        this.proName[element.id] = element.name
                        this.proPM[element.id] = element.UserId
                    }
                });
        });
        HTTP.get("Users/getAll").then(res => {
            if (res.status == 200)
                res.data.forEach(element => {
                    if (element.isDeleted != true && element.isFinished != true) {
                        this.user.push(element);
                        this.username[element.id] = element.userCode
                    }
                });
        });
    },
    methods: {
        checkPM(id) {
            if (this.proPM[id] == this.token.Id || this.token.Id == 26)
                return true
            return false
        },
        acceptMulti() {
            let bool = true;
            if (this.selectedOT == null) {
                this.showWarn("Please select a Remote to Accept!")
                return
            }
            this.selectedRemote.forEach(element => {
                if (element.status != 0) {
                    bool = false
                }
            })
            if (bool) {
                this.selectedRemote.forEach(element => {
                    this.accept(true, element.id, element.leadCreate)
                })
            }
            else this.showWarn("Can not accept Remotes that don't have Waiting status!")
            this.selectedRemote = []
        },
        countTime() {
            if (this.num == 0) {
                this.submit()
                return
            }
            this.num = this.num - 1
            this.timeout = setTimeout(() => this.countTime(), 1000)
        },
        submit() {
            clearTimeout(this.timeout)
            router.push('/')
        },
        onToggle(value) {
            this.selectedColumns = this.columns.filter(col => value.includes(col));
        },
        formatDate(value) {
            return new Date(value).toLocaleDateString('en-CA', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
            });
        },
        getAllRemote() {
            HTTP.get("OTs/getByType/1")
                .then((res) => {
                    if (res.data) {
                        this.data = res.data;
                    }
                });
        },
        async canEdit (status, token, lead) {
            if (status == 1 || status == 3)
                return true
            if (lead != token.Id || !await UserRoleHelper.isLeader())
                return true
            return false
        },
        deleteData: (id, token) => {
            HTTP.put('OTs/deleteOT?' + 'idOT=' + id + '&PM=' + token)
                .catch(err => { alert(err) });
        },
        EditData: (id) => {
            router.push({ name: 'editRemote', params: { id: id } });
        },
        color(status) {
            if (status == 0)
                return 'bg-primary'
            if (status == 1)
                return 'bg-success'
            if (status == 2)
                return 'bg-secondary'
            return 'bg-danger'
        },
        add() {
            router.push('/remotes/addRemote')
        },

        confirmDelete(id, token) {
            this.$confirm.require({
                message: 'Do you want to delete this Remote?',
                header: 'Delete Confirmation',
                icon: 'pi pi-question-circle',
                acceptClass: 'p-button-danger',
                accept: () => {
                    this.deleteData(id, token.Id)
                    setTimeout(() => { this.getAllRemote() }, 500)
                    this.showSuccess()
                },
                reject: () => {
                    return
                }
            });
        },
        showSuccess() {
            this.$toast.add({ severity: 'success', summary: 'Success Message', detail: 'Successful!', life: 3000 });
        },
        exportCSV() {
            this.$refs.dt.exportCSV();
        },
        accept(accepted, id, lead) {
            let user = jwtDecode(localStorage.getItem('token'));
            this.PM = user.Id
            if (accepted) {
                this.status = 1;
                HTTP.put('OTs/acceptOT', { id: id, status: this.status, PM: this.PM })
                HTTP.get('Users/getUserById/' + lead).then(res => {
                    if (res.status == 200) {
                        this.mail.to = res.data.email
                        this.mail.subject = 'Accepted!'
                        this.mail.body = 'Your proposed was accepted!'
                        HTTP.post('Users/SendEmail', this.mail)
                        this.showSuccess()
                        setTimeout(() => { this.getAllRemote() }, 500)
                    }
                })

            }
            else {
                this.displayDialog2 = true
                this.id = id
                this.lead = lead
            }
        },
        closeBasic() {
            this.displayDialog2 = false
            this.entered = false
        },
        onSubmit() {
            if (this.reason != null && this.reason != '') {
                HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(res => {
                    if (res.status == 200) {
                        HTTP.get('Users/getUserById/' + this.lead).then(res => {
                            if (res.status == 200) {
                                this.mail.to = res.data.email
                                this.mail.subject = 'Accepted!'
                                this.mail.body = 'Your proposed was accepted!'
                                HTTP.post('Users/SendEmail', this.mail)
                                this.closeBasic()
                                this.showSuccess()
                                setTimeout(() => { this.getAllRemote() }, 500)

                            }
                        })
                    }

                })
            }
            else this.entered = true
        },
        clearFilter() {
            this.filters = {
                'global': { value: null, matchMode: FilterMatchMode.CONTAINS },
                'status': { value: null, matchMode: FilterMatchMode.EQUALS }
            }
        },
        showWarn(message) {
            this.$toast.add({ severity: "warn", summary: "Warn Message", detail: message, life: 5000 });
        },
    },
    components: { Edit, Delete, Export }
}

</script>

<style lang="scss" scoped>
::v-deep(.p-paginator) {
    .p-paginator-current {
        margin-left: auto;
    }
}

::v-deep(.p-input-icon-left) {
    i:first-of-type {
        left: 1rem;
        color: #6c757d;
    }
}

::v-deep(.p-inputtext) {
    // padding: 0.64rem 0.75rem 0.57rem 2rem;
    margin: 0 10px 0 10px;
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

    .p-dropdown-label.p-placeholder {
        padding-top: 6px;
    }

    .p-dropdown-label:not(.p-placeholder) {
        text-transform: uppercase;
        padding-top: 6px;
    }

    .p-multiselect {
        height: 100%;

        .p-multiselect-label {
            height: 100%;
            padding: 0.41rem 0.41rem;
        }
    }

    .p-datatable-header {
        background-color: #607D8B;
    }
}

::v-deep(.p-dropdown) {
    height: 100%;
    position: static;
}

.p-button-outlined {
    height: 100%;
}

.flex {
    display: flex;
    justify-content: space-between;
}

.filter-pro {
    display: flex;
    justify-content: center;
    height: 40px;
    float: right;
    align-items: center;
}

.p-input-icon-left {
    height: 100%;

    .p-inputtext {
        height: 100%;
        padding-top: 15px;

    }
}

button {
    margin: 0 3px 0 3px;
    padding: 0.57rem 0.41rem;
}
</style>
