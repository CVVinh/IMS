<template>
    <div class="form-layout">
        <div class="header-outside">
            <div class="title">
                <h1>Edit Handover</h1>
            </div>
        </div>
        <form class="form-addproject" @submit.prevent="submitUseDevice()">
            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v$.formUseDevice.idDevice.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >Device Name<span style="color: red">*</span></label
                >
                <Dropdown
                    v-model="v$.formUseDevice.idDevice.$model"
                    :options="devices"
                    optionLabel="deviceName"
                    class="input"
                    placeholder="Tên thiết bị"
                />
                <small class="p-error" v-if="!this.device && isSubmit">{{
                    v$.formUseDevice.idDevice.required.$message.replace('Value', 'Name device')
                }}</small>
            </div>

            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v$.formUseDevice.amount.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >Amount
                    <span style="color: red">*</span>
                </label>
                <InputText v-model="v$.formUseDevice.amount.$model" class="input" min="1" type="number" />
                <small class="p-error" v-if="v$.formUseDevice.amount.required.$invalid && isSubmit">{{
                    v$.formUseDevice.amount.required.$message.replace('Value', 'Amount')
                }}</small>
                <small class="p-error" v-if="!v$.formUseDevice.amount.required.$invalid && !checkAmount()">{{
                    `Amount greater than 1`
                }}</small>
            </div>
            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v$.formUseDevice.dateUpdated.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >Date Updated<span style="color: red">*</span></label
                >
                <Calendar
                    v-model="v$.formUseDevice.dateUpdated.$model"
                    @change="configDateCreated()"
                    :showIcon="true"
                    class="input"
                    :showTime="true"
                    hourFormat="24"
                />
                <small class="p-error" v-if="v$.formUseDevice.dateUpdated.required.$invalid && isSubmit">{{
                    v$.formUseDevice.dateUpdated.required.$message.replace('Value', 'Date Send')
                }}</small>
            </div>

            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v$.formUseDevice.userReceive.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >User Received<span style="color: red">*</span></label
                >
                <Dropdown
                    v-model="v$.formUseDevice.userReceive.$model"
                    :options="users"
                    optionLabel="userCode"
                    class="input"
                    placeholder="Tên người nhận"
                />
                <small class="p-error" v-if="v$.formUseDevice.userReceive.required.$invalid && isSubmit">{{
                    v$.formUseDevice.userReceive.required.$message.replace('Value', 'User Received')
                }}</small>
            </div>

            <div class="group-button">
                <div>
                    <Button label="Save" type="submit" icon="pi pi-check" />{{ ' ' }}
                    <Button label="Hủy" class="p-button-secondary" v-on:click="closeDialog()" />
                </div>
            </div>
        </form>
    </div>
