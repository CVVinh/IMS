<template>
    <layout-default-dynamic>
        <div class="container mt-4">
            <div class="flex justify-content-center container">
                <div class="card">
                    <h2 v-if="!edit">Add Remote</h2>
                    <h2 v-else>Edit Remote</h2>
                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.date.$error }">
                                <label for="date" class="label"
                                    :class="{ 'p-error': (v$.data.date.$invalid || !checkday() || !valid) && submitted }">Remote
                                    Date*</label>
                                <Calendar id="date" v-model="v$.data.date.$model" :showIcon="true"
                                    :class="{ 'p-invalid': (v$.data.date.$invalid || !checkday() || !valid) && submitted }" />

                            </div>
                            <small v-if="(v$.data.date.$invalid && submitted) || v$.data.date.$pending.$response"
                                class="p-error">{{
                                v$.data.date.required.$message.replace('Value', 'Remote Date')
                                }}</small>
                            <small v-if="!v$.data.date.$invalid && !checkday() && submitted" class="p-error">
                                Remote Date must be bigger than today!
                            </small>
                            <small v-if="!v$.data.date.$invalid && !valid && submitted" class="p-error">
                                Remote Date must be different from Saturday and Sunday!
                            </small>
                        </div>
                        <div class="field" style="margin-bottom: 0;">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.user.$error }">
                                <Dropdown v-model="v$.data.user.$model" :options="userDropdown" optionLabel="userCode"
                                    optionValue="id" :class="{ 'p-invalid': v$.data.user.$invalid && submitted }" />
                                <label for="user"
                                    :class="{ 'p-error': v$.data.user.$invalid && submitted }">Remoter*</label>
                            </div>
                            <small v-if="(v$.data.user.$invalid && submitted) || v$.data.user.$pending.$response"
                                class="p-error">{{
                                v$.data.user.required.$message.replace('Value', 'Remoter')
                                }}</small>
                        </div>
                        <div class="field" style="margin-bottom: 0;">
                            <div :class="{ 'form-group--error': v$.data.start.$error }">
                                <label for="start" class="label"
                                    :class="{ 'p-error': (v$.data.start.$invalid || timeError || invalidTime1) && submitted }">Start
                                    Time*</label>
                                <InputText id="start" v-model="v$.data.start.$model" type="time" timeformat="24h"
                                    :class="{ 'p-invalid': v$.data.start.$invalid || (submitted && invalidTime1) }" />

                            </div>
                            <span v-if="v$.data.start.$error && submitted">
                                <span id="start-error" v-for="(error, index) of v$.data.start.$errors" :key="index">
                                    <small class="p-error">{{ error.$message.replace('Value', 'Start Time') + '. '
                                    }}</small>
                                </span>
                            </span>
                            <small v-else-if="(v$.data.start.$invalid && submitted) || v$.data.start.$pending.$response"
                                class="p-error">{{
                                v$.data.start.required.$message.replace('Value', 'Start Time')
                                }}</small>
                            <small v-if="!v$.data.start.$invalid && invalidTime1 && submitted" class="p-error">
                                Remote time should be work time!
                            </small>
                        </div>
                        <div class="field">
                            <div :class="{ 'form-group--error': v$.data.end.$error }">
                                <label for="end" class="label"
                                    :class="{ 'p-error': (v$.data.end.$invalid || timeError || invalidTime2) && submitted }">End
                                    Time*</label>
                                <InputText id="end" v-model="v$.data.end.$model" type="time"
                                    :class="{ 'p-invalid': v$.data.end.$invalid || (submitted && timeError) || ((submitted && invalidTime2)) }" />
                            </div>
                            <small v-if="(v$.data.end.$invalid && submitted) || v$.data.end.$pending.$response"
                                class="p-error">{{
                                v$.data.end.required.$message.replace('Value', 'End Time')
                                }}</small>
                            <small v-if="!v$.data.end.$invalid && timeError && submitted" class="p-error">
                                End Time must be bigger than Start Time!
                            </small>
                            <small v-if="!v$.data.start.$invalid && invalidTime2 && submitted" class="p-error">
                                Remote time should be work time!
                            </small>
                        </div>
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.realTime.$error }">
                                <InputText id="realTime" v-model="v$.data.realTime.$model" :disabled="true"
                                    :class="{ 'p-invalid': v$.data.realTime.$invalid && submitted }"
                                    :value="changeTime" />
                                <label for=" realTime"
                                    :class="{ 'p-error': v$.data.realTime.$invalid && submitted }">Real
                                    Time</label>
                            </div>
                        </div>
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.description.$error }">
                                <Textarea id="description" v-model="v$.data.description.$model" :showIcon="true"
                                    :class="{ 'p-invalid': v$.data.description.$invalid && submitted }" />
                                <label for=" description"
                                    :class="{ 'p-error': v$.data.description.$invalid && submitted }">Description*</label>
                            </div>
                            <small
                                v-if="(v$.data.description.$invalid && submitted) || v$.data.description.$pending.$response"
                                class="p-error">{{
                                v$.data.description.required.$message.replace('Value', 'Description')
                                }}</small>
                        </div>
                        <div class="field">
                            <div class="p-float-label" :class="{ 'form-group--error': v$.data.idProject.$error }">
                                <Dropdown v-model="v$.data.idProject.$model" :options="project" optionLabel="name"
                                    optionValue="id" :class="{ 'p-invalid': v$.data.idProject.$invalid && submitted }"
                                    :disabled="ispermission" />
                                <label for="idProject"
                                    :class="{ 'p-error': v$.data.idProject.$invalid && submitted }">Project*</label>
                            </div>
                            <small
                                v-if="(v$.data.idProject.$invalid && submitted) || v$.data.idProject.$pending.$response"
                                class="p-error">{{
                                v$.data.idProject.required.$message.replace('Value', 'Project')
                                }}</small>
                        </div>
                        <div class="button-div">
                            <button type="submit" class="btn btn-primary">Save</button>&nbsp;
                            <button type="button" class="btn btn-secondary" v-on:click="backToRemote()">Cancel</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </layout-default-dynamic>
