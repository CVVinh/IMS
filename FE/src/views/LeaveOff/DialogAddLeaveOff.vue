<template>
    <Dialog
        :header="selectedLeaveOff.id ? 'Sửa' : 'Thêm'"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '55vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container">
            <form class="form-addproject" @submit.prevent="submitRegisterLeaveOff()">
                <div class="col-md-12 mb-3 mt-3 d-flex">
                    <div class="d-flex align-items-center">
                        <InputSwitch id="onLeaveOff" v-model="onLeaveOff" />
                        <label class="ms-2" for="onLeaveOff">Nghỉ trong ngày</label>
                    </div>
                </div>
                <div class="row mb-2">
                    <div class="col-md-4">
                        <div class="field">
                            <label
                                class="mb-2"
                                for="dateStart"
                                :class="{ 'p-error': v$.leaveOff.startTime.$invalid && submitted }"
                            >
                                Ngày bắt đầu
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.startTime.$error }">
                                <Calendar
                                    id="dateStart"
                                    v-model="v$.leaveOff.startTime.$model"
                                    :showIcon="true"
                                    :showTime="onLeaveOff"
                                    :showSeconds="true"
                                    autocomplete="off"
                                    inputId="time12"
                                    dateFormat="yy-mm-dd"
                                    :class="{ 'p-invalid': v$.leaveOff.startTime.$invalid && submitted }"
                                />
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.startTime.required.$invalid && isSubmit">
                                {{ v$.leaveOff.startTime.required.$message.replace('Value', 'Start Time') }}
                            </small>
                        </div>
                    </div>
                    <div class="col-md-4">
                        <div class="field">
                            <label
                                class="mb-2"
                                for="dateEnd"
                                :class="{ 'p-error': v$.leaveOff.endTime.$invalid && submitted }"
                            >
                                Ngày kết thúc
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.endTime.$error }">
                                <Calendar
                                    id="dateEnd"
                                    v-model="v$.leaveOff.endTime.$model"
                                    :showIcon="true"
                                    :showTime="onLeaveOff"
                                    :showSeconds="true"
                                    autocomplete="off"
                                    inputId="time12"
                                    dateFormat="yy-mm-dd"
                                    :class="{ 'p-invalid': v$.leaveOff.endTime.$invalid && submitted }"
                                />
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.endTime.required.$invalid && isSubmit">
                                {{ v$.leaveOff.endTime.required.$message.replace('Value', 'Ngày kết thúc') }}
                            </small>
                        </div>
                    </div>
                    <div class="col-md-4 p-float-labe">
                        <label
                            class="mb-2"
                            for="dateEnd"
                            :class="{ 'p-error': v$.leaveOff.idCompanyBranh.$invalid && submitted }"
                        >
                            Chọn chi nhánh
                            <span style="color: red">*</span>
                        </label>
                        <Dropdown
                            id="idCompanyBranh"
                            v-model="v$.leaveOff.idCompanyBranh.$model"
                            :options="arrCompany"
                            optionLabel="name"
                            optionValue="id"
                            placeholder="Chọn Chi Nhánh"
                        />
                        <small class="p-error" v-if="v$.leaveOff.idCompanyBranh.required.$invalid && isSubmit">
                            {{ v$.leaveOff.idCompanyBranh.required.$message.replace('Value', 'Chi nhánh công ty') }}
                        </small>
                    </div>
                </div>
                <div class="input-layout w-100">
                    <label class="mb-2" for="Reason" :class="{ 'p-error': v$.leaveOff.reason.$invalid && submitted }">
                        Lý do
                        <span style="color: red">*</span>
                    </label>
                    <Textarea
                        id="Reason"
                        v-model="v$.leaveOff.reason.$model"
                        placeholder="Nhập lý do nghỉ tại đây..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.leaveOff.reason.required.$invalid && isSubmit">
                        {{ v$.leaveOff.reason.required.$message.replace('Value', 'Reason') }}
                    </small>
                </div>
                <div class="group-button mt-3">
                    <div>
                        <Button label="Lưu" class="p-button-sm me-1" type="submit" icon="pi pi-check" />{{ ' ' }}
                        <Button label="Hủy" class="p-button-sm p-button-secondary" @click="closeDialog()" />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import { HTTP, GET_LEAVE_OFF_BY_ID, ENDPIONTS, UPDATE_LEAVE_OFF } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { LeaveOffDto } from '@/views/LeaveOff/LeaveOff.dto'
    import { required } from '@vuelidate/validators'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'
    import jwtDecode from 'jwt-decode'
    import { Company } from './Company'
    export default {
        props: ['isOpen', 'selectedLeaveOff'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                leaveOff: new LeaveOffDto(),
                isSubmit: false,
                submited: false,
                userAccept: jwtDecode(localStorage.getItem('token')),
                timeFormat: new Date(),
                onLeaveOff: false,
                idCompanyBranh: 1,
                arrCompany: Company,
            }
        },
        created() {
            this.timeFormat.setHours(0)
            this.timeFormat.setMinutes(0)
            this.timeFormat.setSeconds(0)
        },
        watch: {
            onLeaveOff: {
                handler: function Change(status) {
                    if (status == false) {
                        this.timeFormat.setHours(0)
                        this.timeFormat.setMinutes(0)
                        this.timeFormat.setSeconds(0)
                        this.leaveOff.startTime = this.timeFormat
                        this.leaveOff.endTime = this.timeFormat
                    }
                },
                deep: true,
            },
        },
        beforeUpdate() {
            this.onLeaveOff = false
            this.leaveOff.startTime = this.timeFormat
            this.leaveOff.endTime = this.timeFormat
            if (this.selectedLeaveOff.id) {
                HTTP.get(GET_LEAVE_OFF_BY_ID(this.selectedLeaveOff.id))
                    .then((res) => {
                        if (res.status === HttpStatus.OK) {
                            var date1 = new Date(res.data._Data.startTime)
                            var date2 = new Date(res.data._Data.endTime)

                            if (
                                date1.getHours() != 0 ||
                                date1.getMinutes() != 0 ||
                                date2.getHours() != 0 ||
                                date2.getMinutes() != 0
                            ) {
                                this.onLeaveOff = true
                            }
                            this.leaveOff.reason = res.data._Data.reasons
                            this.leaveOff.startTime = new Date(res.data._Data.startTime)
                            this.leaveOff.endTime = new Date(res.data._Data.endTime)
                            this.leaveOff.idCompanyBranh = res.data._Data.idCompanyBranh
                        }
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: err.message,
                            life: 2000,
                        })
                    })
            }
        },
        methods: {
            submitRegisterLeaveOff() {
                this.isSubmit = true
                var date1 = new Date(this.leaveOff.startTime)
                var date2 = new Date(this.leaveOff.endTime)
                if (!this.v$.$invalid && this.checkDate(this.leaveOff.startTime, this.leaveOff.endTime)) {
                    return
                }
                if (this.onLeaveOff) {
                    if (
                        date1.getDate() != date2.getDate() ||
                        date1.getMonth() != date2.getMonth() ||
                        date1.getFullYear() != date2.getFullYear()
                    ) {
                        this.toastWarn('Chỉ cho phép nghỉ trong ngày, ngày tháng năm phải trùng nhau !')
                        return
                    }
                    if (date1.getDay() == 0 || date1.getDay() == 6 || date2.getDay() == 0 || date2.getDay() == 6) {
                        this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                        return
                    }
                }
                if (date1.getDay() == 0 || date1.getDay() == 6) {
                    this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                    return
                } else if (
                    (date1.getDay() == 5 && date2.getDay() == 6) ||
                    (date1.getDay() == 5 && date2.getDay() == 0)
                ) {
                    this.toastWarn('Không được phép nghỉ thứ 7, CN !')
                    return
                }
                if (!this.v$.$invalid) {
                    if (this.selectedLeaveOff.id) {
                        this.handlerEditLeaveOff()
                    } else {
                        this.handlerAddLeaveOff()
                    }
                }
            },
            handlerAddLeaveOff() {
                const addNewLeaveOffDto = {
                    idLeaveUser: this.userAccept.Id,
                    startTime: DateHelper.formatDateTime(this.leaveOff.startTime),
                    endTime: DateHelper.formatDateTime(this.leaveOff.endTime),
                    reasons: this.leaveOff.reason,
                    idCompanyBranh: this.leaveOff.idCompanyBranh,
                }
                if (addNewLeaveOffDto) {
                    HTTP.post(ENDPIONTS.ADD_NEW_LEAVE_OFF, addNewLeaveOffDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.toastSuccess(res.data._Message)
                                this.closeDialog()
                                this.getAllLeaveOff()
                            } else {
                                this.closeDialog()
                            }
                        })
                        .catch((err) => {
                            this.toastWarn(err.message)
                        })
                }
            },
            handlerEditLeaveOff() {
                const editRegisterLeaveOffDtos = {
                    idLeaveUser: this.userAccept.Id,
                    startTime: DateHelper.formatDateTime(this.leaveOff.startTime),
                    endTime: DateHelper.formatDateTime(this.leaveOff.endTime),
                    reasons: this.leaveOff.reason,
                    idCompanyBranh: this.leaveOff.idCompanyBranh,
                }
                if (editRegisterLeaveOffDtos) {
                    HTTP.put(UPDATE_LEAVE_OFF(this.selectedLeaveOff.id), editRegisterLeaveOffDtos)
                        .then((res) => {
                            switch (res.status) {
                                case HttpStatus.OK:
                                    this.toastSuccess(res.data._Message)
                                    this.closeDialog()
                                    this.getAllLeaveOff()
                                    break
                                case HttpStatus.FORBIDDEN:
                                case HttpStatus.UNAUTHORIZED:
                                    this.toastError(res.data._Message)
                                    this.onClickCancel()
                                    break
                                default:
                            }
                        })
                        .catch((err) => {
                            this.toastWarn(err.message)
                        })
                }
            },
            getAllLeaveOff() {
                this.$emit('getAllLeaveOff')
            },
            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            resetForm() {
                this.isSubmit = false
                this.leaveOff = {
                    startTime: null,
                    endTime: null,
                    reason: null,
                    idCompanyBranh: 1,
                }
            },
            toastSuccess(message) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: message,
                    life: 3000,
                })
            },
            toastWarn(err) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: err,
                    life: 3000,
                })
            },
            toastError(err) {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Lỗi',
                    detail: err,
                    life: 3000,
                })
            },
            checkDate(startDate, endDate) {
                if (startDate > endDate) {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Lỗi',
                        detail: 'Ngày kết thúc không được nhỏ hơn ngày bắt đầu!',
                        life: 3000,
                    })
                    return true
                }
                return false
            },
        },
        validations() {
            return {
                leaveOff: {
                    endTime: { required },
                    startTime: { required },
                    reason: { required },
                    idCompanyBranh: { required },
                },
            }
        },
    }
</script>
<style>
    .p-dropdown-panel {
        width: 9.3%;
        min-width: 9.3%;
    }
</style>
