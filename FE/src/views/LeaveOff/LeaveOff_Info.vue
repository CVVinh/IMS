<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4 mt-3">
            <Card class="border-1">
                <!-- Header -->
                <template #header>
                    <div class="card card-body" style="background-color: #607d8b">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <h5 style="color: White">Danh sách nghỉ phép</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <Export style="height: 50px" label="Xuất Excel" @click="exportCSV($event)" />

                                <!-- <Button label="Đăng ký nghỉ phép" class="p-button-info" style="margin-left: 10px;" @click="toRegisterPage" /> -->
                                <router-link  to="/leaveoff/Registerlists" 
                                style="
                                    display: flex;
                                    align-items: center;
                                    justify-content: center;
                                    color: #ffffff;
                                    background-color: #F59E0B;
                                    border: 1px solid #F59E0B ;
                                    padding: 10px;
                                    border-radius: 5px;
                                    margin-left: 10px;
                                    font-weight: 600;
                                    text-decoration: none;  
                                "
                                 v-if="showButton.add"
                                >
                                    <span>Đăng ký nghỉ phép</span>        
                                </router-link>
                            </div> 
                            <div class="d-flex justify-content-end">
                                <div class="input-text">
                                    <Button
                                        type="button"
                                        style="background-color: antiquewhite"
                                        icon="pi pi-filter-slash"
                                        class="p-button-outlined right"
                                        @click="handlerReload()"
                                    />
                                </div>
                                <div class="input-text">
                                    <date-picker
                                        v-model:value="fillterLeaveOff.sortMonth"
                                        type="year"
                                        class="date_time_pick"
                                        placeholder="Chọn năm"
                                    ></date-picker>
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
                        :value="arr"
                        ref="dt"
                        :paginator="true"
                        class="p-datatable-customers"
                        :rows="5"
                        dataKey="id"
                        :rowHover="true"
                        sortField="dateCreated"
                        :sortOrder="-1"
                        :loading="loading"
                        responsiveLayout="scroll"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                        :rowsPerPageOptions="pageIndex"
                        currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                        :globalFilterFields="['#', 'name', 'startDate', 'endDate', 'isDeleted', 'isFinished']"
                    >
                        <template #empty> Không tìm thấy </template>
                        <template #loading>
                            <ProgressSpinner />
                        </template>
                        <Column field="#" header="#" dataType="date">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column sortable field="name" header="User">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column sortable field="acceptBy" header="Người phê duyệt">
                            <template #body="{ data }">
                                {{ data.acceptBy }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Ngày bắt đầu">
                            <template #body="{ data }">
                                {{ data.startTime }}
                            </template>
                        </Column>
                        <Column field="" header="Ngày kết thúc">
                            <template #body="{ data }">
                                {{ data.endTime }}
                            </template>
                        </Column>
                        <Column field="" header="Thời gian thực tế">
                            <template #body="{ data }">
                                {{ data.realTime }}
                            </template>
                        </Column>
                        <Column field="" header="Lý do">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="status" header="Trạng thái">
                            <template #body="{ data }">
                                <span :class="checkStatus(data.status).class">
                                    {{ checkStatus(data.status).title }}
                                </span>
                            </template>
                        </Column>
                    </DataTable>
                </template>
            </Card>
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP, GET_USER_BY_ID, GET_BY_YEAR, GET_LEAVEOFF_BY_STATUS, HTTP_LOCAL } from '@/http-common'
    import View from '../../components/buttons/View.vue'
    import Export from '../../components/buttons/Export.vue'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import router from '../../router/index'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import dayjs from 'dayjs'
    import DatePicker from 'vue-datepicker-next'
    import 'vue-datepicker-next/index.css'
    export default {
        data() {
            return {
                searchtext: null,
                sortMonth: null,
                idUser: null,
                arr: [],
                resultCount: null,
                loading: true,
                displayBasic: false,
                pageIndex: [5, 10, 15, 20],
                num: 5,
                isOpenDialog: false,
                defaultPageSize: 100,
                statusLeave: [
                    {
                        id: 2,
                        title: 'Đã duyệt',
                        class: 'badge bg-success',
                    },
                ],
                fillterLeaveOff: {
                    sortMonth: null,
                    searchLeaveOff: '',
                    idstatus: [2],
                },
                showButton:{
                    add : false
                }
            }
        },
        async created() {
            try {
                this.token = LocalStorage.jwtDecodeToken()
                let indexCut = this.$route.path.lastIndexOf('/')
                let string = this.$route.path.slice(1, indexCut)

                await UserRoleHelper.isAccessModule(string)

                if (UserRoleHelper.isAccess) {
                    // Check quyền
                    
                    this.token = LocalStorage.jwtDecodeToken()

                    if(Number(this.token.IdGroup) === 2){
                        this.showButton.add = true
                    }

                    if (
                        Number(this.token.IdGroup) == 5 ||
                        Number(this.token.IdGroup) == 2 ||
                        Number(this.token.IdGroup) == 1
                    ) {
                        await this.getLeaveOff()
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
                } else {
                    alert('bạn không có quyền giờ đến trang HOME nhé')
                    router.push('/')
                }
            } catch (err) {
                alert('Ooopps Có gì đó sai sai rồi chuyển bạn đến trang home nhé')
                console.log(err)
                router.push('/')
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
            async handlerIdUser() {
                for (let i = 0; i < this.arr.length; i++) {
                    const user = await this.getUserById(this.arr[i].idLeaveUser)
                    this.arr[i].user = user
                    this.arr[i].name = `${user.firstName} ${user.lastName}`
                    this.arr[i].userCode = user.userCode

                    const accept = await this.getUserById(this.arr[i].idAcceptUser)
                    this.arr[i].acceptBy = `${accept.firstName} ${accept.lastName}`
                }
            },

            async handlerReload() {
                this.loading = true
                this.arr = []
                this.fillterLeaveOff.searchLeaveOff = ''
                this.fillterLeaveOff.sortMonth = null
                await this.getLeaveOff()
            },
            getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },
            
            toRegisterPage(){
                router.push('/leaveoff/Registerlists')

            },

            async getLeaveOff() {
                let status = 2
                await HTTP.get(GET_LEAVEOFF_BY_STATUS(status)).then((res) => {
                    this.arr = []
                    res.data._Data.forEach((el) => {
                        this.arr.push({
                            id: el.id,
                            startTime: this.formatDate(el.startTime),
                            endTime: this.formatDate(el.endTime),
                            acceptTime: el.acceptTime,
                            createTime: el.createTime,
                            idAcceptUser: el.idAcceptUser,
                            idLeaveUser: el.idLeaveUser,
                            reasons: el.reasons,
                            status: el.status,
                            user: null,
                            name: null,
                            acceptBy: null,
                            realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh),
                            idCompanyBranh: el.idCompanyBranh,
                            userCode: null,
                        })
                    })
                })
                await this.handlerIdUser()
                this.loading = false
            },
            formatDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD (HH:mm)')
            },
            async handlerFillterLeaveOff() {
                this.loading = true
                if (this.fillterLeaveOff.searchLeaveOff != '' || this.fillterLeaveOff.sortMonth != null) {
                    if (this.fillterLeaveOff.sortMonth == null) {
                        var currentDate = new Date()
                    } else {
                        var currentDate = new Date(this.fillterLeaveOff.sortMonth)
                    }
                    var theFirst = new Date(currentDate.getFullYear(), 0, 1)
                    var findByNameStatusDateDtos = {
                        fullName: this.fillterLeaveOff.searchLeaveOff,
                        date: DateHelper.convertUTC(theFirst),
                        status: this.fillterLeaveOff.idstatus,
                    }
                    console.log('input', findByNameStatusDateDtos)

                    await HTTP.post(GET_BY_YEAR, findByNameStatusDateDtos).then((res) => {
                        this.arr = []
                        res.data._Data.forEach((el) => {
                            this.arr.push({
                                id: el.id,
                                startTime: this.formatDate(el.startTime),
                                endTime: this.formatDate(el.endTime),
                                acceptTime: el.acceptTime,
                                createTime: el.createTime,
                                idAcceptUser: el.idAcceptUser,
                                idLeaveUser: el.idLeaveUser,
                                reasons: el.reasons,
                                status: el.status,
                                user: null,
                                name: null,
                                acceptBy: null,
                                realTime: this.mathLeaveOffDate(el.startTime, el.endTime, el.idCompanyBranh),
                                idCompanyBranh: el.idCompanyBranh,
                                userCode: null,
                            })
                        })
                    })
                    await this.handlerIdUser()
                    this.loading = false
                } else {
                    await this.getLeaveOff()
                    this.loading = false
                }
            },
            exportCSV() {
                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.arr

                    const Header = [
                        'Mã nhân viên',
                        'Tên nhân viên',
                        'Ngày bắt đầu',
                        'Ngày kết thúc',
                        'Thời gian thực tế',
                        'Lý do',
                        'Người duyệt',
                    ]

                    const Field = ['userCode', 'name', 'startTime', 'endTime', 'realTime', 'reasons', 'acceptBy']

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Danh sách nghỉ phép',
                        filename: 'Danh sách nghỉ phép',
                        autoWidth: true,
                        bookType: 'xlsx',
                    })
                })
            },

            FormatJSon(FilterData, JsonData) {
                return JsonData.map((v) =>
                    FilterData.map((j) => {
                        return v[j]
                    }),
                )
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
                console.log(totalWorkingHours)

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
        },
        components: {
            View,
            Export,
        },
    }
