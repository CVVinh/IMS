<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <!--
        <div class="mx-4">
            <Breadcrumb :home="timeSheet" :model="itemsTimeSheet" />
        </div>
        -->
        <div class="mx-4 mt-3">
            <DataTable
                :value="paids"
                :paginator="true"
                :rows="5"
                :rowHover="true"
                :loading="loading"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[5, 10, 20, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                responsiveLayout="scroll"
                filterDisplay="menu"
                v-model:filters="filters"
                :globalFilterFields="['customerFullName', 'nameProject']"
                showGridlines
            >
                <template #header>
                    <h5 class="m-0 mb-2">Danh sách thu chi</h5>
                    <div class="header-container">
                        <Add label="Thêm" @click="Openmodal" />
                        <div class="input-text">
                            <Button
                                type="button"
                                style="background-color: antiquewhite; width: 120px"
                                icon="pi pi-filter-slash"
                                class="p-button-outlined right me-2"
                                @click="reload()"
                            />

                            <!-- <InputText type="date" v-model="filterStartDate" @change="filterEventStartDate()"  class="form-control me-2" />
                            <InputText type="date" v-model="filterEndDate" @change="filterEventEndDate()" class="form-control me-2" /> -->

                            <InputText type="date" v-model="filterStartDate" class="form-control me-2" />
                            <InputText type="date" v-model="filterEndDate" class="form-control me-2" />

                            <Button
                                type="button"
                                style="background-color: antiquewhite; width: 120px"
                                icon="pi pi-filter"
                                class="p-button-outlined right me-2"
                                @click="btnFilterByDate()"
                            />
                        </div>
                    </div>
                </template>

                <template #empty> Không tìm thấy dữ liệu. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>

                <Column field="#" header="#" dataType="date">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>

                <Column field="nameProject" header="Dự án">
                    <template #body="{ data }">
                        {{ data.nameProject }}
                    </template>
                    <template #filter="{ filterModel }">
                        <InputText
                            type="text"
                            v-model="filterModel.value"
                            class="p-column-filter"
                            placeholder="Nhập tên"
                        />
                    </template>
                </Column>

                <Column field="paidPersonName" header="Người chi tiêu">
                    <template #body="{ data }">
                        {{ data.paidPersonName }}
                    </template>
                </Column>

                <Column field="personConfirmName" header="Người xác nhận">
                    <template #body="{ data }">
                        {{ data.personConfirmName }}
                    </template>
                </Column>

                <Column field="customerFullName" header="Khách hàng">
                    <template #body="{ data }">
                        {{ data.customerFullName }}
                    </template>
                    <template #filter="{ filterModel }">
                        <InputText
                            type="text"
                            v-model="filterModel.value"
                            class="p-column-filter"
                            placeholder="Nhập tên"
                        />
                    </template>
                </Column>

                <Column field="amountPaidName" header="Giá tiền">
                    <template #body="{ data }">
                        {{ data.amountPaidName }}
                    </template>
                </Column>

                <Column field="paidReasonName" header="Lý do">
                    <template #body="{ data }">
                        {{ data.paidReasonName }}
                    </template>
                </Column>

                <Column sortable field="paidDate" header="Ngày Chấp Nhận">
                    <template #body="{ data }">
                        {{ data.paidDate }}
                    </template>
                </Column>

                <Column field="isPaid" header="Trạng thái" style="width: 5rem">
                    <template #body="{ data }">
                        <div
                            :class="
                                data.isPaid && data.isAccept
                                    ? 'badge bg-success'
                                    : data.isPaid && !data.isAccept
                                    ? 'badge bg-danger'
                                    : 'badge bg-warning'
                            "
                        >
                            {{
                                data.isPaid && data.isAccept
                                    ? 'Đã Thanh Toán'
                                    : data.isPaid && !data.isAccept
                                    ? 'Từ chối thanh toán'
                                    : 'Chưa thanh toán'
                            }}
                        </div>
                    </template>
                </Column>

                <Column header="Thực thi" style="width: 15rem; text-align: left">
                    <template #body="{ data }">
                        <div class="actions-buttons">
                            <Button
                                icon="pi pi-eye"
                                @click="openModalDetails(data)"
                                class="p-button p-component me-2"
                            />
                            <div class="actions-buttons" v-if="data.isPaid == false">
                                <Button
                                    icon="pi pi-pencil"
                                    class="p-button p-component p-button-warning me-2"
                                    @click="Openeditmodal(data)"
                                />
                                <Button
                                    icon="pi pi-trash"
                                    class="p-button p-component p-button-danger me-2"
                                    @click="Delete(data)"
                                />
                                <Button
                                    icon="pi pi-check"
                                    class="p-button p-component p-button-success me-2"
                                    @click="paymentConfirmation(data)"
                                    v-if="showButton.confirm"
                                />
                                <Button
                                    icon="pi pi-times"
                                    class="p-button p-component p-button-danger"
                                    @click="paymentNotConfirmation(data)"
                                    v-if="showButton.confirm"
                                />
                            </div>
                        </div>
                    </template>
                </Column>
            </DataTable>

            <AddPaid
                :status="openStatus"
                @closemodal="Closemodal"
                :optionmodule="OptionModule"
                @reloadpage="getData"
                :customerArr="customerArr"
                :paidReasonArr="paidReasonArr"
            />

            <EditPaid
                :status="openStatusEdit"
                @closemodal="closeeditmodal"
                :dataedit="editdata"
                :optionmodule="OptionModule"
                @reloadpage="getData"
                :customerArr="customerArr"
                :paidReasonArr="paidReasonArr"
            />

            <DetailPaid
                :status="displayImage"
                @closemodal="closeDetails"
                :dataDetail="detailData"
                @confirmPayment="paymentConfirmation($event)"
            />

            <Toast />
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL, GET_PROJECT_BY_ID, GET_USER_BY_ID, HTTP_API_GITLAB } from '@/http-common'
    import Add from '../../components/buttons/Add.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import dayjs from 'dayjs'
    import { FilterMatchMode, FilterOperator } from 'primevue/api'
    import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import AddPaid from './addPaid.vue'
    import EditPaid from './editPaid.vue'
    import DetailPaid from './detailPaid.vue'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '@/router'
    import { DateHelper } from '@/helper/date.helper'

    export default {
        data() {
            return {
                openStatus: false,
                openStatusEdit: false,
                editdata: null,
                detailData: null,
                OptionModule: [],
                customerArr: [],
                paidReasonArr: [],
                paids: [],
                loading: false,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    customerFullName: {
                        operator: FilterOperator.AND,
                        constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                    },
                    nameProject: {
                        operator: FilterOperator.AND,
                        constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                    },
                },
                filterStartDate: '',
                filterEndDate: '',
                token: null,
                displayImage: false,
                showButton: {
                    add: false,
                    edit: false,
                    confirm: false,
                    delete: false,
                    view: false,
                },
            }
        },

        async mounted() {
            this.token = LocalStorage.jwtDecodeToken()
            await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))

            if (UserRoleHelper.isAccess) {
                await this.getData()
            } else {
                alert('Bạn không có quyền truy cập module này')
                router.push('/')
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

            openModalDetails(data) {
                this.displayImage = true
                this.detailData = data
            },

            closeDetails() {
                this.displayImage = false
            },

            resetFieldDate() {
                this.filterStartDate = ''
                this.filterEndDate = ''
            },

            async reload() {
                this.resetFieldDate()
                await this.getData()
            },

            formatDate(date) {
                var dateFormat = new Date(date)
                return dayjs(dateFormat).format('DD/MM/YYYY')
            },

            async getProjects(id) {
                return await HTTP.get(`Project/getProByIdDel/${id}`)
                    .then((respone) => respone.data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getUsers(id) {
                return await HTTP.get(GET_USER_BY_ID(id))
                    .then((respone) => respone.data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getCustomerId(id) {
                return await HTTP.get(`Customer/GetById/${id}`)
                    .then((respone) => respone.data._Data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getPaidReasonId(id) {
                return await HTTP.get(`PaidReason/GetById/${id}`)
                    .then((respone) => respone.data._Data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },

            showInfo(message) {
                this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 })
            },

            showResponseApi(status, message = '') {
                switch (status) {
                    case 401:
                    case 403:
                        this.showError('Bạn không có quyền thực hiện chức năng này!')
                        break

                    case 404:
                        this.showError('Lỗi! Load dữ liệu!')
                        break

                    default:
                        if (message != '') {
                            this.showError(message)
                        } else {
                            this.showError('Có lỗi trong quá trình thực hiện!')
                        }
                        break
                }
            },

            async getData() {
                try {
                    this.paids = []

                    if (Number(this.token.IdGroup) === 1) {
                        // getAPI (Admin)
                        await this.getPaid()
                        await this.getAllProject()
                        this.showButton.confirm = true
                    } else if (Number(this.token.IdGroup) === 2) {
                        // (Sample)
                        await this.getPaid(this.token.Id)
                        await this.getAllProject()
                        this.showButton.confirm = true
                    }

                    // getAPI tất cả role còn lại
                    else if (Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1) {
                        if (Number(this.token.IdGroup) === 5) {
                            // pm
                            await this.getPaidByIdUser(this.token.Id)
                            await this.getAllProject()
                        } else {
                            await this.getPaidByIdUser(this.token.Id)
                            await this.getAllProject(this.token.Id)
                        }
                    }
                    await this.getAllCustomer()
                    await this.getAllPaidReason()
                } catch (error) {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                    router.push('/')
                }
            },

            async DeletePaid(id) {
                try {
                    await HTTP.delete(`Paid/${id}`).then(async (res) => {
                        if (res.status == 200) {
                            this.showSuccess('Xóa thành công!')
                            await this.getData()
                        } else {
                            this.showResponseApi(res.status)
                        }
                    })
                } catch (error) {
                    var message = error.response.data != '' ? error.response.data : error.response.statusText
                    this.showResponseApi(error.response.status, message)
                }
            },

            Delete(data) {
                this.$confirm.require({
                    message: `Bạn có chắc muốn xóa thu chi của của khách hàng này?`,
                    header: 'Xóa thu chi',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        await this.DeletePaid(data.id)
                    },
                    reject: () => {
                        return
                    },
                })
            },

            async getAllCustomer() {
                this.customerArr = []
                await HTTP.get('Customer/getAllCustomer')
                    .then((res) => {
                        this.customerArr = res.data._Data
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getAllPaidReason() {
                this.paidReasonArr = []
                await HTTP.get('PaidReason/getAllPaidReason')
                    .then((res) => {
                        this.paidReasonArr = res.data._Data
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getAllProject(idUser = null) {
                this.OptionModule = []
                if (idUser == null) {
                    await HTTP.get('Project/getAllProjectRunning')
                        .then((res) => {
                            this.OptionModule = res.data
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                } else {
                    if (Number(this.token.IdGroup) === 3) {
                        // lead
                        await HTTP.get(`Project/getAllProjectByLead/${idUser}`)
                            .then((res) => {
                                this.OptionModule = res.data
                            })
                            .catch((error) => {
                                var message =
                                    error.response.data != '' ? error.response.data : error.response.statusText
                                this.showResponseApi(error.response.status, message)
                            })
                    }
                    if (Number(this.token.IdGroup) === 4) {
                        // staff
                        await HTTP.get(`Project/getAllProjectByStaff/${idUser}`)
                            .then((res) => {
                                this.OptionModule = res.data
                            })
                            .catch((error) => {
                                var message =
                                    error.response.data != '' ? error.response.data : error.response.statusText
                                this.showResponseApi(error.response.status, message)
                            })
                    }
                }
            },

            async getPaidByIdUser(iduser) {
                try {
                    this.loading = true
                    await HTTP.get(`Paid/GetByUserId?id=${iduser}`)
                        .then((res) => {
                            this.paids = res.data._Data
                        })
                        .catch((error) => {
                            var message = error.response.data != '' ? error.response.data : error.response.statusText
                            this.showResponseApi(error.response.status, message)
                        })
                    await this.getWithName()
                } catch (err) {
                    console.log('Lỗi! Lấy thông tin nhân viên:')
                    console.log(err)
                } finally {
                    this.loading = false
                }
            },

            async getPaid(idSample = null) {
                try {
                    this.loading = true
                    this.paids = []

                    if (idSample == null) {
                        await HTTP.get(GET_LIST_PAID)
                            .then((respone) => {
                                this.paids = respone.data._Data
                            })
                            .catch((error) => {
                                var message =
                                    error.response.data != '' ? error.response.data : error.response.statusText
                                this.showResponseApi(error.response.status, message)
                            })
                    } else {
                        await HTTP.get(`Paid/GetByIdSample/${idSample}`)
                            .then((respone) => {
                                this.paids = respone.data._Data
                            })
                            .catch((error) => {
                                var message =
                                    error.response.data != '' ? error.response.data : error.response.statusText
                                this.showResponseApi(error.response.status, message)
                            })
                    }

                    await this.getWithName()
                } catch (error) {
                    console.log('Lỗi! Lấy danh sách thu chi')
                    console.log(error)
                } finally {
                    this.loading = false
                }
            },

            async getWithName() {
                for (let i = 0; i < this.paids.length; i++) {
                    if (this.paids[i].projectId != 0) {
                        var project = await this.getProjects(this.paids[i].projectId)
                        this.paids[i].isDelPro = project.isDeleted
                        this.paids[i].nameProject = project.name
                    } else {
                        this.paids[i].nameProject = ''
                    }

                    if (this.paids[i].personConfirm != null) {
                        var confirmUser = await this.getUsers(this.paids[i].personConfirm)
                        //this.paids[i].confirmUser = confirmUser;
                        this.paids[i].personConfirmName = confirmUser.fullName
                    } else {
                        this.paids[i].personConfirmName = ''
                    }

                    var paidUser = await this.getUsers(this.paids[i].paidPerson)

                    var customer = await this.getCustomerId(parseInt(this.paids[i].customerName))
                    var paidReason = await this.getPaidReasonId(parseInt(this.paids[i].paidReason))

                    if (this.paids[i].paidDate === '0001-01-01T00:00:00') {
                        this.paids[i].paidDate = ''
                    } else {
                        this.paids[i].paidDate = DateHelper.formatDate(this.paids[i].paidDate)
                    }

                    this.paids[i].amountPaidName = this.paids[i].amountPaid.toLocaleString('it-IT', {
                        style: 'currency',
                        currency: 'VND',
                    })

                    //this.paids[i].customer = customer;
                    this.paids[i].customerName = parseInt(this.paids[i].customerName)
                    this.paids[i].customerFullName = customer.fullName

                    //this.paids[i].paidUser = paidUser;
                    this.paids[i].paidPersonName = paidUser.fullName

                    this.paids[i].paidReason = parseInt(this.paids[i].paidReason)
                    this.paids[i].paidReasonName = paidReason.name
                }
            },

            async callApiSearch(objSearch) {
                this.loading = true
                this.paids = []
                await HTTP.post('Paid/SearchPaidByDay', objSearch)
                    .then(async (respone) => {
                        this.paids = respone.data._Data
                        if (this.paids.length == 0) {
                            this.showInfo('Không tìm thấy dữ liệu!')
                        } else {
                            await this.getWithName()
                        }
                    })
                    .catch((error) => {
                        //this.showError(error.response.data._Message);
                        console.log(error)
                    })
                    .finally(() => {
                        this.loading = false
                    })
            },

            checkValidateDay() {
                if (this.filterStartDate > this.filterEndDate) {
                    this.showInfo('Ngày kết thúc phải lớn hơn ngày bắt đầu!')
                    return false
                }
                return true
            },

            checkStartDateEmpty() {
                if (this.filterStartDate != '') {
                    return true
                }
                return false
            },

            checkEndDateEmpty() {
                if (this.filterEndDate != '') {
                    return true
                }
                return false
            },

            async callApiFilterDay(startDate = null, endDate = null) {
                // getAPI (Sample) || (Admin)
                if (Number(this.token.IdGroup) === 2 || Number(this.token.IdGroup) === 1) {
                    await this.callApiSearch({
                        startDate: startDate,
                        endDate: endDate,
                    })
                }
                // getAPI tất cả role còn lại
                if (Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1) {
                    await this.callApiSearch({
                        id: this.token.Id,
                        startDate: startDate,
                        endDate: endDate,
                    })
                }
            },

            async filterEventStartDate() {
                if (this.checkStartDateEmpty() && this.checkEndDateEmpty() == false) {
                    await this.callApiFilterDay(this.filterStartDate, null)
                    return
                }
                if (this.checkStartDateEmpty() && this.checkEndDateEmpty()) {
                    if (this.checkValidateDay()) {
                        await this.callApiFilterDay(this.filterStartDate, this.filterEndDate)
                    } else {
                        this.resetFieldDate()
                    }
                }
            },

            async filterEventEndDate() {
                if (this.checkStartDateEmpty() == false) {
                    this.showInfo('Nhập ngày bắt đầu trước!')
                    this.resetFieldDate()
                    return
                }
                await this.filterEventStartDate()
            },

            async btnFilterByDate() {
                if (this.checkStartDateEmpty() == false && this.checkEndDateEmpty() == false) {
                    this.showInfo('Ngày lọc rỗng!')
                } else {
                    await this.filterEventEndDate()
                }
            },

            async CallApiPaymentConfirm(idPaid) {
                await HTTP.put(`Paid/acceptPayment/${idPaid}`, { PersonConfirm: this.token.Id })
                    .then(async (res) => {
                        if (res.status == 200) {
                            this.showSuccess('Thanh toán thành công!')
                            await this.getData()
                        } else {
                            this.showError('Lỗi! cập nhật thanh toán!')
                        }
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            paymentConfirmation(item) {
                this.$confirm.require({
                    message: `Bạn có chắc muốn thanh toán cho khách hàng này?`,
                    header: 'Xác nhận thanh toán',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-info',
                    accept: () => {
                        this.displayImage = false
                        this.CallApiPaymentConfirm(item.id)
                    },
                    reject: () => {
                        this.showInfo('Bạn đã từ chối')
                    },
                })
            },
            async CallApiNotAccept(idPaid) {
                await HTTP.put(`Paid/NotAcceptPayment/${idPaid}`, { PersonConfirm: this.token.Id })
                    .then(async (res) => {
                        if (res.status == 200) {
                            this.showSuccess('Không Chấp Nhận Thanh toán thành công!')
                            await this.getData()
                        } else {
                            this.showError('Lỗi! cập nhật thanh toán!')
                        }
                    })
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },
            paymentNotConfirmation(item) {
                this.$confirm.require({
                    message: `Bạn có chắc muốn không thanh toán cho khách hàng này?`,
                    header: 'Xác nhận không thanh toán',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-info',
                    accept: () => {
                        this.displayImage = false
                        this.CallApiNotAccept(item.id)
                    },
                    reject: () => {
                        this.showInfo('Bạn đã từ chối')
                    },
                })
            },
        },

        components: { LayoutDefaultDynamic, Add, Edit, Delete, AddPaid, EditPaid, DetailPaid },
    }
</script>
<style lang="scss">
    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1rem;
        height: 40px;
    }

    .input-text {
        display: flex;
        height: 45px;
    }

    .p-datatable.p-datatable-gridlines .p-datatable-header {
        background-color: #607d8b;
        color: white;
    }

    .p-dialog-content img {
        width: 100%;
    }

    .content_box {
        box-shadow: -3px 3px 5px -3px #888888, 4px 5px 3px -4px #888888, 4px 5px 2px -5px #888888 inset;
        padding: 10px;
        border-radius: 10px;
    }

    .actions-buttons {
        display: flex;
    }
</style>
