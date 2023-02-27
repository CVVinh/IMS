<template>
    <layout-default-dynamic>
        <!-- <ProgressBar mode="indeterminate" style="height: .5em" /> -->
        <div>
            <Toast position="top-right" />
        </div>
        <div class="form-demo">
            <div class="flex justify-content-center container">
                <div class="card cardrelax">
                    <div class="admin__form--header">
                        <div></div>
                        <h5 class="text-center">Thêm</h5>
                        <div class="field-checkbox">
                            <Checkbox inputId="binary" v-model="checked" :binary="true" class="field-checkbox-items" />
                            <label for="binary">Send mail</label>
                        </div>
                    </div>

                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.userCode.$error }">
                                        <InputText
                                            id=" userCode"
                                            v-model="v$.userCode.$model"
                                            :class="{ 'p-invalid': v$.userCode.$invalid && submitted }"
                                        />
                                        <label for="userCode" :class="{ 'p-error': v$.userCode.$invalid && submitted }"
                                            >Account*</label
                                        >
                                    </div>
                                    <span v-if="v$.userCode.$error && submitted">
                                        <span
                                            id="userCode-error"
                                            v-for="(error, index) of v$.userCode.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.userCode.$invalid && submitted) || v$.userCode.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.userCode.required.$message.replace('Value', 'Account') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label">
                                        <Password
                                            id="password"
                                            v-model="v$.password.$model"
                                            :class="{ 'p-invalid': v$.password.$invalid && submitted }"
                                            toggleMask
                                        >
                                            <template #header>
                                                <h6>Pick a password</h6>
                                            </template>
                                            <template #footer="sp">
                                                {{ sp.level }}
                                                <Divider />
                                            </template>
                                        </Password>
                                        <label for="password" :class="{ 'p-error': v$.password.$invalid && submitted }"
                                            >Password*</label
                                        >
                                    </div>
                                    <span v-if="v$.password.$error && submitted">
                                        <span
                                            id="password-error"
                                            v-for="(error, index) of v$.password.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.password.$invalid && submitted) || v$.password.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.password.required.$message.replace('Value', 'Password') }}</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.firstName.$error }">
                                        <InputText
                                            id=" firstName"
                                            v-model="v$.firstName.$model"
                                            :class="{ 'p-invalid': v$.firstName.$invalid && submitted }"
                                        />
                                        <label
                                            for="firstName"
                                            :class="{ 'p-error': v$.firstName.$invalid && submitted }"
                                            >FirstName*</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.firstName.$invalid && submitted) || v$.firstName.$pending.$response"
                                        class="p-error"
                                        >{{ v$.firstName.required.$message.replace('Value', 'First name') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.lastName.$error }">
                                        <InputText
                                            id=" lastName"
                                            v-model="v$.lastName.$model"
                                            :class="{ 'p-invalid': v$.lastName.$invalid && submitted }"
                                        />
                                        <label for="lastName" :class="{ 'p-error': v$.lastName.$invalid && submitted }"
                                            >LastName*</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.lastName.$invalid && submitted) || v$.lastName.$pending.$response"
                                        class="p-error"
                                        >{{ v$.lastName.required.$message.replace('Value', 'Last name') }}</small
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
                                            v-model="v$.email.$model"
                                            :class="{ 'p-invalid': v$.email.$invalid && submitted }"
                                            aria-describedby="email-error"
                                        />
                                        <label for="email" :class="{ 'p-error': v$.email.$invalid && submitted }"
                                            >Email*</label
                                        >
                                    </div>
                                    <span v-if="v$.email.$error && submitted">
                                        <span id="email-error" v-for="(error, index) of v$.email.$errors" :key="index">
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="(v$.email.$invalid && submitted) || v$.email.$pending.$response"
                                        class="p-error"
                                        >{{ v$.email.required.$message.replace('Value', 'Email') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.phoneNumber.$error }">
                                        <InputText
                                            id=" phoneNumber"
                                            v-model="v$.phoneNumber.$model"
                                            :class="{ 'p-invalid': v$.phoneNumber.$invalid && submitted }"
                                        />
                                        <label
                                            for="phoneNumber"
                                            :class="{ 'p-error': v$.phoneNumber.$invalid && submitted }"
                                            >PhoneNumber</label
                                        >
                                    </div>
                                    <span v-if="v$.phoneNumber.$error && submitted">
                                        <span
                                            id="phoneNumber-error"
                                            v-for="(error, index) of v$.phoneNumber.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.phoneNumber.$invalid && submitted) || v$.phoneNumber.$pending.$response
                                        "
                                        class="p-error"
                                        >{{ v$.phoneNumber.required.$message.replace('Value', 'PhoneNumber') }}</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-12 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.address.$error }">
                                        <Textarea
                                            rows="3"
                                            cols="30"
                                            id=" address"
                                            v-model="v$.address.$model"
                                            :class="{ 'p-invalid': v$.address.$invalid && submitted }"
                                        />

                                        <label for="address" :class="{ 'p-error': v$.address.$invalid && submitted }"
                                            >Address</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.address.$invalid && submitted) || v$.address.$pending.$response"
                                        class="p-error"
                                        >{{ v$.address.required.$message.replace('Value', 'Address') }}</small
                                    >
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.identitycard.$error }">
                                        <InputText
                                            id=" identitycard"
                                            v-model="v$.identitycard.$model"
                                            :class="{ 'p-invalid': v$.identitycard.$invalid && submitted }"
                                        />
                                        <label
                                            for="identitycard"
                                            :class="{ 'p-error': v$.identitycard.$invalid && submitted }"
                                            >Identity Card*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.identitycard.$invalid && submitted) ||
                                            v$.identitycard.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.identitycard.required.$message.replace('Value', 'Identity card')
                                        }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.maritalStatus.$error }"
                                    >
                                        <Dropdown
                                            v-model="v$.maritalStatus.$model"
                                            :options="optionMaritalStatus"
                                            optionLabel="name"
                                            :class="{ 'p-invalid': v$.maritalStatus.$invalid && submitted }"
                                        />
                                        <label
                                            for="maritalStatus"
                                            :class="{ 'p-error': v$.maritalStatus.$invalid && submitted }"
                                            >Marital Status</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.maritalStatus.$invalid && submitted) ||
                                            v$.maritalStatus.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.maritalStatus.required.$message.replace('Value', 'Marital Status')
                                        }}</small
                                    >
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col col-sm-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.gender.$error }">
                                        <Dropdown
                                            v-model="v$.gender.$model"
                                            :options="optionGender"
                                            optionLabel="name"
                                            :class="{ 'p-invalid': v$.gender.$invalid && submitted }"
                                        />
                                        <label for="gender" :class="{ 'p-error': v$.gender.$invalid && submitted }"
                                            >Gender*</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.gender.$invalid && submitted) || v$.gender.$pending.$response"
                                        class="p-error"
                                        >{{ v$.gender.required.$message.replace('Value', 'Gender') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-sm-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.idRole.$error }">
                                        <Dropdown
                                            v-model="v$.idRole.$model"
                                            :options="optionRoles"
                                            optionLabel="name"
                                            :class="{ 'p-invalid': v$.idRole.$invalid && submitted }"
                                        />
                                        <label for="idRole" :class="{ 'p-error': v$.idRole.$invalid && submitted }"
                                            >Role*</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.idRole.$invalid && submitted) || v$.idRole.$pending.$response"
                                        class="p-error"
                                        >{{ v$.idRole.required.$message.replace('Value', 'Role') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.skype.$error }">
                                        <InputText
                                            id=" skype"
                                            v-model="v$.skype.$model"
                                            :class="{ 'p-invalid': v$.skype.$invalid && submitted }"
                                        />
                                        <label for="skype" :class="{ 'p-error': v$.skype.$invalid && submitted }"
                                            >Skype</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.skype.$invalid && submitted) || v$.skype.$pending.$response"
                                        class="p-error"
                                        >{{ v$.skype.required.$message.replace('Value', 'Skype') }}</small
                                    >
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.university.$error }">
                                        <InputText
                                            id=" university"
                                            v-model="v$.university.$model"
                                            :class="{ 'p-invalid': v$.university.$invalid && submitted }"
                                        />
                                        <label
                                            for="university"
                                            :class="{ 'p-error': v$.university.$invalid && submitted }"
                                            >University</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.university.$invalid && submitted) || v$.university.$pending.$response"
                                        class="p-error"
                                        >{{ v$.university.required.$message.replace('Value', 'University') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.yearGraduated.$error }"
                                    >
                                        <InputText
                                            id=" yearGraduated"
                                            v-model="v$.yearGraduated.$model"
                                            :class="{ 'p-invalid': v$.yearGraduated.$invalid && submitted }"
                                        />
                                        <label
                                            for="yearGraduated"
                                            :class="{ 'p-error': v$.yearGraduated.$invalid && submitted }"
                                            >Year graduated</label
                                        >
                                    </div>
                                    <span v-if="v$.yearGraduated.$error && submitted">
                                        <span
                                            id="yearGraduated-error"
                                            v-for="(error, index) of v$.yearGraduated.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">{{ error.$message }}</small>
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.yearGraduated.$invalid && submitted) ||
                                            v$.yearGraduated.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.yearGraduated.required.$message.replace('Value', 'yearGraduated')
                                        }}</small
                                    >
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col col-md-3 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.dateStartWork.$error }"
                                    >
                                        <Calendar
                                            id="dateStartWork"
                                            v-model="v$.dateStartWork.$model"
                                            :showIcon="true"
                                            :class="{ 'p-invalid': v$.dateStartWork.$invalid && submitted }"
                                        />
                                        <label
                                            for="dateStartWork"
                                            :class="{ 'p-error': v$.dateStartWork.$invalid && submitted }"
                                            >Date start work*</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.dateStartWork.$invalid && submitted) ||
                                            v$.dateStartWork.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.dateStartWork.required.$message.replace('Value', 'Date start work')
                                        }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-md-3 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.dob.$error }">
                                        <Calendar
                                            id="dob"
                                            v-model="v$.dob.$model"
                                            :showIcon="true"
                                            :class="{ 'p-invalid': (v$.dob.$invalid || dOBValidate) && submitted }"
                                        />
                                        <label
                                            for="dob"
                                            :class="{ 'p-error': (v$.dob.$invalid || dOBValidate) && submitted }"
                                            >Birthday</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.dob.$invalid && submitted) || v$.dob.$pending.$response"
                                        class="p-error"
                                        >{{ v$.dob.required.$message.replace('Value', 'Birthday') }}</small
                                    >
                                    <small v-if="!this.checkDateOfBirth(v$.dob.$model) && submitted" class="p-error">
                                        User must be at least 18 years old
                                    </small>
                                </div>
                            </div>
                            <div class="col col-md-6 col-12">
                                <div class="field">
                                    <div
                                        class="p-float-label"
                                        :class="{ 'form-group--error': v$.dob.$error } + ' checkbox__admin'"
                                    ></div>
                                    <small
                                        v-if="(v$.gender.$invalid && submitted) || v$.gender.$pending.$response"
                                        class="p-error"
                                        >{{ v$.gender.required.$message.replace('Value', 'Gender') }}</small
                                    >
                                </div>
                            </div>
                            <div class="col col-sm-6 col-12">
                                <div class="field">
                                    <div class="p-float-label" :class="{ 'form-group--error': v$.IdGroup.$error }">
                                        <Dropdown
                                            v-model="v$.IdGroup.$model"
                                            :options="optionRoles"
                                            optionLabel="name"
                                            :class="{ 'p-invalid': v$.IdGroup.$invalid && submitted }"
                                        />
                                        <label for="IdGroup" :class="{ 'p-error': v$.IdGroup.$invalid && submitted }"
                                            >Role*</label
                                        >
                                    </div>
                                    <small
                                        v-if="(v$.IdGroup.$invalid && submitted) || v$.IdGroup.$pending.$response"
                                        class="p-error"
                                        >{{ v$.IdGroup.required.$message.replace('Value', 'Role') }}</small
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
                                <Button label="Hủy" class="p-button-secondary" @click.prevent="backToUser()" />
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </layout-default-dynamic>
</template>

