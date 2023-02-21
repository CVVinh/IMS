<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <!--
        <div class="mx-4">
            <Breadcrumb :home="timeSheet" :model="itemsTimeSheet" />
        </div>
        -->
        <div class="mx-4 mt-3">
            <div v-if="this.filterArr.length === 0">
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
                    :globalFilterFields="['customerName', 'nameProject']"
                    showGridlines="true"
                >
                    <template #header>
                        <h5 class="m-0 mb-2">Danh sách thu chi</h5>
                        <div class="d-flex justify-content-between align-items-center">
                            <!--
                            <button class="btn btn-primary" @click="Openmodal()">Add Paid</button>
                            -->
                            <Add label="Thêm" class="itemsbutton" @click="Openmodal" />
                            <div class="d-flex">
                                <input type="date" v-model="filterStartDate" class="form-control me-2" />
                                <input type="date" v-model="filterEndDate" class="form-control me-2" />
                                <!--
                                <button
                                    class="btn btn-primary me-2"
                                    @click="filterWithDate(filterStartDate, filterEndDate)"
                                >
                                    Lọc
                                </button>
                                -->
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite; width: 120px"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined right me-2"
                                    @click="filterWithDate(filterStartDate, filterEndDate)"
                                />
                                <Button
                                    class="p-button p-button-secondary ms-2 justify-content-center"
                                    @click="reload()"
                                >
                                    <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                                ></Button>
                                <!--
                                <button type="button" class="btn btn-dark" @click="Reload()">Reload</button>
                                -->
                            </div>
                        </div>
                    </template>
                    <template #empty> Không tìm thấy. </template>
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
                    <Column field="customerName" header="Khách hàng">
                        <template #body="{ data }">
                            {{ data.customerName }}
                        </template>
                        <template #filter="{ filterModel }">
                            <InputText
                                type="text"
                                v-model="filterModel.value"
                                class="p-column-filter"
                                placeholder="Nhập"
                            />
                        </template>
                    </Column>
                    <Column field="amountPaid" header="Giá tiền">
                        <template #body="{ data }">
                            {{ data.amountPaid.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }) }}
                        </template>
                    </Column>
                    <Column field="paidReason" header="Lý do">
                        <template #body="{ data }">
                            {{ data.paidReason }}
                        </template>
                    </Column>

                    <Column sortable field="paidDate" header="Ngày">
                        <template #body="{ data }">
                            {{ formatDate(data.paidDate) }}
                        </template>
                    </Column>
                    <Column field="isPaid" header="Trạng thái">
                        <template #body="{ data }">
                            {{ data.isPaid == true ? 'Đã Thanh Toán' : 'Chưa Thanh Toán' }}
                        </template>
                    </Column>
                    <Column header="Thực thi">
                        <template #body="{ data }">
                            <!--
                            <button class="btn btn-warning me-2" @click="Openeditmodal(data.id)" :disabled=data.isPaid>Edit</button>
                            <button class="btn btn-danger" @click="Delete(data.id)">Delete</button>
                            -->
                            <Edit @click="Openeditmodal(data.id)" :disabled="data.isPaid" class="me-2" />

                            <Delete @click="Delete(data.id)" :disabled="data.isPaid" />
                        </template>
                    </Column>
                </DataTable>
            </div>
            <div v-else>
                <DataTable
                    :value="filterArr"
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
                    :globalFilterFields="['customerName', 'nameProject']"
                    showGridlines="true"
                >
                    <template #header>
                        <h5 class="m-0 mb-2">Danh sách thu chi</h5>
                        <div class="d-flex justify-content-between align-items-center">
                            <!--
                            <button class="btn btn-primary" @click="Openmodal()">Add Paid</button>
                            -->
                            <Add label="Thêm" class="itemsbutton" @click="Openmodal" />
                            <div class="d-flex">
                                <input type="date" v-model="filterStartDate" class="form-control me-2" />
                                <input type="date" v-model="filterEndDate" class="form-control me-2" />
                                <!--
                                <button
                                    class="btn btn-primary me-2"
                                    @click="filterWithDate(filterStartDate, filterEndDate)"
                                >
                                    Lọc
                                </button>
                                -->
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite; width: 120px"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined right me-2"
                                    @click="filterWithDate(filterStartDate, filterEndDate)"
                                />
                                <Button
                                    class="p-button p-button-secondary ms-2 justify-content-center"
                                    @click="reload()"
                                >
                                    <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                                ></Button>
                                <!--
                                <button type="button" class="btn btn-dark" @click="Reload()">Reload</button>
                                -->
                            </div>
                        </div>
                    </template>
                    <template #empty> Không tìm thấy. </template>
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
                                placeholder="Nhập"
                            />
                        </template>
                    </Column>
                    <Column field="paidPersonName" header="Người chi tiêu">
                        <template #body="{ data }">
                            {{ data.paidPersonName }}
                        </template>
                    </Column>
                    <Column field="customerName" header="Khách hàng">
                        <template #body="{ data }">
                            {{ data.customerName }}
                        </template>
                        <template #filter="{ filterModel }">
                            <InputText
                                type="text"
                                v-model="filterModel.value"
                                class="p-column-filter"
                                placeholder="Nhập"
                            />
                        </template>
                    </Column>
                    <Column field="amountPaid" header="Giá tiền">
                        <template #body="{ data }">
                            {{ data.amountPaid.toLocaleString('en-US', { style: 'currency', currency: 'VND' }) }}
                        </template>
                    </Column>
                    <Column field="paidReason" header="Lý do">
                        <template #body="{ data }">
                            {{ data.paidReason }}
                        </template>
                    </Column>

                    <Column sortable field="paidDate" header="Ngày">
                        <template #body="{ data }">
                            {{ formatDate(data.paidDate) }}
                        </template>
                    </Column>
                    <Column field="isPaid" header="Trạng thái">
                        <template #body="{ data }">
                            {{ data.isPaid == true ? 'Đã Thanh Toán' : 'Chưa Thanh Toán' }}
                        </template>
                    </Column>
                    <Column header="Action">
                        <template #body="{ data }">
                            <!--
                            <button class="btn btn-warning me-2" @click="Openeditmodal(data.id)" :disabled=data.isPaid>Edit</button>
                            <button class="btn btn-danger" @click="Delete(data.id)">Delete</button>
                            -->
                            <Edit @click="Openeditmodal(data.id)" :disabled="data.isPaid" class="me-2" />

                            <Delete @click="Delete(data.id)" />
                        </template>
                    </Column>
                </DataTable>
            </div>
            <AddPaid
                :status="openStatus"
                @closemodal="Closemodal"
                @failed="showError1"
                @success="showSuccess1"
                :optionmodule="OptionModule"
                @reloadpage="getPaidReload"
                @getPaid="getPaidReload"
                @reloadpageother = "ReloadgetPaidByIdUser(this.token.Id)"
            />
            <EditPaid
                :status="openStatusEdit"
                @closemodal="closeeditmodal"
                @failed="showError2"
                @success="showSuccess2"
                :dataedit="editdata"
                :optionmodule="OptionModule"
                @reloadpage="getPaidReload"
                @reloadpageother = "ReloadgetPaidByIdUser(this.token.Id)"
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
import { LocalStorage } from '@/helper/local-storage.helper'
import { UserRoleHelper } from '@/helper/user-role.helper'
    export default {
        data() {
            return {
                openStatus: false,
                openStatusEdit: false,
                editdata: null,
                OptionModule: [],
                paids: [],
                loading: true,
                projects: [],
                users: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    customerName: {
                        operator: FilterOperator.AND,
                        constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                    },
                    nameProject: {
                        operator: FilterOperator.AND,
                        constraints: [{ value: null, matchMode: FilterMatchMode.STARTS_WITH }],
                    },
                },
                filterStartDate: null,
                filterEndDate: null,
                filterArr: [],
                token : null,
                //timeSheet: { label: 'Report', icon: 'me-1 bx bx-spreadsheet' },
                //itemsTimeSheet: [{ label: 'Paid', to: '/Paid' }],
            }
        },
        async mounted() {
            try
            {
                this.token = LocalStorage.jwtDecodeToken();
                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if(UserRoleHelper.isAccess){
                    // getAPI (Sample) || (Admin)
                    if(Number(this.token.IdGroup) === 2 || Number(this.token.IdGroup) === 1){
                        this.getPaid()
                        console.log('sample or admin');
                    }
                    // getAPI tất cả role còn lại
                    if(Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1){
                        this.getPaidByIdUser(this.token.Id)
                    }
                }else{
                    alert("Bạn không có quyền truy cập module này")
                }
                await this.getAllProject()
            }
            catch(err)
            {
                console.log(err);
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
            DeletePaid(id) {
                HTTP.delete('Paid?id=' + id).then((res) => {
                    console.log(res)
                    this.$toast.add({
                        severity: 'success',
                        summary: 'Thành công',
                        detail: 'Xóa thành công!',
                        life: 3000,
                    })
                    window.location.reload()
                })
            },
            Delete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.DeletePaid(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },

            async getAllProject() {
                HTTP.get('Project/getProjectOnGitlab').then((res) => {
                    if (res.data) {
                        const result = res.data
                        this.OptionModule = res.data
                    }
                    this.loading = false
                })
            },

            async ReloadgetPaidByIdUser(iduser){
                this.paids = [];
                try {
                    await HTTP.get(`Paid/GetByUserId?id=${iduser}`).then(res=>{
                    res.data._Data.forEach(ele=>{
                        this.paids.push({
                                id: ele.id,
                                amountPaid: ele.amountPaid,
                                customerName: ele.customerName,
                                isPaid: ele.isPaid,
                                paidDate: ele.paidDate,
                                paidPerson: ele.paidPerson,
                                paidReason: ele.paidReason,
                                projectId: ele.projectId,
                                project: null,
                                user: null,
                                nameProject: null,
                                paidPersonName: null,
                            })
                        })
                      
                    }).catch(err=>console.log(err))
                    await this.getWithName()
                    this.loading = false
                } catch(err){
                    console.log('something went wrong');
                }
            },

            async getPaidByIdUser(iduser){
                try {
                    await HTTP.get(`Paid/GetByUserId?id=${iduser}`).then(res=>{
                    res.data._Data.forEach(ele=>{
                        this.paids.push({
                                id: ele.id,
                                amountPaid: ele.amountPaid,
                                customerName: ele.customerName,
                                isPaid: ele.isPaid,
                                paidDate: ele.paidDate,
                                paidPerson: ele.paidPerson,
                                paidReason: ele.paidReason,
                                projectId: ele.projectId,
                                project: null,
                                user: null,
                                nameProject: null,
                                paidPersonName: null,
                            })
                        })
                      
                    }).catch(err=>console.log(err))
                    await this.getWithName()
                    this.loading = false
                } catch(err){
                    console.log('something went wrong');
                }
            },
            async getPaidReload (){
                this.paids = [];
                this.loading = true
                await HTTP.get(GET_LIST_PAID)
                    .then((respone) => {
                        respone.data._Data.forEach((el) => {
                            this.paids.push({
                                id: el.id,
                                amountPaid: el.amountPaid,
                                customerName: el.customerName,
                                isPaid: el.isPaid,
                                paidDate: el.paidDate,
                                paidPerson: el.paidPerson,
                                paidReason: el.paidReason,
                                projectId: el.projectId,
                                project: null,
                                user: null,
                                nameProject: null,
                                paidPersonName: null,
                            })
                        })
                        console.log(res.data._Data);
                    })         
                    .catch((error) => {
                        console.log(error)
                    })
                await this.getWithName()
                this.loading = false
            },
            async getPaid() {
                this.loading = true
                await HTTP.get(GET_LIST_PAID)
                    .then((respone) => {
                        respone.data._Data.forEach((el) => {
                            this.paids.push({
                                id: el.id,
                                amountPaid: el.amountPaid,
                                customerName: el.customerName,
                                isPaid: el.isPaid,
                                paidDate: el.paidDate,
                                paidPerson: el.paidPerson,
                                paidReason: el.paidReason,
                                projectId: el.projectId,
                                project: null,
                                user: null,
                                nameProject: null,
                                paidPersonName: null,
                            })
                        })
                        console.log(res.data._Data);
                    })         
                    .catch((error) => {
                        console.log(error)
                    })
                await this.getWithName()
                this.loading = false
            },



            async getPaid() {
                this.loading = true
                await HTTP.get(GET_LIST_PAID)
                    .then((respone) => {
                        respone.data._Data.forEach((el) => {
                            this.paids.push({
                                id: el.id,
                                amountPaid: el.amountPaid,
                                customerName: el.customerName,
                                isPaid: el.isPaid,
                                paidDate: el.paidDate,
                                paidPerson: el.paidPerson,
                                paidReason: el.paidReason,
                                projectId: el.projectId,
                                project: null,
                                user: null,
                                nameProject: null,
                                paidPersonName: null,
                            })
                        })
                        console.log(res.data._Data);
                    })         
                    .catch((error) => {
                        console.log(error)
                    })
                await this.getWithName()
                this.loading = false
            },
            getProjects(id) {
                return HTTP_LOCAL.get(GET_PROJECT_BY_ID(id)).then((respone) => respone.data)
            },
            getUsers(id) {
                return HTTP_LOCAL.get(GET_USER_BY_ID(id)).then((respone) => respone.data)
            },
            async getWithName() {
                for (let i = 0; i < this.paids.length; i++) {   
                    var project = await this.getProjects(this.paids[i].projectId)
                    var user = await this.getUsers(this.paids[i].paidPerson)
                    this.paids[i].project = project
                    this.paids[i].user = user
                    this.paids[i].nameProject = project.name
                    this.paids[i].paidPersonName = user.fullName
                }
            },
            formatDate(date) {
                var dateFormat = new Date(date)
                return dayjs(dateFormat).format('DD/MM/YYYY')
            },
            async filterWithDate(startDate, endDate) {
                this.filterArr = await this.paids.filter((el) => {
                    return (
                        this.dateToYMD(startDate) <= this.dateToYMD(el.paidDate) &&
                        this.dateToYMD(endDate) >= this.dateToYMD(el.paidDate)
                    )
                })
                if (this.filterArr.length > 0) {
                    console.log(this.filterArr)
                    this.$toast.add({
                        severity: 'success',
                        summary: 'Thành công',
                        detail: 'Lọc thành công',
                        life: 3000,
                    })
                    return this.filterArr
                } else {
                    console.log('no record')
                    return this.$toast.add({
                        severity: 'info',
                        summary: 'Thông tin',
                        detail: 'Không tìm thấy',
                        life: 3000,
                    })
                }
            },
            reload() {
                this.loading = true
                this.paids = []
                this.filterArr = []
                this.getPaid()
            },
            showError() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: 'Lỗi', life: 3000 })
            },
            showSuccess1() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Thêm mới thành công!',
                    life: 3000,
                })
            },
            showError1() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: 'Thêm mới lỗi!', life: 3000 })
            },
            showSuccess2() {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: 'Sửa thành công!', life: 3000 })
            },
            showError2() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: 'Sửa Lỗi!', life: 3000 })
            },
            showSuccess3() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Xóa thành công!',
                    life: 3000,
                })
            },

            async handlerReload() {
                alert()
                this.loading = true
                this.paids = []
                //this.fillterLeaveOff.searchLeaveOff = ''
                filterModel.value = ''
                //this.fillterLeaveOff.selectedDate = null
                //this.fillterLeaveOff.selectedLeaveOff = []
                //await this.getAllLeaveOffRegister()
                await this.getPaid()
            },
            dateToYMD(end_date) {
                var ed = new Date(end_date)
                var d = ed.getDate()
                var m = ed.getMonth() + 1
                var y = ed.getFullYear()
                return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d)
            },
        },

        components: { LayoutDefaultDynamic, Add, Edit, Delete, AddPaid, EditPaid },
    }
</script>
<style>
    .p-datatable.p-datatable-gridlines .p-datatable-header {
        background-color: #607d8b;
        color: white;
    }
</style>
