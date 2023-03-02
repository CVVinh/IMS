<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                showGridlines
                ref="dt"
                :paginator="true"
                class="p-datatable-customers"
                :rows="5"
                dataKey="id"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedProject"
                sortField="dateCreated"
                :sortOrder="-1"
                :loading="loading"
                responsiveLayout="scroll"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="pageIndex"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="['name']"
            >
                <!-- Header -->
                <template #header>
                    <div class="flex justify-content-center">
                        <h5 class="" style="color: white">Danh sách dự án</h5>
                        <div class="inline">
                            <div style="display: flex; justify-content: space-between">
                                <div>
                                    <Export
                                        label="Xuất Excel"
                                        class="me-2"
                                        @click="exportToExcel()"
                                        v-if="this.showButton.export"
                                    />
                                    <Button
                                        @click="finishMulti()"
                                        label="Hoàn tất"
                                        class="p-button-sm me-2"
                                        icon="pi pi-check"
                                        v-if="this.showButton.finishMulti"
                                    />
                                    <Add @click="openDialogAdd()" label="Thêm" v-if="this.showButton.add" />
                                </div>

                                <div>
                                    <div class="p-input-icon-left layout-left">
                                        <i class="pi pi-search" />
                                        <InputText
                                            class="p-inputtext-sm"
                                            v-model="filters['name'].value"
                                            placeholder="Tìm kiếm"
                                        />
                                    </div>
                                    <div class="layout-left">
                                        <MultiSelect
                                            :modelValue="selectedColumns"
                                            :options="columns"
                                            optionLabel="header"
                                            @update:modelValue="onToggle"
                                            placeholder="Chọn"
                                            style="width: 20em"
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty>Không tìm thấy. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>
                <Column selectionMode="multiple"></Column>
                <Column field="" header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="projectCode" header="Mã dự án " sortable>
                    <template #body="{ data }">
                        {{ data.projectCode }}
                    </template>
                </Column>
                <Column field="name" header="Tên dự án " sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.name }}
                    </template>
                </Column>

                <Column field="startDate" header="Ngày bắt đầu " sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ getFormattedDate(new Date( data.startDate)) }}
                    </template>
                </Column>
                <Column field="endDate" header="Ngày kết thúc " sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{  data.endDate == '1970-01-01' ? 'Chưa giải quyết..' :  getFormattedDate(new Date(data.endDate))}}
                    </template>
                </Column>
                <Column
                    style="min-width: 8rem"
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="col.field + '_' + index"
                ></Column>
                <Column header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 15rem">
                    <template #body="{ data }">
                        <Member
                            @click="toDetailProject(data.id)"
                            class="p-button-info mazin"
                            :disabled="canOperation(data.isDeleted, data.isFinished)"
                            v-if="this.showButton.member"
                        />

                        <Edit
                            v-if="data.isOnGitlab == false && this.showButton.edit"
                            class="p-button-warning mazin"
                            @click="openDialogEdit(data)"
                            :disabled="canOperation(data.isDeleted, data.isFinished)"
                        />

                        <Delete
                            class="p-button-danger mazin"
                            @click="confirmDelete(data.id)"
                            :disabled="canOperation(data.isDeleted, data.isFinished)"
                            v-if="this.showButton.delete"
                        />

                        <Button
                            @click="finishProject(data.id)"
                            class="p-button-sm mt-1 p-button-success"
                            icon="pi pi-check"
                            :disabled="canOperation(data.isDeleted, data.isFinished)"
                            v-if="this.showButton.finish"
                        />
                    </template>
                </Column>
                <Column header="Trạng thái" sortable field="isDeleted">
                    <template #body="{ data }">
                        <p
                            :style="{
                                color:
                                    statusText(data.isFinished, data.isDeleted) === 'Đang chạy'
                                        ? 'green'
                                        : statusText(data.isFinished, data.isDeleted) === 'Đã hoàn thành'
                                        ? 'orange'
                                        : statusText(data.isFinished, data.isDeleted) === 'Đã xóa'
                                        ? 'red'
                                        : null,
                            }"
                        >
                            {{ statusText(data.isFinished, data.isDeleted) }}
                        </p>
                    </template>
                </Column>
            </DataTable>
        </div>
        <Dialog
            header="Không có quyền truy cập!"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!
            </medium>
            <template #footer>
                <Button label="Hoàn tất" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
        <DialogAddEdit
            :isOpenDialog="isOpenDialog"
            :projectSelected="{ ...projectSelected }"
            @closeDialog="closeDialog"
            @getAllProject="getAllProject"
            :user="user"
            :leader="leader"
            :projectGit="dataProjects"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import router from '@/router/index'
    import { GET_USER_BY_ID, HTTP } from '@/http-common'
    import { FilterMatchMode } from 'primevue/api'
    import jwtDecode from 'jwt-decode'
    import Add from '../../components/buttons/Add.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Member from '../../components/buttons/Member.vue'
    import Export from '../../components/buttons/Export.vue'
    import DialogAddEdit from '@/views/Project/DialogAddEdit.vue'
    import { ProjectDto } from '@/views/Project/Project.dto'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { HTTP_API_GITLAB, GET_ALL_PROJECT } from '@/http-common'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { DateHelper } from '@/helper/date.helper'
    export default {
        data() {
            return {
                filters: {
                    name: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                userInfo: [],
                selectedProject: [],
                selectedColumns: null,
                columns: null,
                arr: [],
                data: [],
                loading: true,
                displayBasic: false,
                pageIndex: [5, 10, 15, 20],
                num: 5,
                timeout: null,
                isOpenDialog: false,
                projectSelected: new ProjectDto(),
                user: [],
                token: null,
                showButton: {
                    export: false,
                    add: false,
                    finishMulti: false,
                    finish: false,
                    delete: false,
                    edit: false,
                    member: false,
                },
                acceptRole: ['pm', 'admin', 'director', 'leader'],
                dataProjects: [],
            }
        },
        async created() {
            await this.handlerGetInfoProjects()
        },
        async mounted() {
            try {
                this.token = LocalStorage.jwtDecodeToken()

                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if (await UserRoleHelper.isAccess) {
                    console.log('hello')
                    this.Permission(Number(this.token.IdGroup), this.token.Id)
                } else {
                    this.countTime()
                    this.displayBasic = true
                }
            } catch (error) {
                console.log(error)
                this.countTime()
                this.displayBasic = true
            }
            this.dataProjects.map((ele) => {
                console.log(ele)
            })

            this.columns = [
                { field: 'description', header: 'Mô tả' },
                { field: 'userId', header: 'PM' },
                { field: 'leader', header: 'Leader' },
                { field: 'userCreated', header: 'Người tạo' },
                { field: 'dateCreated', header: 'Ngày tạo' },
                { field: 'userUpdate', header: 'Người chỉnh sửa' },
                { field: 'dateUpdate', header: 'Ngày chỉnh sửa' },
            ]
        },
        methods: {
            openDialogAdd() {
                this.isOpenDialog = true
                this.projectSelected = []
            },
            closeDialog() {
                this.isOpenDialog = false
                this.projectSelected = []
            },
            openDialogEdit(data) {
                this.projectSelected = { ...data }
                this.isOpenDialog = true
            },
            async canAddProject() {
                return false
            },
            finishMulti() {
                let bool = this.selectedProject.filter(function (element, index) {
                    if (element.isFinished === true || element.isDeleted === true) {
                        return false
                    } else {
                        return true
                    }
                })
                if (this.selectedProject == null) {
                    this.showWarn('Vui lòng chọn một dự án để kết thúc!')
                    return
                }
                if (bool.length > 0) {
                    bool.forEach((element) => {
                        this.finishProject(element.id)
                    })
                } else this.showWarn('Không thể hoàn thành dự án!')
                this.selectedProject = []
            },
            finishProject(id) {
                this.$confirm.require({
                    message: 'Are you sure you want to proceed?',
                    header: 'Confirmation',
                    icon: 'pi pi-exclamation-triangle',
                    accept: () => {
                        let userlogin = jwtDecode(localStorage.getItem('token'))
                        let idUser = userlogin.Id
                        HTTP.put('Project/FinishProject/' + id, { UserId: idUser })
                            .then((res) => {
                                if (res.status == 200) {
                                    this.getAllProject()
                                    this.$toast.add({
                                        severity: 'success',
                                        summary: 'Thành công',
                                        detail: 'Dự án hoàn tất!',
                                        life: 2000,
                                    })
                                }
                            })
                            .catch((err) => {
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Không có quyền thực hiện thao tác kết thúc dự án!',
                                    life: 2000,
                                })
                            })
                    },
                    reject: () => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Hủy bỏ',
                            detail: 'Bạn đã hủy thao tác',
                            life: 3000,
                        })
                    },
                })
            },
            onToggle(value) {
                this.selectedColumns = this.columns.filter((col) => value.includes(col))
            },
            formatDate(value) {
                return DateHelper.convertFullDate(value)
            },
            statusText(isfinish, isdelete) {
                if (isdelete === false && isfinish === false) {
                    return 'Đang chạy'
                }
                if (isfinish === true) {
                    return 'Đã hoàn thành'
                }
                if (isdelete === true) {
                    return 'Đã xóa'
                }
            },
            async getAllProject() {
                this.loading = true
                await HTTP.get('Project/getAllProject')
                    .then((res) => {
                        if (res.data) {
                            this.data = res.data
                            console.log(res)
                        }
                    })
                    .catch((err) => {
                        console.log(err)
                    })
                this.getPMName()
                this.getUserCreated()
                this.getUserEdited()
                console.log(this.data)
                this.loading = false
            },
            async getPMName() {
                for (let i = 0; i < this.data.length; i++) {
                    if (this.data[i].userId != 0) {
                        var PM = await this.getUserById(this.data[i].userId)
                        this.data[i].userId = PM.fullName
                        this.data[i].dateUpdate = this.formatDate(this.data[i].dateUpdate)
                        this.data[i].dateCreated = this.formatDate(this.data[i].dateCreated)
                    } else {
                        this.data[i].PMName = ''
                    }
                }
            },
            async getUserCreated() {
                for (let i = 0; i < this.data.length; i++) {
                    if (this.data[i].userCreated != 0) {
                        var usercreate = await this.getUserById(this.data[i].userCreated)
                        this.data[i].userCreated = usercreate.fullName
                    } else {
                        this.data[i].userCreated = ''
                    }
                }
            },
            async getUserEdited() {
                for (let i = 0; i < this.data.length; i++) {
                    if (this.data[i].userUpdate != 0) {
                        var userEdited = await this.getUserById(this.data[i].userUpdate)
                        this.data[i].userUpdate = userEdited.fullName
                    } else {
                        this.data[i].userEdited = ''
                    }
                }
            },
            addProject() {
                this.$router.push('/project/add')
            },
            editProject(id) {
                router.push('/project/edit/' + id)
            },
            deleteProject(id) {
                let userlogin = jwtDecode(localStorage.getItem('token'))
                let idUser = userlogin.Id
                HTTP.put('Project/DeleteProject/' + id, { userId: idUser })
                    .then((res) => {
                        if (res.status == 200) {
                            this.getAllProject()
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xóa thành công!',
                                life: 2000,
                            })
                        }
                    })
                    .catch(() => {
                        this.showWarn('Không có quyền thực hiện thao tác xóa dự án.')
                    })
            },
            canOperation(isDeleted, isFinished) {
                if (isDeleted === true || isFinished === true) {
                    return true
                } else {
                    return false
                }
            },
            getProjectByLead(idlead) {
                HTTP.get(`/Project/getAllProjectByLead/${idlead}`)
                    .then((res) => {
                        this.data = res.data
                        this.loading = false
                    })
                    .catch((err) => console.log(err))
            },
            getProjectByStaff(idstaff) {
                HTTP.get(`/Project/getAllProjectByStaff/${idstaff}`)
                    .then((res) => {
                        this.data = res.data
                        console.log(res.data)
                        this.loading = false
                    })
                    .catch((err) => console.log(err))
            },
            Permission(value, id) {
                if (value !== null) {
                    if (value === 1) {
                        this.showButton.add = true
                        this.showButton.delete = true
                        this.showButton.edit = true
                        this.showButton.export = true
                        this.showButton.finish = true
                        this.showButton.finishMulti = true
                        this.showButton.member = true
                        this.getAllProject()
                    }

                    // sample
                    if (value === 2) {
                        this.getAllProject()
                    }
                    // lead
                    if (value === 3) {
                        this.showButton.member = true
                        this.getProjectByLead(id)
                    }
                    // staff
                    if (value === 4) {
                        this.getProjectByStaff(id)
                    }
                    // pm
                    if (value === 5) {
                        this.showButton.add = true
                        this.showButton.delete = true
                        this.showButton.edit = true
                        this.showButton.export = true
                        this.showButton.finish = true
                        this.showButton.finishMulti = true
                        this.showButton.member = true
                        this.getAllProject()
                    }
                }
            },
            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteProject(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Xóa thành công!',
                    life: 2000,
                })
            },
            showWarn(message) {
                this.$toast.add({ severity: 'warn', summary: 'Warn Message', detail: message, life: 2000 })
            },
            toDetailProject(id) {
                this.$router.push({ name: 'detailproject', params: { id: id } })
            },
            colorDeleted(status) {
                if (status == false) return 'bg-success'
                return 'bg-danger'
            },
            colorFinished(status) {
                if (status == true) return 'bg-warning'
                return 'bg-info'
            },
            exportToExcel() {
                HTTP.get(`Project/exportExcel/`)
                    .then((res) => {
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'File excel đã được lưu thành công.',
                                life: 3000,
                            })
                            window.location = res.data
                        }
                    })
                    .catch(() => {
                        this.$toast.add({
                            severity: 'warn',
                            summary: 'Cảnh báo ',
                            detail: 'Bạn không có quyền thực hiện thao tác xuất file excel!',
                            life: 3000,
                        })
                    })
            },
            getFormattedDate(date) {
                var year = date.getFullYear()

                var month = (1 + date.getMonth()).toString()
                month = month.length > 1 ? month : '0' + month

                var day = date.getDate().toString()
                day = day.length > 1 ? day : '0' + day

                return day + '-' + month + '-' + year
            },
            showMember() {
                location.reload()
            },
            text(value) {
                if (value == true) return 'Đã đóng'
                return 'Đang chạy'
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
            getAllProects(page) {
                return HTTP_API_GITLAB.get(GET_ALL_PROJECT(100, page)).then((res) => res.data)
            },
            async handlerGetInfoProjects() {
                let resultCountPr = 100
                let resultPr = []
                let page = 1
                while (resultCountPr === 100) {
                    let newResultsPr = await this.getAllProects(page)
                    page++
                    this.dataProjects.push(...newResultsPr)
                    resultCountPr = newResultsPr.length
                    resultPr = resultPr.concat(newResultsPr)
                }
            },
            getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },
        },
        components: { Add, Edit, Delete, Member, Export, DialogAddEdit },
    }
</script>

<style lang="scss" scoped>
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
            display: inline;
        }

        .p-inputtext-sm {
            font-size: 0.96rem;
        }

        .layout-left {
            float: right;
            display: inline;
        }

        .p-multiselect .p-multiselect-label {
            padding: 0.41rem 0.41rem;
        }

        .p-datatable-header {
            background-color: #607d8b;
        }

        .mazin {
            // margin-left: 5px;
            margin-right: 5px;
        }

        .maz {
            margin-right: 5px;
        }
    }
</style>
<style>
    .p-multiselect-panel.p-component.p-ripple-disabled {
        margin-top: 4px;
    }
</style>
