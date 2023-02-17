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
                                <h5 style="color: White">LEAVE REGISTRATION LIST</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <Button
                                    style="height: 50px"
                                    label="Leave off list"
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
                                        placeholder="Select Status"
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
                                            placeholder="Enter name..."
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> No data found. </template>
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
                        currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                        :globalFilterFields="[
                            '#',
                            'id',
                            'startTime',
                            'reasons',
                            'notAcceptUser',
                            'idLeaveUser',
                            'endTime',
                            'user',
                            'name',
                        ]"
                    >
                        <Column field="#" header="N.o">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column field="name" header="Staff name" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Leave day">
                            <template #body="{ data }">
                                {{
                                    formartDate(data.startTime) +
                                    ' - ' +
                                    formartDate(data.endTime) +
                                    ` (${formatHours(mathLeaveOffDate(data.startTime, data.endTime))})`
                                }}
                            </template>
                        </Column>
                        <Column field="reasons" header="Reason">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="notAcceptUser" header="Reason for not approving">
                            <template #body="{ data }">
                                <span v-if="data.status == 3">
                                    {{ data.notAcceptUser }}
                                </span>
                                <span v-else> No Content... </span>
                            </template>
                        </Column>
                        <Column field="status" header="Status">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                        <Column header="Action" v-if="Action">
                            <template #body="{ data }" >
                                <div class="d-flex justify-content-center">
                                    <Button
                                        @click="confirmBrowseVacation(data)"
                                        class="p-button-sm mt-1 me-2 p-button-success"
                                        icon="pi pi-check"
                                        :disabled="data.status == 2 || data.status == 3"
                                        :class="{
                                            'p-button-success': data.status != 2,
                                            'p-button-secondary': data.status == 2,
                                        }"     
                                    />
                                    <Button
                                        @click="showConfirmLeaveOff(data)"
                                        class="p-button-sm p-button-danger mt-1 me-2"
                                        icon="pi pi-times"
                                        :disabled="data.status == 3 || data.status == 2"  
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
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP, ENDPIONTS, GET_USER_BY_ID, ACCEPT_LEAVE_OFF } from '@/http-common'
    import dayjs from 'dayjs'
    import DialogConfirmLeave from './DialogConfirmLeave.vue'
    import jwtDecode from 'jwt-decode'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'
