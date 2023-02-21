<template>
    <LayoutDefaultDynamic>
        <div class="wrap">
            <h2>Thêm</h2>
            <form class="input-form">
                <div class="form-group">
                    <div class="form-element">
                        <label for="taskparent">Công việc chính </label>
                        <Dropdown
                            id="taskparent"
                            style="width: 25rem"
                            v-model="selectedTaskParent"
                            :options="tasklist"
                            optionLabel="taskName"
                            optionValue="idTask"
                            placeholder="Task parent"
                        />
                    </div>
                    <div class="form-element">
                        <label for="taskname">Tên công việc</label>
                        <InputText type="text" v-model="taskName" id="taskname" style="width: 25rem" />
                    </div>
                </div>

                <div class="form-group">
                    <div class="form-element">
                        <label for="description">Mô tả</label>
                        <InputText id="description" v-model="description" type="text" style="width: 25rem" />
                    </div>
                    <div class="form-element">
                        <label for="status">Trạng thái</label>
                        <Dropdown
                            id="status"
                            style="width: 25rem"
                            v-model="selectedStatus"
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
                        <Calendar id="startTaskDate" v-model="startTaskDate" :showIcon="true" style="width: 25rem" />
                    </div>
                    <div class="form-element">
                        <label for="endTaskDate">Ngày kết thúc</label>
                        <Calendar id="endTaskDate" v-model="endTaskDate" :showIcon="true" style="width: 25rem" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="button-group">
                        <Button id="btn-add" label="Thêm" @click="addTask()" />
                        <Button id="btn-cancel" label="Hủy" class="p-button-secondary" @click="backToTasksPage()" />
                    </div>
                </div>
            </form>
        </div>
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP } from '@/http-common.js'
    import jwt_decode from 'jwt-decode'
    import moment from 'moment'
    import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
    import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    export default {
        data() {
            return {
                taskParent: null,
                taskName: null,
                description: null,
                startTaskDate: null,
                endTaskDate: null,
                selectedTaskParent: null,
                selectedStatus: null,
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
            }
        },
        mounted() {
            this.getAllTask()
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
            addTask() {
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)

                let task = {
                    idTaskParent: this.selectedTaskParent,
                    taskName: this.taskName,
                    description: this.description,
                    startTaskDate: moment(this.startTaskDate).format('YYYY-MM-DD'),
                    endTaskDate: moment(this.endTaskDate).format('YYYY-MM-DD'),
                    status: this.selectedStatus,
                    createUser: decode.Id,
                    idProject: parseInt(localStorage.getItem('idProject')),
                }
                HTTP.post('tasks/addNewTask', task)
                    .then((response) => {
                        this.$router.push({ name: 'tasks' })
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Thêm mới thành công!',
                            life: 3000,
                        })
                    })
                    .catch((error) => {
                        this.$toast.add({
                            severity: 'warn',
                            summary: 'Cảnh báo ',
                            detail: error.response.data._Message,
                            life: 3000,
                        })
                    })
            },
        },
        components: { LayoutDefault, LayoutDefaultDynamic },
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
        margin-top: 2em;
    }

    #btn-add {
        margin-right: 20px;
    }

    #btn-cancel {
        margin-right: 32px;
    }
</style>
