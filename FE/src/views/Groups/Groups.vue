<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="this.groups"
                :paginator="true"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :rowHover="true"
                filterDisplay="menu"
                v-model:filters="filters"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                responsiveLayout="scroll"
                ref="dt"
                :globalFilterFields="['idGroup', 'nameGroup', 'desc']"
            >
                <template #header>
                    <div>
                        <h5 class="header-tilte">Groups</h5>
                        <div class="d-flex justify-content-between">
                            <div class="header-left">
                                <Export label="Export" style="margin-right: 10px" @click="exportToExcel()" />
                                <Add label="Add" style="margin-right: 10px" v-on:click="showAddGroup()" />
                            </div>
                            <div class="d-flex">
                                <Button
                                    style="margin-right: 12px"
                                    class="p-button-sm"
                                    icon="bx bx-refresh bx-sm"
                                    :label="'Refresh'"
                                    @click="this.getAllGroup()"
                                >
                                    <ProgressSpinner v-if="!this.groups" style="width: 24px; height: 24px" />
                                </Button>
                                <span class="p-input-icon-left">
                                    <i class="pi pi-search" />
                                    <InputText placeholder="Search" v-model="filters['global'].value" />
                                </span>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> No groups found. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>

                <Column field="id" header="Id Group" sortable>
                    <template #body="{ data }">
                        {{ data.id }}
                    </template>
                </Column>
                <Column field="nameGroup" header="Name Group" sortable>
                    <template #bodFEy="{ data }">
                        {{ data.nameGroup }}
                    </template>
                </Column>
                <Column field="discription" header="Desc" sortable>
                    <template #body="{ data }">
                        {{ data.discription }}
                    </template>
                </Column>

                <Column header="Action" style="min-width: 10rem">
                    <template #body="{ data }">
                        <!-- <Edit :disabled="data.isDeleted" /> -->
                        <Edit @click="showEditGroup(data)" :disabled="data.isDeleted" />
                        &nbsp;
                        <Delete @click="confirmDelete(data.id)" :disabled="data.isDeleted" />
                    </template>
                </Column>
            </DataTable>
        </div>
        <EditGroupVue
            :isOpen="this.isOpenEdit"
            @closeDialog="closeDialogEdit()"
            :group="this.group"
            @setChange="setChange()"
        />
        <AddGroupVue :isOpen="this.isOpenAdd" @closeDialog="closeDialogAdd()" @setChange="setChange()" />
    </LayoutDefaultDynamic>
</template>

<script>
    import Add from '../../components/buttons/Add.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Member from '../../components/buttons/Member.vue'
    import Export from '../../components/buttons/Export.vue'
    import { HTTP } from '@/http-common'
    import jwtDecode from 'jwt-decode'
    import { FilterMatchMode } from 'primevue/api'
    import AddGroupVue from './AddGroup.vue'
    import EditGroupVue from './EditGroup.vue'
    export default {
        data() {
            return {
                groups: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                data: '',
                user: '',
                isOpenAdd: false,
                isOpenEdit: false,
                group: '',
                isChange: false,
            }
        },
        computed: {},
        async mounted() {
            this.user = jwtDecode(localStorage.getItem('token'))
            this.getAllGroup()
        },
        methods: {
            setChange() {
                this.isChange = true
            },
            getAllGroup() {
                this.groups = []
                HTTP.get('Group/getListGroup').then((res) => {
                    if (res.status == 200) {
                        this.groups = res.data
                    }
                })
            },
            deleteGroup(id) {
                if (id) {
                    HTTP.put(`Group/deleteGroup?id=${id}&user=${this.user.Id}`)
                        .then((res) => {
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Successfully',
                                    detail: 'Delete group successfully!',
                                    life: 3000,
                                })
                                this.getAllGroup()
                            }
                        })
                        .catch((err) => {
                            if (err.data) {
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Error',
                                    details: 'Delete group fails!',
                                    life: 3000,
                                })
                            }
                        })
                }
            },
            exportToExcel() {
                HTTP.get(`Group/exportExcel/`)
                    .then((res) => {
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Successfully',
                                detail: 'Export to excel successfully!',
                                life: 3000,
                            })
                            window.location = res.data
                        }
                    })
                    .catch((err) => {
                        if (err.data) {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Error',
                                details: 'Export to excel fails!',
                                life: 3000,
                            })
                        }
                    })
            },
            showAddGroup() {
                this.isOpenAdd = true
            },
            closeDialogAdd() {
                this.isOpenAdd = false;
                if (this.isChange === true) {
                    this.getAllGroup()
                    this.isChange = false
                }
            },
            showEditGroup(group) {
                this.group = group
                this.isOpenEdit = true
            },
            closeDialogEdit() {
                this.isOpenEdit = false
                if (this.isChange === true) {
                    this.getAllGroup()
                    this.isChange = false
                }
            },
            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Do you want to delete this group?',
                    header: 'Delete Confirmation',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        this.deleteGroup(id)
                    },
                    reject: () => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Rejected',
                            detail: 'You have rejected',
                            life: 3000,
                        })
                    },
                })
            },
        },
        components: {
            Add,
            Edit,
            Delete,
            Member,
            Export,
            AddGroupVue,
            EditGroupVue,
        },
    }
</script>

<style lang="scss" scoped>
    .header-left {
        height: 100%;
        display: flex;
        flex-direction: row;
        justify-content: end;
        align-items: flex-end;
        margin-right: 10px;
    }

    .button-gray {
        color: #6c757d;
        background-color: #fff;
        border-color: #ced4da;
    }

    .margin-right {
        margin-right: 14px;
    }

    .header-tilte {
        color: #fff;
    }

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

    ::v-deep(.p-datatable.p-datatable-customers) {
        .p-datatable-header {
            background-color: #607d8b;
        }
    }

    ::v-deep(.p-dropdown .p-dropdown-label.p-placeholder) {
        padding: 0.582rem 0.75rem;
    }

    ::v-deep(.p-dropdown .p-dropdown-label) {
        padding: 0.582rem 0.75rem;
    }
</style>
