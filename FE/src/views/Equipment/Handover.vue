<template>
    <LayoutDefaultDynamic>
        <DynamicDialog />
        <Toast />
        <div class="mx-3 mt-3">
            <DataTable
                :value="handoverComputed"
                :paginator="true"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :loading="loading"
                :rowHover="true"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="['userSend']"
                responsiveLayout="scroll"
                ref="dt"
            >
                <template #header>
                    <div>
                        <h5 class="header-tilte">Handover</h5>
                        <div class="d-flex justify-content-between">
                            <div class="header-left">
                                <Export label="Export" style="margin-right: 10px" @click="exportCSV($event)" />
                                <Add label="Add" style="margin-right: 10px" v-on:click="openModaladd" />
                                <Confirm label="Confirm" @click="showYourDevice()" />
                            </div>
                            <div class="d-flex">
                                <Button
                                    @click="resetFilter"
                                    icon="pi pi-filter-slash"
                                    class="p-button-sm margin-right button-gray"
                                />
                                <div style="float: right; display: inline; margin-right: 10px">
                                    <Dropdown
                                        v-model="selectedStatus"
                                        :options="statusList"
                                        optionValue="code"
                                        optionLabel="name"
                                        placeholder="Select Status"
                                    />
                                </div>
                                <div style="float: right; display: inline; margin-right: 10px">
                                    <Dropdown
                                        v-model="selectedBranch"
                                        :options="branchs"
                                        placeholder="Select Branch"
                                        optionValue="idBranch"
                                        optionLabel="branchName"
                                    />
                                </div>
                                <span class="p-input-icon-left" style="margin-right: 10px">
                                    <i class="pi pi-search" />
                                    <InputText type="text" placeholder="User Received" v-model="searchUserReceived" />
                                </span>
                                <span class="p-input-icon-left">
                                    <i class="pi pi-search" />
                                    <InputText type="text" placeholder="Device Name" v-model="searchDeviceName" />
                                </span>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> No project found. </template>
                <template #loading><ProgressSpinner /></template>
                <Column field="" header="#"
                    ><template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="userReceivedName" header="User Received" sortable>
                    <template #body="{ data }">
                        {{ data.userReceivedName }}
                    </template>
                </Column>
                <Column field="deviceName" header="Device Name" sortable filterField="userSend">
                    <template #body="{ data }">
                        {{ data.deviceName }}
                    </template>
                </Column>
                <Column field="amount" header="Amount" sortable>
                    <template #body="{ data }">
                        {{ data.amount }}
                    </template>
                </Column>
                <Column field="branchName" header="Branch">
                    <template #body="{ data }">
                        {{ data.branchName }}
                    </template>
                </Column>
                <Column header="Action" style="min-width: 10rem">
                    <template #body="{ data }">
                        <!-- <Edit :disabled="data.isDeleted" /> -->
                        <Edit v-on:click="showEditUseDevice(data.idHandover)" :disabled="data.isDeleted" />
                        &nbsp;
                        <Delete @click="handleDelete(data.idHandover)" :disabled="data.isDeleted" />
                    </template>
                </Column>
                <Column field="status" header="Status">
                    <template #body="{ data }">
                        <span :class="'badge ' + renderStatusClass(data.status)">{{
                            renderStatusString(data.status)
                        }}</span>
                    </template>
                </Column>
            </DataTable>
          
        </div>
      <AddUseDevicenew :status="statusOpen" @reloadpage="getAllHandover" 
        @CloseModal = "closeModaladd"
      />

    </LayoutDefaultDynamic>
</template>