</template>
<script>
    import moment from 'moment'
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import jwtDecode from 'jwt-decode'
    import { required } from '@vuelidate/validators'
    export default {
        inject: ['dialogRef'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                formUseDevice: {
                    idDevice: null,
                    dateUpdated: '',
                    userReceive: null,
                    amount: null,
                },
                device: '',
                isSubmit: false,
                users: '',
                user: '',
                handovers: '',
                handover: '',
                idHandover: this.dialogRef.data.idHandover,
                devices: [],
            }
        },

        methods: {
            submitUseDevice() {
                this.isSubmit = true

                if (!this.v$.$invalid) {
                    this.formUseDevice.idDevice = this.formUseDevice.idDevice
                        ? this.formUseDevice.idDevice.idDevice
                        : ''
                    this.formUseDevice.userReceive = this.formUseDevice.userReceive.id
                    this.formUseDevice.dateUpdated = moment(new Date(this.formUseDevice.dateUpdated)).format(
                        'YYYY-MM-DD[T]HH:mm:ss',
                    )
                }
                this.putFormUseDevice()
                if (!this.v$.$invalid) {
                    this.resetForm()
                    this.devices = []
                }
            },
            closeDialog() {
                this.dialogRef.close()
            },
            configDateCreated() {
                this.formUseDevice.dateUpdated = moment(new Date(this.formUseDevice.dateUpdated)).format(
                    'YYYY-MM-DD[T]HH:mm:ss',
                )
            },
            resetForm() {
                this.isSubmit = false
                this.formUseDevice = {
                    idDevice: null,
                    dateUpdated: '',
                    userReceive: null,
                    amount: null,
                }
            },
            checkAmount() {
                if (this.formUseDevice.amount < 1) {
                    return false
                } else {
                    return true
                }
            },
            putFormUseDevice() {
                HTTP.put('Handovers/updateHandover', {
                    idHandover: this.idHandover,
                    ...this.formUseDevice,
                    userUpdated: this.user.Id,
                }).then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: `Chỉnh sửa Bàn giao thành công!`,
                            life: 3000,
                        })
                    }
                })
            },
            getAllUserAndPutToForm() {
                HTTP.get('Users/getAll').then((res) => {
                    if (res.status == 200) {
                        this.users = res.data
                        this.users.forEach((user) => {
                            if (user.id == this.formUseDevice.userReceive) {
                                this.formUseDevice.userReceive = user
                            }
                        })
                    }
                })
            },
            getAllDevicesAndPutToForm() {
                HTTP.get('Devices/getListDevices').then((res) => {
                    if (res.status == 200) this.devices = res.data
                    if (this.devices)
                        this.devices.forEach((element) => {
                            if (element.idDevice == this.formUseDevice.idDevice) {
                                this.formUseDevice.idDevice = element
                                return
                            }
                        })
                })
            },
            getAllHandoverAndPutToForm() {
                HTTP.get('Handovers/getListHandover').then((res) => {
                    if (res.status == 200) this.handovers = res.data

                    if (this.handovers && this.idHandover)
                        this.handovers.forEach((element) => {
                            if (element.hand1.idHandover == this.idHandover) {
                                this.handover = element.hand1
                                return
                            }
                        })

                    if (this.handover) {
                        this.formUseDevice.amount = this.handover.amount
                        this.formUseDevice.idDevice = this.handover.idDevice
                        this.formUseDevice.userReceive = this.handover.userReceive
                        this.formUseDevice.dateUpdated = this.handover.dateUpdated
                    }

                    if (this.formUseDevice.idDevice) this.getAllDevicesAndPutToForm()
                    if (this.formUseDevice.userReceive) this.getAllUserAndPutToForm()
                })
            },
        },
        mouting() {},
        mounted() {
            this.user = jwtDecode(localStorage.getItem('token'))
            this.formUseDevice.userUpdated = this.user.Id
            this.getAllHandoverAndPutToForm()

            // this.formUseDevice.dateSend = this.formUseDevice.dateSend.substring(10)

            //GET ALL USER
        },
        validations() {
            return {
                formUseDevice: {
                    idDevice: { required },
                    dateUpdated: { required },
                    userReceive: { required },
                    amount: { required },
                },
            }
        },
    }
</script>
<style scoped>
    .form-layout {
        width: 60vw;
        height: 95vh;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-items: center;
    }
    .input {
        width: 100%;
        max-width: 600px;
    }
    .input-layout {
        width: calc(100% - 36px);
        margin: 24px 18px 0px 18px;
        display: flex;
        justify-items: center;
        align-items: center;
        flex-direction: column;
    }
    .input-title {
        width: 100%;
        max-width: 600px;
    }
    .p-error {
        width: 100%;
        max-width: 600px;
    }
    .form-addproject {
        width: 100%;
        min-width: 120px;
    }
    .container {
        width: 90%;
    }
    .header-outside {
        z-index: 1;
        width: 100%;
        height: 84px;
        display: flex;
        justify-content: center;
        align-items: center;
        color: white;
        background-color: #33adff;
        position: sticky;
        top: 0px;
    }
    .group-button {
        width: calc(100% - 36px);
        margin: 24px 18px 24px 18px;
        display: flex;
        justify-items: center;
        align-items: center;
        flex-direction: column;
    }
    .note {
        font-size: small;
        font-style: italic;
    }
</style>
<style>
    .p-dialog-header-close-icon {
        z-index: 2000;
    }
</style>