</template>
<script>
import jwtDecode from 'jwt-decode';
import { HTTP } from '@/http-common.js'
import moment from 'moment'
import { required } from "@vuelidate/validators";
import { useVuelidate } from "@vuelidate/core";
import LayoutDefault from "../../layouts/LayoutDefault/LayoutDefault.vue";
import router from '@/router';
import { UserRoleHelper } from '@/helper/user-role.helper'
export default {
    setup: () => ({
        v$: useVuelidate()
    }),
    data() {
        return {
            data: {
                type: 1,
                date: "",
                start: null,
                end: null,
                realTime: null,
                status: 0,
                description: "",
                leadCreate: null,
                dateCreate: "",
                updateUser: "",
                dateUpdate: "",
                note: "",
                user: null,
                idProject: null,
            },
            userDropdown: [],
            project: [],
            edit: false,
            submitted: false,
            time: null,
            valid: true,
            timeError: true,
            invalidTime1: true,
            invalidTime2: true,
            statuses: [
                { num: 0, text: 'Waiting' },
                { num: 1, text: 'Accepted' },
                { num: 2, text: 'Denied' },
                { num: 3, text: 'Deleted' }
            ]
        };
    },
    validations() {
        return {
            data: {
                date: {
                    required
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
                    required
                },
                idProject: {
                    required
                },
            }
        };
    },
    computed: {
        changeTime() {
            if (this.data.start != null && this.data.end != null) {
                let start_hour = parseInt(this.data.start.substring(0, 2))
                let start_minute = parseInt(this.data.start.substring(3, 5))
                let end_hour = parseInt(this.data.end.substring(0, 2))
                let end_minute = parseInt(this.data.end.substring(3, 5))
                if ((8 <= start_hour && start_hour < 12) || (14 <= start_hour && start_hour < 17))
                    this.invalidTime1 = false
                else if ((start_hour == 13 && start_minute >= 30) || (start_hour == 17 && start_minute <= 30))
                    this.invalidTime1 = false
                else this.invalidTime1 = true
                if ((8 <= end_hour && end_hour <= 12) || (14 <= end_hour && end_hour <= 17))
                    this.invalidTime2 = false
                else if ((end_hour == 13 && end_minute >= 30) || (end_hour == 17 && end_minute <= 30))
                    this.invalidTime2 = false
                else this.invalidTime2 = true
                if (start_hour <= 12 && end_hour > 13)
                    this.invalidTime2 = true
                if (start_hour < end_hour) {
                    if (start_minute <= end_minute)
                        if (end_minute - start_minute >= 10)
                            this.time = (end_hour - start_hour) + ':' + (end_minute - start_minute)
                        else
                            this.time = (end_hour - start_hour) + ':0' + (end_minute - start_minute)
                    else
                        if (end_minute + 60 - start_minute >= 10)
                            this.time = (end_hour - start_hour - 1) + ':' + (end_minute + 60 - start_minute)
                        else
                            this.time = (end_hour - start_hour - 1) + ':0' + (end_minute + 60 - start_minute)
                    this.data.realTime = this.time
                    this.timeError = false
                    return this.time

                }
                else
                    if (start_hour == end_hour)
                        if (start_minute < end_minute) {
                            if (end_minute - start_minute >= 10)
                                this.time = '00:' + (end_minute - start_minute)
                            else
                                this.time = '00:0' + (end_minute - start_minute)
                            this.timeError = false
                            this.data.realTime = this.time
                            return this.time
                        }
                this.timeError = true
            }
        }
    },
    methods: {
        handleSubmit(isFormValid) {
            this.submitted = true;
            if (!isFormValid) {
                return;
            }
            if (!this.valid || this.timeError || this.invalidTime1 || this.invalidTime2 || !this.checkday())
                return
            this.addData();
        },
        addData() {
            this.data.date = new Date(this.data.date).toLocaleDateString()
            this.data.dateUpdate = new Date();
            let userlogin = jwtDecode(localStorage.getItem("token"));
            this.data.updateUser = userlogin.Id;
            if (this.edit) {
                HTTP.put("OTs/updateOT/" + this.$route.params.id, this.data).then(res => {
                    if (res.status == 200) {
                        this.showSuccess();
                        this.$router.push("/remotes");
                    }
                });
            }
            else if (this.data) {
                this.data.dateCreate = new Date();
                this.data.leadCreate = userlogin.Id;
                HTTP.post("OTs/createOT", this.data).then(res => {
                    if (res.status == 200) {

                        this.showSuccess();
                        this.$router.push("/remotes");
                    }
                })
                    .catch(err => {
                        this.showWarn();
                    });
            }
        },
        check_status(status) {
            if (status == 2)
                return true;
            return false;
        },
        backToRemote() {
            this.$router.push("/remotes");
        },
        showSuccess() {
            this.$toast.add({ severity: "success", summary: "Success Message", detail: "Successful!", life: 3000 });
        },
        showWarn() {
            this.$toast.add({ severity: "warn", summary: "Warn Message", detail: "Existed!", life: 3000 });
        },
        checkday() {
            if (new Date(moment(this.data.date).format("MM-DD-YYYY")).getDay() == 0 || new Date(moment(this.data.date).format("MM-DD-YYYY")).getDay() == 6) {
                this.valid = false
            }
            else this.valid = true
            if (this.data.date < new Date(new Date().toLocaleDateString('en-EU'))) {
                return false
            }
            else {
                return true
            }

        },
    },
    mounted() {
        if (!UserRoleHelper.isLeader() || !UserRoleHelper.isAdmin())
            router.push('/remotes')
        if (this.$route.params.id) {
            this.edit = true;
            HTTP.get("OTs/getOTByID/" + this.$route.params.id).then(res => {
                if (res.status == 200) {
                    this.data = res.data;
                    this.data.date = moment(res.data.date).format("YYYY-MM-DD");
                    this.data.date = new Date(new Date(this.data.date).toLocaleDateString('en-EU'))
                }
            });
        }
        else {
            this.data.start = '00:00';
            this.data.end = '00:00';
        }
        HTTP.get("Users/getAll").then(res => {
            if (res.status == 200)
                this.userDropdown = res.data;
        });
        HTTP.get("Project/getAllProject").then(res => {
            if (res.status == 200)
                res.data.forEach(element => {
                    if (element.isDeleted != true && element.isFinished != true) {
                        this.project.push(element);
                    }
                });
        });
    },
    components: { LayoutDefault }
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
    margin: auto;
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
</style>