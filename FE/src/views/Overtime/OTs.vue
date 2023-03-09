<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                showGridlines
                :paginator="true"
                ref="dt"
                class="p-datatable-customers"
                sortField="dateCreate"
                :sortOrder="-1"
                :rows="10"
                :loading="loading"
                dataKey="x.id"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedOT"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[5, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    'x.id',
                    'x.date',
                    'x.start',
                    'x.end',
                    'x.realTime',
                    'x.status',
                    'x.description',
                    'x.leadCreate',
                    'name',
                    'nameLead',
                    'nameUser',
                    'nameUserUpdate',
                    'x.dateCreate',
                    'x.updateUser',
                    'x.dateUpdate',
                    'x.note',
                    'x.user',
                    'x.idProject',
                ]"
                responsiveLayout="scroll"
            >
                <!-- Header -->
                <template #header>
                    <h5 style="color: white">Danh sách tăng ca</h5>
                    <div class="flex align-items-center">
                        <div class="header-left">
                            <Button
                                @click="exportToExcelFollowRole()"
                                label="Xuất Excel"
                                icon="pi pi-file-excel"
                                class="p-button p-component p-button-sm me-2"
                                v-if="showButton.export"
                            />
                            <Button
                                label="Thêm"
                                icon="pi pi-plus"
                                @click="openFormAddEdit(null)"
                                class="p-button p-component p-button-sm me-2"
                                v-if="showButton.add"
                            />
                            <Button
                                label="Phê duyệt"
                                icon="pi pi-check-square"
                                @click="acceptMulti()"
                                class="p-button p-component p-button-sm me-2"
                                v-if="showButton.confirmMulti"
                            />
                            <div class="p-input-icon-left left" style="display: inline">
                                <i class="pi pi-search" />
                                <InputText v-model="filters['global'].value" placeholder="Tìm kiếm" />
                            </div>
                        </div>
                        <div class="filter-pro">
                            <div class="filter-pro-input">
                                <div class="filter-pro-top">
                                    <Button
                                        type="button"
                                        style="background-color: antiquewhite"
                                        icon="pi pi-filter-slash"
                                        class="p-button-outlined p-button p-button-sm me-2"
                                        @click="clearFilter()"
                                    />
                                </div>
                                <Dropdown
                                    class="p-column-filter filter-pro-item right"
                                    v-model="selectedMonth"
                                    :options="selectMonth"
                                    optionLabel="label"
                                    placeholder="Chọn tháng"
                                    :filter="true"
                                    filterPlaceholder="Tìm tháng"
                                    @change="Fillter()"
                                />
                                <Dropdown
                                    class="p-column-filter filter-pro-item right"
                                    v-model="selectedProject"
                                    :options="selectProject"
                                    optionLabel="name"
                                    placeholder="Chọn dự án"
                                    :filter="true"
                                    filterPlaceholder="Tìm dự án"
                                    @change="Fillter()"
                                />
                                <MultiSelect
                                    class="filter-pro-item"
                                    :modelValue="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    @update:modelValue="onToggle"
                                    placeholder="Chọn "
                                />
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>

                <!-- Body -->
                <Column selectionMode="multiple" headerStyle="width: 2rem"></Column>
                <Column header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="x.date" header="Ngày tăng ca" sortable dataType="date" style="min-width: 7.5rem">
                    <template #body="{ data }">
                        {{ getFormattedDate(new Date(data.x.date)) }}
                    </template>
                </Column>
                <Column field="x.user" header="Nhân viên tăng ca" sortable style="min-width: 4rem">
                    <template #body="{ data }">
                        {{ data.nameUser }}
                    </template>
                </Column>
                <Column field="x.leadCreate" header="Leader" sortable style="min-width: 6rem">
                    <template #body="{ data }">
                        {{ data.nameLead }}
                    </template>
                </Column>

                <Column field="x.realTime" header="Thời gian tăng ca" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.realTime + 'h' }}
                    </template>
                </Column>
                <Column field="x.start" header="Thời gian bắt đầu" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.start + 'h' }}
                    </template>
                </Column>
                <Column field="x.end" header="Ngày kết thúc" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.end + 'h' }}
                    </template>
                </Column>

                <Column
                    field="x.description"
                    header="Mô tả"
                    sortable
                    :filterMenuStyle="{ width: '14rem' }"
                    style="min-width: 10rem; width: 10rem"
                >
                    <template #body="{ data }">
                        {{ data.x.description }}
                    </template>
                </Column>
                <!-- <Column field="updateUser" header="Approved by" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.x.updateUser }}
                    </template>
                </Column> -->

                <Column
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="col.field + '_' + index"
                >
                    <template #body="{ data }" v-if="col.field === 'dateUpdate'">
                        {{ data.dateUpdate !== null ? getFormattedDate(new Date(data.dateUpdate)) : null }}
                    </template>
                </Column>
                <Column field="x.idProject" header="Dự án" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ data.name }}
                    </template>
                </Column>
                <Column field="x.dateCreate" header="Ngày tạo" sortable dataType="date" style="min-width: 9.5rem">
                    <template #body="{ data }">
                        {{ getFormattedDate(new Date(data.x.dateCreate)) }}
                    </template>
                </Column>
                <Column field="" header="Thực thi" style="max-width: 12rem">
                    <template #body="{ data }">
                        <div v-if="data.x.status == 0 || this.token.IdGroup == 5">
                            <!-- confirm -->
                            <Edit
                                icon="pi pi-check"
                                @click="openConfirm(true, data.x.id, data.x.leadCreate)"
                                class="right p-button-success"
                                v-if="showButton.confirm && data.x.status == 0"
                            />
                            <!--  VIEW  -->
                            <Button
                                icon="pi pi-eye"
                                @click="OpenDetailOT(data)"
                                class="right top p-button-sm"
                            />
                            <!-- Edit -->
                            <Edit
                                @click="openFormAddEdit(data.x.id)"
                                class="right top p-button-warning"
                                v-if="showButton.update"
                            >
                            </Edit>
                            <!-- Refuse   -->
                            <Delete
                                icon="pi pi-times"
                                @click="accept(false, data.x.id, data.x.leadCreate)"
                                :class="showButton.refuse === true ? 'right top' : 'right'"
                                v-if="showButton.refuse && data.x.status == 0"
                            />
                            <!--  Delete -->
                            <Delete
                                @click="confirmDelete(data.x.id, token)"
                                class="right top"
                                v-if="
                                    (showButton.delete && this.token.IdGroup == 5 && data.x.status == 1) ||
                                    (showButton.delete && this.token.IdGroup == 3 && data.x.status == 0)
                                "
                            ></Delete>
                        </div>
                    </template>
                </Column>
                <Column
                    field="x.status"
                    header="Trạng thái"
                    sortable
                    :filterMenuStyle="{ width: '14rem' }"
                    style="min-width: 6rem"
                >
                    <template #body="{ data }">
                        <span :class="'badge ' + color(data.x.status)">{{ statuses[data.x.status].text }}</span>
                    </template>
                </Column>
            </DataTable>
        </div>
        <Dialog
            header="Không có quyền truy cập !"
            :visible="displayDialog1"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!</medium
            >
            <template #footer>
                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <Dialog
            header="Please enter reason!"
            :visible="displayDialog2"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
        >
            <Textarea v-model="reason" style="margin: auto; width: 100%; height: 100%"></Textarea>
            <p v-if="entered" class="p-error">Lý do bắt buộc nhập!</p>
            <template #footer>
                <Button label="No" icon="pi pi-times" @click="closeBasic" class="p-button-text" />
                <Button label="Lưu" icon="pi pi-check" @click="onSubmit(data.leadCreate)" autofocus />
            </template>
        </Dialog>
        <AddOTsDialog
            :display="this.displayFormAddEdit"
            @close="closeFormAddEdit"
            @reloadData="getOTsByLead(this.token.Id)"
            :idproject="this.idproject"
        />
        <DetailOT
            :show="DetailOT"
            @open="OpenDetailOT"
            @close="CloseDetailOT"
            :OTS="ots"
            :isPm="isPM"
            @reloadAPI="getAllOT"
            @alert="showSuccess"
            :displayDialog2="displayDialog2"
            :id="id"
            :lead="lead"
            @OpenFormRefuse="OpenFormRefuse"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import Edit from '@/components/buttons/Edit.vue'
    import Export from '@/components/buttons/Export.vue'
    import router from '@/router/index'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HTTP } from '@/http-common.js'
    import { FilterMatchMode } from 'primevue/api'
    import Delete from '@/components/buttons/Delete.vue'
    import jwtDecode from 'jwt-decode'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { HttpStatus } from '@/config/app.config'
    import DetailOT from './DetailOT.vue'
    import { cloneVNode } from '@vue/runtime-core'
    import storeRole from '@/stores/role'
    import AddOTsDialog from './AddOTsDialog.vue'
