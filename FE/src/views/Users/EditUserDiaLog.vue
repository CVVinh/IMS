<template>
    <Dialog :visible="statusopen" :closable="false" :maximizable="true" header="Sửa" :modal="true" :show="onShow()">
        <div>
            <Toast position="top-right" />
        </div>
        <div class="form-demo">
            <div class="flex justify-content-center container">
                <div class="card cardrelax">
                    <div class="admin__form--header">
                        <div></div>
                        <h5 class="text-center"></h5>
                    </div>
                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.userCode.$error }"
                                    >
                                        <InputText
                                            id=" userCode"
                                            v-model="v$.form.userCode.$model"
                                            :class="{ 'p-invalid': v$.form.userCode.$invalid && submitted }"
                                            :disabled="true"
                                        />
                                        <label
                                            for="userCode"
                                            :class="{ 'p-error': v$.form.userCode.$invalid && submitted }"
                                            >Tài khoản*</label
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
                                            (v$.form.userCode.$invalid && submitted) ||
                                            v$.form.userCode.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.form.userCode.required.$message.replace('Value', 'Account') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-sm-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.phoneNumber.$error }"
                                    >
                                        <InputText
                                            id=" phoneNumber"
                                            v-model="v$.form.phoneNumber.$model"
                                            :class="{ 'p-invalid': v$.form.phoneNumber.$invalid && submitted }"
                                        />
                                        <label
                                            for="phoneNumber"
                                            :class="{ 'p-error': v$.form.phoneNumber.$invalid && submitted }"
                                            >Số điện thoại</label
                                        >
                                    </div>
                                    <span v-if="v$.form.phoneNumber.$error && submitted">
                                        <span
                                            id="phoneNumber-error"
                                            v-for="(error, index) of v$.form.phoneNumber.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{ error.$message + ' ' }} </small>
                                        </span>
                                    </span>
                                    <small
                                        v-if="
                                            (v$.form.phoneNumber.$invalid && submitted) ||
                                            v$.form.phoneNumber.$pending.$response
                                        "
                                        class="p-error"
                                        >Độ dài điện thoại phải là 10
                                    </small>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.firstName.$error }"
                                    >
                                        <InputText
                                            id=" firstName"
                                            v-model="v$.form.firstName.$model"
                                            :class="{ 'p-invalid': v$.form.firstName.$invalid && submitted }"
                                        />
                                        <label
                                            for="firstName"
                                            :class="{ 'p-error': v$.form.firstName.$invalid && submitted }"
                                            >Tên*</label
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
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.lastName.$error }"
                                    >
                                        <InputText
                                            id=" lastName"
                                            v-model="v$.form.lastName.$model"
                                            :class="{ 'p-invalid': v$.form.lastName.$invalid && submitted }"
                                        />
                                        <label
                                            for="lastName"
                                            :class="{ 'p-error': v$.form.lastName.$invalid && submitted }"
                                            >Họ*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.lastName.$invalid && submitted) ||
                                            v$.form.lastName.$pending.$response
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
                                        <span
                                            id="email-error"
                                            v-for="(error, index) of v$.form.email.$errors"
                                            :key="index"
                                        >
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
                                            >Căn cước công dân*</label
                                        >
                                    </div>
                                    <span
                                        v-if="
                                            (v$.form.identitycard.$invalid && submitted) ||
                                            v$.form.identitycard.$pending.$response
                                        "
                                        class="p-error"
                                    >
                                        <span v-for="(error, index) of v$.form.identitycard.$errors" :key="index">
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                        <small
                                            v-if="
                                                (v$.form.identitycard.$invalid && submitted) ||
                                                v$.form.identitycard.$pending.$response
                                            "
                                            class="p-error"
                                            >. Độ dài phải từ 9 đến 12
                                        </small>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-sm-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.form.gender.$error }">
                                        <Dropdown
                                            v-model="v$.form.gender.$model"
                                            :options="optionGender"
                                            optionValue="code"
                                            optionLabel="name"
                                            :class="{ 'p-invalid': v$.form.gender.$invalid && submitted }"
                                            :disabled="true"
                                        />
                                        <label for="gender" :class="{ 'p-error': v$.form.gender.$invalid && submitted }"
                                            >Giới tính*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.gender.$invalid && submitted) || v$.form.gender.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.form.gender.required.$message.replace('Value', 'Role') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-sm-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.form.idGroup.$error }">
                                        <Dropdown
                                            v-model="v$.form.idGroup.$model"
                                            :options="optionRoles"
                                            optionLabel="nameGroup"
                                            optionValue="id"
                                            :class="{ 'p-invalid': v$.form.idGroup.$invalid && submitted }"
                                        />
                                        <label
                                            for="idGroup"
                                            :class="{ 'p-error': v$.form.idGroup.$invalid && submitted }"
                                            >Nhóm*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.idGroup.$invalid && submitted) ||
                                            v$.form.idGroup.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.form.idGroup.required.$message.replace('Value', 'Role') }}</small
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
                            <div class="col col-md-3 col-12">
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
                                        />
                                        <label
                                            for="dateStartWork"
                                            :class="{ 'p-error': v$.form.dateStartWork.$invalid && submitted }"
                                            >Ngày bắt đầu làm*</label
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
                            <div class="col col-md-3 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.dateLeave.$error }"
                                    >
                                        <Calendar
                                            id="dateLeave"
                                            v-model="v$.form.dateLeave.$model"
                                            :showIcon="true"
                                            :class="{ 'p-invalid': v$.form.dateLeave.$invalid && submitted }"
                                        />
                                        <label
                                            for="dateLeave"
                                            :class="{ 'p-error': v$.form.dateLeave.$invalid && submitted }"
                                            >Ngày thôi việc
                                        </label>
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.dateLeave.$invalid && submitted) ||
                                            v$.form.dateLeave.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.form.dateLeave.required.$message.replace('Value', 'Date start work')
                                        }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-3 col-12">
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
                                            >Tình trạng hôn nhân
                                        </label>
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
                            <div class="col col-md-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.form.dob.$error }">
                                        <Calendar
                                            id="dob"
                                            v-model="v$.form.dob.$model"
                                            :showIcon="true"
                                            :class="{
                                                'p-invalid': (v$.form.dob.$invalid || !checkDateOfBirth) && submitted,
                                            }"
                                        />
                                        <label
                                            for="dob"
                                            :class="{
                                                'p-error': (v$.form.dob.$invalid || !checkDateOfBirth) && submitted,
                                            }"
                                            >Ngày sinh</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.form.dob.$invalid && submitted) || v$.form.dob.$pending.$response"
                                        class="p-error"
                                        >{{ v$.form.dob.required.$message.replace('Value', 'Birthday') }}</small
                                    >
                                    <small v-if="!checkDateOfBirth && submitted" class="p-error">
                                        Người dùng phải trên 18 tuổi và dưới 60 tuổi
                                    </small>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.workStatus.$error }"
                                    >
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
                                            >Tình trạng làm việc*</label
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
                                            >Năm tốt nghiệp</label
                                        >
                                    </div>
                                    <span
                                        v-if="
                                            (v$.form.yearGraduated.$invalid && submitted) ||
                                            v$.form.yearGraduated.$pending.$response
                                        "
                                        class="p-error"
                                    >
                                        <span v-for="(error, index) of v$.form.yearGraduated.$errors" :key="index">
                                            <small class="p-error">{{ error.$message }}. </small>
                                        </span>
                                    </span>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-12 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.form.address.$error }">
                                        <Textarea
                                            rows="3"
                                            cols="30"
                                            id=" address"
                                            v-model="v$.form.address.$model"
                                            :class="{ 'p-invalid': v$.form.address.$invalid && submitted }"
                                        />

                                        <label
                                            for="address"
                                            :class="{ 'p-error': v$.form.address.$invalid && submitted }"
                                            >Địa chỉ</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.address.$invalid && submitted) ||
                                            v$.form.address.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.form.address.required.$message.replace('Value', 'Address') }}</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.form.university.$error }"
                                    >
                                        <InputText
                                            id=" university"
                                            v-model="v$.form.university.$model"
                                            :class="{ 'p-invalid': v$.form.university.$invalid && submitted }"
                                        />
                                        <label
                                            for="university"
                                            :class="{ 'p-error': v$.form.university.$invalid && submitted }"
                                            >Đại học</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.form.university.$invalid && submitted) ||
                                            v$.form.university.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.form.university.required.$message.replace('Value', 'University')
                                        }}</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="d-flex justify-content-end">
                            <div class="col-2">
                                <Button type="submit" label="Lưu" />
                            </div>
                            &emsp;
                            <div class="col-2">
                                <Button label="Hủy" class="p-button-secondary" @click="closeModal()" />
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </Dialog>
</template>

