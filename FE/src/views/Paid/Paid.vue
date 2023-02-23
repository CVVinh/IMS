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
                    <Column field="image" header="Hình ảnh">
                        <template #body="{ data }">
                            <Button
                                class="p-button-raised p-button-info p-button-text"
                                @click="openDetails(data.paidImages)"
                                label="Chi Tiết"
                            />
                        </template>
                    </Column>
                    <Column header="Thực thi">
                        <template #body="{ data }">
                            <!--
                            <button class="btn btn-warning me-2" @click="Openeditmodal(data.id)" :disabled=data.isPaid>Edit</button>
                            <button class="btn btn-danger" @click="Delete(data.id)">Delete</button>
                            -->
                            <Edit @click="Openeditmodal(data)" :disabled="data.isPaid" class="me-2" />

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
                    <Column field="image" header="Hình ảnh">
                        <template #body="{ data }">
                            <Button
                                label="Chi tiết"
                                class="p-button-raised p-button-info p-button-text"
                                @click="openDetails(data.paidImages)"
                            />
                        </template>
                    </Column>
                    <Column header="Action">
                        <template #body="{ data }">
                            <Edit @click="Openeditmodal(data.id)" :disabled="data.isPaid" class="me-2" />

                            <Delete @click="Delete(data.id)" />
                        </template>
                    </Column>
                </DataTable>
            </div>

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

            <Dialog
                header="Hình ảnh hóa đơn"
                v-model:visible="displayImage"
                :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
                :style="{ width: '50vw' }"
                :maximizable="true"
                :modal="true"
            >
                <div v-if="paidImage.length > 0">
                    <!-- <div v-for="(item, index) in paidImage" :key="index" class="mb-2">
                        <Image v-bind:src="item.imagePath" alt="Image" width="100%" preview />
                    </div> -->
                    
                    <Galleria :value="dataImgDetail" >
                        <template #item="slotProps">
                            <img :src="slotProps.item.itemImageSrc" :alt="slotProps.item.alt" />
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
            </Dialog>

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
    import router from '@/router'
    
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
            await this.getData();
        },

        methods: {
            async getData() {
                try {
                    this.token = LocalStorage.jwtDecodeToken()
                    await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                    this.paids = [];
                    if (UserRoleHelper.isAccess) {
                        // getAPI (Sample) || (Admin)
                        if (Number(this.token.IdGroup) === 2 || Number(this.token.IdGroup) === 1) {
                            this.getPaid()
                        }
                        // getAPI tất cả role còn lại
                        if (Number(this.token.IdGroup) !== 2 && Number(this.token.IdGroup) !== 1) {
                            this.getPaidByIdUser(this.token.Id)
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
                HTTP.delete(`Paid/${id}`).then((res) => {                    
                    this.getData();
                    this.showSuccess('Xóa thành công!');
                })
            },

            Delete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa? ' + id,
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
                HTTP.get('Project/getAllProject')
                    .then((res) => {
                        this.OptionModule = res.data
                    })
                    .catch((error) => {
                        console.log(error)
                    })
            },

            async getPaidByIdUser(iduser) {
                try {
                    await HTTP.get(`Paid/GetByUserId?id=${iduser}`)
                        .then((res) => {
                            res.data._Data.forEach((ele) => {
                                this.paids = res.data._Data;
                                console.log("this.paids user: "+ JSON.stringify(this.paids))
                            })
                        })
                        .catch((err) => console.log(err))
                    await this.getWithName()
                    this.loading = false
                } catch (err) {
                    console.log('something went wrong')
                }
            },

            async getPaid() {
                this.loading = true
                await HTTP.get(GET_LIST_PAID)
                    .then((respone) => {
                        this.paids = respone.data._Data;    
                        console.log("this.paids: "+ JSON.stringify(this.paids))                   
                    })
                    .catch((error) => {
                        this.showError(error.response.data);
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
                    this.showSuccess("Lọc thành công");
                    return this.filterArr
                } 
                else {
                    return this.showInfo("Không tìm thấy dữ liệu");
                }
            },

            reload() {
                this.paids = []
                this.filterArr = []
                this.getPaid()
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

            dateToYMD(end_date) {
                var ed = new Date(end_date)
                var d = ed.getDate()
                var m = ed.getMonth() + 1
                var y = ed.getFullYear()
                return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d)
            },

            openDetails(images) {
                this.displayImage = true
                this.paidImage = images ?? [];

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
            },

            closeDetails() {
                this.displayImage = false
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
    .p-dialog-content img {
        width: 100%;
    }
</style>
