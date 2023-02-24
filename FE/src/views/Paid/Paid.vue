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
                :globalFilterFields="['customerName', 'nameProject']"
                showGridlines="true"
            >
                <template #header>
                    <h5 class="m-0 mb-2">Danh sách thu chi</h5>
                    <div class="d-flex justify-content-between align-items-center">
                        
                        <Add label="Thêm" class="itemsbutton" @click="Openmodal" />

                        <div class="d-flex">
                            <Button
                                type="button"
                                style="background-color: antiquewhite; width: 120px"
                                icon="pi pi-filter-slash"
                                class="p-button-outlined right me-2"
                                @click="reload()"
                            />
                            <InputText type="date" placeholder="-------- ----"  v-model="filterStartDate" @change="filterEventStartDate()"  class="form-control me-2" />
                            <InputText type="date" placeholder="-------- ----" v-model="filterEndDate" @change="filterEventEndDate()" class="form-control me-2" />
                            
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

                <Column field="isPaid" header="Trạng thái" style="width: 5rem; ">
                    <template #body="{ data }">
                        <div :class="data.isPaid == true ? 'badge bg-success' : 'badge bg-warning' ">
                            {{ data.isPaid == true ? 'Đã Thanh Toán' : 'Chưa Thanh Toán' }}
                        </div>
                    </template>
                </Column>
                
                <Column header="Thực thi" style="width: 15rem; text-align: left">
                    <template #body="{ data }">
                        <div class="actions-buttons">
                            <Button icon="pi pi-check" class="p-button p-component p-button-success me-2" @click="paymentConfirmation(data)" :disabled="data.isPaid" />
                            <!-- <Button icon="pi pi-eye" @click="openDetails(data.paidImages)" class="p-button p-component me-2" /> -->
                            <Button icon="pi pi-eye" @click="openModalDetails(data)" class="p-button p-component me-2" />
                            <Button icon="pi pi-pencil" class="p-button p-component p-button-warning me-2" @click="Openeditmodal(data)" :disabled="data.isPaid" />
                            <Button icon="pi pi-trash" class="p-button p-component p-button-danger" @click="Delete(data.id)" :disabled="data.isPaid" />
                        </div>
                    </template>
                </Column>
            </DataTable>
            
            <AddPaid
                :status="openStatus"
                @closemodal="Closemodal"
                :optionmodule="OptionModule"
                @reloadpage="getData"
            />

            <EditPaid
                :status="openStatusEdit"
                @closemodal="closeeditmodal"
                :dataedit="editdata"
                :optionmodule="OptionModule"
                @reloadpage="getData"
            />

            <DetailPaid
                :status="displayImage"
                @closemodal="closeDetails"
                :dataDetail="detailData"
            />

            <!-- <Dialog
                header="Hình ảnh hóa đơn"
                v-model:visible="displayImage"
                :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
                :style="{ width: '50vw' }"
                :maximizable="true"
                :modal="true"
            >
                <div v-if="paidImage.length > 0" class="content_box">
                    <Galleria :value="dataImgDetail" :responsiveOptions="responsiveOptions" :numVisible="5">
                        <template #item="slotProps">
                            <img :src="slotProps.item.itemImageSrc" :alt="slotProps.item.alt" style="width: 100%" />
                        </template>
                        <template #thumbnail="slotProps">
                            <img :src="slotProps.item.thumbnailImageSrc" :alt="slotProps.item.alt" />
                        </template>
                    </Galleria>
                </div>
                <div v-else>
                    <h3>Không có hình ảnh để hiển thị</h3>
                </div>
                <template #footer>
                    <button class="btn btn-secondary" @click="closeDetails()">Trở Về</button>
                </template>
            </Dialog> -->

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
    
    export default {
        data() {
            return {
                openStatus: false,
                openStatusEdit: false,
                editdata: null,
                detailData: null,
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
                filterStartDate: "",
                filterEndDate: "",
                token: null,
                displayImage: false,
                paidImage: [],
                dataImgDetail: [],
                responsiveOptions: [
                    {
                        breakpoint: '1024px',
                        numVisible: 5
                    },
                    {
                        breakpoint: '768px',
                        numVisible: 3
                    },
                    {
                        breakpoint: '560px',
                        numVisible: 1
                    }
                ]
            }
        },

        async mounted() {
            this.token = LocalStorage.jwtDecodeToken()
            await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
            await this.getData();
        },

        methods: {
            Openmodal() {
                this.openStatus = true;
            },

            Closemodal() {
                this.openStatus = false;
            },

            Openeditmodal(id) {
                this.openStatusEdit = true;
                this.editdata = id
            },

            closeeditmodal() {
                this.openStatusEdit = false;              
            },
            
            /* openDetails(images) {
                this.displayImage = true
                this.paidImage = images ?? [];
                this.dataImgDetail = [];

                if(this.paidImage!=null){
                    this.paidImage.forEach(item => {
                        var imgObj = {
                            "itemImageSrc": item.imagePath,
                            "thumbnailImageSrc": item.imagePath,
                            "alt": "Image "+item.imageId,
                        }
                        this.dataImgDetail.push(imgObj);
                    });
                }
            }, */

            openModalDetails(data){
                this.displayImage = true;
                this.detailData = data;
            },

            closeDetails() {
                this.displayImage = false;
            },

            resetFieldDate () {
                this.filterStartDate = "";
                this.filterEndDate = "";
            },

            async reload() {
                this.resetFieldDate();
                await this.getData();
            },
            
            formatDate(date) {
                var dateFormat = new Date(date)
                return dayjs(dateFormat).format('DD/MM/YYYY')
            },

            getProjects(id) {
                return HTTP_LOCAL.get(GET_PROJECT_BY_ID(id)).then((respone) => respone.data)
            },

            getUsers(id) {
                return HTTP_LOCAL.get(GET_USER_BY_ID(id)).then((respone) => respone.data)
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

            async getData() {
                try {
                    this.paids = [];
                    if (UserRoleHelper.isAccess) {
                        // getAPI (Sample) || (Admin)
                        if (Number(this.token.IdGroup) === 2 || Number(this.token.IdGroup) === 1) {
                            await this.getPaid()
                        }
                        // getAPI tất cả role còn lại
                        if (Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1) {
                            await this.getPaidByIdUser(this.token.Id)
                        }
                        await this.getAllProject()                        
                    }else{
                        alert("Bạn không có quyền truy cập module này")
                        router.push('/')
                    }
                }
                catch(err)
                {
                    router.push('/')
                    console.log(err);
                }               
            },

            async DeletePaid(id) {
                await HTTP.delete(`Paid/${id}`).then(async (res) => {                    
                    await this.getData();
                    this.showSuccess('Xóa thành công!');
                })
            },

            Delete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa? ' + id,
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: async () => {
                        await this.DeletePaid(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },

            async getAllProject() {
                HTTP.get('Project/getAllProject')
                    .then((res) => {
                        this.OptionModule = res.data
                    })
                    .catch((error) => {
                        this.showError(error.response.data);
                        console.log(error)
                    })
            },

            async getPaidByIdUser(iduser) {
                try {
                    await HTTP.get(`Paid/GetByUserId?id=${iduser}`)
                        .then((res) => {
                            res.data._Data.forEach((ele) => {
                                this.paids = res.data._Data;
                            })
                        })
                        .catch((err) => {
                            this.showError(error.response.data);
                            console.log(err)
                        })
                    await this.getWithName()
                    this.loading = false
                } catch (err) {
                    console.log('something went wrong:' + err )
                }
            },

            async getPaid() {
                this.loading = true;
                this.paids = [];
                await HTTP.get(GET_LIST_PAID)
                    .then((respone) => {
                        this.paids = respone.data._Data;    
                    })
                    .catch((error) => {
                        this.showError(error.response.data);
                        console.log(error)
                    })
                await this.getWithName()
                this.loading = false
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

            async callApiSearch(objSearch){
                this.loading = true;
                this.paids = [];
                await HTTP_LOCAL.post("Paid/SearchPaidByDay", objSearch).then(async (respone) => {
                    this.paids = respone.data._Data;
                    if(this.paids.length == 0){
                        this.showInfo("Không tìm thấy dữ liệu!");
                    }
                })
                .catch((error) => {
                    this.showError(error.response.data._Message);
                    console.log(error)
                })
                .finally(() => {
                    this.loading = false;
                });
            },

            checkValidateDay () {
                if (this.filterStartDate > this.filterEndDate) {
                    this.showInfo("Ngày kết thúc phải lớn hơn ngày bắt đầu!");
                    return false;
                } 
                return true;
            },

            checkStartDateEmpty () {
                if(this.filterStartDate != "" ){
                    return true;
                }
                return false;
            },

            checkEndDateEmpty () {
                if(this.filterEndDate != "" ){
                    return true;
                }
                return false;
            },

            async callApiFilterDay(startDate = null, endDate = null) {
                // getAPI (Sample) || (Admin)
                if (Number(this.token.IdGroup) === 2 || Number(this.token.IdGroup) === 1) {
                    await this.callApiSearch({
                        "startDate": startDate, 
                        "endDate": endDate,
                    });
                }
                // getAPI tất cả role còn lại
                if (Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1) {
                    await this.callApiSearch({
                        "id": this.token.Id,
                        "startDate": startDate, 
                        "endDate": endDate,
                    });
                }
            },

            async filterEventStartDate(){
                if(this.checkStartDateEmpty() && this.checkEndDateEmpty() == false){
                    await this.callApiFilterDay(this.filterStartDate, null);
                    return ;
                }
                if(this.checkStartDateEmpty() && this.checkEndDateEmpty() ){
                    if(this.checkValidateDay()){
                        await this.callApiFilterDay(this.filterStartDate, this.filterEndDate);
                    }
                    else {
                        this.resetFieldDate();
                    }
                }
            },

            async filterEventEndDate(){
                if(this.checkStartDateEmpty() == false){
                    this.showInfo("Nhập ngày bắt đầu trước!");
                    this.resetFieldDate();
                    return ;
                }
                if(this.checkStartDateEmpty() && this.checkEndDateEmpty() ){
                    if(this.checkValidateDay()){
                        await this.callApiFilterDay(this.filterStartDate, this.filterEndDate);
                    }
                    else {
                        this.resetFieldDate();
                    }
                }
            },           

            async CallApiPaymentConfirm (idPaid) {
                const res = await HTTP_LOCAL.put(`Paid/acceptPayment/${idPaid}`, {"PaidPerson": this.token.Id}).then(async (res) => {
                    if(res.status == 200){
                        await this.getPaid()  ;                      
                        this.showSuccess('Thanh toán thành công!');
                    }
                    else {
                        this.showError('Lỗi! cập nhật thanh toán!')
                    }
                })
                .catch((error) => {
                    this.showError(error.response.data);
                    console.log(error);
                });
            },

            paymentConfirmation(item) {
                this.$confirm.require({
                    message: `Bạn có chắc muốn thanh toán cho khách hàng "${item.customerName}"?`,
                    header: 'Xác nhận thanh toán',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-info',
                    accept: () => {
                        this.CallApiPaymentConfirm(item.id);
                    },
                    reject: () => {
                        this.showInfo('Bạn đã từ chối');
                    },
                });
            },
           
        },

        components: { LayoutDefaultDynamic, Add, Edit, Delete, AddPaid, EditPaid, DetailPaid },
    }
</script>
<style lang="scss">
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

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>
