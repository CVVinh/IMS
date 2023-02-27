<template>
    <LayoutDefaultDynamic>
        <div>
            <Toast position="top-right" />
        </div>

        <div class="adduser__div">
            <div class="adduser__div-form">
                <div class="adduser__div-form-title">Add new User</div>
                <form class="adduser__div-form-content">
                    <div class="adduser__div-form-content-items">
                        <!-- Account -->
                        <div
                            class="p-float-label"
                            :class="{ 'form-group--error': v$.userCode.$error } + ' div__inputtext'"
                        >
                            <InputText
                                id=" userCode"
                                v-model="v$.userCode.$model"
                                :class="{ 'p-invalid': v$.userCode.$invalid && submitted } + ' inputtext__admin'"
                            />
                            <label for="userCode" :class="{ 'p-error': v$.userCode.$invalid && submitted }"
                                >Account*</label
                            >
                        </div>
                        <span v-if="v$.userCode.$error && submitted">
                            <span id="userCode-error" v-for="(error, index) of v$.userCode.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.userCode.$invalid && submitted) || v$.userCode.$pending.$response"
                            class="p-error"
                            >{{ v$.userCode.required.$message.replace('Value', 'Account') }}</small
                        >
                        <!-- FirstName -->
                    </div>

                    <div class="adduser__div-form-content-items">
                        <!-- Passwork -->
                        <div class="p-float-label div__inputtext">
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
                            <span id="password-error" v-for="(error, index) of v$.password.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-else-if="(v$.password.$invalid && submitted) || v$.password.$pending.$response"
                            class="p-error"
                            >{{ v$.password.required.$message.replace('Value', 'Password') }}</small
                        >
                    </div>
                </form>
            </div>
        </div>
    </LayoutDefaultDynamic>
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
                idRole: '',
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
                messages: [{ severity: 'info', content: 'Dynamic Info Message' }],
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
                idRole: {
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
                    idRole: this.idRole.code,
                }
                const CallApi = async () => {
                    try {
                        const res = await HTTP.post('Users/addUser', data)
                        if (res.status == 200)
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Info Message',
                                detail: 'Add user success!',
                                life: 3000,
                            })
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
                CallApi()
            },
            resetForm() {
                this.userCode = ''
                this.email = ''
                this.password = ''
                this.date = null
                this.country = null
                this.accept = null
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

<style scoped>
    .adduser__div {
        width: 100%;
        height: 100%;
        display: flex;
        align-items: center;
        justify-content: center;
        padding-top: 50px;
    }

    .adduser__div-form {
        width: 50%;
        height: 650px;
        border: 1px solid rgba(0, 0, 0, 0.125);
        display: flex;
        flex-direction: column;
    }

    .adduser__div-form-title {
        width: 100%;
        margin-top: 20px;
        display: flex;
        align-items: center;
        justify-content: center;
        font-weight: 600;
        font-size: 20px;
    }
    .adduser__div-form-content {
        width: 100%;
        height: 100%;
        display: flex;
    }

    .adduser__div-form-content-items {
        width: 50%;
        height: 100%;
        display: flex;
        justify-content: flex-start;
        align-items: center;
        padding: 10px;
        flex-direction: column;
    }

    .div__inputtext {
        width: 90%;
        margin-top: 20px;
        display: flex;
        justify-content: space-between;
    }

    .inputtext__admin {
        width: 100%;
    }
</style>