import router from '@/router'

    export default {
        async created() {
            ///leaveoff/acceptregisterlists     
           try{
            this.token = LocalStorage.jwtDecodeToken()
            let indexCut = this.$route.path.lastIndexOf('/')
            let string = this.$route.path.slice(1,indexCut) 
            await UserRoleHelper.isAccessModule(string)
            if(UserRoleHelper.isAccess){
                // Phân quyền button
                if(Number(this.token.IdGroup) == 5){
                     this.Action = true;
                }
                // Check quyền
                if (Number(this.token.IdGroup) == 5 || Number(this.token.IdGroup) == 2 || Number(this.token.IdGroup) == 1) {
                    await this.getAllLeaveOffRegister()
                }
                if (Number(this.token.IdGroup) == 4 || Number(this.token.IdGroup) == 3) {
                    setTimeout(() => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Error message',
                            detail: 'Người dùng không có quyền!',
                            life: 3000,
                        })
                        this.$router.push('/')
                    }, 800)
                }
            }else{
                alert('bạn không có quyền giờ đến trang HOME nhé')
                router.push('/')
            }
           }catch(err){
                alert('Ooopps Có gì đó sai sai rồi chuyển bạn đến trang home nhé')
                router.push('/')
           }
           
            
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
                Action : false,
                statusLeave: [
                    {
                        id: 1,
                        title: 'Waiting',
                        class: 'badge bg-warning',
                    },
                    {
                        id: 2,
                        title: 'Done',
                        class: 'badge bg-success',
                    },
                    {
                        id: 3,
                        title: 'Cancel',
                        class: 'badge bg-secondary ',
                    },
                ],
                loading: true,
                userAccept: jwtDecode(localStorage.getItem('token')),
            }
        },
        watch: {
            fillterLeaveOff: {
                handler: function change(newVal) {
                    console.log(newVal)
                    this.handlerFillterLeaveOff()
                },
                deep: true,
            },
        },
        methods: {
            checkStatus(id) {
                var fillter = this.statusLeave.filter(function (el) {
                    return el.id == id
                })
                return Object.assign({}, fillter[0])
            },
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD')
            },
            formatHours(numberOfHours) {
                var Days = Math.floor(numberOfHours / 8)
                var Remainder = numberOfHours % 8
                var Hours = Math.floor(Remainder)
                var Minutes = Math.floor(60 * (Remainder - Hours))
                return `${Math.abs(Days)}d ${Math.abs(Hours)}h ${Math.abs(Minutes)}m`
            },
            mathLeaveOffDate(startDate, endDate) {
                var result = 0
                var start = new Date(startDate)
                var end = new Date(endDate)
                var st = Number(dayjs(start).format('HH')) * 60 + Number(dayjs(start).format('MM'))
                var en = Number(dayjs(end).format('HH')) * 60 + Number(dayjs(end).format('MM'))
                var convertSMonth = Number(dayjs(start).format('MM'))
                var convertEMonth = Number(dayjs(end).format('MM'))
                var convertSDay = Number(dayjs(start).format('DD'))
                var convertEDay = Number(dayjs(end).format('DD'))
                if (convertSDay == convertEDay) {
                    result = 8 - (8 * 60 - (en - st)) / 60
                }
                if (convertSMonth != convertEMonth) {
                    var startMonth = dayjs(start).daysInMonth()
                    var endMonth = dayjs(end).daysInMonth()
                    var arrMonth = [1, 2, 3, 5, 7, 8, 10, 12]
                    var arrMonthBt = [4, 6, 9, 11]
                    var sta = 0
                    var ent = 0
                    var kq = 0
                    arrMonth.forEach((val) => {
                        if (convertSMonth == 2 || convertEMonth == 2) {
                            if (
                                Number(dayjs(start).format('YYYY')) % 4 == 0 ||
                                Number(dayjs(end).format('YYYY')) % 4 == 0
                            ) {
                                sta = convertSDay + convertSMonth * startMonth
                                ent = convertEDay + convertEMonth * endMonth
                            } else {
                                sta = convertSDay + convertSMonth * startMonth
                                ent = convertEDay + convertEMonth * endMonth
                            }
                        } else {
                            if (convertSMonth == val) {
                                sta = convertSDay + convertSMonth * 31
                            }
                            if (convertEMonth == val) {
                                ent = convertEDay + convertEMonth * 31
                            }
                        }
                    })
                    arrMonthBt.forEach((val) => {
                        if (convertSMonth == val) {
                            sta = convertSDay + convertSMonth * 30
                        }

                        if (convertEMonth == val) {
                            ent = convertEDay + convertEMonth * 30
                        }
                    })
                    kq = +ent - sta
                    result = kq * 8 - (8 * 60 - (en - st)) / 60
                }
                if (convertSDay != convertEDay) {
                    var day = (convertEDay - convertSDay) * 8
                    result = day - (8 * 60 - (en - st)) / 60
                }
                return Math.abs(result)
            },
            async handlerBrowseVacation(item) {
                var idAcceptUser = this.userAccept.Id
                await HTTP.put(ACCEPT_LEAVE_OFF(item.id), { idAcceptUser: idAcceptUser })
                    .then((res) => {
                        console.log(res)
                        if (res.status == 200) {
                            this.dataLeaveOff = []
                            this.loading = true
                            this.getAllLeaveOffRegister()
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Successfull',
                                detail: res.data._Message,
                                life: 3000,
                            })
                        }
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Error',
                            detail: err.message,
                            life: 2000,
                        })
                    })
            },
            confirmBrowseVacation(item) {
                this.$confirm.require({
                    message: `Are you sure you want to review for "${item.name}" to think?`,
                    header: 'Leave Confirmation',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-info',
                    accept: () => {
                        this.handlerBrowseVacation(item)
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
            async handlerLoadData() {
                for (let i = 0; i < this.dataLeaveOff.length; i++) {
                    const user = await this.getUserByIdLeaveOff(this.dataLeaveOff[i].idLeaveUser)
                    this.dataLeaveOff[i].user = user
                    this.dataLeaveOff[i].name = `${user.firstName} ${user.lastName}`
                }
            },
            getUserByIdLeaveOff(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
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
                    console.log(this.fillterLeaveOff.selectedDate)
                    var findByNameStatusDateDtos = {
                        fullName: this.fillterLeaveOff.searchLeaveOff,
                        date: DateHelper.convertUTC(this.fillterLeaveOff.selectedDate),
                        status: this.fillterLeaveOff.selectedLeaveOff,
                    }
                    console.log('data', findByNameStatusDateDtos)

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
                                })
                            })
                        })
                        .catch((err) => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Error',
                                detail: err.message,
                                life: 2000,
                            })
                        })
                    await this.handlerLoadData()
                    this.loading = false
                } else {
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
                this.$router.push({ name: 'Leave Off' })
            },
        },
        components: {
            DialogConfirmLeave,
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
