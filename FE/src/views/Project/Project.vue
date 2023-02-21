<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-3 mt-3">
            <DataTable
                :value="arr"
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
                :globalFilterFields="[
                    '#',
                    'id',
                    'name',
                    'projectCode',
                    'description',
                    'startDate',
                    'endDate',
                    'isDeleted',
                    'isFinished',
                    'userId',
                    'leader',
                    'userUpdate',
                    'isOnGitlab',
                ]"
            >
                <!-- Header -->
                <template #header>
                    <div class="flex justify-content-center">
                        <h5 class="" style="color: white">Danh sách dự án</h5>
                        <div class="inline">
                            <Export label="Xuất Excel" class="me-2" @click="exportToExcel()" />
                            <Button
                                @click="finishMulti()"
                                label="Hoàn tất"
                                class="p-button-sm me-2"
                                icon="pi pi-check"
                            />
                            <Add @click="openDialogAdd()" label="Thêm" v-if="canAddProject()" />
                            <div class="p-input-icon-left layout-left">
                                <i class="pi pi-search" />
                                <InputText
                                    class="p-inputtext-sm"
                                    v-model="filters['global'].value"
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
                        {{ data.startDate }}
                    </template>
                </Column>
                <Column field="endDate" header="Ngày kết thúc " sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.endDate == '1970-01-01' ? 'Chưa giải quyết..' : data.endDate }}
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
                            class="mazin"
                            :disabled="canEdit(data.isDeleted, data.isFinished, data.userCreated)"
                        />

                        <Edit
                            v-if="data.isOnGitlab == false"
                            class="mazin"
                            @click="openDialogEdit(data)"
                            :disabled="canEdit(data.isDeleted, data.isFinished, data.userCreated)"
                        />

                        <Delete
                            class="mazin"
                            @click="confirmDelete(data.id)"
                            :disabled="canEdit(data.isDeleted, data.isFinished, data.userCreated)"
                        />

                        <Button
                            @click="finishProject(data.id)"
                            class="p-button-sm mt-1 mazin"
                            icon="pi pi-check"
                            :hidden="canFinished(data.isFinished, data.isDeleted)"
                            :disabled="
                                canFinished(data.isFinished) ||
                                canEdit(data.isDeleted, data.isFinished, data.userCreated)
                            "
                        />
                    </template>
                </Column>
                <Column field="isFinished" header="Finished" sortable>
                    <template #body="{ data }">
                        <span :class="'badge ' + colorFinished(data.isFinished)">{{ text(data.isFinished) }}</span>
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
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!</medium
            >
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
    import { HTTP } from '@/http-common'
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

    export default {
        data() {
            return {
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                userInfo: [],
                selectedProject: [],
                selectedColumns: null,
                columns: null,
                arr: [],
                loading: true,
                displayBasic: false,
                pageIndex: [5, 10, 15, 20],
                num: 5,
                timeout: null,
                isOpenDialog: false,
                projectSelected: new ProjectDto(),
                user: [],
                leader: [
                    {
                        id: 1,
                        firstName: 'Nguyễn',
                        lastName: 'Mạnh 1',
                    },
                    {
                        id: 2,
                        firstName: 'Nguyễn',
                        lastName: 'Mạnh 2',
                    },
                    {
                        id: 3,
                        firstName: 'Nguyễn',
                        lastName: 'Mạnh 3',
                    },
                ],
                acceptRole: ['pm', 'admin', 'director', 'leader'],
                dataProjects: [],
            }
        },
        async created() {
            await this.handlerGetInfoProjects()
        },
        async mounted() {
            try {
                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if (await UserRoleHelper.isAccess) {
                    await this.getUserInfo()
                    await UserRoleHelper.getUserRole(this.user, this.leader)
                } else {
                    this.countTime()
                    this.displayBasic = true
                }
                this.columns = [
                    { field: 'description', header: 'Description' },
                    { field: 'userId', header: 'Project manager' },
                    { field: 'leader', header: 'Leader' },
                    { field: 'userCreated', header: 'User created' },
                    { field: 'dateCreated', header: 'Date created' },
                    { field: 'userUpdate', header: 'User update' },
                    { field: 'dateUpdate', header: 'Date update' },
                ]
            } catch (error) {
                this.countTime()
                this.displayBasic = true
            }
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
                let bool = false
                if (this.selectedProject == null) {
                    this.showWarn('Vui lòng chọn một dự án để kết thúc!')
                    return
                }
                this.selectedProject.forEach((element) => {
                    if (!element.isFinished && !element.isDeleted) {
                        bool = true
                    }
                })
                if (bool) {
                    this.selectedProject.forEach((element) => {
                        this.finishProject(element.id)
                    })
                } else this.showWarn('Không thể hoàn thành dự án!')
                this.selectedProject = []
            },
            finishProject(id) {
                let userlogin = jwtDecode(localStorage.getItem('token'))
                let idUser = userlogin.Id
                HTTP.put('Project/FinishProject/' + id, { UserId: idUser })
                    .then((res) => {
                        if (res.status == 200) {
                            this.arr = []
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
            getUsername(userId) {
                if (userId && userId !== 0) {
                    for (var user of this.userInfo) {
                        if (user.id == userId) {
                            const arrName = user.lastName.split(' ')
                            var fullName = user.firstName.replace(' ', '') + ' '
                            arrName.forEach((x) => {
                                fullName += x.substring(0, 1) + '.'
                            })
                            return fullName
                        }
                    }
                }
            },
            getUserInfo() {
                HTTP.get('Users/getInfo').then(async (res) => {
                    if (res.data) {
                        this.userInfo = res.data
                        await this.getAllProject()
                    }
                })
            },
            getAllProject() {
                this.arr = []
                HTTP.get('Project/getAllProject').then((res) => {
                    if (res.data) {
                        const result = res.data
                        let temp = []
                        result.forEach((element) => {
                            element.startDate = this.formatDate(element.startDate)
                            element.endDate = this.formatDate(element.endDate)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateUpdate = this.formatDate(element.dateUpdate)
                            element.userCreated = this.getUsername(element.userCreated)
                            element.userUpdate = this.getUsername(element.userUpdate)
                            element.userId = this.getUsername(element.userId)
                            element.leader = this.getUsername(element.leader)
                            temp.push(element)
                        })
                        this.arr = temp
                    }
                    this.loading = false
                })
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
            canEdit(isDeleted, isFinished, userCreated) {
                let userlogin = jwtDecode(localStorage.getItem('token'))
                let idPM = this.getUsername(userlogin.Id)
                if (isDeleted == true || isFinished == true || userCreated != idPM) return true
                else return false
            },
            canFinished(isFinished, isDeleted) {
                return !!(isFinished || isDeleted)
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
            showMember() {
                location.reload()
            },
            text(value) {
                if (value == true) return 'close'
                return 'open'
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
