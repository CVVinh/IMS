<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                :sort="1"
                :paginator="true"
                ref="dt"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedUser"
                filterDisplay="menu"
                responsiveLayout="scroll"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    'userCode',
                    'userCreated',
                    'dateCreated',
                    'userModified',
                    'dateModified',
                    'fullName',
                    'phoneNumber',
                    'dOB',
                    'gender',
                    'address',
                    'university',
                    'yearGraduated',
                    'email',
                    'skype',
                    'dateStartWork',
                    'idGroup',
                    'workStatus',
                ]"
            >
                <template #header>
                    <h5 style="color: white">Danh sách người dùng</h5>
                    <div class="header-container">
                        <div class="button-group">
                            <Add
                                label="Thêm"
                                :disabled="this.disableAddButton"
                                @click="OpenAdd"
                                style="margin-right: 5px"
                            />
                            <Export label="Xuất Excel" @click="exportCSV($event)" />
                        </div>
                        <div class="input-text">
                            <MultiSelect
                                v-model="selectedColumns"
                                :options="columns"
                                optionLabel="header"
                                placeholder="Chọn"
                                style="width: 20em; height: 100%; margin-right: 15px"
                            />
                            <span class="p-input-icon-left">
                                <i class="pi pi-search" />
                                <InputText
                                    class="p-inputtext-sm"
                                    v-model="filters['global'].value"
                                    placeholder="Tìm kiếm"
                                    style="width: 20em; height: 100%; font-size: 16px"
                                />
                            </span>
                        </div>
                    </div>
                </template>
                <template #loading> </template>
                <template #empty>
                    <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                        <ProgressSpinner style="width: 42px" />
                    </div>
                    <div v-else>Không tìm thấy.</div>
                </template>
                <Column field="#" header="#" dataType="date">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="lastName" header="Họ và tên" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.fullName }}
                    </template>
                </Column>
                <Column field="email" header="Email" sortable>
                    <template #body="{ data }">
                        {{ data.email }}
                    </template>
                </Column>
                <Column field="idGroup" header="Nhóm" sortable>
                    <template #body="{ data }">
                        {{ data.idGroup }}
                    </template>
                </Column>
                <Column field="userCode" header="Tài khoản" sortable>
                    <template #body="{ data }">
                        {{ data.userCode }}
                    </template>
                </Column>
                <Column field="workStatus" header="Trạng thái làm việc" sortable>
                    <template #body="{ data }">
                        {{ data.workStatus }}
                    </template>
                </Column>
                <Column
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="index"
                    style="min-width: 14rem"
                ></Column>
                <Column field="" header="Thực thi" style="width: 10rem; text-align: left">
                    <template #body="{ data }">
                        <div class="actions-buttons" v-if="data.workStatus !== 'Nghỉ việc'">
                            <Edit @click="OpenEdit(data.id)" />
                            &nbsp;
                            <Delete @click="confirmDelete(data.id, data.workStatus)" />
                        </div>
                    </template>
                </Column>
            </DataTable>
        </div>

        <Dialog
            header="Không có quyền truy cập !"
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

        <AddUserDiaLog
            :statusopen="openAddform"
            @closeAdd="closeAdd()"
            :roleoption="Optionrole"
            @reloadpage="getData()"
        />
        <EditUserDiaLog
            :statusopen="OpenEditform"
            @closeModal="closeModal()"
            :iduser="Iduser"
            :roleoption="Optionrole"
            @reloadpage="getData"
        />
    </LayoutDefaultDynamic>
</template>

