<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable :value="modules" :paginator="true" class="p-datatable-customers" :rows="10" dataKey="id"
                :loading="loading" :rowHover="true" filterDisplay="menu" v-model:filters="filters"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[5, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="['#', 'nameModule', 'note']" responsiveLayout="scroll" ref="dt">
                <template #header>
                    <div>
                        <h5 class="header-tilte">Module List</h5>
                        <div class="d-flex justify-content-between">
                            <div class="header-left">
                                <Export label="Export" style="margin-right: 10px" @click="exportCSV($event)" />
                                <AddModulesVue @render="getAllModules" />
                            </div>
                            <div class="d-flex">
                                <div class="p-input-icon-left layout-left">
                                    <i class="pi pi-search" />
                                    <InputText class="p-inputtext-sm" v-model="filters['global'].value"
                                        placeholder="Keyword Search" />
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> No module found. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>
                <Column field="" header="#"><template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="nameModule" header="Module Name" sortable>
                    <template #body="{ data }">
                        {{ data.nameModule }}
                    </template>
                </Column>
                <Column field="note" header="Note">
                    <template #body="{ data }">
                        {{ data.note }}
                    </template>
                </Column>
                <Column header="Action" style="min-width: 10rem">
                    <template #body="{ data }">
                        <!-- <Edit :disabled="data.isDeleted" /> -->
                        <UpdateModuleVue :checkEdit="checkEdit(data.isDeleted)" :module="data"
                            @render="getAllModules" />
                        &nbsp;
                        <Delete @click="handleDelete(data.id)" :disabled="checkEdit(data.isDeleted)" />
                    </template>
                </Column>
            </DataTable>
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
import Edit from '../../components/buttons/Edit.vue'
import Delete from '../../components/buttons/Delete.vue'
import Export from '../../components/buttons/Export.vue'
import { FilterMatchMode } from 'primevue/api'
import { HTTP } from '@/http-common'
import jwtDecode from 'jwt-decode'
import AddModulesVue from './AddModule.vue'
import UpdateModuleVue from './UpdateModule.vue'
export default {
    data() {
        return {
            loading: true,
            modules: [
                {
                    id: 1,
                    nameModule: 'Mot',
                    note: 'Ghi chu',
                },
            ],
            filters: {
                global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
        }
    },
    mounted() {
        this.getAllModules()
    },
    methods: {
        getAllModules() {
            HTTP.get('Modules/getListModule')
                .then((res) => res.data)
                .then((res) => {
                    this.modules = res
                    this.loading = false
                })
                .catch((err) => { console.log(err) })
        },
        handleEdit(id) { },
        handleDelete(id) {
            this.$confirm.require({
                message: 'Do you want to delete this handover?',
                header: 'Delete Confirmation',
                icon: 'pi pi-info-circle',
                acceptClass: 'p-button-danger',
                accept: async () => {
                    HTTP.put(`Modules/deleteModule/${id}`)
                        .then((res) => {
                            this.getAllModules()
                            this.$toast.add({
                                severity: 'info',
                                summary: 'Confirmed',
                                detail: 'Module deleted',
                                life: 3000,
                            })
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Error Message',
                                detail: 'Server Error',
                                life: 3000,
                            })
                        })
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
        exportCSV() {
            this.$refs.dt.exportCSV()
        },
        checkEdit(isDeleted) {
            return isDeleted === 1
        },
    },
    components: {
        Edit,
        Delete,
        Export,
        AddModulesVue,
        UpdateModuleVue,
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
