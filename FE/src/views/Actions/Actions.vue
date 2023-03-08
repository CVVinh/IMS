<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="this.actions"
                :paginator="true"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :rowHover="true"
                filterDisplay="menu"
                v-model:filters="filters"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                responsiveLayout="scroll"
                ref="dt"
                showGridlines 
                :globalFilterFields="['id', 'name', 'description']"
            >
                <template #header>
                    <div>
                        <h5 class="header-tilte">Thao tác</h5>
                        <div class="d-flex justify-content-between">
                            <div class="header-left">
                                <Export label="Xuất Excel" style="margin-right: 10px" @click="exportToExcel()" />
          
                                <addActions @render="getAllActions" />


                            </div>
                           
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>

                <Column header="STT">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
            
                <Column field="name" header="Tên nhóm" sortable>
                    <template #bodFEy="{ data }">
                        {{ data.name }}
                    </template>
                </Column>
                <Column field="description" header="Mô tả" sortable>
                    <template #bodFEy="{ data }">
                        {{ data.description }}
                    </template>
                </Column>

                <Column header="Thực thi" style="min-width: 10rem">
                    <template #body="{ data }">
                        <!-- <Edit :disabled="data.isDeleted" /> -->

                        <editActions       
                            :checkEdit="checkEdit(data.isDeleted)"
                            :action="data"
                            @render="getAllActions"
                        />                     
                        <Delete @click="confirmDelete(data.id)" :disabled="data.isDeleted" style="margin-left: 10px;"/>
                    </template>
                </Column>
            </DataTable>
        </div>
       
    </LayoutDefaultDynamic>
</template>

<script>
import Delete from '@/components/buttons/Delete.vue';
import Edit from '@/components/buttons/Edit.vue';
import Export from '@/components/buttons/Export.vue';
import { LocalStorage } from '@/helper/local-storage.helper'
import editActions from './editActions.vue';
import addActions from './addActions.vue';
import { HTTP } from '@/http-common';


    export default {
    data() {
        return {
            actions: [],
            filters: {
            //  global: { value: null, matchMode: FilterMatchMode.CONTAINS },
            },
            data: "",
            user: "",
            isOpenAdd: false,
            isOpenEdit: false,
            group: "",
            isChange: false,
            token: null,
        };
    },
    computed: {},
    async mounted() {
        this.token = LocalStorage.jwtDecodeToken();
        this.getAllActions();
    },
    methods: {
        setChange() {
            this.isChange = true;
        },
        getAllActions() {
            this.groups = [];
            HTTP.get("ActionModule/getAllActionModule").then((res) => {
                if (res.status == 200) {
                    this.actions =res.data._Data;
                }
            });
        },
        deleteGroup(id) {
            if (id) {
                const UserDelete= {
                    userDeleted : this.token.Id
                }
                HTTP.put(`ActionModule/deleteActionModule/${id}`,UserDelete)
                    .then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: "success",
                            summary: "Thành công",
                            detail: "Xóa thành công!",
                            life: 3000,
                        });
                        this.getAllActions();
                    }
                })
                    .catch((err) => {
                    if (err.data) {
                        this.$toast.add({
                            severity: "error",
                            summary: "Lỗi",
                            details: "Lỗi khi xóa!",
                            life: 3000,
                        });
                    }
                });
            }
        },
        exportToExcel() {
            HTTP.get(`Group/exportExcel/`)
                .then((res) => {
                if (res.status == 200) {
                    this.$toast.add({
                        severity: "success",
                        summary: "Thành công",
                        detail: "Xuất file excel thành công!",
                        life: 3000,
                    });
                    window.location = res.data;
                }
            })
                .catch((err) => {
                if (err.data) {
                    this.$toast.add({
                        severity: "error",
                        summary: "Lỗi",
                        details: "Lỗi khi xuất excel!",
                        life: 3000,
                    });
                }
            });
        },
        showAddGroup() {
            this.isOpenAdd = true;
        },
        closeDialogAdd() {
            this.isOpenAdd = false;
            if (this.isChange === true) {
                this.getAllGroup();
                this.isChange = false;
            }
        },
        showEditGroup(group) {
            this.group = group;
            this.isOpenEdit = true;
        },
        closeDialogEdit() {
            this.isOpenEdit = false;
            if (this.isChange === true) {
                this.getAllGroup();
                this.isChange = false;
            }
        },
        confirmDelete(id) {
            this.$confirm.require({
                message: "Bạn có chắc chắn muốn xóa?",
                header: "Xóa",
                icon: "pi pi-exclamation-triangle",
                acceptClass: "p-button-danger",
                accept: async () => {
                    this.deleteGroup(id);
                },
                reject: () => {
                    this.$toast.add({
                        severity: "error",
                        summary: "Từ chối",
                        detail: "Bạn đã bị từ chối",
                        life: 3000,
                    });
                },
            });
        },
        checkEdit(isDeleted) {
                return isDeleted === 1
        },
    },
    components: { Delete, Edit, Export, editActions,addActions }
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
