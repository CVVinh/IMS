<template>
    <div>
        <Dialog
            :header="deviceSelected.idDevice ? 'Edit Device' : 'Add Device'"
            :visible="isOpenDialog"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '50vw' }"
            :maximizable="true"
            :modal="true"
            :closable="false"
        >
            <div class="container">
                <form ref="formAddEditDevice" class="p-fluid" @submit.prevent="handleSubmit(!v$.$invalid)">
                    <div class="col-12 row">
                        <div class="field mb-4">
                            <label
                                for="device"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataDevices.deviceName.$invalid && submitted }"
                                >Tên thiết bị<span style="color: red">*</span></label
                            >
                            <InputText
                                :class="{ 'p-invalid': v$.dataDevices.deviceName.$invalid && submitted }"
                                v-model="v$.dataDevices.deviceName.$model"
                            />
                            <small
                                v-if="
                                    (v$.dataDevices.deviceName.$invalid && submitted) ||
                                    v$.dataDevices.deviceName.$pending.$response
                                "
                                class="p-error"
                                >{{
                                    v$.dataDevices.deviceName.required.$message.replace('Value', 'Device Name')
                                }}</small
                            >
                        </div>
                    </div>

                    <div class="col-12 row">
                        <div class="field mb-4">
                            <label
                                for="amount"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataDevices.info.$invalid && submitted }"
                                >Thông tin<span style="color: red">*</span></label
                            >
                            <Textarea
                                :class="{ 'p-invalid': v$.dataDevices.info.$invalid && submitted }"
                                v-model="v$.dataDevices.info.$model"
                                rows="3"
                            />
                            <small
                                v-if="
                                    (v$.dataDevices.info.$invalid && submitted) ||
                                    v$.dataDevices.info.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataDevices.info.required.$message.replace('Value', 'Device Info') }}</small
                            >
                        </div>
                    </div>

                    <div class="col-12 row">
                        <div class="field mb-4">
                            <label
                                for="device"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataDevices.note.$invalid && submitted }"
                                >Ghi chú<span style="color: red">*</span></label
                            >
                            <Textarea
                                :class="{ 'p-invalid': v$.dataDevices.note.$invalid && submitted }"
                                v-model="v$.dataDevices.note.$model"
                                rows="3"
                            />
                            <small
                                v-if="
                                    (v$.dataDevices.note.$invalid && submitted) ||
                                    v$.dataDevices.note.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataDevices.note.required.$message.replace('Value', 'Device Note') }}</small
                            >
                        </div>
                    </div>
                    <div class="">
                        <button type="submit" class="btn btn-primary">Lưu</button>&nbsp;
                        <button type="button" class="btn btn-secondary" @click="closeDialog">Hủy</button>
                    </div>
                </form>
            </div>
        </Dialog>
    </div>
</template>

<script>
    import { HttpStatus } from '@/config/app.config'
    import { required } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    // import { DevicesDto } from '@/views/Devices/Devices.dto'
    import { HTTP } from '@/http-common'
    import { LocalStorage } from '@/helper/local-storage.helper'

    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        name: 'AddDevices',
        props: ['isOpenDialog', 'deviceSelected'],
        data() {
            return {
                dataDevices: new DevicesDto(),
                submitted: false,
                valid: true,
            }
        },
        validations() {
            return {
                dataDevices: {
                    deviceName: { required },
                    info: { required },
                    note: { required },
                },
            }
        },
        beforeUpdate() {
            if (this.deviceSelected.idDevice) {
                this.v$.dataDevices.deviceName.$model = this.deviceSelected.deviceName
                this.v$.dataDevices.info.$model = this.deviceSelected.info
                this.v$.dataDevices.note.$model = this.deviceSelected.note
            }
        },
        methods: {
            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            getAllDevices() {
                this.$emit('getAllDevices')
            },
            handleSubmit(isFormValid) {
                this.submitted = true
                if (!isFormValid) {
                    return
                }
                if (!this.valid) return
                if (this.deviceSelected.idDevice) {
                    this.editData()
                } else {
                    this.addData()
                }
            },
            addData() {
                let userLogin = LocalStorage.jwtDecodeToken()
                this.dataDevices.userCreated = userLogin.Id
                if (this.dataDevices) {
                    HTTP.post('Devices/addDevices', this.dataDevices)
                        .then((res) => {
                            if (res.status === HttpStatus.OK) {
                                this.showSuccess()
                                this.closeDialog()
                                this.getAllDevices()
                                this.$emit('Reloadlist')
                            }
                        })
                        .catch((er) => {
                            this.showWarn()
                        })
                }
            },
            editData() {
                let userLogin = LocalStorage.jwtDecodeToken()
                let data = {
                    idDevice: this.deviceSelected.idDevice,
                    deviceName: this.dataDevices.deviceName,
                    info: this.dataDevices.info,
                    isDelete: this.deviceSelected.isDelete,
                    dateUpdated: new Date(),
                    userUpdated: userLogin.Id,
                    dateCreated: this.deviceSelected.dateCreated,
                    userCreated: this.deviceSelected.userCreated,
                    note: this.dataDevices.note,
                }
                if (data) {
                    HTTP.put('Devices/updateDevices', data)
                        .then((res) => {
                            if (res.status === HttpStatus.OK) {
                                this.showSuccess()
                                this.closeDialog()
                                this.getAllDevices()
                            }
                        })
                        .catch((er) => {
                            this.showWarn()
                        })
                }
            },
            resetForm() {
                this.$refs.formAddEditDevice.reset()
                this.v$.$reset()
                const initialData = this.$options.data.call(this)
                Object.assign(this.$data, initialData)
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Lưu thiết bị thành công!!!',
                    life: 3000,
                })
            },
            showWarn() {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: 'Lưu thiết bị không thành công !!!',
                    life: 3000,
                })
            },
        },
    }
</script>

<style></style>