<script>
    import Edit from '../../components/buttons/Edit.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import { HTTP } from '@/http-common'
    import Add from '../../components/buttons/Add.vue'
    import Export from '../../components/buttons/Export.vue'
    import { FilterMatchMode } from 'primevue/api'
    import AddUserDiaLog from './AddUserDiaLog.vue'
    import EditUserDiaLog from './EditUserDiaLog.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HttpStatus } from '@/config/app.config'
    export default {
        name: 'users',
        data() {
            return {
                isLoading: true,
                openAddform: false,
                OpenEditform: false,
                Iduser: null,
                selectedUser: null,
                data: [],
                idArr: [],
                usercodeArr: [],
                isAsc: false,
                isDesc: false,
                isSort: false,
                decode: null,
                disableAddButton: true,
                disableEditButton: true,
                disableDeleteButton: true,
                displayBasic: false,
                num: 5,
                timeOut: null,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                selectedColumns: null,
                columns: [
                    { field: 'dOB', header: 'Ngày sinh' },
                    { field: 'gender', header: 'Giới tính' },
                    { field: 'identitycard', header: 'Căn cước công dân' },
                    { field: 'phoneNumber', header: 'Số điện thoại' },
                    { field: 'address', header: 'Địa chỉ' },
                    { field: 'dateStartWork', header: 'Ngày bắt đầu làm việc' },
                    { field: 'university', header: 'Đại học' },
                    { field: 'yearGraduated', header: 'Năm tốt nghiệp' },
                    { field: 'skype', header: 'Skype' },
                    { field: 'maritalStatus', header: 'Tình trạng hôn nhân' },
                    { field: 'dateLeave', header: 'Ngày thôi việc' },
                    { field: 'userCreated', header: 'Người tạo ' },
                    { field: 'dateCreated', header: 'Ngày tạo ' },
                    { field: 'userModified', header: 'Người chỉnh sửa ' },
                    { field: 'dateModified', header: 'Ngày chỉnh sửa ' },
                ],
                Optionrole: [],
                roleList: [],
            }
        },
        async mounted() {
            try {
                this.decode = LocalStorage.jwtDecodeToken()
                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if (UserRoleHelper.isAccess) {
                    await this.getData()
                    await this.getRoles()
                    this.disableAddButton = false
                } else {
                    this.countTime()
                    this.displayBasic = true
                }
            } catch (error) {
                this.countTime()
                this.displayBasic = true
            }
        },
        methods: {
            OpenAdd() {
                this.openAddform = true
            },

            closeAdd() {
                this.openAddform = false
            },

            OpenEdit(id) {
                this.Iduser = id
                this.OpenEditform = true
            },

            closeModal() {
                this.OpenEditform = false
            },

            getRoles() {
                HTTP.get('Group/getListGroup').then((res) => {
                    this.Optionrole = res.data
                    this.roleList = res.data
                })
            },

            getData() {
                HTTP.get('Users/getAll').then((res) => {
                    if (res.status == 200) {
                        const temp = res.data

                        temp.forEach((element) => {
                            if (element.isDeleted == 0) {
                                this.data.push({ ...element, fullName: '' })
                            }
                        })
                        this.data.forEach((element) => {
                            element.fullName = this.mergeString(element.lastName, element.firstName)
                            element.dateStartWork = this.formatDate(element.dateStartWork)
                            element.dateLeave = this.formatDate(element.dateLeave)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateModified = this.formatDate(element.dateModified)
                            element.dOB = this.formatDate(element.dOB)
                            element.workStatus = this.getWorkStatus(element.workStatus)
                            element.gender = this.getGender(element.gender)
                            element.maritalStatus = this.getMaritalStatus(element.maritalStatus)
                            temp.forEach((user) => {
                                if (user.id === element.userCreated)
                                    element.userCreated = user.lastName + ' ' + user.firstName
                                if (user.id === element.userModified)
                                    element.userModified = user.lastName + ' ' + user.firstName
                            })
                        })
                        this.isLoading = false
                    }
                })
            },
            countTime() {
                if (this.num == 0) {
                    this.submit()
                    return
                }
                this.num = this.num - 1
                this.timeOut = setTimeout(() => this.countTime(), 1000)
            },
            submit() {
                clearTimeout(this.timeOut)
                this.$router.push({ name: 'home' })
            },
            getGender(index) {
                if (index == 1) return 'Nam'
                else if (index == 2) return 'Nữ'
                else if (index == 3) return 'Khác'
            },
            getWorkStatus(index) {
                if (index == 1) return 'Đang làm việc'
                else if (index == 2) return 'Nghỉ việc'
                else if (index == 3) return 'Nghỉ thai sản'
            },
            getMaritalStatus(index) {
                if (index == 1) return 'Đã kết hôn'
                else if (index == 2) return 'Chưa kết hôn'
                else if (index == 3) return 'Chưa xác định'
            },
            toAddUserPage() {
                this.$router.push({ name: 'adduser' })
            },
            toEditUserPage(id) {
                this.$router.push({ name: 'edituser', params: { id: id } })
            },
            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteUser(id)
                    },
                    reject: () => {
                        return
                    },
                })
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
            deleteUser(userId) {
                let API_URL = 'Users/deleteUser/' + userId
                HTTP.put(API_URL, {
                    userModified: this.decode.Id,
                    dateModified: new Date(),
                    isDeleted: 1,
                })
                    .then((res) => {
                        if (res.status == HttpStatus.OK) {
                            this.data = []
                            this.getData()
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Xóa thành công!',
                                life: 2000,
                            })
                        }
                    })
                    .catch((error) => {
                        this.$toast.add({ severity: 'warn', summary: 'Cảnh báo ', detail: error, life: 2000 })
                    })
            },

            exportCSV() {
                this.$refs.dt.exportCSV()
            },
            mergeString(str1, str2) {
                return str1 + ' ' + str2
            },
        },
        components: { Export, Add, Edit, Delete, AddUserDiaLog, EditUserDiaLog },
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

        .p-datatable-header {
            background-color: #607d8b;
        }
    }

    .input-text {
        display: flex;
        height: 45px;
    }

    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1rem;
        height: 40px;
    }

    .actions-buttons {
        display: flex;
        width: 110px;
        justify-content: space-evenly;
    }
</style>