<script>
    import { HTTP } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import moment from 'moment'
    import { email, required, alphaNum, numeric, between, maxLength, minLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { ToastSeverity } from 'primevue/api'
    import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { HttpStatus } from '@/config/app.config'
    export default {
        props: ['statusopen', 'iduser', 'roleoption'],
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
                    idGroup: null,
                    dOBValidate: true,
                },
                submitted: false,
                optionGender: [
                    { name: 'Nam', code: 1 },
                    { name: 'Nữ', code: 2 },
                    { name: 'Khác', code: 3 },
                ],
                optionMaritalStatus: [
                    { name: 'Kết hôn', code: 1 },
                    { name: 'Chưa kết hôn', code: 2 },
                    { name: 'Khác', code: 3 },
                ],
                optionRoles: [
                    { name: 'President', code: 1 },
                    { name: 'HR', code: 2 },
                    { name: 'PM', code: 3 },
                    { name: 'Leader', code: 4 },
                    { name: 'Accountant', code: 5 },
                    { name: 'Staff', code: 6 },
                    { name: 'Admin', code: 7 },
                ],
                optionworkStatus: [
                    { name: 'Đang làm', code: 1 },
                    { name: 'Đã từ chức', code: 2 },
                    { name: 'Nghỉ thai sản', code: 3 },
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
                        checkPhoneNumber(value) {
                            0.3
                            if (!value) return true
                            if (value.length === 0) return true
                            if (value.includes('.')) return false
                            if (value.length === 10) return true
                            else return false
                        },
                    },
                    dob: {},
                    identitycard: {
                        numeric,
                        required,
                        valueNineOrTwelve(value) {
                            if (value.includes('.')) return false
                            if (value.length === 9 || value.length === 12) return true
                            else return false
                        },
                    },
                    workStatus: {
                        required,
                    },
                    gender: {},
                    address: {},
                    university: {},
                    yearGraduated: {
                        numeric,
                        between: between(1980, 2100),
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
                    idGroup: {
                        required,
                    },
                },
            }
        },

        beforeUpdate() {},
        computed: {
            async ispermission() {
                if ((await UserRoleHelper.isHr()) || (await UserRoleHelper.isDirector())) return false
                return true
            },
            checkDateLeave() {
                if (this.v$.form.workStatus.$model === 2) return false
                else {
                    this.v$.form.dateLeave.$model = null
                    return true
                }
            },
            checkDateOfBirth() {
                const dOB = this.v$.form.dob.$model
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
        mounted() {
            this.getAllGroup()
        },
        methods: {
            getAllGroup() {
                HTTP.get('Group/getListGroup/')
                    .then((res) => {
                        if (res.status === 200) {
                            this.optionRoles = res.data
                        }
                    })
                    .catch((er) => {
                        this.$toast.add({
                            severity: ToastSeverity.ERROR,
                            summary: 'Lỗi ',
                            detail: 'Không thể lấy thông tin của người dùng !',
                            life: 3000,
                        })
                    })
            },
            closeModal() {
                this.$emit('closeModal')
                this.submitted = false
            },
            handleSubmit(isFormValid) {
                this.submitted = true

                if (!isFormValid) {
                    return
                }
                this.Submit()
            },
            async Submit() {
                if (this.iduser) {
                    const token = localStorage.getItem('token')
                    const decode = jwt_decode(token)
                    if (!this.checkDateOfBirth) return
                    const dataPost = {
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
                        idGroup: this.form.idGroup,
                    }
                    try {
                        const res = await HTTP.put('Users/updateUser/' + this.iduser, dataPost)
                        switch (res.status) {
                            case HttpStatus.OK:
                                this.closeModal()
                                this.$emit('reloadpage')
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Thành công',
                                    detail: 'Thêm mới thành công!',
                                    life: 3000,
                                })
                                break
                            case HttpStatus.UNAUTHORIZED:
                            case HttpStatus.FORBIDDEN:
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Không có quyền thực hiện thao tác thêm dự án!',
                                    life: 2000,
                                })
                                break
                            default:
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Lỗi ',
                                    life: 3000,
                                })
                        }
                    } catch (error) {
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
                    }
                }
            },
            backToUser() {
                this.$router.push('/users')
            },
            closeDialog() {
                this.submitted = false
            },
            onShow() {
                if (this.iduser) {
                    HTTP.get('Users/getUserById/' + this.iduser)
                        .then((res) => {
                            if (res.status == 200) {
                                this.form = res.data
                                // this.userCode = res.data.userCode
                                this.form.dateStartWork =
                                    res.data.dateStartWork == null
                                        ? null
                                        : new Date(
                                              res.data.dateStartWork,
                                          ) /*moment(res.data.dateStartWork).format('DD/MM/YYYY')*/
                                this.form.dateLeave =
                                    res.data.dateLeave == null
                                        ? null
                                        : new Date(
                                              res.data.dateLeave,
                                          ) /*moment(res.data.dateLeave).format('DD/MM/YYYY')*/
                                this.form.dob =
                                    res.data.dOB == null
                                        ? null
                                        : new Date(res.data.dOB) /*moment(res.data.dOB).format('DD/MM/YYYY')*/
                                this.optionworkStatus = [
                                    { name: 'Đang làm', code: 1 },
                                    { name: 'Đã từ chức', code: 2 },
                                    { name: 'Nghỉ thai sản', code: 3 },
                                ]
                                if (this.form.gender != 2) {
                                    this.optionworkStatus = [
                                        { name: 'Đang làm', code: 1 },
                                        { name: 'Đã từ chức', code: 2 },
                                    ]
                                }
                            }
                        })
                        .catch((er) => {
                            this.$toast.add({
                                severity: ToastSeverity.ERROR,
                                summary: 'Lỗi ',
                                detail: 'Không thể lấy thông tin của người dùng !',
                                life: 3000,
                            })
                        })
                }
            },
        },
        components: { LayoutDefault },
    }
</script>

<style lang="scss" scoped>
    .form-demo {
        .card {
            min-width: 450px;

            border: none;
            form {
                margin-top: 2rem;
            }

            .text-center {
                font-size: 30px;
                width: 64%;
                display: flex;
            }
            .field-checkbox {
                width: 36%;
                display: flex;
                justify-content: flex-end;
            }
            .field {
                margin-bottom: 1.5rem;
                font-size: small;
            }
        }
        .admin__form--header {
            display: flex;
        }
        .checkbox__admin {
            width: 100%;
            height: 50px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        .field-checkbox-items {
            margin-right: 5px;
        }

        .cardrelax {
        }

        @media screen and (max-width: 960px) {
        }
    }
</style>
