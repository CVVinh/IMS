<template>
    <Dialog v-model:visible="this.display" :closable="false" :show="getData()">
        <template #header v-if="!edit">
            <h2>{{ !edit ? 'Thêm thẻ tăng ca mới' : 'Chỉnh sửa thẻ tăng ca ' }}</h2>
        </template>

        <div class="container mt-4">
            <div class="container text-center">
                <div class="container text-center">
                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                        <div class="row">
                            <div class="col">
                                <!-- Ngay OT -->
                                <div class="field">
                                    <div :class="{ 'form-group--error': v$.data.date.$error }">
                                        <label
                                            for="date"
                                            class="label"
                                            :class="{ 'p-error': (v$.data.date.$invalid || !checkday()) && submitted }"
                                            >Ngày OT*</label
                                        >
                                        <Calendar
                                            id="date"
                                            v-model="v$.data.date.$model"
                                            :minDate="new Date()"
                                            :showIcon="true"
                                            :date-select="changeDate"
                                            :class="{
                                                'p-invalid': (v$.data.date.$invalid || !checkday()) && submitted,
                                            }"
                                        />
                                    </div>
                                    <small
                                        v-if="(v$.data.date.$invalid && submitted) || v$.data.date.$pending.$response"
                                        class="p-error"
                                    >
                                        Bạn chưa nhập ngày OT
                                    </small>
                                    <small v-if="!v$.data.date.$invalid && !checkday() && submitted" class="p-error">
                                        Ngày OT phải là ngày mai trở đi
                                    </small>
                                </div>
                            </div>
                            <div class="col">
                                <!-- Du an -->
                                <div class="field">
                                    <div :class="{ 'form-group--error': v$.data.idProject.$error }">
                                        <label
                                            for="idProject"
                                            class="label"
                                            :class="{ 'p-error': v$.data.idProject.$invalid && submitted }"
                                            >Dự án*</label
                                        >
                                        <Dropdown
                                            v-model="v$.data.idProject.$model"
                                            :options="project"
                                            optionLabel="name"
                                            optionValue="id"
                                            :class="{ 'p-invalid': v$.data.idProject.$invalid && submitted }"
                                        />
                                    </div>
                                    <small
                                        v-if="
                                            (v$.data.idProject.$invalid && submitted) ||
                                            v$.data.idProject.$pending.$response
                                        "
                                        class="p-error"
                                        >Bạn chưa chon dự án OT</small
                                    >
                                </div>
                            </div>
                            <div class="col">
                                <!-- Edit -->
                                <div class="field" style="margin-bottom: 0" v-if="this.idproject !== null">
                                    <div :class="{ 'form-group--error': v$.data.user.$error }">
                                        <label
                                            for="user"
                                            class="label"
                                            :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                            >Người tăng ca</label
                                        >
                                        <Dropdown
                                            v-model="v$.data.user.$model"
                                            :options="userDropdown"
                                            optionLabel="name"
                                            optionValue="x.id"
                                            :class="{ 'p-invalid': v$.data.user.$invalid && submitted }"
                                            disabled
                                        />
                                    </div>
                                    <small
                                        v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                        class="p-error"
                                        >Bạn chưa chọn người OT</small
                                    >
                                </div>

                                <!-- Add -->
                                <div class="field" style="margin-bottom: 0" v-if="this.idproject === null">
                                    <div :class="{ 'form-group--error': v$.data.user.$error }">
                                        <label
                                            for="user"
                                            class="label"
                                            :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                            >Người tăng ca</label
                                        >
                                        <MultiSelect
                                            v-model="v$.data.user.$model"
                                            :options="userDropdown"
                                            optionLabel="name"
                                            optionValue="x.id"
                                            :class="{ 'p-invalid': v$.data.user.$invalid && submitted }"
                                        />
                                    </div>
                                    <small
                                        v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                        class="p-error"
                                        >Bạn chưa chọn người OT</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="row top">
                            <div class="col">
                                <!-- Gio bat dau -->
                                <div class="field" style="margin-bottom: 0">
                                    <div :class="{ 'form-group--error': v$.data.start.$error }">
                                        <label
                                            for="start"
                                            class="label"
                                            :class="{
                                                'p-error':
                                                    (v$.data.start.$invalid ||
                                                        timeError ||
                                                        invalidTime1 ||
                                                        invalidTime3) &&
                                                    submitted,
                                            }"
                                            >Giờ bắt đầu OT*</label
                                        >
                                        <InputText
                                            id="start"
                                            v-model="v$.data.start.$model"
                                            type="time"
                                            timeformat="24h"
                                            :class="{
                                                'p-invalid':
                                                    v$.data.start.$invalid ||
                                                    (submitted && invalidTime1) ||
                                                    (submitted && invalidTime3),
                                            }"
                                        />
                                    </div>
                                    <span v-if="v$.data.start.$error && submitted">
                                        <span
                                            id="start-error"
                                            v-for="(error, index) of v$.data.start.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{
                                                error.$message.replace('Value', 'Start Time') + '. '
                                            }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.data.start.$invalid && submitted) || v$.data.start.$pending.$response
                                        "
                                        class="p-error"
                                        >Bạn chưa nhập giờ bắt đầu OT</small
                                    >
                                    <small v-if="!v$.data.start.$invalid && invalidTime1 && submitted" class="p-error">
                                        Giờ OT phải khác giờ làm việc
                                    </small>
                                </div>
                            </div>
                            <div class="col">
                                <!-- Gio ket thuc -->
                                <div class="field">
                                    <div :class="{ 'form-group--error': v$.data.end.$error }">
                                        <label
                                            for="end"
                                            class="label"
                                            :class="{
                                                'p-error':
                                                    (v$.data.end.$invalid ||
                                                        timeError ||
                                                        invalidTime2 ||
                                                        invalidTime3) &&
                                                    submitted,
                                            }"
                                            >Giờ kết thúc OT*</label
                                        >
                                        <InputText
                                            id="end"
                                            v-model="v$.data.end.$model"
                                            type="time"
                                            :class="{
                                                'p-invalid':
                                                    v$.data.end.$invalid ||
                                                    (submitted && timeError) ||
                                                    (submitted && invalidTime2) ||
                                                    (submitted && invalidTime3),
                                            }"
                                        />
                                    </div>
                                    <small
                                        v-if="(v$.data.end.$invalid && submitted) || v$.data.end.$pending.$response"
                                        class="p-error"
                                        >Bạn chưa nhập giờ kết thúc OT</small
                                    >
                                    <small v-if="!v$.data.end.$invalid && timeError && submitted" class="p-error">
                                        Giờ kết thúc phải lớn hơn giờ bắt đầu OT
                                    </small>
                                    <small v-if="!v$.data.start.$invalid && invalidTime2 && submitted" class="p-error">
                                        Giờ OT phải khác giờ làm việc
                                    </small>
                                </div>
                            </div>
                            <div class="col">
                                <!-- Thoi gian thuc te -->
                                <div class="field">
                                    <div :class="{ 'form-group--error': v$.data.realTime.$error }">
                                        <label
                                            for="realTime"
                                            class="label"
                                            :class="{ 'p-error': v$.data.realTime.$invalid && submitted }"
                                            >Thời gian OT thực tế</label
                                        >
                                        <InputText
                                            id="realTime"
                                            v-model="v$.data.realTime.$model"
                                            :disabled="true"
                                            :class="{ 'p-invalid': v$.data.realTime.$invalid && submitted }"
                                            :value="changeTime"
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <!-- Mota  -->
                        <div class="row top">
                            <div class="col">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.data.description.$error }"
                                    >
                                        <Textarea
                                            id="description"
                                            v-model="v$.data.description.$model"
                                            :showIcon="true"
                                            :class="{ 'p-invalid': v$.data.description.$invalid && submitted }"
                                        />
                                        <label
                                            for=" description"
                                            :class="{ 'p-error': v$.data.description.$invalid && submitted }"
                                            >Mô tả*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.data.description.$invalid && submitted) ||
                                            v$.data.description.$pending.$response
                                        "
                                        class="p-error"
                                        >Bạn chưa nhập mô tả</small
                                    >
                                </div>
                            </div>
                        </div>
                        <!--  Button -->
                        <div class="row top">
                            <div class="col"></div>
                            <div class="col"></div>
                            <div class="col">
                                <div class="btn-right justify-content-end">
                                    <button type="submit" style="line-height: revert" class="btn btn-primary">
                                        Xác nhận</button
                                    >&nbsp;
                                    <button
                                        type="button"
                                        style="line-height: revert"
                                        class="btn btn-outline-secondary"
                                        v-on:click="closeForm"
                                    >
                                        Quay lại
                                    </button>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </Dialog>
