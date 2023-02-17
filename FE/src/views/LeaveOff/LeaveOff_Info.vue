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
                                <h5 style="color: White">LEAVE OFF LIST</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <Export style="height: 50px" label="Export" @click="exportCSV($event)" />
                            </div>
                            <div class="d-flex justify-content-end">
                                <div class="input-text">
                                    <Button
                                        type="button"
                                        style="background-color: antiquewhite"
                                        icon="pi pi-filter-slash"
                                        class="p-button-outlined right"
                                        @click="clearFilter()"
                                    />
                                </div>
                                <div class="input-text">
                                    <InputText
                                        style="width: 200px"
                                        type="month"
                                        v-model="fillterLeaveOff.sortMonth"
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
                        currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                        :globalFilterFields="['#', 'name', 'startDate', 'endDate', 'isDeleted', 'isFinished']"
                    >
                        <template #empty> No data found. </template>
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
                        <Column sortable field="acceptBy" header="Accept by">
                            <template #body="{ data }">
                                {{ data.acceptBy }}
                            </template>
                        </Column>
                        <Column field="startTime" header="Start time">
                            <template #body="{ data }">
                                {{ data.startTime }}
                            </template>
                        </Column>
                        <Column field="" header="End time">
                            <template #body="{ data }">
                                {{ data.endTime }}
                            </template>
                        </Column>
                        <Column field="" header="Reasons">
                            <template #body="{ data }">
                                {{ data.reasons }}
                            </template>
                        </Column>
                        <Column field="status" header="Status">
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
import router from '../../router'
import { UserRoleHelper } from '@/helper/user-role.helper'
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
                        title: 'Done',
                        class: 'badge bg-success',
                    },
                ],
                fillterLeaveOff: {
                    sortMonth: null,
                    searchLeaveOff: '',
                    idstatus: [2],
                },
            }
        },
        async created() {
            try{
            this.token = LocalStorage.jwtDecodeToken()
            let indexCut = this.$route.path.lastIndexOf('/')
            let string = this.$route.path.slice(1,indexCut) 
          
            await UserRoleHelper.isAccessModule(string)
       
            if(UserRoleHelper.isAccess){
                // Check quyền
                this.token = LocalStorage.jwtDecodeToken()
            if (Number(this.token.IdGroup) == 5 || Number(this.token.IdGroup) == 2 || Number(this.token.IdGroup) == 1) {
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
            }else{
                alert('bạn không có quyền giờ đến trang HOME nhé')
                router.push('/') 
            }         
           }catch(err){
                alert('Ooopps Có gì đó sai sai rồi chuyển bạn đến trang home nhé')
                console.log(err);
                router.push('/')
           }          
        },
        watch: {
            fillterLeaveOff: {
                handler: function change(newVal) {
                    if (newVal != '') {
                        this.handlerFillterLeaveOff()
                    } else {
                        this.arr = []
                        this.getLeaveOff()
                    }
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
                    var user = await this.getUserById(this.arr[i].idLeaveUser)
                    this.arr[i].user = user
                    this.arr[i].name = user.firstName + ' ' + user.lastName
                    var accept = await this.getUserById(this.arr[i].idAcceptUser)
                    this.arr[i].acceptBy = accept.firstName + ' ' + accept.lastName
                }
            },

            clearFilter() {
                this.arr = []
                this.fillterLeaveOff.searchLeaveOff = ''
                this.fillterLeaveOff.sortMonth = null
                this.getLeaveOff()
            },
            async getUserById(id) {
                return await HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },

            async getLeaveOff() {
                let status = 2
                await HTTP.get(GET_LEAVEOFF_BY_STATUS(status)).then((res) => {
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
                        })
                    })
                })
                await this.handlerIdUser()
                this.loading = false
            },

            formatDate(value) {
                if (value == null) return null
                else
                    return new Date(value).toLocaleDateString('en-CA', {
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                    })
            },
            async handlerFillterLeaveOff() {
                this.arr = []
                if (this.fillterLeaveOff.searchLeaveOff != '' || this.fillterLeaveOff.sortMonth != null) {
                    var currentDate = new Date(this.fillterLeaveOff.sortMonth)
                    var theFirst = new Date(currentDate.getFullYear(), 0, 1)
                    var findByNameStatusDateDtos = {
                        fullName: this.fillterLeaveOff.searchLeaveOff,
                        date: DateHelper.convertUTC(theFirst),
                        status: this.fillterLeaveOff.idstatus,
                    }

                    await HTTP.post(GET_BY_YEAR, findByNameStatusDateDtos).then((res) => {
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
                            })
                        })
                        this.handlerIdUser()
                    })
                }
                this.loading = false
            },
            exportCSV() {
                import('../../plugins/Export2Excel.js').then((excel) => {
                    const OBJ = this.arr

                    const Header = ['Name', 'AcceptBy', 'Start Time', 'End Time', 'Reasons']

                    const Field = ['name', 'acceptBy', 'startTime', 'endTime', 'reasons']

                    const Data = this.FormatJSon(Field, OBJ)
                    excel.export_json_to_excel({
                        header: Header,
                        data: Data,
                        sheetName: 'Leave Off List',
                        filename: 'Leave Off List',
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
        },

        components: {
            View,
            Export,
        },
    }
</script>

<style lang="scss" scoped>
    .input-text {
        margin-right: 10px;
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

    ::v-deep(.p-datepicker) {
        min-width: 25rem;

        td {
            font-weight: 400;
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
