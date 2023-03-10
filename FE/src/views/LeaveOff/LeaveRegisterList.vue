<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <!-- <div class="mx-4 mb-3"></div> -->
        <div class="mx-4 mt-3">
            <Card class="border-1">
                <template #header>
                    <div class="card card-body" style="background-color: #607d8b">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <h5 style="color: White">Danh sách đăng ký nghỉ phép</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <Button
                                    style="height: 50px"
                                    label="Tổng hợp nghỉ phép"
                                    class="p-button-outlined p-button-sm p-button-info py-2 border"
                                    @click="RedirectToAction"
                                    icon="pi pi-arrow-right"
                                />
                            </div>
                            <div class="d-flex justify-content-end">
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined right me-2"
                                    @click="handlerReload()"
                                />
                                <div class="input-text">
                                    <MultiSelect
                                        style="width: 200px"
                                        v-model="fillterLeaveOff.selectedLeaveOff"
                                        :options="statusLeave"
                                        optionLabel="title"
                                        optionValue="id"
                                        placeholder="Chọn trạng thái"
                                        display="chip"
                                    />
                                </div>
                                <div class="input-text">
                                    <InputText
                                        style="width: 200px"
                                        type="month"
                                        v-model="fillterLeaveOff.selectedDate"
                                        placeholder=" Sort by month"
                                    />
                                </div>
                                <div class="input-text">
                                    <div class="d-flex">
                                        <InputText
                                            style="width: 200px"
                                            class="p-inputtext"
                                            v-model="fillterLeaveOff.searchLeaveOff"
                                            placeholder="Nhập tên..."
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>
                <template #content>
                    <DataTable
                        :value="dataLeaveOff"
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
                            'startTime',
                            'reasons',
                            'notAcceptUser',
                            'reasonAccept',
                            'idLeaveUser',
                            'endTime',
                            'user',
                            'name',
                        ]"
                    >
                        <Column field="#" header="No">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column field="name" header="Tên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày nghỉ">
                            <template #body="{ data }">
                                {{ formartDate(data.startTime) + ' - ' + formartDate(data.endTime) }}
                            </template>
                        </Column>
                        <Column field="realTime" header="Thời gian thực tế">
                            <template #body="{ data }">
                                {{ data.realTime }}
                            </template>
                        </Column>
                        <Column field="reasons" header="Lý do">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="reasonAccept" header="Lý do duyệt nghỉ phép">
                            <template #body="{ data }">
                                <span v-if="data.status == 2">
                                    {{ data.reasonAccept }}
                                </span>
                                <span v-else style="color: burlywood"> Chưa nhập lý do... </span>
                            </template>
                        </Column>
                        <Column field="notAcceptUser" header="Lý do không cho phép nghỉ">
                            <template #body="{ data }">
                                <span v-if="data.status == 3">
                                    {{ data.notAcceptUser }}
                                </span>
                                <span v-else style="color: burlywood"> Chưa nhập lý do... </span>
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                        <Column header="Action" v-if="Action">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-center">
                                    <Button
                                        @click="showConfirmLeaveOffReasonAccept(data)"
                                        class="p-button-sm mt-1 me-2 p-button-success"
                                        icon="pi pi-check"
                                        :disabled="data.status == 2 || data.status == 3"
                                        :class="{
                                            'p-button-success': data.status != 2,
                                            'p-button-secondary': data.status == 2,
                                        }"
                                        v-if="this.showButton.confirm"
                                    />
                                    <Button
                                        @click="showConfirmLeaveOff(data)"
                                        class="p-button-sm p-button-danger mt-1 me-2"
                                        icon="pi pi-times"
                                        :disabled="data.status == 3 || data.status == 2"
                                        v-if="this.showButton.refuse"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </template>
            </Card>
        </div>
        <DialogConfirmLeave
            :idLeaveOff="idLeaveoff"
            :isOpen="this.isOpenDialog"
            @closeDialog="closeDialogLeaveoff()"
            @setChange="setChange()"
        />
        <DialogReasonAcceptLeveOff
            :idLeaveOff="idLeaveOffReasonAccept"
            :isOpen="this.isOpenDialogReasonAccept"
            @closeDialog="closeDialogLeaveoffReasonAccept()"
            @setChange="setChangeReasonAccept()"
        />
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP, ENDPIONTS, GET_USER_NAME_BY_ID, ACCEPT_LEAVE_OFF } from '@/http-common'
    import dayjs from 'dayjs'
    import DialogConfirmLeave from './DialogConfirmLeave.vue'
    import DialogReasonAcceptLeveOff from './DialogReasonAcceptLeveoff.vue'
    import jwtDecode from 'jwt-decode'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '@/router'
    import checkAccessModule from '@/stores/checkAccessModule'
    export default {
        async created() {
            ///leaveoff/acceptregisterlists
            if(checkAccessModule.checkAccessModule(this.$route.path.replace('/', '').slice(0,9)) === true){
                checkAccessModule.checkShowButton(this.$route.path.replace('/', '').slice(0,9),this.showButton);
                this.checkPermissionGroup() ;
            }
            else{
                this.countTime()
                 this.displayDialog1 = true
            }

            // try {
            //     this.token = LocalStorage.jwtDecodeToken()
            //     let indexCut = this.$route.path.lastIndexOf('/')
            //     let string = this.$route.path.slice(1, indexCut)
            //     await UserRoleHelper.isAccessModule(string)
            //     if (UserRoleHelper.isAccess) {
            //         // Phân quyền button
            //         if (Number(this.token.IdGroup) == 5 || Number(this.token.IdGroup) == 1) {
            //             this.Action = true
            //         }
            //         // Check quyền
            //         if (Number(this.token.IdGroup) == 5 || Number(this.token.IdGroup) == 1) {
            //             await this.getAllLeaveOffRegister()
            //         }
            //         if (
            //             Number(this.token.IdGroup) == 4 ||
            //             Number(this.token.IdGroup) == 3 ||
            //             Number(this.token.IdGroup) == 2
            //         ) {
            //             setTimeout(() => {
            //                 this.$toast.add({
            //                     severity: 'error',
            //                     summary: 'Lỗi',
            //                     detail: 'Người dùng không có quyền!',
            //                     life: 3000,
            //                 })
            //                 this.$router.push('/')
            //             }, 800)
            //         }
            //     } else {
            //         alert('bạn không có quyền giờ đến trang HOME nhé')
            //         router.push('/')
            //     }
            // } catch (err) {
            //     alert('Ooopps Có gì đó sai sai rồi chuyển bạn đến trang home nhé')
            //     router.push('/')
            // }
        },
        data() {
            return {
                isOpenDialog: false,
                isChange: false,
                idLeaveoff: null,
                isSearchAdvanced: false,
                fillterLeaveOff: {
                    selectedLeaveOff: [],
                    selectedDate: null,
                    searchLeaveOff: '',
                },
                dataLeaveOff: [],
                Action: false,
                statusLeave: [
                    {
                        id: 1,
                        title: 'Đang chờ',
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
                showButton: {
                    add: false,
                    update: false,
                    delete: false,
                    deleteMulti: false,
                    confirm: false,
                    confirmMulti: false,
                    refuse: false,
                    addMember: false,
                    export: false,
                },
                loading: true,
                idLeaveOffReasonAccept: null,
                isOpenDialogReasonAccept: false,
                isChangeReasonAccept: false,
            }
        },
        watch: {
            fillterLeaveOff: {
                handler: function change(newVal) {
                    this.handlerFillterLeaveOff()
                },
                deep: true,
            },
        },
        mounted() {
            var date1 = '2023-02-21 08:00:00'
            var date2 = ' 2023-02-23 10:00:00'
            this.mathLeaveOffDate(date1, date2)
        },
        methods: {
            async checkPermissionGroup() {
                if(checkAccessModule.getListGroup().includes("1") || checkAccessModule.getListGroup().includes("5")) {
                    this.Action = true
                    await this.getAllLeaveOffRegister()
                }
                else {
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
            },
            
            checkStatus(id) {
                var fillter = this.statusLeave.filter(function (el) {
                    return el.id == id
                })
                return Object.assign({}, fillter[0])
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD (HH:mm)')
            },
            mathLeaveOffDate(startTime, endTime, idCompanyBranh) {
                const start = new Date(startTime)
                const end = new Date(endTime)

                // Kiểm tra xem thời gian bắt đầu và kết thúc có cùng ngày không
                const isSameDay =
                    start.getFullYear() === end.getFullYear() &&
                    start.getMonth() === end.getMonth() &&
                    start.getDate() === end.getDate()

                let diff = end - start

                // Trừ thời gian nghỉ trưa nếu trong giờ làm việc
                if (start.getHours() <= 12 && end.getHours() >= 13 && isSameDay) {
                    let lunchTime
                    if (idCompanyBranh == 1) {
                        lunchTime = 90
                    }
                    if (idCompanyBranh == 2) {
                        lunchTime = 60
                    }
                    diff -= lunchTime * 60 * 1000
                }

                const millisecondsPerDay = 24 * 60 * 60 * 1000
                let count = 0
                let currentDate = new Date(start)
                let countT7CN = 0
                while (currentDate <= end) {
                    if (currentDate.getDay() !== 0 && currentDate.getDay() !== 6) {
                        // Bỏ qua ngày thứ 7 và chủ nhật
                        let workHours = 8.5 // Số giờ làm việc trong ngày
                        if (currentDate.getHours() >= 12 && currentDate.getHours() < 13) {
                            workHours -= 1 // Nếu là giờ nghỉ trưa, trừ đi 1 giờ
                        } else if (currentDate.getHours() >= 13 && currentDate.getHours() < 17) {
                            workHours -= 0.5 // Nếu là giờ chiều, trừ đi 0.5 giờ
                        }
                        count += workHours // Cộng số giờ làm việc của ngày đó vào tổng số giờ làm việc
                    } else {
                        countT7CN += 1
                    }
                    currentDate.setDate(currentDate.getDate() + 1)
                }

                const totalWorkingHours = count

                let days = Math.floor(diff / millisecondsPerDay)
                diff -= days * millisecondsPerDay
                days -= countT7CN

                const hours = Math.floor(diff / (1000 * 60 * 60))
                diff -= hours * (1000 * 60 * 60)

                const minutes = Math.floor(diff / (1000 * 60))

                // Định dạng chuỗi kết quả
                let result = ''
                if (days > 0) {
                    result += days + 'd '
                }
                if (hours > 0) {
                    result += hours + 'h '
                }
                if (minutes > 0) {
                    result += minutes + 'm'
                }

                return result.trim()
            },
            async handlerBrowseVacation(item) {
                var idAcceptUser = checkAccessModule.getUserIdCurrent();
                await HTTP.put(ACCEPT_LEAVE_OFF(item.id), { idAcceptUser: idAcceptUser })
                    .then((res) => {
                        if (res.status == 200) {
                            this.dataLeaveOff = []
                            this.loading = true
                            this.getAllLeaveOffRegister()
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
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
                            life: 2000,
                        })
                    })
            },
            confirmBrowseVacation(item) {
                this.$confirm.require({
                    message: `Bạn có chắc muốn cho "${item.name}" nghỉ phép ?`,
                    header: 'Xác nhận nghỉ phép',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-info',
                    accept: () => {
                        this.handlerBrowseVacation(item)
                    },
                    reject: () => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Từ chối',
                            detail: 'Bạn đã từ chối',
                            life: 3000,
                        })
                    },
                })
            },
            async setChange() {
                this.isChange = true
            },
            showConfirmLeaveOff(data) {
                this.idLeaveoff = data.id
                this.isOpenDialog = true
            },
            async closeDialogLeaveoff() {
                this.isOpenDialog = false
                if (this.isChange === true) {
                    this.dataLeaveOff = []
                    await this.getAllLeaveOffRegister()
                    this.isChange = false
                }
            },
            async setChangeReasonAccept() {
                this.isChangeReasonAccept = true
            },
            showConfirmLeaveOffReasonAccept(data) {
                this.idLeaveOffReasonAccept = data.id
                this.isOpenDialogReasonAccept = true
            },
            async closeDialogLeaveoffReasonAccept() {
                this.isOpenDialogReasonAccept = false
                if (this.isChangeReasonAccept === true) {
                    this.dataLeaveOff = []
                    await this.getAllLeaveOffRegister()
                    this.isChangeReasonAccept = false
                }
            },
            async handlerLoadData() {
                for (let i = 0; i < this.dataLeaveOff.length; i++) {
                    const user = await this.getUserByIdLeaveOff(this.dataLeaveOff[i].idLeaveUser)
                    this.dataLeaveOff[i].user = user
                    this.dataLeaveOff[i].name = `${user.firstName} ${user.lastName}`
                }
            },
            getUserByIdLeaveOff(id) {
                return HTTP.get(GET_USER_NAME_BY_ID(id)).then((res) => res.data)
            },
            async getAllLeaveOffRegister() {
                await HTTP.get(ENDPIONTS.LEAVEOFF_REGISTER_LIST).then((res) => {
                    res.data._Data.forEach((el) => {
                        this.dataLeaveOff.push({
                            id: el.id,
                            startTime: el.startTime,
                            endTime: el.endTime,
                            acceptTime: el.acceptTime,
                            createTime: el.createTime,
                            idAcceptUser: el.idAcceptUser,
                            idLeaveUser: el.idLeaveUser,
                            reasons: el.reasons,
                            status: el.status,
                            notAcceptUser: el.reasonNotAccept,
                            user: null,
                            realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh),
                            idCompanyBranh: el.idCompanyBranh,
                            reasonAccept: el.reasonAccept,
                        })
                    })
                })
                await this.handlerLoadData()
                this.loading = false
            },
            async handlerFillterLeaveOff() {
                if (
                    this.fillterLeaveOff.searchLeaveOff != '' ||
                    this.fillterLeaveOff.selectedDate != null ||
                    this.fillterLeaveOff.selectedLeaveOff.length > 0
                ) {
                    this.loading = true
                    this.dataLeaveOff = []
                    var findByNameStatusDateDtos = {
                        fullName: this.fillterLeaveOff.searchLeaveOff,
                        date: DateHelper.convertUTC(this.fillterLeaveOff.selectedDate),
                        status: this.fillterLeaveOff.selectedLeaveOff,
                    }

                    await HTTP.post(ENDPIONTS.SEARCH_REGISTER_LIST, findByNameStatusDateDtos)
                        .then((res) => {
                            this.dataLeaveOff = []
                            res.data._Data.forEach((el) => {
                                this.dataLeaveOff.push({
                                    id: el.id,
                                    startTime: el.startTime,
                                    endTime: el.endTime,
                                    acceptTime: el.acceptTime,
                                    createTime: el.createTime,
                                    idAcceptUser: el.idAcceptUser,
                                    idLeaveUser: el.idLeaveUser,
                                    reasons: el.reasons,
                                    status: el.status,
                                    notAcceptUser: el.reasonNotAccept,
                                    user: null,
                                    realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh),
                                    idCompanyBranh: el.idCompanyBranh,
                                })
                            })
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: err.message,
                                life: 2000,
                            })
                        })
                    await this.handlerLoadData()
                    this.loading = false
                } else {
                    this.dataLeaveOff = []
                    await this.getAllLeaveOffRegister()
                }
            },
            async handlerReload() {
                this.loading = true
                this.dataLeaveOff = []
                this.fillterLeaveOff.searchLeaveOff = ''
                this.fillterLeaveOff.selectedDate = null
                this.fillterLeaveOff.selectedLeaveOff = []
                await this.getAllLeaveOffRegister()
            },
            RedirectToAction() {
                this.$router.push({ name: 'Leaveoff' })
            },
        },
        components: {
            DialogConfirmLeave,
            DialogReasonAcceptLeveOff,
        },
    }
</script>
<style scoped>
    .p-card-header {
        padding: 20px 20px 0px 20px !important;
    }

    .p-card .p-card-body {
        padding: 0px 1.25rem 1.25rem 1.25rem !important;
    }

    .d-grid {
        display: grid;
    }

    .p-breadcrumb {
        border: none !important;
    }

    .p-button.p-button-info.p-button-outlined {
        background-color: #3b82f6;
        color: white;
        border: 1px solid;
    }

    .p-button.p-button-secondary.p-button-outlined,
    .p-buttonset.p-button-secondary > .p-button.p-button-outlined,
    .p-splitbutton.p-button-secondary > .p-button.p-button-outlined {
        background-color: transparent;
        color: white;
        border: 1px solid;
    }

    .input-text {
        margin-right: 10px;
    }
</style>
