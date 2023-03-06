<template>
    <Dialog :visible="status" modal="true" :closable="false">
        <template #header>
            <h3>Add Handovers</h3>
        </template>
        <div class="form-layout">
            <div class="header-outside">
                <div class="title">
                    <h1>Add Handovers</h1>
                </div>
            </div>
        </div>
        <form class="form-addproject" @submit.prevent="submitMultiUseDevice()">
            <div class="input-layout">
                <label
                    :class="{
                        'p-error': !device && isSubmit,
                        'input-title': true,
                    }"
                    >Device Name<span style="color: red">*</span></label
                >
                <Dropdown
                    v-model="device"
                    :options="allDevice"
                    optionLabel="deviceName"
                    class="input"
                    placeholder="Tên thiết bị"
                />
                <small class="p-error" v-if="!this.device && isSubmitAdd">{{
                    v1$.formUseDevice.idDevice.required.$message.replace('Value', 'Name device')
                }}</small>
            </div>

            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v1$.formUseDevice.amount.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >Amount
                    <span style="color: red">*</span>
                    <span class="note">
                        {{ device.amount ? `( Số lượng nhập tối đa là ${device.amount} )` : '' }}
                    </span>
                </label>
                <InputText v-model="v1$.formUseDevice.amount.$model" class="input" min="1" type="number" />
                <small class="p-error" v-if="v1$.formUseDevice.amount.required.$invalid && isSubmitAdd">{{
                    v1$.formUseDevice.amount.required.$message.replace('Value', 'Amount')
                }}</small>
                <small class="p-error" v-if="!v1$.formUseDevice.amount.required.$invalid && !checkAmount()">{{
                    `Amount greater than 1`
                }}</small>
            </div>
            <div class="outSideField">
                <Button v-on:click="addDevice()" label="Add Device" class="p-button-primary addDevice" />
                <Button
                    v-on:click="openDialogAdd()"
                    style="margin-left: 12px"
                    label="New Device"
                    class="p-button-primary addDevice"
                />
            </div>

            <DataTable :value="devices" class="input-layout">
                <Column field="device.idDevice" sortable header="Id Device"></Column>
                <Column field="device.deviceName" sortable header="Name Device"></Column>
                <Column field="amount" sortable header="Amount"></Column>
                <Column header="Delete">
                    <template #body="slotProps">
                        <Button
                            class="p-button-danger p-button-sm"
                            icon="pi pi-trash"
                            v-on:click="deleteDevice(slotProps.data.device.id)"
                        />
                    </template>
                </Column>
            </DataTable>
            <div class="outSideField" v-if="devices.length == 0">
                <p style="font-size: medium; font-style: italic">Chưa có dữ liệu</p>
            </div>
            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v1$.formUseDevice.dateCreated.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >Date Created<span style="color: red">*</span></label
                >
                <Calendar
                    v-model="v1$.formUseDevice.dateCreated.$model"
                    :showIcon="true"
                    class="input"
                    :showTime="true"
                    hourFormat="24"
                />
                <small class="p-error" v-if="v1$.formUseDevice.dateCreated.required.$invalid && isSubmit">{{
                    v1$.formUseDevice.dateCreated.required.$message.replace('Value', 'Date Send')
                }}</small>
            </div>

            <div class="input-layout">
                <label
                    :class="{
                        'p-error': v1$.formUseDevice.userReceive.$invalid && isSubmit,
                        'input-title': true,
                    }"
                    >User Received<span style="color: red">*</span></label
                >
                <Dropdown
                    v-model="v1$.formUseDevice.userReceive.$model"
                    :options="users"
                    optionLabel="userCode"
                    class="input"
                    placeholder="Tên người nhận"
                />
                <small class="p-error" v-if="v1$.formUseDevice.userReceive.required.$invalid && isSubmit">{{
                    v1$.formUseDevice.userReceive.required.$message.replace('Value', 'User Received')
                }}</small>
            </div>

            <AddDevices
                :isOpenDialog="isOpenDialog"
                @closeDialog="closeDialog"
                :deviceSelected="selectedEdit"
                @Reloadlist="getAllDevices"
            />

            <div class="group-button">
                <div>
                    <Button label="Save" type="submit" icon="pi pi-check" />{{ ' ' }}
                    <Button label="Hủy" class="p-button-secondary" @click="Closeopen" />
                </div>
            </div>
        </form>
    </Dialog>
</template>

