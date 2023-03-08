<template>
    <LayoutDefault>
        <div class="container mt-4">
            <h2 style="text-align: center">CHỈNH SỬA DỰ ÁN</h2>
            <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluid">
                <div class="field">
                    <label for="name" :class="{ 'p-error': v$.data.name.$invalid && submitted }"
                        >Tên dự án<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.name.$error }">
                        <InputText
                            id="name"
                            v-model="v$.data.name.$model"
                            :class="{ 'p-invalid': v$.data.name.$invalid && submitted }"
                        />
                    </div>
                    <span v-if="v$.data.name.$error && submitted">
                        <span id="name-error" v-for="(error, index) of v$.data.name.$errors" :key="index">
                            <small class="p-error">{{ error.$message.replace('Value', 'Tên dự án') }}</small>
                        </span>
                    </span>
                    <small
                        v-else-if="(v$.data.name.$invalid && submitted) || v$.data.name.$pending.$response"
                        class="p-error"
                        >{{ v$.data.name.required.$message.replace('Value', 'Tên dự án') }}</small
                    >
                </div>
                <div class="field">
                    <label for="projectCode" :class="{ 'p-error': v$.data.projectCode.$invalid && submitted }"
                        >Mã dự án<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.projectCode.$error }">
                        <InputText
                            disabled="true"
                            id="projectCode"
                            v-model="v$.data.projectCode.$model"
                            :class="{ 'p-invalid': v$.data.projectCode.$invalid && submitted }"
                        />
                    </div>
                    <span v-if="v$.data.projectCode.$error && submitted">
                        <span id="projectCode-error" v-for="(error, index) of v$.data.projectCode.$errors" :key="index">
                            <small class="p-error">{{ error.$message.replace('Value', 'Mã dự án') }}</small>
                        </span>
                    </span>
                    <small
                        v-else-if="
                            (v$.data.projectCode.$invalid && submitted) || v$.data.projectCode.$pending.$response
                        "
                        class="p-error"
                        >{{ v$.data.projectCode.required.$message.replace('Value', 'Mã dự án') }}</small
                    >
                </div>
                <div class="field">
                    <label for="description" :class="{ 'p-error': v$.data.description.$invalid && submitted }"
                        >Mô tả</label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.description.$error }">
                        <Textarea
                            id="description"
                            style="height: 150px"
                            v-model="v$.data.description.$model"
                            :class="{ 'p-invalid': v$.data.description.$invalid && submitted }"
                        />
                    </div>
                    <span v-if="v$.data.description.$error && submitted">
                        <span id="description-error" v-for="(error, index) of v$.data.description.$errors" :key="index">
                            <small class="p-error">{{ error.$message.replace('Value', 'Mô tả') }}</small>
                        </span>
                    </span>
                </div>
                <div class="field">
                    <label
                        for=" startDate"
                        :class="{ 'p-error': (v$.data.startDate.$invalid || !checkStartdate()) && submitted }"
                        >Ngày bắt đầu<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.startDate.$error }">
                        <Calendar
                            id="startDate"
                            v-model="v$.data.startDate.$model"
                            :showIcon="true"
                            :class="{ 'p-invalid': (v$.data.startDate.$invalid || !checkStartdate()) && submitted }"
                        />
                    </div>
                    <small
                        v-if="(v$.data.startDate.$invalid && submitted) || v$.data.startDate.$pending.$response"
                        class="p-error"
                        >{{ v$.data.startDate.required.$message.replace('Value', 'Ngày bắt đầu') }}</small
                    >
                    <small v-if="!v$.data.startDate.$invalid && !checkStartdate() && submitted" class="p-error">
                        Ngày bắt đầu phải lớn hơn ngày hôm nay!
                    </small>
                </div>
                <div class="field">
                    <label
                        for=" endDate"
                        :class="{ 'p-error': (v$.data.endDate.$invalid || !checkEnddate()) && submitted }"
                        >Ngày kết thúc<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.endDate.$error }">
                        <Calendar
                            id="endDate"
                            v-model="v$.data.endDate.$model"
                            :showIcon="true"
                            :class="{ 'p-invalid': (v$.data.endDate.$invalid || !checkEnddate()) && submitted }"
                        />
                    </div>
                    <small
                        v-if="(v$.data.endDate.$invalid && submitted) || v$.data.endDate.$pending.$response"
                        class="p-error"
                        >{{ v$.data.endDate.required.$message.replace('Value', 'Ngày kết thúc') }}</small
                    >
                    <small v-if="!v$.data.endDate.$invalid && !checkEnddate() && submitted" class="p-error">
                        Ngày kết thúc phải lớn hơn ngày bắt đầu!
                    </small>
                </div>
                <div class="field">
                    <label for="userId" :class="{ 'p-error': v$.data.userId.$invalid && submitted }"
                        >Quản lý dự án<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.userId.$error }">
                        <Dropdown
                            disabled="true"
                            v-model="v$.data.userId.$model"
                            :options="user"
                            optionLabel="userCode"
                            optionValue="id"
                            :class="{ 'p-invalid': v$.data.userId.$invalid && submitted }"
                        />
                    </div>
                    <small
                        v-if="(v$.data.userId.$invalid && submitted) || v$.data.userId.$pending.$response"
                        class="p-error"
                        >{{ v$.data.userId.required.$message.replace('Value', 'Quản lý dự án') }}</small
                    >
                </div>
                <div class="field">
                    <label for="leader" :class="{ 'p-error': v$.data.leader.$invalid && submitted }"
                        >Leader<span style="color: red">*</span></label
                    >
                    <div class="p-float-label" :class="{ 'form-group--error': v$.data.leader.$error }">
                        <Dropdown
                            v-model="v$.data.leader.$model"
                            :options="leser"
                            optionLabel="userCode"
                            optionValue="id"
                            :class="{ 'p-invalid': v$.data.leader.$invalid && submitted }"
                        />
                    </div>
                    <small
                        v-if="(v$.data.leader.$invalid && submitted) || v$.data.leader.$pending.$response"
                        class="p-error"
                        >{{ v$.data.leader.required.$message.replace('Value', 'Leader') }}</small
                    >
                </div>
                <div class="">
                    <button type="submit" class="btn btn-primary">Lưu</button>&nbsp;
                    <button type="button" class="btn btn-secondary" v-on:click="cancel()">Hủy</button>
                </div>
            </form>
        </div>
    </LayoutDefault>