import checkAccessModule from '@/stores/checkAccessModule'
    export default {
        name: 'ots',
        data() {
            return {
                selectedOT: null,
                data: null,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    status: { value: null, matchMode: FilterMatchMode.EQUALS },
                },
                selectedColumns: null,
                columns: null,
                project: [],
                proName: [],
                proPM: [],
                user: [],
                username: [],
                cantAdd: false,
                token: null,
                rolePM: true,
                displayDialog1: false,
                displayDialog2: false,
                num: 5,
                timeout: null,
                canAccept: true,
                entered: false,
                reason: '',
                id: '',
                lead: '',
                statuses: [
                    { num: 0, text: 'Đang chờ' },
                    { num: 1, text: 'Đã duyệt' },
                    { num: 2, text: 'Đã từ chối' },
                    { num: 3, text: 'Đã xóa' },
                ],
                userInfo: [],
                selectMonth: [],
                selectedMonth: null,
                selectProject: [],
                selectedProject: null,
                //axios: 'http://api.imsdemo.tk/api/',
                axios: import.meta.env.VITE_VUE_API_URL,
                DetailOT: false,
                ots: 0,
                isPM: false,
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
                displayFormAddEdit: false,
                idproject: null,
            }
        },
        async mounted() {
            // try {
            //     this.token = LocalStorage.jwtDecodeToken()
            //     await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
            //     if (await UserRoleHelper.isAccess) {
                  
            //     } else {
            //         this.countTime()
            //         this.displayDialog1 = true
            //     }
            //     this.columns = [
            //         { field: 'dateUpdate', header: 'Ngày phê duyệt' },
            //         { field: 'note', header: 'Ghi chú' },
            //     ]
            //     this.getMonthFrom()
            //     this.getProject()
            // } catch (error) {
            //     this.countTime()
            //     this.displayDialog1 = true
            // }

            if(checkAccessModule.checkAccessModule(this.$route.path.replace('/', '')) === true){
                
                checkAccessModule.checkShowButton(this.$route.path.replace('/', ''),this.showButton);
              
                await this.getAllOT()
                this.columns = [
                    { field: 'dateUpdate', header: 'Ngày phê duyệt' },
                    { field: 'note', header: 'Ghi chú' },
                ]
                this.getMonthFrom()
                this.getProject()
                this.loading= false
            } else{
                this.countTime()
                 this.displayDialog1 = true
            }

          



         

        },
        methods: {
            // GET OTS BY ROLE PM
            getOTsByPM(idPM) {
                HTTP.get(`OTs/getOTsByidPM/${idPM}`)
                    .then((res) => {
                        this.data = res.data
                    })
                    .catch((err) => console.log(err))
            },
            // GET OTS BY ROLE LEAD
            getOTsByLead(idLEAD) {
                HTTP.get(`OTs/GetAllOTsByLead/${idLEAD}`)
                    .then((res) => {
                        this.data = res.data
                    })
                    .catch((err) => console.log(err))
            },
            // GET OTS BY ROLE STAFF
            getOTsByStaff(idSTAFF) {
                HTTP.get(`OTs/GetAllOTsByStaff/${idSTAFF}`)
                    .then((res) => {
                        this.data = res.data
                    })
                    .catch((err) => console.log(err))
            },
            // GET OTS BY ROLE SAMPLE
            getOTsBySample() {
                HTTP.get('OTs/GetAllOTs')
                    .then((res) => {
                        this.data = res.data
                    })
                    .catch((err) => console.log(err))
            },

            getFormattedDate(date) {
                var year = date.getFullYear()

                var month = (1 + date.getMonth()).toString()
                month = month.length > 1 ? month : '0' + month

                var day = date.getDate().toString()
                day = day.length > 1 ? day : '0' + day

                return day + '-' + month + '-' + year
            },
            acceptMulti() {
                let bool = true
                if (this.selectedOT == null) {
                    this.showWarn('Chọn một mục để phê duyệt!')
                    return
                }
                this.selectedOT.forEach((element) => {
                    if (element.x.status != 0) {
                        bool = false
                    }
                })
                if (bool) {
                    this.selectedOT.forEach((element) => {
                        this.accept(true, element.x.id, element.x.leadCreate)
                    })
                } else this.showWarn('Không thể phê duyệt mục không có trạng thái đang chờ!')
                this.selectedOT = []
            },
            countTime() {
                if (this.num == 0) {
                    this.submit()
                    return
                }
                this.num = this.num - 1
                this.timeout = setTimeout(() => this.countTime(), 1000)
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
            onToggle(value) {
                this.selectedColumns = this.columns.filter((col) => value.includes(col))
            },
            formatDate(value) {
                return new Date(value).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
            },
            async getAllOT() {      
                if (this.token) {
                    
                }
                this.loading = false
            },
            CheckButtonGroup(value) {
                // 1 admin
                if (value == 1) {
                    this.showButton.ExportButton = true
                    this.showButton.confirmButton = true
                    this.showButton.allAcceptButton = true
                    this.showButton.deleteButton = true
                    this.showButton.viewButton = true
                    this.showButton.refuseButton = true
                    this.getOTsByPM(this.token.Id)
                }
                // 2 sample
                if (value == 2) {
                    this.showButton.ExportButton = true
                    this.getOTsBySample()
                }
                // 3 lead
                if (value == 3) {
                    this.showButton.ExportButton = true
                    this.showButton.editButton = true
                    this.showButton.addButton = true
                    this.showButton.deleteButton = true
                    this.showButton.viewButton = true
                    this.getOTsByLead(this.token.Id)
                }
                // 4 staff
                if (value == 4) {
                    this.showButton.ExportButton = true
                    this.getOTsByStaff(this.token.Id)
                }
                // 5 pm
                if (value == 5) {
                    this.showButton.ExportButton = true
                    this.showButton.confirmButton = true
                    this.showButton.allAcceptButton = true
                    this.showButton.viewButton = true
                    this.showButton.refuseButton = true
                    this.showButton.deleteButton = true
                    this.isPM = true
                    this.getOTsByPM(this.token.Id)
                }
            },
            getUsername(userId) {
                if (userId && userId !== 0) {
                    for (var user of this.userInfo) {
                        if (user.id == userId) {
                            const arrName = user.lastName.split(' ')
                            var fullName = user.firstName + ' '
                            arrName.forEach((x) => {
                                fullName += x.substring(0, 1) + '.'
                            })
                            return fullName
                        }
                    }
                }
            },
            async getUserInfo() {
                HTTP.get('Users/getInfo')
                    .then((res) => {
                        this.userInfo = res.data
                    })
                    .catch((err) => console.log(err))
            },
            async canEdit(status, token, lead) {
                let user = this.getUsername(token.Id)
                if (status == 1 || status == 3) return true
                if (lead != user || !(await UserRoleHelper.isLeader())) return true
                return false
            },

            deleteData(id, token) {
                HTTP.put('OTs/deleteOT?' + 'idOT=' + id + '&PM=' + token)
                    .then((res) => {
                        if (res.status == 200) {
                            this.showSuccess('Xóa thẻ OTs thành công.')
                        }
                    })
                    .catch((err) => {
                        this.showWarn('Bạn không có quyền thực hiện thao tác xóa OT.')
                    })
            },
            EditData: (id) => {
                router.push({ name: 'editOT', params: { id: id } })
            },
            color(status) {
                if (status == 0) return 'bg-primary'
                if (status == 1) return 'bg-success'
                if (status == 2) return 'bg-secondary'
                return 'bg-danger'
            },
            add() {
                router.push('/ots/addOT')
            },
            confirmDelete(id, token) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-question-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteData(id, token.Id)
                        setTimeout(() => {
                            this.getAllOT()
                        }, 500)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            showSuccess(err) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: err,
                    life: 3000,
                })
            },

            exportToExcelFollowRole() {
                var month = 0
                var year = 0
                var idProject = 0
                if (this.selectedMonth != null) {
                    month = this.selectedMonth.month
                    year = this.selectedMonth.year
                }
                if (this.selectedProject != null) idProject = this.selectedProject.code
                HTTP.get(
                    `OTs/exportExcelFollowRole/${month}/${year}/${idProject}/${this.token.IdGroup}/${this.token.Id}`,
                )
                    .then((res) => {
                        if (res.status === 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xuất file excel thành công!',
                                life: 3000,
                            })
                            window.location = res.data
                        }
                    })
                    .catch((err) => {
                        console.log(err)
                        this.showWarn('Bạn không có quyền thực hiện thao tác xuất file excel!')
                    })
            },

            exportToExcel() {
                var month = 0
                var year = 0
                var idProject = 0
                if (this.selectedMonth != null) {
                    month = this.selectedMonth.month
                    year = this.selectedMonth.year
                }
                if (this.selectedProject != null) idProject = this.selectedProject.code
                HTTP.get('OTs/exportExcel/month=' + month + '&year=' + year + '&idProject=' + idProject)
                    .then((res) => {
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xuất file excel thành công!',
                                life: 3000,
                            })
                            window.location = res.data
                        }
                    })
                    .catch((err) => {
                        this.showWarn('Bạn không có quyền thực hiện thao tác xuất file excel!')
                    })
            },
            accept(accepted, id, lead) {
                let user = jwtDecode(localStorage.getItem('token'))
                this.PM = user.Id
                if (accepted) {
                    this.status = 1
                    HTTP.put('OTs/acceptOT', { id: id, status: this.status, pm: this.PM })
                        .then((res) => {
                            this.showSuccess('Xét duyệt thành công')
                        })
                        .catch((err) => {
                            console.log(err)
                        })
                    setTimeout(() => {
                        this.getAllOT()
                    }, 500)
                } else {
                    this.displayDialog2 = true
                    this.id = id
                    this.lead = lead
                }
            },
            closeBasic() {
                this.displayDialog2 = false
                this.entered = false
            },
            onSubmit() {
                if (this.reason != null && this.reason != '') {
                    HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(
                        async (res) => {
                            if (res.status == 200) {
                                this.closeBasic()
                                this.showSuccess()
                                await this.getAllOT()
                            }
                        },
                    )
                } else this.entered = true
            },
            clearFilter() {
                this.filters = {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    status: { value: null, matchMode: FilterMatchMode.EQUALS },
                }
                this.getAllOT()
                this.selectedMonth = null
                this.selectedProject = null
            },
            showWarn(message) {
                this.$toast.add({ severity: 'warn', summary: 'Cảnh báo ', detail: message, life: 5000 })
            },
            getMonthFrom() {
                const now = new Date()
                var code = 1
                for (var y = 2020; y <= now.getFullYear(); y++) {
                    for (var m = 1; m <= 12; m++) {
                        if (y == now.getFullYear() && m > now.getMonth() + 1) {
                            break
                        }
                        this.selectMonth.push({ id: code, month: m, year: y, label: m + '/' + y })
                        code++
                    }
                }
            },

            Fillter() {
                var month = 0
                var year = 0
                var idProject = 0
                if (this.selectedMonth !== null) {
                    month = this.selectedMonth.month
                    year = this.selectedMonth.year
                }
                if (this.selectedProject !== null) {
                    idProject = this.selectedProject.code
                }
                var stringGetAPI = `OTs/filterByRole/${month}/${year}/${idProject}/${this.token.IdGroup}/?iduser=${this.token.Id}`
                HTTP.get(stringGetAPI)
                    .then((res) => {
                        this.data = res.data
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            getProject() {
                HTTP.get('Project/getAllProject')
                    .then((res) => {
                        if (res.data) {
                            res.data.forEach((element) => {
                                this.selectProject.push({ code: element.id, name: element.name })
                            })
                        }
                    })
                    .catch((err) => {
                        console.log(err)
                    })
            },
            OpenDetailOT(id) {
                this.DetailOT = true
                this.ots = id
            },
            CloseDetailOT() {
                this.DetailOT = false
            },
            OpenFormRefuse() {
                this.displayDialog2 = true
            },
            OpenFormRefuse() {
                this.displayDialog2 = true
            },

            openFormAddEdit(value) {
                this.displayFormAddEdit = true
                this.idproject = value
            },

            closeFormAddEdit() {
                this.displayFormAddEdit = false
                this.idproject = 0
            },

            openConfirm(accepted, id, lead) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn xác nhận duyệt tăng ca này',
                    header: 'Duyệt tăng ca',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-success',
                    accept: () => {
                        this.accept(accepted, id, lead)
                    },
                    reject: () => {},
                    onShow: () => {
                        //callback to execute when dialog is shown
                    },
                    onHide: () => {
                        //callback to execute when dialog is hidden
                    },
                })
            },
        },
        components: { Edit, Delete, Export, DetailOT, AddOTsDialog },
    }
