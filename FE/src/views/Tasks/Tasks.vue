<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                :paginator="true"
                ref="dt"
                class="p-datatable-customers"
                :rows="10"
                dataKey="idTask"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedTask"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="[
                    'idTask',
                    'idParent',
                    'taskName',
                    'description',
                    'isDeleted',
                    'status',
                    'tag',
                    'startTaskDate',
                    'endTaskDate',
                    'createTaskDate',
                    'createUser',
                    'idProject',
                ]"
                responsiveLayout="scroll"
            >
                <!-- Header -->
                <template #header>
                    <div class="flex justify-content-center align-items-center">
                        <h5 class="" style="color: white">Danh sách công việc</h5>
                        <div class="inline">
                            <Dropdown
                                class="p-inputtext-sm"
                                v-model="selectedProject"
                                :options="projects"
                                optionLabel="name"
                                optionValue="id"
                                placeholder="Chọn dự án"
                                style="width: 15rem; margin-right: 15px"
                                @change="this.getTaskByProject()"
                            />
                            <Export label="Xuất Excel" @click="exportCSV($event)" /> &nbsp;
                            <Add label="Add Task" @click="toAddTask()" v-if="this.booleanProject" />
                            <span class="p-input-icon-left">
                                <i class="pi pi-search" />
                                <InputText v-model="filters['global'].value" placeholder="Tìm kiếm" />
                            </span>
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>

                <!-- Body -->
                <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>
                <Column field="id" header="#" sortable style="min-width: 3rem">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="taskName" header="Tên" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.taskName }}
                    </template>
                </Column>
                <Column field="description" header="Mô tả" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.description }}
                    </template>
                </Column>
                <Column field="status" header="Trạng thái" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ this.status[data.status] }}
                    </template>
                </Column>
                <Column
                    field="startTaskDate"
                    header="Ngày bắt đầu (Dự kiến)"
                    sortable
                    dataType="date"
                    style="min-width: 8rem"
                >
                    <template #body="{ data }">
                        {{ formatDate(data.startTaskDate) }}
                    </template>
                </Column>
                <Column
                    field="endTaskDate"
                    header="Ngày kết thúc (Dự kiến)"
                    sortable
                    dataType="date"
                    style="min-width: 8rem"
                >
                    <template #body="{ data }">
                        {{ formatDate(data.endTaskDate) }}
                    </template>
                </Column>
                <Column field="createTaskDate" header="Ngày tạo" sortable dataType="date" style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ formatDate(data.createTaskDate) }}
                    </template>
                </Column>
                <Column field="createUser" header="Người tạo" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ getUsername(data.createUser) }}
                    </template>
                </Column>
                <Column field="idParent" header="Mã dự án" sortable style="min-width: 7rem">
                    <template #body="{ data }">
                        {{ data.idParent }}
                    </template>
                </Column>
                <!-- <Column field="taskCode" header="Task code" sortable style="min-width: 7rem">
                    <template #body="{ data }">
                        {{ data.taskCode }}
                    </template>
                </Column> -->
                <Column field="" header="&emsp;&emsp;&emsp;Thực thi" style="min-width: 18rem">
                    <template #body="{ data }">
                        <Edit label="Xuất Excel" @click="toEditTask(data.idTask)" :disabled="canEdit(data.status)" />
                        &nbsp;
                        <Delete label="Xóa" @click="confirmDelete(data.idTask)" :disabled="canEdit(data.status)" />
                    </template>
                </Column>
            </DataTable>
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common.js'
    import { FilterMatchMode, FilterOperator } from 'primevue/api'
    import Add from '../../components/buttons/Add.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import Export from '../../components/buttons/Export.vue'
    // import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue';

    export default {
        data() {
            return {
                data: [],
                selectedTask: [],
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                status: ['', 'Đang mở', 'Ưu tiên', 'Đang làm', 'Đã xong', 'Lỗi', 'Đã xóa', 'Chưa giải quyết'],
                userInfo: [],
                projects: [],
                selectedProject: null,
                booleanProject: false,
            }
        },
        mounted() {
            this.getUserInfo()
            this.getProjects()
        },
        methods: {
            formatDate(value) {
                return new Date(value).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
            },
            // formatCurrency(value) {
            //     return value.toLocaleString('en-US', { style: 'currency', currency: 'USD' });
            // },
            getTaskByProject() {
                this.data = []
                this.booleanProject = true
                localStorage.setItem('idProject', this.selectedProject)
                HTTP.get('tasks/getAllTaskByIdProject/' + this.selectedProject)
                    .then((response) => {
                        this.data = response.data._Data
                    })
                    .catch((error) => {
                        this.$toast.add({
                            severity: 'info',
                            summary: 'Thông tin ',
                            detail: error.response.data,
                            life: 2000,
                        })
                    })
            },
            toEditTask(idTask) {
                localStorage.setItem('idTask', idTask)
                this.$router.push({ name: 'edittask' })
            },
            canEdit(status) {
                if (status == 6) return true
                else return false
            },
            toAddTask() {
                this.$router.push({ name: 'addtask' })
            },
            confirmDelete(idTask) {
                this.$confirm.require({
                    message: 'Bạn có chắc chắn muốn xóa?',
                    header: 'Xóa',
                    icon: 'pi pi-info-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteTask(idTask)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            deleteTask(idTask) {
                let API_URL = 'tasks/deletedTask/' + idTask
                HTTP.put(API_URL).then((res) => {
                    this.showSuccess()
                    this.getTaskByProject()
                })
            },
            exportCSV() {
                this.$refs.dt.exportCSV()
            },
            colorDeleted(status) {
                if (status == false) return 'bg-success'
                return 'bg-danger'
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Xóa thành công!',
                    life: 3000,
                })
            },
            getUserInfo() {
                HTTP.get('Users/getInfo').then((res) => {
                    if (res.data) {
                        this.userInfo = res.data
                    }
                })
            },
            getUsername(userId) {
                if (userId && userId !== 0) {
                    for (var user of this.userInfo) {
                        if (user.id === userId) {
                            return user.lastName + ' ' + user.firstName
                        }
                    }
                }
            },
            getProjects() {
                HTTP.get('Project/getAllProject').then((res) => {
                    if (res.data) {
                        this.projects = res.data
                    }
                })
            },
            getProjectName(idProject) {
                for (var p of this.projects) {
                    if (p.id === idProject) {
                        return p.name
                    }
                }
            },
        },
        components: { Add, Edit, Delete, Export },
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

    .accept {
        float: right;
    }

    .p-input-icon-left {
        float: right;
    }
</style>