</template>

<script>
    import jwtDecode from 'jwt-decode'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import moment from 'moment'
    import { required } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
    import router from '@/router'
    import axios from 'axios'
    import { HTTP } from '@/http-common'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
import checkAccessModule from '@/stores/checkAccessModule'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: ['display', 'idproject'],
        data() {
            return {
                hello: true,
                data: {
                    type: 0,
                    date: '',
                    start: null,
                    end: null,
                    realTime: null,
                    status: 0,
                    description: '',
                    leadCreate: null,
                    dateCreate: '',
                    updateUser: '',
                    dateUpdate: '',
                    note: '',
                    user: null,
                    idProject: null,
                },
                token: null,
                userDropdown: [],
                project: [],
                edit: false,
                submitted: false,
                time: null,
                valid: true,
                timeError: true,
                invalidTime1: true,
                invalidTime2: true,
                invalidTime3: false,
                statuses: [
                    { num: 0, text: 'Waiting' },
                    { num: 1, text: 'Accepted' },
                    { num: 2, text: 'Denied' },
                    { num: 3, text: 'Deleted' },
                ],
                //axios: 'http://api.imsdemo.tk/api/',
                axios: import.meta.env.VITE_VUE_API_URL,
            }
        },
        validations() {
            return {
                data: {
                    date: {
                        required,
                    },
                    start: {
                        required,
                    },
                    end: {
                        required,
                    },
                    realTime: {
                        required,
                    },
                    description: {
                        required,
                    },
                    user: {
                        required,
                    },
                    idProject: {
                        required,
                    },
                },
            }
        },
        computed: {
            changeTime() {
                if (this.data.start != null && this.data.end != null) {
                    let start_hour = parseInt(this.data.start.substring(0, 2))
                    let start_minute = parseInt(this.data.start.substring(3, 5))
                    let end_hour = parseInt(this.data.end.substring(0, 2))
                    let end_minute = parseInt(this.data.end.substring(3, 5))

                    if ((8 <= start_hour && start_hour < 12) || (14 <= start_hour && start_hour < 17))
                        this.invalidTime1 = true
                    else if ((start_hour == 13 && start_minute >= 30) || (start_hour == 17 && start_minute <= 30))
                        this.invalidTime1 = true
                    else this.invalidTime1 = false
                    if ((8 < end_hour && end_hour < 12) || (14 <= end_hour && end_hour < 17)) this.invalidTime2 = true
                    else if ((end_hour == 13 && end_minute >= 30) || (end_hour == 17 && end_minute <= 30))
                        this.invalidTime2 = true
                    else this.invalidTime2 = false
                    if ((start_hour < 8 && end_hour >= 12) || (12 <= start_hour && start_hour <= 13 && end_hour > 17))
                        this.invalidTime2 = true
                    if (this.data.date != '')
                        if (
                            new Date(moment(this.data.date).format('MM-DD-YYYY')).getDay() == 0 ||
                            new Date(moment(this.data.date).format('MM-DD-YYYY')).getDay() == 6
                        ) {
                            this.invalidTime1 = this.invalidTime2 = false
                        }
                    if (start_hour < end_hour) {
                        if (start_minute <= end_minute)
                            if (end_minute - start_minute >= 10)
                                this.time = end_hour - start_hour + ':' + (end_minute - start_minute)
                            else this.time = end_hour - start_hour + ':0' + (end_minute - start_minute)
                        else if (end_minute + 60 - start_minute >= 10)
                            this.time = end_hour - start_hour - 1 + ':' + (end_minute + 60 - start_minute)
                        else this.time = end_hour - start_hour - 1 + ':0' + (end_minute + 60 - start_minute)
                        this.data.realTime = this.time
                        this.timeError = false
                        return this.time
                    } else if (start_hour == end_hour)
                        if (start_minute < end_minute) {
                            if (end_minute - start_minute >= 10) this.time = '00:' + (end_minute - start_minute)
                            else this.time = '00:0' + (end_minute - start_minute)
                            this.timeError = false
                            this.data.realTime = this.time
                            return this.time
                        }
                    this.timeError = true
                }
            },
            changeDate() {
                var today = new Date(new Date().toLocaleDateString('en-EU'))
                var selectedDate = new Date(this.data.date)
                if (selectedDate.getTime() == today.getTime()) {
                    let start_hour = parseInt(this.data.start.substring(0, 2))
                    let end_hour = parseInt(this.data.end.substring(0, 2))
                    if (start_hour <= 8 || end_hour <= 8) {
                        this.invalidTime3 = true
                    } else this.invalidTime3 = false
                } else {
                    this.invalidTime3 = false
                }
            },
        },
        methods: {
            handleSubmit(isFormValid) {
                this.submitted = true
                if (!isFormValid) {
                    return
                }
                if (!this.valid || this.timeError || this.invalidTime1 || this.invalidTime2 || this.invalidTime3) return
                this.addData()
            },
            reloadform() {
                this.data = {
                    type: 0,
                    date: '',
                    start: null,
                    end: null,
                    realTime: null,
                    status: 0,
                    description: '',
                    leadCreate: null,
                    dateCreate: '',
                    updateUser: '',
                    dateUpdate: '',
                    note: '',
                    user: null,
                    idProject: null,
                }
            },

            addData() {
                this.data.date = moment(new Date(this.data.date)).format('YYYY-MM-DD')
                this.data.dateUpdate = new Date()
                if (this.idproject) {
                    HTTP.put('OTs/updateOT/' + this.idproject, this.data)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Cập nhật OT thành công!')
                                this.reloadform()
                                this.$emit('reloadData')
                                this.$emit('close')
                                this.submitted = false
                            }
                        })
                        .catch((err) => {
                            this.showWarn('Bạn không có quyền thực hiện thao tác sửa OT.')
                            console.log(err)
                        })
                }
                if (this.idproject === null) {
                    this.data.dateCreate = new Date()
                    this.data.leadCreate = checkAccessModule.getUserIdCurrent();
                    HTTP.post('OTs/AddOTs', this.data)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Thêm mới OT thành công!')
                                this.$emit('reloadData')
                                this.reloadform()
                                this.$emit('close')
                                this.submitted = false
                            }
                        })
                        .catch(() => {
                            this.showWarn('Bạn không có quyền thực hiện thao tác thêm OT')
                        })
                }
            },
            check_status(status) {
                if (status == 2) return true
                return false
            },
            backToOT() {
                this.$router.push('/ots')
            },
            showSuccess(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            showWarn(err) {
                this.$toast.add({ severity: 'warn', summary: 'Tin nhắn cảnh báo', detail: err, life: 3000 })
            },
            checkday() {
                this.valid = true
                return true
            },
            getUserByProject(projectid) {
                if (projectid !== null) {
                    HTTP.get('Project/UserInProject/' + projectid)
                        .then((res) => {
                            this.userDropdown = res.data
                        })
                        .catch((err) => console.log(err))
                }
            },
            async closeForm() {
                this.reloadform()
                this.$emit('close')
            },
            getData() {
                if (this.idproject !== null) {
                    this.edit = true
                    HTTP.get('OTs/getOTByID/' + this.idproject).then((res) => {
                        if (res.status == 200) {
                            this.data = res.data
                            this.data.date = moment(res.data.date).format('MM/DD/YYYY')
                            this.data.date = new Date(new Date(this.data.date).toLocaleDateString('en-EU'))
                        }
                    })
                }
            },
            async getProject() {
                await HTTP.get('Project/getAllProject')
                    .then((res) => {
                    if (res.status == 200) var formatDate = DateHelper.formatDate(new Date())
                    res.data.forEach((element) => {
                        if (
                            element.isDeleted != true &&
                            element.isFinished != true &&
                            (element.endDate > formatDate || element.endDate == null)
                        ) {
                            this.project.push(element)
                        }
                    })
                })
                },
        },

        async mounted() {
            this.token = LocalStorage.jwtDecodeToken()
           
            // Kiểm tra cái này phải là edit hay không
            if (this.idproject !== null || this.idproject !== 0) {
                this.edit = true
                HTTP.get('OTs/getOTByID/' + this.idproject).then((res) => {
                    if (res.status == 200) {
                        this.data = res.data
                        this.data.date = moment(res.data.date).format('MM/DD/YYYY')
                        this.data.date = new Date(new Date(this.data.date).toLocaleDateString('en-EU'))
                    }
                })
            } else {
                this.data.start = '00:00'
                this.data.end = '00:00'
            }

            if(checkAccessModule.getListGroup().includes("1")){
                this.getProject();
            }
            else {

            }


            await HTTP.get('Project/GetProjectByIdLead/' + checkAccessModule.getUserIdCurrent()).then((res) => {
                if (res.status == 200) var formatDate = DateHelper.formatDate(new Date())
                res.data.forEach((element) => {
                    if (
                        element.isDeleted != true &&
                        element.isFinished != true &&
                        (element.endDate > formatDate || element.endDate == null)
                    ) {
                        this.project.push(element)
                    }
                })
            })
        },
        watch: {
            // GET User By ID Project when change Id Data.IDProject
            'data.idProject': function (newId, oldId) {
                if (newId !== null || oldId !== null) {
                    this.getUserByProject(newId)
                }
            },
        },

        components: { LayoutDefault },
    }
</script>

<style scoped>
    .form__data {
        display: flex;
        flex-direction: column;
    }
    .form__data--item {
        height: 100px;
        border: 1px solid black;
        display: flex;
        justify-content: space-between;
        align-items: center;
    }

    .top {
        margin-top: 25px;
    }

    .custom__button {
        font-size: 20px;
    }
</style>

<style>
    .p-dropdown-panel {
        width: 14%;
        min-width: 9.3%;
    }
</style>
