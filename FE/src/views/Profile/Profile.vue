<template>
    <LayoutDefaultDynamic>
        <Toast position="top-right" />
        <form @submit.prevent="handleSubmit(!v$.$invalid)" class="container" enctype="multipart/form-data">
            <div class="box">
                <div class="box-top" v-bind="{ backgroundImage: `url('../../assets/svg-profile.svg')` }">
                    <div class="d-flex justify-content-center align-items-center">
                        <image-input v-model="imageData" type="file" />
                        <div
                            class="image-input rounded-circle"
                            :style="{ 'background-image': `url(${imageData})` }"
                            style="width: 270px; height: 270px; border: 2px dashed"
                            @click="chooseImage"
                        >
                            <span v-if="!imageData" class="placeholders rounded-circle">Chọn ảnh đại điện</span>
                            <input class="file-input" ref="fileInput" type="file" @input="onSelectFile" />
                        </div>
                        <!-- <Avatar v-else class="avatar" :label="renderAvatar()" size="large" shape="circle" /> -->
                    </div>
                    <h3 class="user-code">Tên người dùng: {{ user.userCode }}</h3>
                    <h6 class="user-swd">Ngày bắt đầu làm việc: {{ renderDateStartWork() }}</h6>
                </div>
            </div>
            <div class="box box-bottom">
                <div class="row">
                    <div class="field">
                        <label for="firstName" :class="{ 'p-error': v$.user.firstName.$invalid && submitted }"
                            >Họ
                        </label>
                        <InputText
                            id="firstName"
                            type="text"
                            v-model="v$.user.firstName.$model"
                            :class="{ 'form-group--error': v$.user.firstName.$error }"
                        />
                        <small v-if="v$.user.firstName.$invalid && submitted" class="p-error small-error">{{
                            v$.user.firstName.required.$message.replace('Value', 'First name')
                        }}</small>
                    </div>
                    <div class="field">
                        <label for="lastName" :class="{ 'p-error': v$.user.lastName.$invalid && submitted }"
                            >Tên
                        </label>
                        <InputText
                            id="lastName"
                            type="text"
                            v-model="v$.user.lastName.$model"
                            :class="{ 'form-group--error': v$.user.lastName.$error }"
                        />
                        <small v-if="v$.user.lastName.$invalid && submitted" class="p-error small-error">{{
                            v$.user.lastName.required.$message.replace('Value', 'Last name')
                        }}</small>
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="birthday" :class="{ 'p-error': !checkDateOfBirth && submitted }">Ngày sinh</label>
                        <Calendar
                            v-if="!loading"
                            id="birthday"
                            v-model="v$.user.dOB.$model"
                            :showIcon="true"
                            :class="{ 'calendar-error': !checkDateOfBirth }"
                        />
                        <small v-if="!checkDateOfBirth && submitted" class="p-error small-error"
                            >Ngày sinh nhỏ hơn 60 & lớn hơn 18</small
                        >
                    </div>
                    <div class="field">
                        <label for="identityCard" :class="{ 'p-error': v$.user.identitycard.$invalid && submitted }"
                            >Căn cước công dân</label
                        >
                        <InputText
                            id="identityCard"
                            type="text"
                            v-model="v$.user.identitycard.$model"
                            :class="{ 'form-group--error': v$.user.identitycard.$error && submitted }"
                        />
                        <span v-if="v$.user.identitycard.$error && submitted">
                            <span v-for="(error, index) of v$.user.identitycard.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-if="
                                (v$.user.identitycard.$invalid && submitted) || v$.user.identitycard.$pending.$response
                            "
                            class="p-error"
                            >Độ dài số phải là 9 hoặc 12</small
                        >
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="phoneNumber" :class="{ 'p-error': v$.user.phoneNumber.$error && submitted }">
                            Số điện thoại
                        </label>
                        <InputText
                            id="phoneNumber"
                            type="text"
                            v-model="v$.user.phoneNumber.$model"
                            :class="{
                                'form-group--error': v$.user.phoneNumber.$error && submitted,
                            }"
                        />
                        <span v-if="v$.user.phoneNumber.$errors && submitted">
                            <span v-for="(error, index) of v$.user.phoneNumber.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                        <small
                            v-if="(v$.user.phoneNumber.$invalid && submitted) || v$.user.phoneNumber.$pending.$response"
                            class="p-error"
                            >Độ dài số điện thoại phải là 10
                        </small>
                    </div>
                    <!-- ----------------------------- -->
                    <div class="field">
                        <label for="email" :class="{ 'p-error': v$.user.email.$invalid && submitted }">Email</label>
                        <InputText
                            id="email"
                            type="text"
                            v-model="v$.user.email.$model"
                            :class="{ 'form-group--error': v$.user.email.$error }"
                        />
                        <span v-if="v$.user.email.$error && submitted">
                            <span v-for="(error, index) of v$.user.email.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="skype" :class="{ 'p-error': v$.user.skype.$invalid && submitted }">Skype</label>
                        <InputText
                            id="skype"
                            type="text"
                            v-model="v$.user.skype.$model"
                            :class="{ 'form-group--error': v$.user.skype.$error }"
                        />
                        <span v-if="v$.user.skype.$error && submitted">
                            <span v-for="(error, index) of v$.user.skype.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                    </div>
                    <div class="field">
                        <label for="group">Nhóm</label>
                        <Dropdown
                            id="group"
                            type="text"
                            v-model="v$.user.idGroup.$model"
                            :options="groupList"
                            optionLabel="nameGroup"
                            optionValue="id"
                            :disabled="true"
                        />
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="gender" :class="{ 'p-error': v$.user.gender.$invalid && submitted }"
                            >Giới tính</label
                        >
                        <Dropdown
                            id="gender"
                            v-model="v$.user.gender.$model"
                            :options="optionGender"
                            optionValue="code"
                            optionLabel="name"
                            :class="{ 'dropdown-error': v$.user.gender.$error }"
                            v-bind:style="{
                                borderColor: v$.user.gender.$invalid && submitted ? errorColor : '#ced4da',
                            }"
                        />
                        <small v-if="v$.user.gender.$invalid && submitted" class="p-error">Giới tính bắt buộc</small>
                    </div>
                    <div class="field">
                        <label for="maritalStatus">Tình trạng hôn nhân</label>
                        <Dropdown
                            id="maritalStatus"
                            v-model="v$.user.maritalStatus.$model"
                            :options="optionMaritalStatus"
                            optionValue="code"
                            optionLabel="name"
                        />
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="university">Đại học</label>
                        <InputText id="university" type="text" v-model="v$.user.university.$model" :showIcon="true" />
                    </div>
                    <div class="field">
                        <label for="yearGraduated" :class="{ 'p-error': v$.user.yearGraduated.$invalid && submitted }"
                            >Năm tốt nghiệp</label
                        >
                        <InputText
                            id="yearGraduated"
                            type="text"
                            v-model="v$.user.yearGraduated.$model"
                            :class="{ 'form-group--error': v$.user.yearGraduated.$invalid && submitted }"
                        />
                        <span v-if="v$.user.yearGraduated.$invalid && submitted">
                            <span v-for="(error, index) of v$.user.yearGraduated.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                    </div>
                </div>
                <div class="row">
                    <div class="field">
                        <label for="address" :class="{ 'p-error': v$.user.address.$invalid && submitted }"
                            >Địa chỉ</label
                        >
                        <InputText
                            id="address"
                            type="text"
                            v-model="v$.user.address.$model"
                            :showIcon="true"
                            :class="{ 'form-group--error': v$.user.address.$error }"
                        />
                        <span v-if="v$.user.address.$error && submitted">
                            <span v-for="(error, index) of v$.user.address.$errors" :key="index">
                                <small class="p-error">{{ error.$message }}</small>
                            </span>
                        </span>
                    </div>
                </div>
                <div class="row">
                    <div class="bottom-button">
                        <Button style="margin-right: 10px" type="submit" @click="submit">Lưu</Button>
                        <Button class="p-button-secondary" @click="handleReset">Đặt lại</Button>
                    </div>
                </div>
            </div>
        </form>
        <Toast position="top-right" />
    </LayoutDefaultDynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import moment from 'moment'
    import jwt_decode from 'jwt-decode'
    import LayoutDefaultDynamic from '../../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { email, required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { ToastSeverity } from 'primevue/api'
    import { DateHelper } from '@/helper/date.helper'
    import { UserService } from '@/service/user.service'
    export default {
        name: 'profile',
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                submitted: false,
                loading: true,
                groupList: [],
                user: {
                    id: 0,
                    userCode: null,
                    password: null,
                    email: null,
                    userCreate: localStorage.getItem('username'),
                    dateCreate: new Date(),
                    // dateModified: new Date(),
                    passwordEmail: null,
                    firstName: '',
                    lastName: '',
                    phoneNumber: '',
                    dOB: new Date(),
                    gender: null,
                    address: '',
                    university: '',
                    yearGraduated: null,
                    skype: '',
                    passwordSkype: null,
                    dateStartWork: '',
                    dateLeave: '',
                    maritalStatus: null,
                    reasonResignation: null,
                    workStatus: null,
                    idGroup: null,
                    dOBValidate: true,
                    formFile: null,
                },
                optionGender: [
                    { name: 'Nam', code: 1 },
                    { name: 'Nữ', code: 2 },
                    { name: 'Khác', code: 3 },
                ],
                optionMaritalStatus: [
                    { name: 'Đã kêt hôn', code: 1 },
                    { name: 'Chưa kết hôn', code: 2 },
                    { name: 'Không xác định', code: 3 },
                ],
                errorColor: '#ff0053',
                imageData: null,
            }
        },
        validations() {
            return {
                user: {
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
                        required,
                        checkPhoneNumber(value) {
                            if (!value) return true
                            if (value.length === 0) return true
                            if (value.includes('.')) return false
                            if (value.length === 10 || value.length === 11) return true
                            else return false
                        },
                    },
                    dOB: {},
                    identitycard: {
                        numeric,
                        required,
                        valueNineOrTwelve(value) {
                            if (value.includes('.')) return false
                            if (value.length === 9 || value.length === 12) return true
                            else return false
                        },
                    },
                    workStatus: {},
                    gender: {
                        required,
                    },
                    address: {
                        maxLength: maxLength(200),
                    },
                    university: {},
                    yearGraduated: {
                        required,
                        numeric,
                        between: between(1980, Number(new Date().getFullYear())),
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
        computed: {
            checkDateOfBirth() {
                const dOB = this.v$.user.dOB.$model
                if (dOB === '') {
                    return true
                }
                var today = new Date()
                var thisYear = new Date(today).getFullYear()
                var birthYear = new Date(dOB).getFullYear()
                if (thisYear - birthYear >= 18 && thisYear - birthYear <= 60) {
                    return true
                } else {
                    return false
                }
            },
        },
        mounted() {
            this.getAPIGroup()
            this.getAPIUser()
        },
        methods: {
            getAPIGroup() {
                HTTP.get('Group/getListGroup/')
                    .then((res) => {
                        if (res.status === 200) {
                            this.groupList = res.data
                        }
                    })
                    .catch((er) => {
                        this.$toast.add({
                            severity: ToastSeverity.ERROR,
                            summary: 'Lỗi ',
                            detail: 'Không thể lấy thông tin nhóm!',
                            life: 3000,
                        })
                    })
            },
            getAPIUser() {
                const user = LocalStorage.jwtDecodeToken()
                UserService.getUserById(user.Id)
                    .then((res) => {
                        if (res.status == 200) {
                            let arr = res.data
                            arr = res.data
                            arr.dateStartWork =
                                res.data.dateStartWork === null ? null : new Date(res.data.dateStartWork)
                            arr.dateLeave = res.data.dateLeave === null ? null : new Date(res.data.dateLeave)
                            arr.dOB = res.data.dOB === null ? null : new Date(res.data.dOB)
                            this.user = arr
                            this.imageData = res.data.avatarLink
                            this.loading = false
                        }
                    })
                    .catch(() => {
                        this.messageError('Không thể lấy thông tin người dùng!')
                    })
            },
            renderAvatar() {
                return this.user.firstName !== null ? this.user.firstName.charAt(0) : '?'
            },
            chooseImage() {
                this.$refs.fileInput.click()
            },
            onSelectFile() {
                const input = this.$refs.fileInput
                const files = input.files
                if (files && files[0]) {
                    const reader = new FileReader()
                    reader.onload = (e) => {
                        this.imageData = e.target.result
                    }
                    reader.readAsDataURL(files[0])
                    this.$emit('input', files[0])
                }
                this.user.formFile = files[0]
            },
            handleSubmit(isFormValid) {
                this.submitted = true
                if (!isFormValid) {
                    return
                } else {
                    this.submit()
                }
            },
            submit() {
                if (!this.checkDateOfBirth) {
                    return
                }
                const idUser = this.user.id
                if (idUser) {
                    const token = localStorage.getItem('token')
                    const decode = jwt_decode(token)
                    const fromData = new FormData()
                    fromData.append('userModified', parseInt(decode.Id))
                    fromData.append('firstName', this.user.firstName)
                    fromData.append('lastName', this.user.lastName)
                    fromData.append('phoneNumber', this.user.phoneNumber)
                    fromData.append('dOB', DateHelper.formatDateTime(this.user.dOB === null ? null : this.user.dOB))
                    fromData.append('identitycard', this.user.identitycard)
                    fromData.append('gender', this.user.gender)
                    fromData.append('address', this.user.address)
                    fromData.append('university', this.user.university ?? null)
                    fromData.append('yearGraduated', this.user.yearGraduated ?? null)
                    fromData.append('email', this.user.email)
                    fromData.append('skype', this.user.skype)
                    fromData.append('workStatus', this.user.workStatus)
                    fromData.append(
                        'dateLeave',
                        DateHelper.formatDateTime(this.user.dateLeave == null ? null : this.user.dateLeave),
                    )
                    fromData.append('maritalStatus', this.user.maritalStatus)
                    fromData.append('reasonResignation', this.user.reasonResignation)
                    fromData.append('idGroup', this.user.idGroup)
                    fromData.append('formFileAvatar', this.user.formFile)

                    UserService.updateProfileUser(idUser, fromData)
                        .then((res) => {
                            if (res.status == 200) {
                                this.messageSuccess('Sửa thành công!')
                                this.getAPIUser()
                            }
                        })
                        .catch((error) => {
                            switch (error.code) {
                                case 'ERR_NETWORK':
                                    this.messageError('Kiểm tra kết nối !')
                                    break
                                case 'ERR_BAD_REQUEST':
                                    this.messageError('Đã xảy ra một hoặc nhiều lỗi xác thực.')
                                    break
                            }
                        })
                }
            },
            handleReset() {
                this.submitted = false
                this.getAPIUser()
            },
            renderDateStartWork() {
                const date = this.user.dateStartWork
                if (date) return moment(date).format('DD/MM/YYYY')
                else return ''
            },
            async backToDashboard() {
                await this.$router.push('/')
            },
            messageSuccess(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            messageError(mess) {
                this.$toast.add({
                    severity: ToastSeverity.ERROR,
                    summary: 'Lỗi',
                    detail: mess,
                    life: 3000,
                })
            },
        },

        components: { LayoutDefaultDynamic },
    }
</script>

<style lang="scss" scoped>
    .container {
        margin-top: 2vh;
        width: 100%;
        padding: 30px;
        // max-width: 900px;
        display: flex;
    }

    .box {
        width: 100%;
        justify-content: center;
    }

    .box-top {
        height: 100%;
        width: 100%;
        display: flex;
        align-items: center;
        flex-direction: column;
        justify-content: center;
        border-right: 1px solid #eee;
        background-image: url('../../assets/svg-profile.svg');
        background-size: 360px;
        background-repeat: no-repeat;
        background-position: center;
    }

    .box-bottom {
        padding-left: 1.2rem;
        min-width: 700px;
    }

    .avatar {
        margin-bottom: 0;
        background-color: #317de0;
        width: 160px;
        height: 160px;
        font-size: 80px;
        color: #fff;
    }

    .user-code {
        margin-top: 14px;
        text-align: center;
        color: #666666;
    }

    .user-swd {
        text-align: center;
        color: #666666;
    }

    .field {
        font-size: small;
        display: flex;
        flex-direction: column;
        flex: 1;

        label {
            margin-bottom: 5px;
            padding: 0;
        }
    }

    .row {
        display: flex;
        width: 100%;
        padding-top: 16px;
    }

    .bottom-button {
        display: flex;
        width: 100%;
        justify-content: end;
    }

    .form-group--error {
        border-color: #ff0053;
    }

    .p-error {
        color: #ff0053;
    }

    .small-error {
        margin-top: 4px;
        margin-bottom: -10px;
    }

    .calendar-error ::v-deep(.p-inputtext) {
        border-color: #ff0053;
    }

    .avatar-animation:active {
        width: 160px;
        height: 160px;
        animation-iteration-count: initial;
        animation: ani 0.3s;
        animation-timing-function: linear;
        animation-delay: 0s;
        animation-iteration-count: infinite;
        animation-direction: alternate;
    }

    @keyframes ani {
        0% {
            width: 160px;
            height: 160px;
            font-size: 80px;
            box-shadow: 0px 0px 50px #317de0;
        }

        50% {
            width: 190px;
            height: 190px;
            font-size: 90px;
            box-shadow: 0px 0px 100px #317de0;
        }

        100% {
            width: 180px;
            height: 180px;
            font-size: 80px;
            box-shadow: 0px 0px 10px #317de0;
        }
    }

    @media only screen and (max-width: 1200px) {
        .container {
            flex-direction: column;
        }

        .box-top {
            border: none;
            padding-bottom: 20px;
            border-bottom: 1px solid #eee;
            background-size: 260px;
        }
    }
    .image-input {
        display: block;
        width: 200px;
        height: 200px;
        cursor: pointer;
        background-size: cover;
        background-position: center center;
    }

    .placeholders {
        background: #f0f0f0;
        width: 100%;
        height: 100%;
        display: flex;
        justify-content: center;
        align-items: center;
        color: #333;
        font-size: 18px;
        font-family: Helvetica;
    }

    .placeholders:hover {
        background: #e0e0e0;
        cursor: pointer;
    }

    .file-input {
        display: none;
    }
</style>