</script>

<style>
    .date_time_pick .mx-input-wrapper > input {
        height: 49.79px !important;
    }
    .date_time_pick .mx-input-wrapper > i {
        font-size: 20px !important;
    }
</style>

<style lang="scss" scoped>
    .input-text {
        margin-right: 10px;
    }
    date-picker::placeholder {
        color: #ca1414;
    }
    date-picker {
        display: block;
        width: 200px;
        height: 300px;
        border: 1px solid #ccc;
        border-radius: 4px;
        padding: 4px;
        font-size: 14px;
    }
    date-picker .mx-input {
        display: inline-block;
        box-sizing: border-box;
        width: 1%;
        height: 100px;
        padding: 6px 30px;
        padding-left: 10px;
        font-size: 14px;
        line-height: 1.4;
        color: #555;
        background-color: #fff;
        border: 1px solid #ccc;
        border-radius: 4px;
        box-shadow: inset 0 1px 1px rgb(0 0 0 / 8%);
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
            display: inline-flex;
        }

        .p-inputtext-sm {
            font-size: 0.96rem;
        }

        .layout-left {
            float: right;
            display: inline;
        }

        .p-button.p-button-secondary.p-button-outlined {
            float: right;
            height: 46px;
        }

        .p-button.p-button-info.p-button-outlined {
            float: right;
            height: 46px;
            margin-left: 10px;
        }

        .p-datatable-header {
            height: 75px;
        }

        .nav-container {
            position: relative;
        }

        .header-h5 {
            position: absolute;
            margin-top: 10px;
        }

        .header-left {
            height: 100%;
            display: flex;
            flex-direction: row;
            justify-content: end;
            align-items: flex-end;
            margin-right: 10px;
        }

        .mazin {
            margin-left: 5px;
            margin-right: 5px;
        }

        .maz {
            margin-right: 5px;
        }

        .p-dropdown {
            background: #ffffff;
            border: 1px solid #ced4da;
            transition: background-color 0.2s, color 0.2s, border-color 0.2s, box-shadow 0.2s;
            border-radius: 6px;
            float: right;
            height: 46px;
        }
    }
</style>
