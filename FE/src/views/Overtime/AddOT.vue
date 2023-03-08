<template>
    <layout-default-dynamic>
        <div class="container mt-4">
            <div class="flex justify-content-center container">
                <div class="card">
                    <h2 v-if="!edit">Thêm thẻ OT mới</h2>
                    <h2 v-else>Sửa thông tin OT</h2>
                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
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
                                    :showIcon="true"
                                    :date-select="changeDate"
                                    :class="{ 'p-invalid': (v$.data.date.$invalid || !checkday()) && submitted }"
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
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.idProject.$error }">
                                <Dropdown
                                    v-model="v$.data.idProject.$model"
                                    :options="project"
                                    optionLabel="name"
                                    optionValue="id"
                                    :class="{ 'p-invalid': v$.data.idProject.$invalid && submitted }"
                                />
                                <label for="idProject" :class="{ 'p-error': v$.data.idProject.$invalid && submitted }"
                                    >Dự án*</label
                                >
                            </div>
                            <small
                                v-if="(v$.data.idProject.$invalid && submitted) || v$.data.idProject.$pending.$response"
                                class="p-error"
                                >Bạn chưa chon dự án OT</small
                            >
                        </div>

                        <!-- Edit -->
                        <div class="field" style="margin-bottom: 0" v-if="this.$route.params.id">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.user.$error }">
                                <Dropdown
                                    v-model="v$.data.user.$model"
                                    :options="userDropdown"
                                    optionLabel="name"
                                    optionValue="x.id"
                                    :class="{ 'p-invalid': v$.data.user.$invalid && submitted }"
                                    disabled
                                />
                                <label for="user" :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                    >Người OT*</label
                                >
                            </div>
                            <small
                                v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                class="p-error"
                                >Bạn chưa chọn người OT</small
                            >
                        </div>

                        <!-- Add -->
                        <div class="field" style="margin-bottom: 0" v-if="!this.$route.params.id">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.user.$error }">
                                <MultiSelect
                                    v-model="v$.data.user.$model"
                                    :options="userDropdown"
                                    optionLabel="name"
                                    optionValue="x.id"
                                    placeholder="Người OTs"
                                    :class="{ 'p-invalid': v$.data.user.$invalid && submitted }"
                                />
                                <label for="user" :class="{ 'p-error': v$.data.user.$invalid && submitted }"
                                    >Người OT*</label
                                >
                            </div>
                            <small
                                v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                class="p-error"
                                >Bạn chưa chọn người OT</small
                            >
                        </div>
                        <div class="field" style="margin-bottom: 0">
                            <div :class="{ 'form-group--error': v$.data.start.$error }">
                                <label
                                    for="start"
                                    class="label"
                                    :class="{
                                        'p-error':
                                            (v$.data.start.$invalid || timeError || invalidTime1 || invalidTime3) &&
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
                                <span id="start-error" v-for="(error, index) of v$.data.start.$errors" :key="index">
                                    <small class="p-error">{{
                                        error.$message.replace('Value', 'Start Time') + '. '
                                    }}</small>
                                </span>
                            </span>
                            <small
                                v-else-if="(v$.data.start.$invalid && submitted) || v$.data.start.$pending.$response"
                                class="p-error"
                                >Bạn chưa nhập giờ bắt đầu OT</small
                            >
                            <small v-if="!v$.data.start.$invalid && invalidTime1 && submitted" class="p-error">
                                Giờ OT phải khác giờ làm việc
                            </small>
                        </div>
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.end.$error }">
                                <label
                                    for="end"
                                    class="label"
                                    :class="{
                                        'p-error':
                                            (v$.data.end.$invalid || timeError || invalidTime2 || invalidTime3) &&
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
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.realTime.$error }">
                                <InputText
                                    id="realTime"
                                    v-model="v$.data.realTime.$model"
                                    :disabled="true"
                                    :class="{ 'p-invalid': v$.data.realTime.$invalid && submitted }"
                                    :value="changeTime"
                                />
                                <label for=" realTime" :class="{ 'p-error': v$.data.realTime.$invalid && submitted }"
                                    >Thời gian OT thực tế</label
                                >
                            </div>
                        </div>

                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.description.$error }">
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
                        <div class="btn-right">
                            <button type="submit" class="btn btn-primary">Lưu</button>&nbsp;
                            <button type="button" class="btn btn-secondary" v-on:click="backToOT()">Hủy</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </layout-default-dynamic>
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

    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                hello: null,
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
            addData() {
                this.data.date = moment(new Date(this.data.date)).format('YYYY-MM-DD')
                this.data.dateUpdate = new Date()
                if (this.edit) {
                    HTTP.put('OTs/updateOT/' + this.$route.params.id, this.data)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Cập nhât OT thành công!')
                                this.$router.push('/ots')
                            }
                        })
                        .catch((err) => {
                            this.showWarn('Bạn không có quyền thực hiện thao tác sửa OT.')
                            console.log(err)
                        })
                } else if (this.data) {
                    this.data.dateCreate = new Date()
                    this.data.leadCreate = this.token.Id
                    HTTP.post('OTs/AddOTs', this.data)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess('Thêm mới OT thành công!')
                                this.$router.push('/ots')
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
                if (this.data.date <= new Date(new Date().toLocaleDateString('en-EU'))) {
                    this.valid = false
                    return false
                } else {
                    this.valid = true
                    return true
                }
            },
            getUserByProject(projectid) {
                HTTP.get('Project/UserInProject/' + projectid)
                    .then((res) => {
                        this.userDropdown = res.data
                    })
                    .catch((err) => console.log(err))
            },
        },
        mounted() {
            this.token = LocalStorage.jwtDecodeToken()
            // Nếu người dùng không phải là leader hay admin sẽ chuyển đến ots
            if (Number(this.token.IdGroup) !== 3 && Number(this.token.IdGroup) !== 1) {
                router.push('/ots')
            }
            // Kiểm tra cái này phải là edit hay không
            if (this.$route.params.id) {
                this.edit = true
                HTTP.get('OTs/getOTByID/' + this.$route.params.id).then((res) => {
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

            HTTP.get('Project/GetProjectByIdLead/' + this.token.Id).then((res) => {
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
                this.getUserByProject(newId)
            },
        },

        components: { LayoutDefault },
    }
</script>
<style scoped lang="scss">
    form {
        margin-top: 2rem;
    }

    .field {
        margin-bottom: 1.5rem;
        font-size: medium;
    }

    .p-fluid {
        width: 90%;
        margin: auto;
    }

    .card {
        width: 100%;
        padding: 20px;
    }

    small {
        font-size: 88%;
    }

    h2 {
        margin-left: 61.5px;
        margin-bottom: 20px;
    }

    element.style {
        width: 408px;
        min-width: 402px;
        left: 574.7080000000001px;
    }

    ::v-deep(.p-datepicker) {
        width: 382px;
        min-width: 411px;

        td {
            font-weight: 400;
        }
    }

    .label {
        font-size: 12px;
        margin-left: 0.75rem;
    }
    .btn-right {
        float: right;
        width: 110px;
        display: inline-flex;
    }
</style>
