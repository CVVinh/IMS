<template>
    <layout-default-dynamic>
        <div class="wrap">
            <h2>Sửa</h2>
            <form class="input-form">
                <div class="form-group">
                    <div class="form-element">
                        <label for="taskparent">Công việc chính </label>
                        <Dropdown
                            id="taskparent"
                            style="width: 25rem"
                            v-model="data.selectedTaskParent"
                            :options="tasklist"
                            optionLabel="taskName"
                            optionValue="idTask"
                            placeholder="Task parent"
                        />
                    </div>
                    <div class="form-element">
                        <label for="taskname">Tên công việc</label>
                        <InputText type="text" v-model="data.taskName" id="taskname" style="width: 25rem" />
                    </div>
                </div>

                <div class="form-group">
                    <div class="form-element">
                        <label for="description">Mô tả</label>
                        <InputText id="description" v-model="data.description" type="text" style="width: 25rem" />
                    </div>
                    <div class="form-element">
                        <label for="status">Trạng thái</label>
                        <Dropdown
                            id="status"
                            style="width: 25rem"
                            v-model="data.selectedStatus"
                            :options="status"
                            optionLabel="name"
                            optionValue="code"
                            placeholder="Trạng thái"
                        />
                    </div>
                </div>
                <div class="form-group">
                    <div class="form-element">
                        <label for="endTaskDate">Ngày bắt đầu</label>
                        <Calendar
                            id="startTaskDate"
                            v-model="data.startTaskDate"
                            :showIcon="true"
                            style="width: 25rem"
                        />
                    </div>
                    <div class="form-element">
                        <label for="endTaskDate">Ngày kết thúc</label>
                        <Calendar id="endTaskDate" v-model="data.endTaskDate" :showIcon="true" style="width: 25rem" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="button-group">
                        <Button id="btn-add" label="Lưu" @click="editTask()" />
                        <Button id="btn-cancel" label="Hủy" class="p-button-secondary" @click="backToTasksPage()" />
                    </div>
                </div>
            </form>
        </div>
    </layout-default-dynamic>
</template>
<script>
    import { HTTP } from '@/http-common.js'
    import moment from 'moment'
    export default {
        data() {
            return {
                status: [
                    { name: 'Đang mở', code: 1 },
                    { name: 'Ưu tiên', code: 2 },
                    { name: 'Đang làm', code: 3 },
                    { name: 'Đã xong', code: 4 },
                    { name: 'Lỗi', code: 5 },
                    { name: 'Đã xóa', code: 6 },
                    { name: 'Chưa giải quyết', code: 7 },
                ],
                tasklist: [],
                data: {
                    idTask: null,
                    taskName: null,
                    description: null,
                    startTaskDate: null,
                    endTaskDate: null,
                    status: null,
                    selectedTaskParent: null,
                },
                selectedStatus: null,
                selectedTaskParent: null,
            }
        },

        mounted() {
            this.getAllTask()
            this.getTaskById()
        },
        methods: {
            getAllTask() {
                HTTP.get('tasks/getAllTaskByIdProject/' + parseInt(localStorage.getItem('idProject')))
                    .then((response) => {
                        this.tasklist = response.data._Data
                    })
                    .catch((error) => {})
            },
            backToTasksPage() {
                this.$router.push({ name: 'tasks' })
            },
            editTask() {
                let data = {
                    idTask: parseInt(localStorage.getItem('idTask')),
                    taskName: this.data.taskName,
                    description: this.data.description,
                    startTaskDate: moment(this.data.startTaskDate).format('YYYY-MM-DD'),
                    endTaskDate: moment(this.data.endTaskDate).format('YYYY-MM-DD'),
                    status: this.data.selectedStatus,
                    idParent: this.data.selectedTaskParent,
                }
                HTTP.put('tasks/editTaskById/' + parseInt(localStorage.getItem('idTask')), data)
                    .then((response) => {
                        this.$router.push({ name: 'tasks' })
                    })
                    .catch((error) => {})
            },

            getTaskById() {
                HTTP.get('tasks/getAllTasksById/' + parseInt(localStorage.getItem('idTask')))
                    .then((response) => {
                        this.data.idTask = response.data._Data.idTask
                        this.data.taskName = response.data._Data.taskName
                        this.data.description = response.data._Data.description
                        this.data.startTaskDate = moment(response.data._Data.startTaskDate).format('YYYY-MM-DD')
                        this.data.endTaskDate = moment(response.data._Data.endTaskDate).format('YYYY-MM-DD')
                        this.data.selectedStatus = response.data._Data.status
                        this.data.selectedTaskParent = response.data._Data.idParent
                    })
                    .catch((error) => {})
            },
        },
    }
</script>
<style scoped>
    .wrap {
        padding-top: 50px;
        width: 50%;
        margin: 0 auto;
        height: 700px;
        /* border: 1px solid #333; */
    }

    .form-group {
        margin-bottom: 1em;

        width: 100%;
        display: flex;
        justify-content: space-between;
    }

    .form-element {
        width: 100%;
    }

    label {
        width: 30%;
        font-size: 1em;
    }

    h2 {
        margin-bottom: 1em;
        text-align: center;
    }

    .button-group {
        width: 100%;
        display: flex;
        justify-content: flex-end;
    }

    #btn-add {
        margin-right: 20px;
    }

    #btn-cancel {
        margin-right: 55px;
    }
</style>