</template>
<script>
    import jwtDecode from 'jwt-decode'
    import { useVuelidate } from '@vuelidate/core'
    import { required, maxLength, helpers } from '@vuelidate/validators'
    import LayoutDefault from '../../layouts/LayoutDefault/LayoutDefault.vue'
    import { HTTP } from '@/http-common'
    import router from '../../router/index.js'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    export default {
        components: { LayoutDefault },
        setup: () => ({
            v$: useVuelidate(),
        }),

        data() {
            return {
                data: {
                    name: null,
                    projectCode: null,
                    description: null,
                    startDate: '',
                    endDate: '',
                    userCreated: null,
                    userUpdate: null,
                    dateCreted: null,
                    dateUpdate: null,
                    userId: null,
                    leader: null,
                },
                user: [],
                leser: [],
                valid: true,
                submitted: false,
            }
        },
        validations() {
            return {
                data: {
                    name: {
                        required: helpers.withMessage('Tên dự án bắt buộc phải có', required),
                        maxLength: maxLength(50),
                    },
                    projectCode: {
                        required: helpers.withMessage('Mã dự án bắt buộc phải có', required),
                        maxLength: helpers.withMessage('Tối đa 10 ký tự', maxLength(10)),
                    },
                    description: {
                        maxLength: helpers.withMessage('Tối đa 100 ký tự', maxLength(1000)),
                    },
                    startDate: {
                        required: helpers.withMessage('Ngày bắt đầu bắt buộc phải có', required),
                    },
                    endDate: {
                        required: helpers.withMessage('Ngày kết thúc bắt buộc phải có', required),
                    },
                    userId: {
                        required: helpers.withMessage('IdUser bắt buộc phải có', required),
                    },
                    leader: {
                        required: helpers.withMessage('Leader bắt buộc phải có', required),
                    },
                },
            }
        },
        methods: {
            handleSubmit(isFormValid) {
                this.submitted = true
                if (!isFormValid) {
                    return
                }
                if (!this.valid || !this.checkStartdate() || !this.checkEnddate()) return
                this.editData()
            },
            editData() {
                let data = {
                    id: this.data.id,
                    name: this.data.name,
                    projectCode: this.data.projectCode,
                    description: this.data.description,
                    startDate: this.data.startDate,
                    endDate: this.data.endDate,
                    isFinished: this.data.isFinished,
                    isDeleted: this.data.isDeleted,
                    userId: this.data.userId,
                    leader: this.data.leader,
                    dateUpdate: this.data.dateUpdate,
                    userUpdate: this.data.userUpdate,
                }

                if (data) {
                    this.data.dateUpdate = new Date()
                    let userlogin = jwtDecode(localStorage.getItem('token'))
                    this.data.userUpdate = userlogin.Id
                    data.startDate = new Date(this.data.startDate).toLocaleString()
                    data.endDate = new Date(this.data.endDate).toLocaleString()

                    HTTP.put('Project/updateProject', data)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess()
                                router.push('/project')
                            }
                        })
                        .catch((er) => {
                            this.showWarn()
                        })
                }
            },
            cancel() {
                router.push('/project')
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Chỉnh sửa dự án thành công !!!',
                    life: 3000,
                })
            },
            showWarn() {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: 'Chỉnh sửa dự án thất bại !!!',
                    life: 3000,
                })
            },
            checkStartdate() {
                if (new Date(this.data.startDate) < new Date(new Date().toLocaleDateString('en-EU'))) {
                    this.valid = false
                    return false
                } else {
                    this.valid = true
                    return true
                }
            },
            checkEnddate() {
                if (new Date(this.data.endDate) <= new Date(this.data.startDate)) {
                    this.valid = false
                    return false
                } else {
                    this.valid = true
                    return true
                }
            },
        },
        mounted() {
            if (!UserRoleHelper.isPm() && !UserRoleHelper.isAdmin()) router.push('/project')
            let value = localStorage.getItem('username')
            HTTP.get('Users/getUserByUserCode/' + value).then((res) => {
                if (res.status == 200) {
                    this.data.userUpdate = res.data.id
                }
            })
            if (this.$route.params)
                HTTP.get('Project/getProjectById/' + this.$route.params.id).then((res) => {
                    if (res.status == 200) {
                        this.data = res.data
                        this.data.startDate = new Date(res.data.startDate)
                        this.data.endDate = new Date(res.data.endDate)
                    }
                })
            HTTP.get('Users/getAll').then((res) => {
                if (res.status == 200)
                    res.data.forEach((element) => {
                        if (element.IdGroup == 3) {
                            this.user.push(element)
                        }
                    })
            })
            HTTP.get('Users/getAll').then((res) => {
                if (res.status == 200)
                    res.data.forEach((element) => {
                        if (element.IdGroup == 4) {
                            this.leser.push(element)
                        }
                    })
            })
        },
    }
</script>
<style>
    .form-control {
        width: 400px;
        height: 40px;
        font-size: medium;
    }

    .format {
        margin-left: 250px;
    }

    .butadd {
        margin-left: 56%;
    }

    .field {
        margin-bottom: 1.5rem;
        font-size: medium;
    }
</style>
