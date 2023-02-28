<template>
    <div>
        <Dialog
            :header="projectSelected.id ? 'Sửa dự án' : 'Thêm dự án'"
            :visible="isOpenDialog"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '50vw' }"
            :maximizable="true"
            :modal="true"
            :closable="false"
        >
            <div class="container">
                <form ref="formAddProject" @submit.prevent="() => handleSubmit(!v$.$invalid)" class="p-fluid">
                    <div class="col-md-12 mb-3">
                        <div class="d-flex align-items-center">
                            <InputSwitch id="OnGitlab" v-model="isOnGitlab" />
                            <label class="ms-2" for="OnGitlab">Dự án trên GitLab</label>
                        </div>
                    </div>
                    <div class="col-12 row">
                        <div class="col-6">
                            <div v-if="isOnGitlab == true" class="field mb-4">
                                <label
                                    class="mb-2"
                                    :class="{ 'p-error': v$.dataProject.projectCode.$invalid && submitted }"
                                >
                                    Dự án trên GitLab
                                    <span style="color: red">*</span>
                                </label>
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.dataProject.projectCode.$error }"
                                >
                                    <Dropdown
                                        v-model="v$.dataProject.projectCode.$model"
                                        :options="projectGit"
                                        optionLabel="name"
                                        optionValue="id"
                                        placeholder="Nhập dự án"
                                        :filter="true"
                                        :showClear="true"
                                        :class="{ 'p-invalid': v$.dataProject.projectCode.$invalid && submitted }"
                                    />
                                </div>
                                <small
                                    v-if="
                                        (v$.dataProject.projectCode.$invalid && submitted) ||
                                        v$.dataProject.projectCode.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.dataProject.projectCode.required.$message.replace('Value', 'projectGit')
                                    }}</small
                                >
                            </div>
                            <div v-else class="field mb-4">
                                <label
                                    class="mb-2"
                                    for="name"
                                    :class="{ 'p-error': v$.dataProject.name.$invalid && submitted }"
                                >
                                    Tên dự án
                                    <span style="color: red">*</span>
                                </label>
                                <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.name.$error }">
                                    <InputText
                                        id="name"
                                        :disabled="isOnGitlab === true"
                                        v-model="v$.dataProject.name.$model"
                                        :class="{
                                            input_disabled: isOnGitlab === true,
                                            'p-invalid': v$.dataProject.name.$invalid && submitted,
                                        }"
                                    />
                                </div>
                                <span v-if="v$.dataProject.name.$error && submitted">
                                    <span
                                        id="name-error"
                                        v-for="(error, index) of v$.dataProject.name.$errors"
                                        :key="index"
                                    >
                                        <small class="p-error">
                                            {{ error.$message.replace('Value', 'Project name') }}
                                        </small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.dataProject.name.$invalid && submitted) ||
                                        v$.dataProject.name.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.dataProject.name.required.$message.replace('Value', 'Project name') }}</small
                                >
                            </div>
                        </div>

                        <div class="col-6">
                            <div class="field mb-4">
                                <label
                                    class="mb-2"
                                    for="projectCode"
                                    :class="{ 'p-error': v$.dataProject.projectCode.$invalid && submitted }"
                                >
                                    Mã dự án
                                    <span style="color: red">*</span>
                                </label>
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.dataProject.projectCode.$error }"
                                >
                                    <InputText
                                        id="projectCode"
                                        v-model="v$.dataProject.projectCode.$model"
                                        :disabled="isOnGitlab === true"
                                        :class="{
                                            input_disabled: isOnGitlab === true,
                                            'p-invalid': v$.dataProject.projectCode.$invalid && submitted,
                                        }"
                                    />
                                </div>
                                <span v-if="v$.dataProject.projectCode.$error && submitted">
                                    <span
                                        id="projectCode-error"
                                        v-for="(error, index) of v$.dataProject.projectCode.$errors"
                                        :key="index"
                                    >
                                        <small class="p-error">{{
                                            error.$message.replace('Value', 'Project code')
                                        }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="
                                        (v$.dataProject.projectCode.$invalid && submitted) ||
                                        v$.dataProject.projectCode.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.dataProject.projectCode.required.$message.replace('Value', 'Project code')
                                    }}</small
                                >
                            </div>
                        </div>
                    </div>
                    <div class="col-12 row field mb-4">
                        <label
                            class="mb-2"
                            for="description"
                            :class="{ 'p-error': v$.dataProject.description.$invalid && submitted }"
                            >Mô tả</label
                        >
                        <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.description.$error }">
                            <Textarea
                                id="description"
                                style="height: 150px"
                                v-model="v$.dataProject.description.$model"
                                :class="{
                                    'p-invalid': v$.dataProject.description.$invalid && submitted,
                                }"
                            />
                        </div>
                        <span v-if="v$.dataProject.description.$error && submitted">
                            <span
                                id="description-error"
                                v-for="(error, index) of v$.dataProject.description.$errors"
                                :key="index"
                            >
                                <small class="p-error">{{ error.$message.replace('Value', 'Description') }}</small>
                            </span>
                        </span>
                    </div>
                    <div class="col-12 row">
                        <div class="col-6">
                            <div class="field mb-4">
                                <label
                                    class="mb-2"
                                    for=" startDate"
                                    :class="{
                                        'p-error':
                                            (v$.dataProject.startDate.$invalid || !checkStartDate()) && submitted,
                                    }"
                                >
                                    Ngày bắt đầu
                                    <span style="color: red">*</span>
                                </label>
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.dataProject.startDate.$error }"
                                >
                                    <Calendar
                                        id="startDate"
                                        v-model="v$.dataProject.startDate.$model"
                                        :showIcon="true"
                                        :disabled="isOnGitlab === true"
                                        :class="{
                                            input_disabled: isOnGitlab === true,
                                            'p-invalid':
                                                (v$.dataProject.startDate.$invalid || !checkStartDate()) && submitted,
                                        }"
                                    />
                                </div>
                                <small
                                    v-if="
                                        (v$.dataProject.startDate.$invalid && submitted) ||
                                        v$.dataProject.startDate.$pending.$response
                                    "
                                    class="p-error"
                                    >{{
                                        v$.dataProject.startDate.required.$message.replace('Value', 'Start date')
                                    }}</small
                                >
                                <small
                                    v-if="!v$.dataProject.startDate.$invalid && !checkStartDate() && submitted"
                                    class="p-error"
                                >
                                    Ngày bắt đầu phải lớn hơn hôm nay!
                                </small>
                            </div>
                        </div>

                        <div class="col-6">
                            <div class="field mb-4">
                                <label
                                    class="mb-2"
                                    for=" endDate"
                                    :class="{
                                        'p-error': (v$.dataProject.endDate.$invalid || !checkEndDate()) && submitted,
                                    }"
                                >
                                    Ngày kết thúc
                                    <span style="color: red">*</span>
                                </label>
                                <div
                                    class="p-float-label"
                                    :class="{ 'form-group--error': v$.dataProject.endDate.$error }"
                                >
                                    <Calendar
                                        id="endDate"
                                        v-model="v$.dataProject.endDate.$model"
                                        :showIcon="true"
                                        :disabled="isOnGitlab === true"
                                        :class="{
                                            input_disabled: isOnGitlab === true,
                                            'p-invalid':
                                                (v$.dataProject.endDate.$invalid || !checkEndDate()) && submitted,
                                        }"
                                    />
                                </div>
                                <small
                                    v-if="
                                        (v$.dataProject.endDate.$invalid && submitted) ||
                                        v$.dataProject.endDate.$pending.$response
                                    "
                                    class="p-error"
                                    >{{ v$.dataProject.endDate.required.$message.replace('Value', 'End date') }}</small
                                >
                                <small
                                    v-if="!v$.dataProject.endDate.$invalid && !checkEndDate() && submitted"
                                    class="p-error"
                                >
                                    Ngày kết thúc phải lớn hơn ngày bắt đầu!
                                </small>
                            </div>
                        </div>
                    </div>
                    <div class="col-12 row field mb-4">
                        <label
                            class="mb-2"
                            for="leader"
                            :class="{ 'p-error': v$.dataProject.leader.$invalid && submitted }"
                        >
                            Leader
                            <span style="color: red">*</span>
                        </label>
                        <div class="p-float-label" :class="{ 'form-group--error': v$.dataProject.leader.$error }">
                            <Dropdown
                                v-model="v$.dataProject.leader.$model"
                                :options="Optionleader"
                                optionLabel="fullName"
                                optionValue="id"
                                :class="{ 'p-invalid': v$.dataProject.leader.$invalid && submitted }"
                            />
                        </div>
                        <small
                            v-if="
                                (v$.dataProject.leader.$invalid && submitted) ||
                                v$.dataProject.leader.$pending.$response
                            "
                            class="p-error"
                            >{{ v$.dataProject.leader.required.$message.replace('Value', 'Leader') }}</small
                        >
                    </div>
                    <div class="btn-right">
                        <Button
                            label="Lưu"
                            type="submit"
                            icon="pi pi-check"
                            style="margin-right: 10px"
                            v-on:click="handleSubmit()"
                        />
                        <Button label="Hủy" class="p-button-secondary" v-on:click="onClickCancel()" />
                    </div>
                </form>
            </div>
        </Dialog>
    </div>
