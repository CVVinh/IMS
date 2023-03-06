<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4 mt-3">
            <Card class="border-1">
                <template #header class="custom_header">
                    <div class="card card-body" style="background-color: #607d8b">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <h5 style="color: White">Đăng ký nghỉ phép</h5>
                            </div>
                        </div>
                        <div class="col-md-12">
                            <Button
                                class="p-button-sm mt-1 me-2 p-button-info"
                                @click="handlerAddLeaveOff"
                                icon="pi pi-plus"
                                label="Thêm nghỉ phép"
                            />
                        </div>
                    </div>
                </template>
                <template #content>
                    <DataTable
                        :value="listLeaveOffbyUser"
                        :rows="5"
                        ref="dt"
                        :paginator="true"
                        :loading="loading"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                        :rowsPerPageOptions="[5, 10, 15, 30]"
                        currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                        :globalFilterFields="[
                            '#',
                            'id',
                            'createTime',
                            'startTime',
                            'endTime',
                            'reasonNotAccept',
                            'reasons',
                            'status',
                        ]"
                    >
                        <Column field="#" header="No.">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày bắt đầu">
                            <template #body="{ data }">
                                {{ formartDate(data.startTime) }}
                            </template>
                        </Column>
                        <Column field="endTime" header="Ngày kết thúc ">
                            <template #body="{ data }">
                                {{ formartDate(data.endTime) }}
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                        <Column header="Thực thi" style="width: 13%">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-start">
                                    <Button
                                        class="p-button-sm p-button-info mt-1 me-2"
                                        @click="handlerDetailsLeaveOff(data)"
                                        icon="pi pi-eye"
                                    />
                                    <Button
                                        v-if="data.status == 1 || data.status == 2"
                                        class="p-button-sm mt-1 me-2 p-button-warning"
                                        @click="handlerEditLeaveOff(data)"
                                        icon="pi pi-pencil"
                                        :disabled="data.status == 2"
                                    />
                                    <Button
                                        v-if="data.status == 1 || data.status == 2"
                                        class="p-button-sm p-button-danger mt-1 me-2"
                                        @click="confirmDeleteLeaveOff(data)"
                                        icon="pi pi-trash"
                                        :disabled="data.status == 2"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </template>
            </Card>
        </div>
        <DialogAddLeaveOff
            :isOpen="this.isOpenDialog"
            :selectedLeaveOff="{ ...leaveOffSelected }"
            @getAllLeaveOff="getAllLeaveOffRegister"
            @closeDialog="closeDialogLeaveoff()"
            @setChange="setChange()"
        />
        <DialogViewDetailLeaveOff
            :isOpen="this.isOpenDialogDetailLeaveoff"
            :selectedLeaveOff="{ ...leaveOffDetail }"
            @closeDialog="closeDialogDetailLeaveoff()"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP, DELETE_LEAVE_OFF, GET_LIST_LEAVEOFF_BY_USER } from '@/http-common'
    import dayjs from 'dayjs'
    import DialogAddLeaveOff from './DialogAddLeaveOff.vue'
    import jwtDecode from 'jwt-decode'
    import { LeaveOffDto } from '@/views/LeaveOff/LeaveOff.dto'
    import { HttpStatus } from '@/config/app.config'
    import DialogViewDetailLeaveOff from './DialogViewDetailLeaveOff.vue'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '../../router'
    export default {
        async created() {
            ///leaveoff/Registerlists

            try {
                this.token = LocalStorage.jwtDecodeToken()
                let indexCut = this.$route.path.lastIndexOf('/')
                let string = this.$route.path.slice(1, indexCut)
                await UserRoleHelper.isAccessModule(string)
                if (UserRoleHelper.isAccess) {
                    // Check quyền
                    if (
                        Number(this.token.IdGroup) == 4 ||
                        Number(this.token.IdGroup) == 3 ||
                        Number(this.token.IdGroup) == 1 ||
                        Number(this.token.IdGroup) == 2
                    ) {
                        await this.getAllLeaveOffRegister()
                    }
                    if (Number(this.token.IdGroup) == 5) {
                        setTimeout(() => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Người dùng không có quyền!',
                                life: 3000,
                            })
                            this.$router.push('/')
                        }, 800)
                    }
                } else {
                    alert('bạn không có quyền giờ đến trang HOME nhé')
                    router.push('/')
                }
            } catch (err) {
                alert('Ooopps Có gì đó sai sai rồi chuyển bạn đến trang home nhé')
                router.push('/')
            }
        },
        data() {
            return {
                isOpenDialog: false,
                isOpenDialogDetailLeaveoff: false,
                isChange: false,
                loading: true,
                listLeaveOffbyUser: [],
                userLeave: jwtDecode(localStorage.getItem('token')),
                leaveOffSelected: new LeaveOffDto(),
                leaveOffDetail: new LeaveOffDto(),
                statusLeave: [
                    {
                        id: 1,
                        title: 'Chờ duyệt',
                        class: 'badge bg-warning',
                    },
                    {
                        id: 2,
                        title: 'Đã duyệt',
                        class: 'badge bg-success',
                    },
                    {
                        id: 3,
                        title: 'Đã hủy',
                        class: 'badge bg-secondary ',
                    },
                ],
            }
        },
        methods: {
            async setChange() {
                this.isChange = true
            },
            handlerAddLeaveOff() {
                this.isOpenDialog = true
                this.leaveOffSelected = []
            },
            handlerEditLeaveOff(data) {
                this.leaveOffSelected = { ...data }
                this.isOpenDialog = true
            },
            async closeDialogLeaveoff() {
                this.isOpenDialog = false
                this.leaveOffSelected = []
                if (this.isChange === true) {
                    this.listLeaveOffbyUser = []
                    await this.getAllLeaveOffRegister()
                    this.isChange = false
                }
            },
            checkStatus(id) {
                var fillter = this.statusLeave.filter(function (el) {
                    return el.id == id
                })
                return Object.assign({}, fillter[0])
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            async getAllLeaveOffRegister() {
                var userId = this.userLeave.Id
                await HTTP.get(GET_LIST_LEAVEOFF_BY_USER(userId)).then((res) => {
                    this.listLeaveOffbyUser = res.data._Data
                })
                this.loading = false
            },
            confirmDeleteLeaveOff(data) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.handlerDeleteLeaveOff(data)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            handlerDeleteLeaveOff(data) {
                if (data) {
                    HTTP.delete(DELETE_LEAVE_OFF(data.id))
                        .then((res) => {
                            if (res.status === HttpStatus.OK) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Thành công',
                                    detail: res.data._Message,
                                    life: 3000,
                                })
                                this.getAllLeaveOffRegister()
                            } else {
                                this.$toast.add({
                                    severity: 'warn',
                                    summary: 'Cảnh báo',
                                    detail: res.data._Message,
                                    life: 3000,
                                })
                            }
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: err.message,
                                life: 3000,
                            })
                        })
                }
            },
            handlerDetailsLeaveOff(data) {
                this.isOpenDialogDetailLeaveoff = true
                this.leaveOffDetail = { ...data }
            },
            closeDialogDetailLeaveoff() {
                this.isOpenDialogDetailLeaveoff = false
                this.leaveOffDetail = []
            },
        },
        components: {
            DialogAddLeaveOff,
            DialogViewDetailLeaveOff,
        },
    }
</script>
<style>
    .p-card-header {
        padding: 20px 20px 0px 20px !important;
    }

    .p-card .p-card-body {
        padding: 0px 1.25rem 1.25rem 1.25rem !important;
    }
</style>
