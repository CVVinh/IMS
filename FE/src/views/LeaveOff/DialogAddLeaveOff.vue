<template>
    <Dialog
        :header="selectedLeaveOff.id ? 'Edit leave off' : 'Add leave off'"
        :maximizable="true"
        :closable="false"
        position="center"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container">
            <form class="form-addproject" @submit.prevent="submitRegisterLeaveOff()">
                <div class="row mb-2">
                    <div class="col-md-6">
                        <div class="field">
                            <label
                                class="mb-2"
                                for="dateStart"
                                :class="{ 'p-error': v$.leaveOff.startTime.$invalid && submitted }"
                            >
                                Start Date
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.startTime.$error }">
                                <Calendar
                                    id="dateStart"
                                    v-model="v$.leaveOff.startTime.$model"
                                    :showIcon="true"
                                    :showTime="true"
                                    :showSeconds="true"
                                    autocomplete="off"
                                    inputId="time12"
                                    :class="{ 'p-invalid': v$.leaveOff.startTime.$invalid && submitted }"
                                />
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.startTime.required.$invalid && isSubmit">
                                {{ v$.leaveOff.startTime.required.$message.replace('Value', 'Start Time') }}
                            </small>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="field">
                            <label
                                class="mb-2"
                                for="dateEnd"
                                :class="{ 'p-error': v$.leaveOff.endTime.$invalid && submitted }"
                            >
                                End Date
                                <span style="color: red">*</span>
                            </label>
                            <div class="p-float-label" :class="{ 'form-group--error': v$.leaveOff.endTime.$error }">
                                <Calendar
                                    id="dateEnd"
                                    v-model="v$.leaveOff.endTime.$model"
                                    :showIcon="true"
                                    :showTime="true"
                                    :showSeconds="true"
                                    autocomplete="off"
                                    inputId="time12"
                                    :class="{ 'p-invalid': v$.leaveOff.endTime.$invalid && submitted }"
                                />
                            </div>
                            <small class="p-error" v-if="v$.leaveOff.endTime.required.$invalid && isSubmit">
                                {{ v$.leaveOff.endTime.required.$message.replace('Value', 'End Time') }}
                            </small>
                        </div>
                    </div>
                </div>
                <div class="input-layout w-100">
                    <label class="mb-2" for="Reason" :class="{ 'p-error': v$.leaveOff.reason.$invalid && submitted }">
                        Reason
                        <span style="color: red">*</span>
                    </label>
                    <Textarea
                        id="Reason"
                        v-model="v$.leaveOff.reason.$model"
                        placeholder="Enter the reason for not approving leave here..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.leaveOff.reason.required.$invalid && isSubmit">
                        {{ v$.leaveOff.reason.required.$message.replace('Value', 'Reason') }}
                    </small>
                </div>
                <div class="group-button mt-3">
                    <div>
                        <Button label="Submit" class="p-button-sm me-1" type="submit" icon="pi pi-check" />{{ ' ' }}
                        <Button label="Cancel" class="p-button-sm p-button-secondary" @click="closeDialog()" />
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
    export default {
        props: ['isOpen', 'selectedLeaveOff'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                leaveOff: new LeaveOffDto(),
                isSubmit: false,
                submited: false,
                userAccept: jwtDecode(localStorage.getItem('token')),
            }
        },
        beforeUpdate() {
            if (this.selectedLeaveOff.id) {
                HTTP.get(GET_LEAVE_OFF_BY_ID(this.selectedLeaveOff.id))
                    .then((res) => {
                        if (res.status === HttpStatus.OK) {
                            this.leaveOff.reason = res.data._Data.reasons
                            this.leaveOff.startTime = res.data._Data.startTime
                            this.leaveOff.endTime = res.data._Data.endTime
                        }
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Error message',
                            detail: err.message,
                            life: 2000,
                        })
                    })
            }
        },
        methods: {
            submitRegisterLeaveOff() {
                this.isSubmit = true
                if (!this.v$.$invalid && this.checkDate(this.leaveOff.startTime, this.leaveOff.endTime)) {
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
                }
                if (editRegisterLeaveOffDtos) {
                    HTTP.put(UPDATE_LEAVE_OFF(this.selectedLeaveOff.id), editRegisterLeaveOffDtos)
                        .then((res) => {
                            console.log(res)
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
                }
            },
            toastSuccess(message) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Success message',
                    detail: message,
                    life: 3000,
                })
            },
            toastWarn(err) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Warn message',
                    detail: err,
                    life: 3000,
                })
            },
            toastError(err) {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Error message',
                    detail: err,
                    life: 3000,
                })
            },
            checkDate(startDate, endDate) {
                if (startDate > endDate) {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Error message',
                        detail: 'End date need to be bigger than start date !',
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
                },
            }
        },
    }
</script>
<style scoped></style>
