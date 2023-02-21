<template>
    <Dialog
        header="Lý do không cho phép nghỉ ?"
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
            <form class="form-addproject" @submit.prevent="submitLeaveOff()">
                <div class="input-layout w-100">
                    <label class="mb-2" for="name" :class="{ 'p-error': v$.leaveOff.reason.$invalid && submitted }">
                        Lý do
                        <span style="color: red">*</span>
                    </label>
                    <Textarea
                        v-model="v$.leaveOff.reason.$model"
                        placeholder="Nhập lý do tại đây..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.leaveOff.reason.required.$invalid && isSubmit">
                        {{ v$.leaveOff.reason.required.$message.replace('Value', 'Reason') }}
                    </small>
                </div>
                <div class="group-button mt-3">
                    <div>
                        <Button label="Hoàn tất" class="p-button-sm me-1" type="submit" icon="pi pi-check" />{{ ' ' }}
                        <Button label="Hủy" class="p-button-sm p-button-secondary" @click="closeDialog()" />
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import { HTTP, NOT_ACCEPT_LEAVE_OFF } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwtDecode from 'jwt-decode'
    export default {
        props: ['isOpen', 'idLeaveOff'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                leaveOff: {
                    reason: null,
                },
                isSubmit: false,
                submited: false,
                userAccept: jwtDecode(localStorage.getItem('token')),
            }
        },
        methods: {
            submitLeaveOff() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    console.log(this.idLeaveOff)
                    console.log(this.leaveOff.reason)
                    var notAcceptLeaveOffDto = {
                        idAcceptUser: this.userAccept.Id,
                        ReasonNotAccept: this.leaveOff.reason,
                    }
                    HTTP.put(NOT_ACCEPT_LEAVE_OFF(this.idLeaveOff), notAcceptLeaveOffDto)
                        .then((res) => {
                            console.log(res)
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Successfull',
                                    detail: res.data._Message,
                                    life: 3000,
                                })
                                this.submited = true
                                this.$emit('setChange')
                                this.closeDialog()
                                this.resetForm()
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
            closeDialog() {
                this.$emit('closeDialog')
            },
            resetForm() {
                this.isSubmit = false
                this.leaveOff = {
                    reason: null,
                }
            },
        },
        validations() {
            return {
                leaveOff: {
                    reason: { required },
                },
            }
        },
    }
</script>
<style scoped></style>
