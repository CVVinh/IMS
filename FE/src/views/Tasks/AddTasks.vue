<template>
    <LayoutDefaultDynamic>
        <div class="wrap">
            <h2>Add new task</h2>
            <form class="input-form">
                <div class="form-group">
                    <div class="form-element">
                        <label for="taskparent">Task parent</label>
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
                        <label for="taskname">Task name</label>
                        <InputText type="text" v-model="taskName" id="taskname" style="width: 25rem" />
                    </div>
                </div>

                <div class="form-group">
                    <div class="form-element">
                        <label for="description">Description</label>
                        <InputText id="description" v-model="description" type="text" style="width: 25rem" />
                    </div>
                    <div class="form-element">
                        <label for="status">Status</label>
                        <Dropdown
                            id="status"
                            style="width: 25rem"
                            v-model="selectedStatus"
                            :options="status"
                            optionLabel="name"
                            optionValue="code"
                            placeholder="Status"
                        />
                    </div>
                </div>
                <div class="form-group">
                    <div class="form-element">
                        <label for="endTaskDate">Date start task</label>
                        <Calendar id="startTaskDate" v-model="startTaskDate" :showIcon="true" style="width: 25rem" />
                    </div>
                    <div class="form-element">
                        <label for="endTaskDate">Date end task</label>
                        <Calendar id="endTaskDate" v-model="endTaskDate" :showIcon="true" style="width: 25rem" />
                    </div>
                </div>
                <div class="form-group">
                    <div class="button-group">
                        <Button id="btn-add" label="Add" @click="addTask()" />
                        <Button id="btn-cancel" label="Cancel" class="p-button-secondary" @click="backToTasksPage()" />
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
                    { name: 'Open', code: 1 },
                    { name: 'Priority', code: 2 },
                    { name: 'Progress', code: 3 },
                    { name: 'Done', code: 4 },
                    { name: 'Error', code: 5 },
                    { name: 'Deleted', code: 6 },
                    { name: 'Pending', code: 7 },
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
                            summary: 'Success Message',
                            detail: 'Add task successfully !',
                            life: 3000,
                        })
                    })
                    .catch((error) => {
                        this.$toast.add({
                            severity: 'warn',
                            summary: 'Warning Message',
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
