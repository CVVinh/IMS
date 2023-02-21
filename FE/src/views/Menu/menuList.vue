<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="app-table">
            <DataTable
                :value="dataTable"
                class="Menu-table"
                :loading="loading"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    'nameModule',
                    'menu1.title',
                    'menu1.view',
                    'menu1.controller',
                    'menu1.icon',
                    'menu1.parent',
                ]"
                responsiveLayout="scroll"
                :rowHover="true"
                :rows="10"
                :paginator="true"
                v-model:filters="filters"
            >
                <template #header>
                    <div class="Menu-table-header">
                        <h4 class="Menu-table-header-title">Danh sách Menu</h4>
                        <div class="Menu-table-header-Groupinput">
                            <div class="Menu-table-header-Groupinput-items">
                                <Export label="Xuất Excel" />
                                <Add label="Thêm" class="itemsbutton" @click="Openmodal" />
                            </div>
                            <div class="Menu-table-header-Groupinput-items">
                                <Dropdown
                                    class="itemsinput dropdowninput"
                                    v-model="selectModule"
                                    :options="OptionModule"
                                    optionLabel="nameModule"
                                    optionValue="id"
                                    placeholder="Chọn chức năng"
                                />
                                <span class="p-input-icon-left">
                                    <i class="pi pi-search" />
                                    <InputText
                                        class="p-inputtext-sm"
                                        v-model="filters['global'].value"
                                        placeholder="Tìm kiếm"
                                    />
                                </span>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>

                <Column field="nameModule" header="Tên chức năng" sortable></Column>
                <Column field="menu1.title" header="Tên" sortable></Column>
                <Column field="menu1.view" header="Trang hiển thị" sortable></Column>
                <Column field="menu1.controller" header="Trang điều hướng" sortable></Column>
                <Column field="menu1.icon" header="Icon" sortable>
                    <!--                 
                <template #body="{data}">
                    <div class="table__icon">        
                        <i :class="data.menu1.icon"></i>
                    </div>

                </template> -->
                </Column>
                <Column field="menu1.parent" header="Nhánh/lớp cha" sortable></Column>
                <Column header="&emsp;&emsp;action" style="min-width: 9rem">
                    <template #body="{ data }">
                        <Edit @click="Openeditmodal(data.menu1.id)" :disabled="checkAction(data.menu1.isDeleted)" />

                        <Delete
                            class="itemsbutton"
                            :disabled="checkAction(data.menu1.isDeleted)"
                            @click="Delete(data.menu1.id)"
                        />
                    </template>
                </Column>
                <Column field="menu1.isDeleted" header="status" sortable>
                    <template #body="{ data }">
                        {{ data.menu1.isDeleted === 0 ? 'Đang dùng' : 'Đã xóa' }}
                    </template>
                </Column>
            </DataTable>
        </div>

        <AddmenuList
            :status="openStatus"
            @closemodal="Closemodal"
            @failed="showError1"
            @success="showSuccess1"
            :optionmodule="OptionModule"
            @reloadpage="GetAllMenuList"
        />
        <EditmenuList
            :status="openStatusEdit"
            @closemodal="closeeditmodal"
            @failed="showError2"
            @success="showSuccess2"
            :dataedit="editdata"
            :optionmodule="OptionModule"
            @reloadpage="GetAllMenuList"
        />

        <Toast />
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import Export from '../../components/buttons/Export.vue'
    import Add from '../../components/buttons/Add.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import AddmenuList from './addmenuList.vue'
    import EditmenuList from './editmenuList.vue'
    import { FilterMatchMode } from 'primevue/api'
    export default {
        data() {
            return {
                ListMenu: [],
                Listrendertable: [],
                search: '',
                OptionsModule: [],
                selectModule: '',
                openStatus: false,
                openStatusEdit: false,
                loading: true,
                OptionChooseParent: [],
                editdata: null,
                OptionModule: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
            }
        },
        methods: {
            Openmodal() {
                this.openStatus = true
            },

            Closemodal() {
                this.openStatus = false
            },

            Openeditmodal(id) {
                this.openStatusEdit = true
                this.editdata = id
            },

            closeeditmodal() {
                this.openStatusEdit = false
            },

            GetAllMenuList() {
                HTTP.get('Menu/getListMenu').then((res) => {
                    this.ListMenu = res.data
                    this.loading = false
                })
            },
            Getparentselect() {
                HTTP.get('Menu/getListMenuParent').then((res) => {
                    this.OptionsModule = res.data
                })
            },

            GetMenuModule() {
                HTTP.get('Menu/getListMenuModule').then((res) => {
                    this.OptionModule = res.data
                })
            },
            checkAction(id) {
                if (id === 0) {
                    return false
                } else {
                    return true
                }
            },
            DeleteMenu(id) {
                HTTP.put(`Menu/deleteMenu/${id}`)
                    .then((res) => {
                        if (res.data === 'success') {
                            this.showSuccess()
                        }
                        this.GetAllMenuList()
                    })
                    .catch((err) => {
                        this.showError()
                    })
            },

            Delete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.DeleteMenu(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Thêm mới thành công!',
                    life: 3000,
                })
            },
            showError() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Lỗi', life: 3000 })
            },
            showSuccess1() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công ',
                    detail: 'Thêm mới thành công',
                    life: 3000,
                })
            },
            showError1() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Thêm Lỗi', life: 3000 })
            },
            showSuccess2() {
                this.$toast.add({ severity: 'success', summary: 'Thành công ', detail: 'Sửa thành công!', life: 3000 })
            },
            showError2() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi ', detail: 'Sửa Lỗi', life: 3000 })
            },
        },

        mounted() {
            this.GetMenuModule()
            this.GetAllMenuList()
            this.Getparentselect()
        },

        computed: {
            dataTable() {
                let array = this.ListMenu

                if (this.selectModule) {
                    array = array.filter((ele) => {
                        return ele.menu1.idModule === this.selectModule
                    })
                }

                return array
            },
        },
        components: { LayoutDefaultDynamic, Export, Add, Delete, Edit, AddmenuList, EditmenuList },
    }
</script>

<style scoped>
    .app-table {
        width: 100%;

        display: flex;
        align-content: center;
        justify-content: center;
    }
    .Menu-table {
        width: 98%;

        margin-top: 15px;
    }

    ::v-deep(.p-datatable .p-datatable-header) {
        background-color: #607d8b;
        padding: 15px;
    }

    .Menu-table-header {
        width: 100%;
        height: 100%;
        color: white;
        display: flex;
        flex-direction: column;
    }

    .Menu-table-header-Groupinput {
        height: 50px;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .itemsbutton {
        margin-left: 10px;
    }
    .itemsinput {
        margin-right: 10px;
        height: 50px;
    }
    .dropdowninput {
        min-width: 184px;
    }
    ::v-deep(.p-input-icon-left > .p-inputtext) {
        height: 50px;
    }

    .table__icon {
        width: 40px;
        font-size: 30px;
        color: rgb(83, 83, 83);
        display: flex;
        align-content: center;
        justify-content: center;
    }
</style>