<script>
    import { email, required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                userCode: '',
                password: '',
                email: '',
                userCreate: localStorage.getItem('username'),
                dateCreate: new Date(),
                firstName: '',
                lastName: '',
                phoneNumber: '',
                identitycard: '',
                dob: '',
                gender: '',
                address: '',
                university: '',
                yearGraduated: '',
                skype: '',
                dateStartWork: '',
                maritalStatus: '',
                workStatus: 1,
                dOBValidate: true,
                IdGroup: '',
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
                messages: [{ severity: 'info', content: 'Thông tin' }],
            }
        },
        validations() {
            return {
                userCode: {
                    required,
                },
                email: {
                    required,
                    email,
                    maxLength: maxLength(50),
                },
                password: {
                    required,
                    minLength: minLength(8),
                },
                firstName: {
                    required,
                },
                lastName: {
                    required,
                },
                phoneNumber: {
                    numeric,
                    minLength: minLength(10),
                    maxLength: maxLength(11),
                },
                identitycard: {
                    required,
                    numeric,
                    maxLength: maxLength(12),
                    minLength: minLength(9),
                },
                dob: {},
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
                dateStartWork: {
                    required,
                },
                maritalStatus: {},
                IdGroup: {
                    required,
                },
            }
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
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)
                const data = {
                    userCode: this.userCode,
                    userPassword: this.password,
                    userCreated: parseInt(decode.Id),
                    dateCreated: this.dateCreate,
                    email: this.email,
                    firstName: this.firstName,
                    lastName: this.lastName,
                    phoneNumber: this.phoneNumber,
                    dOB: this.dob,
                    gender: this.gender.code,
                    address: this.address,
                    university: this.university,
                    yearGraduated: this.yearGraduated,
                    identitycard: this.identitycard,
                    skype: this.skype,
                    workStatus: this.workStatus,
                    dateStartWork: this.dateStartWork,
                    dateLeave: null,
                    maritalStatus: this.maritalStatus.code,
                    IdGroup: this.IdGroup.code,
                }
                const CallApi = async () => {
                    try {
                        const res = await HTTP.post('Users/addUser', data)
                        if (res.status == 200)
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Thêm mới thành công!',
                                life: 3000,
                            })
                        this.$router.push('/users')
                    } catch (error) {
                        switch (error.code) {
                            case 'ERR_NETWORK':
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: 'Kiểm tra kết nối !',
                                    life: 3000,
                                })
                                break
                            case 'ERR_BAD_REQUEST':
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: error.response.data,
                                    life: 3000,
                                })
                                break
                        }
                    }
                }
                //CallApi()
            },
            resetForm() {
                this.userCode = ''
                this.password = ''
                this.email = ''
                this.userCreate = localStorage.getItem('username')
                this.dateCreate = new Date()
                this.firstName = ''
                this.lastName = ''
                this.phoneNumber = ''
                this.identitycard = ''
                this.dob = ''
                this.gender = ''
                this.address = ''
                this.university = ''
                this.yearGraduated = ''
                this.skype = ''
                this.dateStartWork = ''
                this.maritalStatus = ''
                this.workStatus = 1
                this.dOBValidate = true
                this.IdGroup = ''
                this.submitted = false
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
                var age = thisYear - birthYear
                if (age >= 18 && age <= 60) {
                    this.dOBValidate = false
                    return true
                } else {
                    this.dOBValidate = true
                    return false
                }
            },
        },
    }
</script>
<style lang="scss" scoped>
    .form-demo {
        margin-top: 2%;

        .card {
            min-width: 450px;
            padding: 10px;
            margin-left: 200px;
            form {
                margin-top: 2rem;
            }

            .text-center {
                width: 60%;
                display: flex;
                justify-content: flex-end;
            }
            .field-checkbox {
                width: 40%;
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
            width: 900px;
        }

        @media screen and (max-width: 960px) {
        }
    }
</style>
