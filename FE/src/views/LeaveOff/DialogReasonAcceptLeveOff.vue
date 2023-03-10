<template>
    <Dialog
        header="Lý do duyệt phép nghỉ ?"
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
                    <label
                        class="mb-2"
                        for="name"
                        :class="{ 'p-error': v$.leaveOff.reasonAccept.$invalid && submitted }"
                    >
                        Lý do duyệt
                        <span style="color: red">*</span>
                    </label>
                    <Textarea
                        v-model="v$.leaveOff.reasonAccept.$model"
                        placeholder="Nhập lý do duyệt tại đây..."
                        class="input form-control"
                        rows="5"
                    />
                    <small class="p-error" v-if="v$.leaveOff.reasonAccept.required.$invalid && isSubmit">
                        {{ v$.leaveOff.reasonAccept.required.$message.replace('Value', 'Lý do duyệt') }}
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
    import { HTTP, ACCEPT_LEAVE_OFF } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwtDecode from 'jwt-decode'
import checkAccessModule from '@/stores/checkAccessModule'
    export default {
        props: ['isOpen', 'idLeaveOff'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                leaveOff: {
                    reasonAccept: null,
                },
                isSubmit: false,
                submited: false,
            }
        },
        methods: {
            submitLeaveOff() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    var acceptLeaveOffDto = {
                        idAcceptUser: checkAccessModule.getUserIdCurrent(),
                        ReasonAccept: this.leaveOff.reasonAccept,
                    }
                    HTTP.put(ACCEPT_LEAVE_OFF(this.idLeaveOff), acceptLeaveOffDto)
                        .then((res) => {
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Thành công',
                                    detail: res.data._Message,
                                    life: 3000,
                                })
                                this.submited = true
                                this.$emit('setChange')
                                this.resetForm()
                                this.closeDialog()
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
                this.resetForm()
                this.$emit('closeDialog')
            },
            resetForm() {
                this.isSubmit = false
                this.leaveOff = {
                    reasonAccept: null,
                }
            },
        },
        validations() {
            return {
                leaveOff: {
                    reasonAccept: { required },
                },
            }
        },
    }
</script>
<style scoped></style>
