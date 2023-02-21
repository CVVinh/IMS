<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable
                :value="ListDeviceOther"
                ref="dt"
                :paginator="true"
                class="p-datatable-customers"
                :rows="5"
                dataKey="id"
                :rowHover="true"
                v-model:filters="filters"
                sortField="dateCreated"
                :sortOrder="-1"
                :loading="loading"
                responsiveLayout="scroll"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="pageIndex"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    '#',
                    'branchName',
                    'deviceName',
                    'lastName',
                    'firstName',
                    'orders1.idOrder',
                    'orders1.idBranch',
                    'orders1.idDevice',
                    'orders1.amount',
                    'orders1.userUpdated',
                    'orders1.dateUpdated',
                    'orders1.userCreated',
                    'orders1.dateCreated',
                    'orders1.isDelete',
                    'orders1.note',
                    'orders1.statusOrder',
                    'orders1.statusDevice',
                ]"
            >
                <template #header>
                    <div class="flex justify-content-center">
                        <h5 class="" style="color: white">Danh sách đặt thiết bị</h5>
                        <div class="inline">
                            <Export label="Xuất Excel" @click="exportCSV($event)" style="margin-right: 10px" />
                            <Add @click="openDialog()" label="Thêm mới" />
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
                <Column field="" header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="orders1.idOrder" header="Đơn đặt" sortable>
                    <template #body="{ data }">
                        {{ data.orders1.idOrder }}
                    </template>
                </Column>
                <Column field="branchName" header="Chi nhánh" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.branchName }}
                    </template>
                </Column>
                <Column field="deviceName" header="Tên thiết bị" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.deviceName }}
                    </template>
                </Column>
                <Column field="orders1.amount" header="Số lượng" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.orders1.amount }}
                    </template>
                </Column>
                <Column field="orders1.statusOrder" header="Tình trạng đơn đặt" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.orders1.statusOrder }}
                    </template>
                </Column>
                <Column field="orders1.statusDevice" header="Tình trạng thiết bị" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.orders1.statusDevice }}
                    </template>
                </Column>
                <Column field="orders1.note" header="Ghi chú" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.orders1.note }}
                    </template>
                </Column>
                <Column header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 15rem">
                    <template #body="{ data }">
                        <Edit
                            class="mazin"
                            @click="openDialogEdit(data)"
                            :disabled="enableDelete(data.orders1.isDeleted)"
                        />

                        <Delete
                            class="maz"
                            @click="confirmDelete(data.orders1.idOrder)"
                            :disabled="enableDelete(data.orders1.isDeleted)"
                        />
                    </template>
                </Column>
            </DataTable>
        </div>

        <Toast />

        <Dialog
            header="Không có quyền truy cập!"
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
                <Button label="Hoàn tất" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <AddDevicesOther
            :isOpenDialog="isOpenDialog"
            :branchs="branchArr"
            :orderSelected="orderSelected"
            :devices="devicesArr"
            @closeDialog="closeDialog"
            @reloadpage="getAllorder"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import Export from '../../components/buttons/Export.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Add from '../../components/buttons/Add.vue'
    import View from '../../components/buttons/View.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import AddDevicesOther from '@/views/ListDeviceOther/AddDevicesOther.vue'
    import { ListDevicesOtherDto, Order } from '@/views/ListDeviceOther/ListDevicesOther.dto'
    // import { DevicesDto } from '@/views/Devices/Devices.dto'
    import { BranchDto } from '@/views/ListDeviceOther/Branch.dto'
    import router from '@/router'
    import { FilterMatchMode } from 'primevue/api'
    import jwtDecode from 'jwt-decode'

    export default {
        data() {
            return {
                dataOrder: new Order(),
                isOpenDialog: false,
                displayBasic: false,
                num: 5,
                ListDeviceOther: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                pageIndex: [5, 10, 15, 20],
                loading: true,
                rendercolumn: [],
                Arrayfilder2: {},
                ShowIsdeleted: [],
                searchName: '',
                deleteMulti: [],
                orderSelected: new Order(),
                devices: new DevicesDto(),
                branch: new BranchDto(),
                devicesArr: [],
                branchArr: [],
            }
        },
        mounted() {
            this.getAllorder()
            this.getAllDevices()
            this.getListBranch()
        },

        methods: {
            getAllorder() {
                HTTP.get('Orders/getAllOrder').then((res) => {
                    this.ListDeviceOther = res.data

                    this.loading = false
                })
            },

            exportCSV() {
                this.$refs.dt.exportCSV()
            },

            enableDelete(isDeleted) {
                return isDeleted === 1
            },

            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Success Message',
                    detail: 'Message Content',
                    life: 3000,
                })
            },
            showError() {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Error Message',
                    detail: 'You refuse to delete',
                    life: 3000,
                })
            },

            deleteDeviceOrder(id) {
                let userLogin = jwtDecode(localStorage.getItem('token'))
                let idUser = userLogin.Id

                HTTP.put('Orders/deleteOrder?id=' + id + '&user=' + idUser).then((res) => {
                    if (res.data === 'Success') {
                        this.showSuccess()
                    } else {
                        this.showError()
                    }

                    this.getAllorder()
                })
            },
            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Do you want to delete this Device Order?',
                    header: 'Delete Confirmation',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteDeviceOrder(id)
                    },
                    reject: () => {
                        this.showError()
                    },
                })
            },

            openDialog() {
                this.isOpenDialog = true
                this.orderSelected = []
            },
            closeDialog() {
                this.isOpenDialog = false
                this.orderSelected = []
            },
            openDialogEdit(data) {
                this.orderSelected = { ...data.orders1 }
                this.isOpenDialog = true
            },
            getAllDevices() {
                HTTP.get('Devices/getListDevices')
                    .then((res) => {
                        if (res.data) {
                            res.data.forEach((item) => {
                                this.devices.idDevice = item.idDevice
                                this.devices.deviceName = item.deviceName
                                this.devices.info = item.info
                                this.devices.isDelete = item.isDelete
                                this.devices.dateUpdated = item.dateUpdated
                                this.devices.userUpdated = item.userUpdated
                                this.devices.dateCreated = item.dateCreated
                                this.devices.userCreated = item.userCreated
                                this.devices.note = item.note
                                this.devicesArr.push(item)
                            })
                        } else {
                            this.devicesArr = []
                        }
                        this.loading = false
                    })
                    .catch((err) => {
                        this.devicesArr = []
                        this.loading = false
                    })
            },
            getListBranch() {
                HTTP.get('Branchs/getListBranch')
                    .then((res) => {
                        if (res.data) {
                            res.data.forEach((item) => {
                                this.branch.idBranch = item.idBranch
                                this.branch.branchName = item.branchName
                                this.branchArr.push(item)
                            })
                        } else {
                            this.branchArr = []
                        }
                        this.loading = false
                    })
                    .catch((err) => {
                        this.branchArr = []
                        this.loading = false
                    })
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
        },
        components: { Export, Delete, Add, View, Edit, AddDevicesOther },
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
