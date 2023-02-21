<template>
    <!-- <LayoutDefault> -->
    <div>
        <Toast position="top-right" />
    </div>
    <div class="form-demo">
        <div class="flex justify-content-center container">
            <div class="card">
                <h5 class="text-center">Sửa</h5>
                <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.userCode.$error }">
                                    <InputText
                                        id=" userCode"
                                        v-model="v$.form.userCode.$model"
                                        :class="{ 'p-invalid': v$.form.userCode.$invalid && submitted }"
                                        disabled
                                    />
                                    <label for="userCode" :class="{ 'p-error': v$.form.userCode.$invalid && submitted }"
                                        >Account</label
                                    >
                                </div>
                                <span v-if="v$.form.userCode.$error && submitted">
                                    <span
                                        id="userCode-error"
                                        v-for="(error, index) of v$.form.userCode.$errors"
                                        :key="index"
                                    >
                                        <small class="p-error">{{ error.$message }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.form.userCode.$invalid && submitted) || v$.form.userCode.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.userCode.required.$message.replace('Value', 'userCode') }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label p-input-icon-right">
                                    <!-- <i class="pi pi-envelope" /> -->
                                    <InputText
                                        id="email"
                                        v-model="v$.form.email.$model"
                                        :class="{ 'p-invalid': v$.form.email.$invalid && submitted }"
                                        aria-describedby="email-error"
                                    />
                                    <label for="email" :class="{ 'p-error': v$.form.email.$invalid && submitted }"
                                        >Email*</label
                                    >
                                </div>
                                <span v-if="v$.form.email.$error && submitted">
                                    <span id="email-error" v-for="(error, index) of v$.form.email.$errors" :key="index">
                                        <small class="p-error">{{ error.$message }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.form.email.$invalid && submitted) || v$.form.email.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.email.required.$message.replace('Value', 'Email') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.firstName.$error }">
                                    <InputText
                                        id=" firstName"
                                        v-model="v$.form.firstName.$model"
                                        :class="{ 'p-invalid': v$.form.firstName.$invalid && submitted }"
                                    />
                                    <label
                                        for="firstName"
                                        :class="{ 'p-error': v$.form.firstName.$invalid && submitted }"
                                        >FirstName*</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.firstName.$invalid && submitted) ||
                                        v$.form.firstName.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.firstName.required.$message.replace('Value', 'First name') }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.lastName.$error }">
                                    <InputText
                                        id=" lastName"
                                        v-model="v$.form.lastName.$model"
                                        :class="{ 'p-invalid': v$.form.lastName.$invalid && submitted }"
                                    />
                                    <label for="lastName" :class="{ 'p-error': v$.form.lastName.$invalid && submitted }"
                                        >LastName*</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.lastName.$invalid && submitted) || v$.form.lastName.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.lastName.required.$message.replace('Value', 'Last name') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.dob.$error }">
                                    <Calendar
                                        id="dob"
                                        v-model="v$.form.dob.$model"
                                        :showIcon="true"
                                        :class="{ 'p-invalid': (v$.form.dob.$model || dOBValidate) && submitted }"
                                    />
                                    <label
                                        for="dob"
                                        :class="{ 'p-error': (v$.form.dob.$model || dOBValidate) && submitted }"
                                        >Birthday</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.form.dob.$invalid && submitted) || v$.form.dob.$pending.$response"
                                    class="p-error"
                                    >{{ v$.form.dob.required.$message.replace('Value', 'Birthday') }}</small
                                >
                                <small v-if="!this.checkDateOfBirth(v$.form.dob.$model) && submitted" class="p-error">
                                    User must be at least 18 years old
                                </small>
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.phoneNumber.$error }">
                                    <InputText
                                        id=" phoneNumber"
                                        v-model="v$.form.phoneNumber.$model"
                                        :class="{ 'p-invalid': v$.form.phoneNumber.$invalid && submitted }"
                                    />
                                    <label
                                        for="phoneNumber"
                                        :class="{ 'p-error': v$.form.phoneNumber.$invalid && submitted }"
                                        >PhoneNumber</label
                                    >
                                </div>
                                <span v-if="v$.form.phoneNumber.$error && submitted">
                                    <span
                                        id="phoneNumber-error"
                                        v-for="(error, index) of v$.form.phoneNumber.$errors"
                                        :key="index"
                                    >
                                        <small class="p-error">{{ error.$message }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.form.phoneNumber.$invalid && submitted) ||
                                        v$.form.phoneNumber.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.phoneNumber.required.$message.replace('Value', 'PhoneNumber') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.form.identitycard.$error }"
                                >
                                    <InputText
                                        id=" identitycard"
                                        v-model="v$.form.identitycard.$model"
                                        :class="{ 'p-invalid': v$.form.identitycard.$invalid && submitted }"
                                    />
                                    <label
                                        for="identitycard"
                                        :class="{ 'p-error': v$.form.identitycard.$invalid && submitted }"
                                        >Identity Card*</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.identitycard.$invalid && submitted) ||
                                        v$.form.identitycard.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.form.identitycard.required.$message.replace('Value', 'Identity card')
                                    }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.workStatus.$error }">
                                    <Dropdown
                                        v-model="v$.form.workStatus.$model"
                                        :options="optionworkStatus"
                                        optionValue="code"
                                        optionLabel="name"
                                        :class="{ 'p-invalid': v$.form.workStatus.$invalid && submitted }"
                                    />
                                    <label
                                        for="workStatus"
                                        :class="{ 'p-error': v$.form.workStatus.$invalid && submitted }"
                                        >workStatus*</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.workStatus.$invalid && submitted) ||
                                        v$.form.workStatus.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.workStatus.required.$message.replace('Value', 'Role') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.address.$error }">
                                    <InputText
                                        id=" address"
                                        v-model="v$.form.address.$model"
                                        :class="{ 'p-invalid': v$.form.address.$invalid && submitted }"
                                    />
                                    <label for="address" :class="{ 'p-error': v$.form.address.$invalid && submitted }"
                                        >Address</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.form.address.$invalid && submitted) || v$.form.address.$pending.$response"
                                    class="p-error"
                                    >{{ v$.form.address.required.$message.replace('Value', 'Address') }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.university.$error }">
                                    <InputText
                                        id=" university"
                                        v-model="v$.form.university.$model"
                                        :class="{ 'p-invalid': v$.form.university.$invalid && submitted }"
                                    />
                                    <label
                                        for="university"
                                        :class="{ 'p-error': v$.form.university.$invalid && submitted }"
                                        >University</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.university.$invalid && submitted) ||
                                        v$.form.university.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.form.university.required.$message.replace('Value', 'University') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.form.yearGraduated.$error }"
                                >
                                    <InputText
                                        id=" yearGraduated"
                                        v-model="v$.form.yearGraduated.$model"
                                        :class="{ 'p-invalid': v$.form.yearGraduated.$invalid && submitted }"
                                    />
                                    <label
                                        for="yearGraduated"
                                        :class="{ 'p-error': v$.form.yearGraduated.$invalid && submitted }"
                                        >Year graduated</label
                                    >
                                </div>
                                <span v-if="v$.form.yearGraduated.$error && submitted">
                                    <span
                                        id="yearGraduated-error"
                                        v-for="(error, index) of v$.form.yearGraduated.$errors"
                                        :key="index"
                                    >
                                        <small class="p-error">{{ error.$message }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.form.yearGraduated.$invalid && submitted) ||
                                        v$.form.yearGraduated.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.form.yearGraduated.required.$message.replace('Value', 'yearGraduated')
                                    }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.skype.$error }">
                                    <InputText
                                        id=" skype"
                                        v-model="v$.form.skype.$model"
                                        :class="{ 'p-invalid': v$.form.skype.$invalid && submitted }"
                                    />
                                    <label for="skype" :class="{ 'p-error': v$.form.skype.$invalid && submitted }"
                                        >Skype</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.form.skype.$invalid && submitted) || v$.form.skype.$pending.$response"
                                    class="p-error"
                                    >{{ v$.form.skype.required.$message.replace('Value', 'Skype') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.form.dateStartWork.$error }"
                                >
                                    <Calendar
                                        id="dateStartWork"
                                        v-model="v$.form.dateStartWork.$model"
                                        :showIcon="true"
                                        :class="{ 'p-invalid': v$.form.dateStartWork.$invalid && submitted }"
                                        :disabled="true"
                                    />
                                    <label
                                        for=" dateStartWork"
                                        :class="{ 'p-error': v$.form.dateStartWork.$invalid && submitted }"
                                        >Date start work</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.dateStartWork.$invalid && submitted) ||
                                        v$.form.dateStartWork.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.form.dateStartWork.required.$message.replace('Value', 'Date start work')
                                    }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-md-6 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.dateLeave.$error }">
                                    <Calendar
                                        id="dateLeave"
                                        v-model="v$.form.dateLeave.$model"
                                        :showIcon="true"
                                        :class="{ 'p-invalid': v$.form.dateLeave.$invalid && submitted }"
                                        :disabled="ispermission"
                                    />
                                    x="{ 'form-group--error': v$.form.reasonResignation.$error }">
                                    <Textarea
                                        v-model="v$.form.reasonResignation.$model"
                                        :autoResize="true"
                                        cols="20"
                                        :class="{ 'p-invalid': v$.form.reasonResignation.$invalid && submitted }"
                                        style="margin-bottom: 20px"
                                    />
                                    <label
                                        for="reasonResignation"
                                        :class="{ 'p-error': v$.form.reasonResignation.$invalid && submitted }"
                                        >Reason Resignation</label
                                    >
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col col-sm-4 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.gender.$error }">
                                    <Dropdown
                                        v-model="v$.form.gender.$model"
                                        :options="optionGender"
                                        optionValue="code"
                                        optionLabel="name"
                                        :class="{ 'p-invalid': v$.form.gender.$invalid && submitted }"
                                    />
                                    <label for="gender" :class="{ 'p-error': v$.form.gender.$invalid && submitted }"
                                        >Gender*</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.form.gender.$invalid && submitted) || v$.form.gender.$pending.$response"
                                    class="p-error"
                                    >{{ v$.form.gender.required.$message.replace('Value', 'Role') }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-sm-4 col-12">
                            <div class="field">
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.form.maritalStatus.$error }"
                                >
                                    <Dropdown
                                        v-model="v$.form.maritalStatus.$model"
                                        :options="optionMaritalStatus"
                                        optionLabel="name"
                                        optionValue="code"
                                        :class="{ 'p-invalid': v$.form.maritalStatus.$invalid && submitted }"
                                    />
                                    <label
                                        for="maritalStatus"
                                        :class="{ 'p-error': v$.form.maritalStatus.$invalid && submitted }"
                                        >Marital Status</label
                                    >
                                </div>
                                <small
                                    v-if="
                                        (v$.form.maritalStatus.$invalid && submitted) ||
                                        v$.form.maritalStatus.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.form.maritalStatus.required.$message.replace('Value', 'Marital Status')
                                    }}</small
                                >
                            </div>
                        </div>
                        <div class="col col-sm-4 col-12">
                            <div class="field">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.form.IdGroup.$error }">
                                    <Dropdown
                                        v-model="v$.form.IdGroup.$model"
                                        :options="optionRoles"
                                        optionLabel="name"
                                        optionValue="code"
                                        :class="{ 'p-invalid': v$.form.IdGroup.$invalid && submitted }"
                                        :disabled="ispermission"
                                    />
                                    <label for="IdGroup" :class="{ 'p-error': v$.form.IdGroup.$invalid && submitted }"
                                        >Role*</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.form.IdGroup.$invalid && submitted) || v$.form.IdGroup.$pending.$response"
                                    class="p-error"
                                    >{{ v$.form.IdGroup.required.$message.replace('Value', 'Role') }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="d-flex justify-content-end">
                        <div class="col-2">
                            <Button type="submit" label="Hoàn tất" />
                        </div>
                        &emsp;
                        <div class="col-2">
                            <Button label="Hủy" class="p-button-secondary" @click.prevent="backToUser()" />
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <!-- </LayoutDefault> -->
</template>
<script>
    import { HTTP } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import moment from 'moment'
    import { email, required, alphaNum, numeric, between, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { ToastSeverity } from 'primevue/api'
    import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        name: 'edituser',
        data() {
            return {
                form: {
                    userCode: null,
                    password: null,
                    email: null,
                    userCreate: localStorage.getItem('username'),
                    dateCreate: new Date(),
                    // dateModified: new Date(),
                    passwordEmail: null,
                    firstName: null,
                    lastName: null,
                    phoneNumber: '',
                    dob: '',
                    gender: null,
                    address: null,
                    university: null,
                    yearGraduated: null,
                    skype: null,
                    passwordSkype: null,
                    dateStartWork: '',
                    dateLeave: '',
                    maritalStatus: null,
                    reasonResignation: null,
                    workStatus: null,
                    IdGroup: null,
                    dOBValidate: true,
                },
                submitted: false,
                optionGender: [
                    { name: 'Male', code: 1 },
                    { name: 'Female', code: 2 },
                    { name: 'Other', code: 3 },
                ],
                optionMaritalStatus: [
                    { name: 'Married', code: 1 },
                    { name: 'Unmarried', code: 2 },
                    { name: 'Undefined', code: 3 },
                ],
                optionRoles: [
                    { name: 'Director', code: 1 },
                    { name: 'Human Resource', code: 2 },
                    { name: 'Project Manager', code: 3 },
                    { name: 'Leader', code: 4 },
                    { name: 'Accountant', code: 5 },
                    { name: 'User', code: 6 },
                    { name: 'Admin', code: 7 },
                ],
                optionworkStatus: [
                    { name: 'Working', code: 1 },
                    { name: 'Resignation', code: 2 },
                    { name: 'Maternity Leaving', code: 3 },
                ],
            }
        },
        validations() {
            return {
                form: {
                    userCode: {
                        required,
                        alphaNum,
                    },
                    email: {
                        required,
                        email,
                        maxLength: maxLength(50),
                    },
                    firstName: {
                        required,
                    },
                    lastName: {
                        required,
                    },
                    phoneNumber: {
                        numeric,
                        maxLength: maxLength(11),
                    },
                    dob: {},
                    identitycard: {
                        numeric,
                        maxLength: maxLength(12),
                        required,
                    },
                    workStatus: {
                        required,
                    },
                    gender: {
                        required,
                    },
                    address: {},
                    university: {},
                    yearGraduated: {
                        between: between(2000, 3000),
                    },
                    skype: {
                        maxLength: maxLength(100),
                    },
                    reasonResignation: {},
                    dateStartWork: {
                        required,
                    },
                    dateLeave: {},
                    maritalStatus: {
                        between: between(1, 3),
                    },
                    IdGroup: {
                        required,
                    },
                },
            }
        },
        mounted() {
            if (this.$route.params) {
                HTTP.get('Users/getUserById/' + this.$route.params.id)
                    .then((res) => {
                        if (res.status == 200) {
                            this.form = res.data
                            this.form.dateStartWork =
                                res.data.dateStartWork == null
                                    ? null
                                    : moment(res.data.dateStartWork).format('DD/MM/YYYY')
                            this.form.dateLeave =
                                res.data.dateLeave == null ? null : moment(res.data.dateLeave).format('DD/MM/YYYY')
                            this.form.dob = res.data.dOB == null ? null : moment(res.data.dOB).format('DD/MM/YYYY')
                            this.optionworkStatus = [
                                { name: 'Working', code: 1 },
                                { name: 'Resignation', code: 2 },
                                { name: 'Maternity Leaving', code: 3 },
                            ]
                            if (this.form.gender != 2) {
                                this.optionworkStatus = [
                                    { name: 'Working', code: 1 },
                                    { name: 'Resignation', code: 2 },
                                ]
                            }
                        }
                    })
                    .catch((er) => {
                        this.$toast.add({
                            severity: ToastSeverity.ERROR,
                            summary: 'Error ',
                            detail: "Can't get information of user create !",
                            life: 3000,
                        })
                    })
            }
        },
        computed: {
            async ispermission() {
                if ((await UserRoleHelper.isAdmin()) || (await UserRoleHelper.isHr())) {
                    return false
                }
                return true
            },
        },
        methods: {
            handleSubmit(isFormValid) {
                this.submitted = true

                if (!isFormValid) {
                    return
                }
                this.Submit()
            },
            Submit() {
                if (this.$route.params.id) {
                    const token = localStorage.getItem('token')
                    const decode = jwt_decode(token)
                    HTTP.put('Users/updateUser/' + this.$route.params.id, {
                        userModified: parseInt(decode.Id),
                        dateModified: new Date(),
                        firstName: this.form.firstName,
                        lastName: this.form.lastName,
                        phoneNumber: this.form.phoneNumber,
                        dOB: this.form.dob == null ? '' : this.form.dob,
                        identitycard: this.form.identitycard,
                        gender: this.form.gender,
                        address: this.form.address,
                        university: this.form.university,
                        yearGraduated: this.form.yearGraduated,
                        email: this.form.email,
                        skype: this.form.skype,
                        workStatus: this.form.workStatus,
                        dateStartWork: this.form.dateStartWork,
                        dateLeave: this.form.dateLeave,
                        maritalStatus: this.form.maritalStatus,
                        reasonResignation: this.form.reasonResignation,
                        IdGroup: this.form.IdGroup,
                    })
                        .then((res) => {
                            if (res.status == 200) {
                                this.$router.push('/users')
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Info Message',
                                    detail: 'Edit user success!',
                                    life: 3000,
                                })
                            }
                        })
                        .catch((error) => {
                            switch (error.code) {
                                case 'ERR_NETWORK':
                                    this.$toast.add({
                                        severity: ToastSeverity.ERROR,
                                        summary: 'Lỗi',
                                        detail: 'Kiểm tra kết nối !',
                                        life: 3000,
                                    })
                                    break
                                case 'ERR_BAD_REQUEST':
                                    this.$toast.add({
                                        severity: ToastSeverity.ERROR,
                                        summary: 'Lỗi',
                                        detail: error.response.data,
                                        life: 3000,
                                    })
                                    break
                            }
                        })
                }
            },
            backToUser() {
                this.$router.push('/users')
            },
            checkDateOfBirth(dOB) {
                if (dOB == '') {
                    this.dOBValidate = false
                    return true
                }
                var today = new Date()
                var thisYear = new Date(today).getFullYear()
                var birthYear = new Date(dOB).getFullYear()
                if (thisYear - birthYear >= 18 && thisYear - birthYear <= 60) {
                    this.dOBValidate = false
                    return true
                } else {
                    this.dOBValidate = true
                    return false
                }
            },
        },
        components: { LayoutDefault },
    }
</script>
<style scoped lang="scss">
    .form-demo {
        .card {
            min-width: 450px;
            padding: 10px;

            form {
                margin-top: 2rem;
            }

            .field {
                margin-bottom: 1.5rem;
                font-size: small;
            }
        }

        @media screen and (max-width: 960px) {
            .card {
                width: 80%;
            }
        }
    }
</style>
