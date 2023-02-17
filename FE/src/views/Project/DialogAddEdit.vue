<template>
    <div>
        <Dialog
            :header="projectSelected.id ? 'Edit Project' : 'Add Project'"
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
                            <label class="ms-2" for="OnGitlab">On GitLab</label>
                        </div>
                    </div>
                    <div class="col-12 row">
                        <div class="col-6">
                            <div v-if="isOnGitlab == true" class="field mb-4">
                                <label
                                    class="mb-2"
                                    :class="{ 'p-error': v$.dataProject.projectCode.$invalid && submitted }"
                                >
                                    Project GitLab
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
                                        placeholder="Select a Project"
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
                                    Project name
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
                                    Project code
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
                            >Description</label
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
                                    Start date
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
                                    Start date must be bigger than today!
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
                                    End date
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
                                    End date must be bigger than Start date!
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
                                :options="leader"
                                optionLabel="name"
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
                    <div class="">
                        <button type="submit" class="btn btn-primary">Save</button>&nbsp;
                        <button type="button" class="btn btn-secondary" v-on:click="onClickCancel()">Cancel</button>
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
            }
        },
        beforeUpdate() {
            this.leader.map((item) => {
                item['name'] = `${item.firstName} ${item.lastName}`
            })
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
            onClickCancel() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            getAllProject() {
                this.$emit('getAllProject')
            },
            handleSubmit(isFormValid) {
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
            },
            AddData() {
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
                console.log(dataSave)
                if (dataSave) {
                    HTTP.post('Project/addProject', dataSave)
                        .then((res) => {
                            if (res.status == 200) {
                                this.showSuccess()
                                this.onClickCancel()
                                this.getAllProject()
                            } else {
                                this.onClickCancel()
                            }
                        })
                        .catch((er) => {
                            this.showWarn('Không có quyền thực hiện thao tác thêm dự án.')
                        })
                }
            },
            editData() {
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
                                        summary: 'Error',
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
                                summary: 'Warn Message',
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
                    summary: 'Success Message',
                    detail: 'Save project success !!!',
                    life: 3000,
                })
            },
            showWarn(err) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Warn Message',
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

<style scoped>
    .select_project_displayNone {
        display: block;
    }
    .input_disabled {
        background: #ececec !important;
    }
</style>