</template>

<script>
    import { useVuelidate } from '@vuelidate/core'
    import { maxLength, required } from '@vuelidate/validators'
    import { ProjectDto } from '@/views/Project/Project.dto'
    import { LocalStorage, LocalStorageKey } from '@/helper/local-storage.helper'
    import { DateHelper } from '@/helper/date.helper'
    import { HttpStatus } from '@/config/app.config'
    import { UserService } from '@/service/user.service'
    import { HTTP } from '@/http-common'

    export default {
        name: 'DialogAddEdit',
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                valid: true,
                submitted: false,
                dataProject: new ProjectDto(),
                isOnGitlab: false,
                Optionleader: false,
            }
        },
        beforeUpdate() {
            this.getListLeader()
            if (this.projectSelected.id) {
                HTTP.get('Project/getProjectById/' + this.projectSelected.id).then((res) => {
                    if (res.status === HttpStatus.OK) {
                        console.log(res.data)
                        this.dataProject = res.data
                        this.dataProject.startDate = new Date(res.data.startDate)
                        this.dataProject.endDate = new Date(res.data.endDate)
                        this.isOnGitlab = res.data.isOnGitlab
                        this.dataProject.projectCode = res.data.projectCode
                    }
                })
            }
        },
        validations() {
            return {
                dataProject: {
                    name: { required, maxLength: maxLength(50) },
                    projectCode: { required, maxLength: maxLength(10) },
                    description: { maxLength: maxLength(1000) },
                    startDate: { required },
                    endDate: { required },
                    leader: { required },
                },
            }
        },
        props: ['isOpenDialog', 'projectSelected', 'user', 'leader', 'projectGit'],
        watch: {
            dataProject: {
                handler: async function Change(newVal) {
                    if (this.isOnGitlab) {
                        var project = this.projectGit.filter(function (el) {
                            return el.id == newVal.projectCode
                        })
                        if (this.dataProject.projectCode === null) {
                            this.dataProject.description = null
                            this.dataProject.startDate = null
                            this.dataProject.endDate = null
                            this.dataProject.name = null
                        }
                        // this.dataProject.description = project[0].description
                        this.dataProject.startDate = new Date(project[0].created_at)
                        this.dataProject.endDate = null
                        this.dataProject.name = project[0].name
                    } else {
                        this.dataProject.name = newVal.name
                    }
                },
                deep: true,
            },
        },
        methods: {
            getListLeader() {
                HTTP.get('/Project/getListLead')
                    .then((res) => {
                        this.Optionleader = res.data
                        console.log(res.data)
                    })
                    .catch((err) => console.log(err))
            },
            onClickCancel() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            getAllProject() {
                this.$emit('getAllProject')
            },
            handleSubmit(isFormValid) {
                this.submitted = true
                if (!this.isOnGitlab) {
                    if (!isFormValid) {
                        return
                    }
                    if (!this.valid || !this.checkStartDate() || !this.checkEndDate()) return
                }
                if (this.projectSelected.id) {
                    this.editData()
                } else {
                    this.AddData()
                }
                console.log(this.projectSelected.id)
            },
            AddData() {
                console.log('add')
                let userLogin = LocalStorage.jwtDecodeToken()
                const dataSave = {
                    name: this.dataProject.name,
                    projectCode: this.dataProject.projectCode,
                    description: this.dataProject.description,
                    startDate: DateHelper.convertUTC(this.dataProject.startDate),
                    endDate: DateHelper.convertUTC(this.dataProject.endDate),
                    isFinished: this.dataProject.isFinished,
                    isDeleted: this.dataProject.isDeleted,
                    userId: userLogin.Id,
                    leader: this.dataProject.leader,
                    dateCreated: new Date(),
                    dateUpdate: new Date(),
                    userUpdate: userLogin.Id,
                    userCreated: userLogin.Id,
                    isOnGitlab: this.isOnGitlab,
                }
                if (this.isOnGitlab) {
                    dataSave.endDate = new Date()
                }

                if (dataSave) {
                    HTTP.post('Project/addProject', dataSave)
                        .then((res) => {
                            console.log(res.data)
                            if (res.status == 200) {
                                this.showSuccess()
                                this.onClickCancel()
                                this.getAllProject()
                            } else {
                                this.onClickCancel()
                            }
                        })
                        .catch((er) => {
                            //this.showWarn('Không có quyền thực hiện thao tác thêm dự án.')
                            console.log(er)
                        })
                }
            },
            editData() {
                console.log('edit')
                let userLogin = LocalStorage.jwtDecodeToken()
                let data = {
                    id: this.dataProject.id,
                    name: this.dataProject.name,
                    projectCode: this.dataProject.projectCode,
                    description: this.dataProject.description,
                    startDate: DateHelper.convertUTC(this.dataProject.startDate),
                    endDate: DateHelper.convertUTC(this.dataProject.endDate),
                    isFinished: this.dataProject.isFinished,
                    isDeleted: this.dataProject.isDeleted,
                    userId: this.dataProject.userId,
                    leader: this.dataProject.leader,
                    dateUpdate: new Date(),
                    userUpdate: userLogin.Id,
                    isOnGitlab: this.isOnGitlab,
                }
                if (this.isOnGitlab) {
                    data.endDate = new Date()
                }
                console.log(data)
                if (data) {
                    HTTP.put(`Project/updateProject/${data.id}`, data)
                        .then((res) => {
                            switch (res.status) {
                                case HttpStatus.OK:
                                    this.showSuccess()
                                    this.onClickCancel()
                                    this.getAllProject()
                                    break
                                case HttpStatus.FORBIDDEN:
                                case HttpStatus.UNAUTHORIZED:
                                    this.$toast.add({
                                        severity: 'error',
                                        summary: 'Lỗi',
                                        detail: 'Không có quyền thực hiện thao tác thêm dự án.',
                                        life: 2000,
                                    })
                                    this.onClickCancel()
                                    break
                                default:
                            }
                        })
                        .catch((er) => {
                            this.$toast.add({
                                severity: 'warn',
                                summary: 'Cảnh báo',
                                detail: 'Không có quyền thực hiện thao tác sửa dự án.',
                                life: 2000,
                            })
                        })
                }
            },
            resetForm() {
                this.$refs.formAddProject.reset()
                this.v$.$reset()
                const initialData = this.$options.data.call(this)
                Object.assign(this.$data, initialData)
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Thêm mới thành công!',
                    life: 3000,
                })
            },
            showWarn(err) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: err,
                    life: 3000,
                })
            },
            checkStartDate() {
                if (this.dataProject.startDate < new Date(new Date().toLocaleDateString('en-EU'))) {
                    return (this.valid = false)
                } else {
                    return (this.valid = true)
                }
            },
            checkEndDate() {
                return (this.valid = DateHelper.compareDate(this.dataProject.startDate, '<=', this.dataProject.endDate))
            },
        },
        mounted() {
            let value = localStorage.getItem(LocalStorageKey.USER_NAME)
            UserService.getUserByCode(value).then((res) => {
                if (res.status === HttpStatus.OK) {
                    this.dataProject.userCreated = res.data.id
                    this.dataProject.userUpdate = res.data.id
                }
            })
        },
    }
</script>

<style>
    .select_project_displayNone {
        display: block;
    }
    .input_disabled {
        background: #ececec !important;
    }
    .btn-right {
        float: right;
        width: 240px;
        display: inline-flex;
        height: 40px;
    }
    .p-multiselect.p-component.p-inputwrapper.p-inputwrapper-filled.p-inputwrapper-focus.p-overlay-open {
        width: 100%;
    }
    .p-multiselect.p-component.p-inputwrapper {
        width: 100%;
    }
</style>