</script>

<style lang="scss" scoped>
    ::v-deep(.p-paginator) {
        .p-paginator-current {
            margin-left: auto;
        }
    }

    ::v-deep(.p-input-icon-left) {
        margin-top: -2px;
        i:first-of-type {
            left: 1rem;
            color: #6c757d;
        }
    }

    ::v-deep(.p-inputtext) {
        margin: 0 4px 0 3px;
        padding-top: 0.613rem;
        padding-right: 0.75rem;
        padding-bottom: 0.613rem;
        padding-left: 2.5rem;
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

    .project_Search_box {
        margin-top: 10px;
        display: flex;
        justify-content: end;
    }

    ::v-deep(.p-datatable.p-datatable-customers) {
        .p-datatable-header {
            padding: 1rem;
            text-align: left;
            font-size: 1.5rem;
            overflow-x: auto;
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

        .p-dropdown-label.p-placeholder {
            // padding-top: 6px;
        }

        .p-dropdown-label.p-inputtext {
            padding-left: 0px;
        }

        .p-dropdown-label:not(.p-placeholder) {
            // text-transform: uppercase;
            // padding-top: 6px;
        }

        .p-multiselect {
            height: 44px;

            .p-multiselect-label {
                height: 100%;
                line-height: normal;
            }
        }

        .p-datatable-header {
            background-color: #607d8b;
            height: auto;
        }
    }

    .header-div {
        height: auto;
    }

    .filter-pro-input {
        display: flex;
        max-width: 900px;
    }

    ::v-deep(.p-dropdown) {
        height: 44px;
        // position: static;
    }
    ::v-deep(.p-inputtext.p-component) {
        height: 44px;
    }
    // .p-button-outlined {
    //     height: 40px;
    // }

    .flex {
        display: flex;
        justify-content: space-between;
    }

    .filter-pro {
        display: flex;
        height: auto;
        float: right;
    }

    .filter-pro-item {
        width: 200px;
    }

    .header-left {
        height: 100%;
        display: flex;
        flex-direction: row;
        justify-content: end;
        align-items: flex-end;
        margin-right: 10px;
        width: max-content;
    }

    .right {
        margin-right: 12px;
    }
    // .left {
    //     margin-left: 10px;
    // }

    .size__Button {
        padding: 0.4rem;
        font-size: 14px;
    }
    .top {
        margin-top: 6.5px;
    }
</style>