<script>
    import Add from '../../components/buttons/Add.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Member from '../../components/buttons/Member.vue'
    import Export from '../../components/buttons/Export.vue'
    import Confirm from '../../components/buttons/Confirm.vue'
    import { HTTP } from '@/http-common'
    import jwtDecode from 'jwt-decode'
    import addUseDeviceVue from './addUseDevice.vue'

    // import ConfirmUseDevicedVue from './ConfirmUseDeviced.vue'
    import EditUseDeviceVue from '@/views/Equipment/EditUseDevice.vue'
    import ConfirmUseDevicedVue from '@/views/Equipment/ConfirmUseDeviced.vue'

    import AddUseDevice from './addUseDevice.vue'
    import AddUseDevicenew from './addUseDevicenew.vue'

    // import EditUseDeviceVue from '../Equipment/EditUseDevice.vue'

    export default {
        data() {
            return {
                loading: true,
                selectedBranch: null,
                branchs: [
                    {
                        idBranch: 1,
                        branchName: 'branch 1',
                    },
                ],
                orders: [
                    {
                        idOder: 1,
                        idBranch: 1,
                        idDevice: 1,
                        amount: 10,
                        note: 'Đây là ghi chú',
                        statusOrder: 1,
                        statusDevice: 1,
                    },
                ],
                handover: [
                    {
                        idHandover: 1,
                        idDevice: 1,
                        userReceived: 3,
                        amount: 2,
                        status: 1,
                        isDeleted: 0,
                    },
                ],
                searchDeviceName: '',
                searchUserReceived: '',
                selectedStatus: null,
                statusOpen: false,
                statusList: [
                    {
                        code: 1,
                        name: 'Waiting',
                    },
                    {
                        code: 2,
                        name: 'Recover',
                    },
                    {
                        code: 3,
                        name: 'Fix',
                    },
                ],
                devices: [
                    {
                        idDevice: 1,
                        deviceName: 'Chuột',
                        type: 1,
                    },
                ],
                Opendialog : false,
            }
        },
        computed: {
            handoverComputed() {
                let filterList = this.handover

                const deviceList = this.devices.map((ele) => {
                    let branch = this.orders
                        .filter((doc) => ele.idDevice === doc.idDevice)
                        .map((doc) => ({ idBranch: doc.idBranch, branchName: doc.branchName }))
                    if (branch.length > 0) {
                        branch = branch[0]
                    }
                    return { ...ele, idBranch: branch.idBranch, branchName: branch.branchName }
                })

                filterList = filterList.map((ele) => {
                    let device = deviceList
                        .filter((doc) => ele.idDevice === doc.idDevice)
                        .map((doc) => ({
                            deviceName: doc.deviceName,
                            idBranch: doc.idBranch,
                            branchName: doc.branchName,
                        }))
                    if (device.length > 0) {
                        device = device[0]
                    }

                    return {
                        ...ele,
                        idBranch: device.idBranch,
                        branchName: device.branchName,
                    }
                })
                if (this.selectedStatus) {
                    filterList = filterList.filter((ele) => {
                        return this.selectedStatus === ele.status
                    })
                }
                if (this.selectedBranch) {
                    filterList = filterList.filter((ele) => {
                        return this.selectedBranch === ele.idBranch
                    })
                }
                if (this.searchDeviceName !== '') {
                    filterList = filterList.filter((ele) => {
                        const deviceName = this.removeVietnameseTones(ele.deviceName.toLowerCase())
                        const searchDeviceName = this.removeVietnameseTones(this.searchDeviceName.toLowerCase())
                        return deviceName.indexOf(searchDeviceName) !== -1
                    })
                }
                if (this.searchUserReceived !== '') {
                    filterList = filterList.filter((ele) => {
                        const userReceivedName = this.removeVietnameseTones(ele.userReceivedName.toLowerCase())
                        const searchUserReceived = this.removeVietnameseTones(this.searchUserReceived.toLowerCase())
                        return userReceivedName.indexOf(searchUserReceived) !== -1
                    })
                }
                return filterList
            },
        },
        mounted() {
            this.getAllBranchs()
            this.getAllOrders()
            this.getAllHandover()
            this.getAllDevices()
        },
        methods: {

            openModaladd() {
                this.statusOpen = true;
            },

            closeModaladd(){
                this.statusOpen = false;
            },

            showAddUseDevice() {
              
                this.$dialog.open(addUseDeviceVue, {
                    props: {
                        header: 'Add Handovers',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                        modal:true
                    },
              
                    
                })
            },
            showYourDevice() {
                this.$dialog.open(ConfirmUseDevicedVue, {
                    props: {
                        header: 'Your Device Using',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                    },
                })
            },
            showEditUseDevice(id) {
                this.$dialog.open(EditUseDeviceVue, {
                    props: {
                        header: 'Edit Use Device',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                    },
                    data: {
                        idHandover: id,
                    },
                    
                })
            },
            getAllHandover() {
                HTTP.get('Handovers/getListHandover')
                    .then((res) => res.data)
                    .then((res) => {
                        const array = res.map((row) => {
                            const handover = row.hand1
                            return {
                                ...handover,
                                deviceName: row.deviceName,
                                userReceivedName: row.lastName + ' ' + row.firstName,
                            }
                        })
                        this.handover = array
                        this.loading = false
                    })
                    .catch((err) )
            },
            getAllOrders() {
                HTTP.get('Orders/getAllOrder')
                    .then((res) => res.data)
                    .then((res) => {
                        const array = res.map((row) => {
                            const order = row.orders1
                            return {
                                ...order,
                                branchName: row.branchName,
                            }
                        })
                        this.orders = array
                    })
                    .catch((err) )
            },
            getAllDevices() {
                HTTP.get('Devices/getListDevices')
                    .then((res) => res.data)
                    .then((res) => {
                        this.devices = res
                    })
                    .catch((err) )
            },
            getAllBranchs() {
                HTTP.get('Branchs/getListBranch')
                    .then((res) => res.data)
                    .then((res) => {
                        this.branchs = res
                    })
                    .catch((err) )
            },
            renderStatusClass(status) {
                if (status === 1) {
                    return 'bg-success'
                }
                if (status === 2) {
                    return 'bg-primary'
                }
                if (status === 3) {
                    return 'bg-danger'
                }
            },
            renderStatusString(status) {
                if (status === 1) {
                    return 'WAITING'
                }
                if (status === 2) {
                    return 'DONE'
                }
                if (status === 3) {
                    return 'CANCEL'
                }
            },
            formatData(dateString) {
                return new Date(dateString).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
            },
            exportCSV() {
                this.$refs.dt.exportCSV()
            },
            resetFilter() {
                this.selectedBranch = null
                this.selectedStatus = null
                this.selectedType = null
                this.searchDeviceName = ''
                this.searchUserReceived = ''
            },
            handleDelete(id) {
                this.$confirm.require({
                    message: 'Do you want to delete this handover?',
                    header: 'Delete Confirmation',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        this.$toast.add({
                            severity: 'info',
                            summary: 'Confirmed',
                            detail: 'Handover deleted',
                            life: 3000,
                        })
                        try {
                            let userlogin = jwtDecode(localStorage.getItem('token'))
                            let idUser = userlogin.Id
                            const idhand = id

                            HTTP.put(`Handovers/deleteHandover/${idhand}`, { idhand: id, user: idUser })
                                .then((res) => {
                                    this.getAllHandover()
                                })
                                .catch((err))
                        } catch (error) {
                        }
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
            removeVietnameseTones(str) {
                str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, 'a')
                str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, 'e')
                str = str.replace(/ì|í|ị|ỉ|ĩ/g, 'i')
                str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, 'o')
                str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, 'u')
                str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, 'y')
                str = str.replace(/đ/g, 'd')
                str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, 'A')
                str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, 'E')
                str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, 'I')
                str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, 'O')
                str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, 'U')
                str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, 'Y')
                str = str.replace(/Đ/g, 'D')
                // Some system encode vietnamese combining accent as individual utf-8 characters
                // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
                str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, '') // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
                str = str.replace(/\u02C6|\u0306|\u031B/g, '') // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
                // Remove extra spaces
                // Bỏ các khoảng trắng liền nhau
                str = str.replace(/ + /g, ' ')
                str = str.trim()
                // Remove punctuations
                // Bỏ dấu câu, kí tự đặc biệt
                str = str.replace(
                    /!|@|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g,
                    ' ',
                )
                return str
            },
        },
        components: {
    Add,
    Edit,
    Delete,
    Member,
    Export,
    Confirm,
    addUseDeviceVue,
    EditUseDeviceVue,
    AddUseDevice,
    AddUseDevicenew,

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
