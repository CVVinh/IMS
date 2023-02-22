<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="arr"
                ref="devices"
                :paginator="true"
                class="p-datatable-customers"
                :rows="5"
                dataKey="idDevice"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedDevice"
                sortField="dateCreated"
                :sortOrder="-1"
                :loading="loading"
                responsiveLayout="scroll"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="pageIndex"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    '#',
                    'idDevice',
                    'deviceName',
                    'info',
                    'isDelete',
                    'userUpdated',
                    'dateUpdated',
                    'userCreated',
                    'dateCreated',
                    'note',
                ]"
            >
                <template #header>
                    <div class="flex justify-content-center">
                        <h5 class="" style="color: white">Danh sách thiết bị</h5>
                        <div class="inline">
                            <Export label="Export" @click="exportCSV()" style="margin-right: 10px" />
                            <Add label="Add" @click="openDialogAdd()" />
                            <div class="p-input-icon-left layout-left">
                                <i class="pi pi-search" />
                                <InputText
                                    class="p-inputtext-sm"
                                    v-model="filters['global'].value"
                                    placeholder="Tìm kiếm"
                                />
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>
                <Column field="" header="No.">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="deviceName" header="Tên thiết bị" sortable>
                    <template #body="{ data }">
                        {{ data.deviceName }}
                    </template>
                </Column>
                <Column field="info" header="Thông tin" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.info }}
                    </template>
                </Column>
                <Column field="note" header="Ghi chú" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.note }}
                    </template>
                </Column>
                <Column header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 15rem">
                    <template #body="{ data }">
                        <Edit
                            class="mazin"
                            @click="openDialogEdit(data)"
                            :disabled="enableDeleteEdit(data.isDeleted)"
                        />
                        <Delete
                            class="maz"
                            @click="confirmDelete(data.idDevice)"
                            :disabled="enableDeleteEdit(data.isDeleted)"
                        />
                    </template>
                </Column>
            </DataTable>
        </div>
        <Dialog
            header="Không có quyền truy cập !"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!</medium
            >
            <template #footer>
                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <AddDevices
            :isOpenDialog="isOpenDialog"
            :deviceSelected="selectedEdit"
            @closeDialog="closeDialog"
            @getAllDevices="getAllDevices"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import router from '@/router'
    import { FilterMatchMode } from 'primevue/api'
    import Add from '@/components/buttons/Add.vue'
    import Edit from '@/components/buttons/Edit.vue'
    import Delete from '@/components/buttons/Delete.vue'
    import Export from '@/components/buttons/Export.vue'
    // import { DevicesDto } from '@/views/Devices/Devices.Dto'
    import AddDevices from '@/views/Devices/AddDevices.vue'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'

    export default {
        data() {
            return {
                data: new DevicesDto(),
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                userInfo: [],
                columns: null,
                arr: [],
                loading: true,
                displayBasic: false,
                pageIndex: [5, 10, 15, 20],
                num: 5,
                timeout: null,
                selectedEdit: new DevicesDto(),
                isOpenDialog: false,
                selectedDevice: [],
            }
        },
        mounted() {
            this.token = LocalStorage.jwtDecodeToken()
            if (UserRoleHelper.isAdmin() || UserRoleHelper.isPm()) {
                this.getAllDevices()
                this.getUserInfo()
            } else {
                this.countTime()
                this.displayBasic = true
            }
        },
        methods: {
            openDialogEdit(data) {
                this.selectedEdit = data
                this.isOpenDialog = true
            },
            openDialogAdd() {
                this.isOpenDialog = true
            },
            closeDialog() {
                this.isOpenDialog = false
                this.selectedEdit = []
            },
            getUserInfo() {
                HTTP.get('Users/getInfo').then((res) => {
                    if (res.data) {
                        this.userInfo = res.data
                    }
                })
            },
            getAllDevices() {
                this.loading = false
                this.arr = []
                HTTP.get('Devices/getListDevices')
                    .then((res) => {
                        if (res.data) {
                            res.data.forEach((item) => {
                                this.data.idDevice = item.idDevice
                                this.data.deviceName = item.deviceName
                                this.data.info = item.info
                                this.data.isDelete = item.isDelete
                                this.data.dateUpdated = item.dateUpdated
                                this.data.userUpdated = item.userUpdated
                                this.data.dateCreated = item.dateCreated
                                this.data.userCreated = item.userCreated
                                this.data.note = item.note
                                this.arr.push(item)
                            })
                        } else {
                            this.arr = []
                        }
                        this.loading = false
                    })
                    .catch((err) => {
                        this.arr = []
                        this.loading = false
                    })
            },
            deleteDevices: (id) => {
                let userLogin = LocalStorage.jwtDecodeToken()
                let idUser = userLogin.Id
                HTTP.put('Devices/deleteDevices?' + 'id=' + id + '&user=' + idUser)
            },
            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        this.deleteDevices(id)
                        this.arr = []
                        setTimeout(() => {
                            this.getAllDevices()
                        }, 500)
                        this.showSuccess()
                    },
                    reject: () => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Bạn không có quyền',
                            life: 3000,
                        })
                    },
                })
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Thêm mới thành công!',
                    life: 2000,
                })
            },
            showWarn(message) {
                this.$toast.add({ severity: 'warn', summary: 'Cảnh báo', detail: message, life: 2000 })
            },
            exportCSV() {
                this.$refs.devices.exportCSV()
            },
            countTime() {
                if (this.num === 0) {
                    return this.submit()
                }
                this.num = this.num - 1
                this.timeout = setTimeout(() => this.countTime(), 1000)
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
            enableDeleteEdit(isDeleted) {
                return isDeleted === 1
            },
        },
        components: { Add, Edit, Delete, Export, AddDevices },
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

        .p-input-icon-left {
            float: right;
            margin-left: 1rem;
            display: inline;
        }

        .p-inputtext-sm {
            font-size: 0.96rem;
        }

        .layout-left {
            float: right;
            display: inline;
        }

        .p-multiselect .p-multiselect-label {
            padding: 0.41rem 0.41rem;
        }

        .p-datatable-header {
            background-color: #607d8b;
        }

        .mazin {
            margin-left: 5px;
            margin-right: 5px;
        }

        .maz {
            margin-right: 5px;
        }
    }
</style>