<script>
    import AddDevices from '@/views/Devices/AddDevices.vue'
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import jwtDecode from 'jwt-decode'
    import moment from 'moment'
    import { required } from '@vuelidate/validators'
    //import { DevicesDto } from '@/views/Devices/Devices.Dto'

    export default {
        inject: ['dialogRef'],
        setup: () => ({ v1$: useVuelidate() }),

        data() {
            return {
                formUseDevice: {
                    idDevice: null,
                    dateCreated: '',
                    userReceive: null,
                    userCreated: null,
                    amount: null,
                },
                allDevice: [],
                device: '',
                devices: [],
                isSubmit: false,
                users: '',
                user: '',
                isSubmitAdd: false,
                isOpenDialog: false,
                selectedEdit: new DevicesDto(),
                statusOpen: true,
            }
        },
        props: {
            status: false,
        },
        methods: {
            Closeopen() {
                this.$emit('CloseModal')
            },
            openDialogAdd() {
                this.isOpenDialog = true
            },
            closeDialog() {
                this.isOpenDialog = false
            },

            submitMultiUseDevice() {
                this.isSubmit = true
                if (this.devices) {
                    this.formUseDevice.idDevice = this.device ? this.device.idDevice : ''
                    this.formUseDevice.userCreated = this.user.Id
                    if (this.checkForm()) {
                        this.formUseDevice.userReceive = this.formUseDevice.userReceive.id
                        this.formUseDevice.dateCreated = moment(new Date(this.formUseDevice.dateCreated)).format(
                            'YYYY-MM-DD[T]HH:mm:ss',
                        )
                    }

                    this.devices.forEach((de) => {
                        this.device = de.device
                        this.formUseDevice.amount = de.amount
                        this.formUseDevice.idDevice = this.device ? this.device.idDevice : ''
                        if (this.checkForm()) {
                            this.submitUseDevice()
                            this.$emit('reloadpage')
                        }
                    })
                    if (this.checkForm()) {
                        this.resetForm()
                        this.devices = []
                    }
                }
            },
            submitUseDevice() {
                this.isSubmit = true

                if (!this.checkForm()) return

                HTTP.post('Handovers/addHandover', this.formUseDevice).then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: `Thêm Bàn giao thành công!`,
                            life: 3000,
                        })
                    }
                })
            },
            checkForm() {
                if (
                    this.formUseDevice.amount &&
                    this.formUseDevice.dateCreated &&
                    this.formUseDevice.idDevice &&
                    this.formUseDevice.userCreated &&
                    this.formUseDevice.userReceive
                ) {
                    return true
                }
            },
            closeDialog1() {
                this.dialogRef.close()
            },

            resetForm() {
                this.isSubmitAdd = false
                this.isSubmit = false
                this.formUseDevice = {
                    idDevice: null,
                    userCreated: null,
                    dateCreated: '',
                    userReceive: null,
                    amount: null,
                }
            },

            addDevice() {
                this.isSubmitAdd = true
                if (this.device && this.formUseDevice.amount && this.checkAmount()) {
                    //check for existence device
                    let exist = false
                    if (this.devices) {
                        this.devices.forEach((de) => {
                            if (de.device.idDevice == this.device.idDevice) {
                                exist = true
                                return
                            }
                        })
                    }
                    if (!exist) this.devices.push({ device: this.device, amount: this.formUseDevice.amount })
                }
            },
            deleteDevice(idDevice) {
                if (this.devices)
                    this.devices.forEach((element, index) => {
                        if (element.device.id == idDevice) this.devices.splice(index, 1)
                    })
            },
            checkAmount() {
                if (this.formUseDevice.amount < 1) {
                    return false
                } else {
                    return true
                }
            },
            getAllDevices() {
                HTTP.get('Devices/getListDevices').then((res) => {
                    if (res.status == 200) this.allDevice = res.data
                })
            },
        },

        mouting() {},
        mounted() {
            this.user = jwtDecode(localStorage.getItem('token'))
            HTTP.get('Users/getAll').then((res) => {
                if (res.status == 200) this.users = res.data
            })
            this.getAllDevices()
        },
        validations() {
            return {
                formUseDevice: {
                    idDevice: { required },
                    dateCreated: { required },
                    userReceive: { required },
                    amount: { required },
                    userCreated: { required },
                },
            }
        },

        components: {
            AddDevices,
        },
    }
</script>

<style scoped>
    .form-layout {
        width: 60vw;

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
    .addDevice {
        margin-top: 24px;
    }
    .outSideField {
        width: 100%;
        display: flex;
        justify-content: center;
    }
</style>
