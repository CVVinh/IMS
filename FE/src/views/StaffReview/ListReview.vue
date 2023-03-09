<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-4 mt-3">
            <!-- <Card class="border-1"> -->
            <DataTable
                :value="reviewList"
                :paginator="true"
                :rows="5"
                :rowHover="true"
                :loading="loading"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[5, 10, 20, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                responsiveLayout="scroll"
                showGridlines
            >
                <template #header>
                    <div class="card card-body" style="background-color: #607d8b">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <h5 style="color: White">Danh Sách Nhân Viên Đã Review</h5>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-md-6 d-flex">
                                <div class="ms-2">
                                    <Button
                                        class="p-button-sm p-button-info me-2"
                                        icon="pi pi-plus"
                                        style="height: 100%"
                                        @click="AddOrEditStaffReview()"
                                        label="Thêm thẻ review nhân viên"
                                    />
                                </div>
                            </div>
                            <div class="d-flex align-items-center justify-content-end col-md-6">
                                <div class="d-flex justify-content-start">
                                    <div class="input-text">
                                        <InputText
                                            style="width: 100%"
                                            type="text"
                                            v-model="name"
                                            placeholder="Nhập tên nhân viên..."
                                        />
                                    </div>
                                    <Button
                                        type="button"
                                        style="background-color: antiquewhite"
                                        icon="pi pi-filter-slash"
                                        class="p-button p-button-outlined ms-2"
                                        @click="handlerReload()"
                                    />
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy dữ liệu. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>
                <Column field="#" header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="userCode" header="Mã nhân viên">
                    <template #body="{ data }">
                        {{ data.userCode }}
                    </template>
                </Column>
                <Column field="staffReview" header="Tên nhân viên">
                    <template #body="{ data }">
                        {{ data.staffReview }}
                    </template>
                </Column>
                <Column field="totalPerformance" header="Hiệu suất lần cuối">
                    <template #body="{ data }"> {{ data.totalPerformance }}% </template>
                </Column>
                <Column field="createdDate" header="Cập nhật lần cuối">
                    <template #body="{ data }"> {{ this.formatDate(data.createdDate) }} </template>
                </Column>
                <Column field="isConfirm" header="Trạng thái">
                    <template #body="{ data }">
                        <div :class="data.isConfirm ? 'badge bg-success' : 'badge bg-warning'">
                            {{ data.isConfirm ? 'Đã xác nhận' : 'Chưa xác nhận' }}
                        </div>
                    </template>
                </Column>
                <Column header="Tác vụ">
                    <template #body>
                        <Button icon="pi pi-eye" class="p-button p-component me-2" />
                        <Button icon="pi pi-pencil" class="p-button p-component p-button-warning me-2" />
                        <Button icon="pi pi-trash" class="p-button p-component p-button-danger me-2" />
                        <Button icon="pi pi-check" class="p-button p-component p-button-success me-2" />
                        <Button icon="pi pi-times" class="p-button p-component p-button-danger" />
                    </template>
                </Column>
            </DataTable>
            <!-- </Card> -->
        </div>
        <Dialog
            v-model:visible="addOrEditDialog"
            :maximizable="true"
            modal
            header="Phiếu đánh giá nhân viên"
            :style="{ width: '50vw' }"
        >
            <p>
                Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et
                dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip
                ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu
                fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia
                deserunt mollit anim id est laborum.
            </p>
        </Dialog>
        <Toast />
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP, HTTP_LOCAL, GET_USER_NAME_BY_ID } from '@/http-common'
    import dayjs from 'dayjs'
    export default {
        data() {
            return {
                reviewList: [],
                loading: false,
                addOrEditDialog: false,
            }
        },
        created() {
            this.getAllReviews()
        },
        methods: {
            formatDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            async getAllReviews() {
                this.loading = true
                await HTTP_LOCAL.get('StaffReview')
                    .then((res) => {
                        this.reviewList = res.data._Data
                        console.log(this.reviewList)
                    })
                    .catch((err) => {
                        console.log(err)
                    })
                this.handleIdOfUser()
                this.loading = false
            },
            async handleIdOfUser() {
                for (let i = 0; i < this.reviewList.length; i++) {
                    if (this.reviewList[i].staffReview !== 0) {
                        var name = await this.getUserName(this.reviewList[i].staffReview)
                        this.reviewList[i].staffReview = name.fullName
                        this.reviewList[i].userCode = name.userCode
                    } else {
                        this.reviewList[i].staffReview = 'Không có dữ liệu...'
                    }
                }
            },
            async getUserName(id) {
                return HTTP.get(GET_USER_NAME_BY_ID(id)).then((res) => res.data)
            },
            AddOrEditStaffReview() {
                this.addOrEditDialog = true
            },
        },
    }
</script>
<style>
    .p-card-header {
        padding: 10px;
    }
    .p-card-body {
        padding: 10px !important;
    }
    .p-card .p-card-content {
        padding: 0px 0px 1.25rem 0px;
    }
    .v3ti {
        height: 100%;
    }
    .v3ti > .v3ti-content > input {
        width: 100%;
    }
    .v3ti:focus-visible {
        border: none;
        box-shadow: none;
    }
    .v3ti:focus {
        border: none;
        box-shadow: none;
    }
</style>
