<template>
    <Dialog
        :header="orderSelected.idOrder ? ' Sửa đơn nhập' : 'Thêm đơn nhập'"
        :visible="isOpenDialog"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
        :maximizable="true"
        :modal="true"
        :closable="false"
    >
        <div class="container">
            <form ref="formAddEditOrder" class="p-fluid" @submit.prevent="saveData(!v$.$invalid)">
                <div class="col-12 row">
                    <div class="col-4">
                        <div class="field mb-4">
                            <label
                                for="branch"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataOrder.idBranch.$invalid && submitted }"
                                >Chi Nhánh</label
                            >
                            <Dropdown
                                :class="{ 'p-invalid': v$.dataOrder.idBranch.$invalid && submitted }"
                                v-model="v$.dataOrder.idBranch.$model"
                                :options="branchs"
                                optionLabel="branchName"
                                optionValue="idBranch"
                                placeholder="Chọn chi nhánh"
                            />
                            <small
                                v-if="
                                    (v$.dataOrder.idBranch.$invalid && submitted) ||
                                    v$.dataOrder.idBranch.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataOrder.idBranch.required.$message.replace('Value', 'Branch') }}</small
                            >
                        </div>
                    </div>

                    <div class="col-4">
                        <div class="field mb-4">
                            <label
                                for="device"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataOrder.idDevice.$invalid && submitted }"
                                >Thiết bị</label
                            >
                            <Dropdown
                                :class="{ 'p-invalid': v$.dataOrder.idDevice.$invalid && submitted }"
                                v-model="v$.dataOrder.idDevice.$model"
                                :options="devices"
                                optionLabel="deviceName"
                                optionValue="idDevice"
                                placeholder="Chọn thiết bị"
                            />
                            <small
                                v-if="
                                    (v$.dataOrder.idDevice.$invalid && submitted) ||
                                    v$.dataOrder.idDevice.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataOrder.idDevice.required.$message.replace('Value', 'Device') }}</small
                            >
                        </div>
                    </div>
                    <div class="col-4">
                        <div class="field mb-4">
                            <label
                                for="amount"
                                class="mb-2"
                                :class="{ 'p-error': v$.dataOrder.amount.$invalid && submitted }"
                                >Số lượng</label
                            >
                            <InputText
                                :class="{ 'p-invalid': v$.dataOrder.amount.$invalid && submitted }"
                                v-model="v$.dataOrder.amount.$model"
                                type="number"
                                min="1"
                            />
                            <small
                                v-if="
                                    (v$.dataOrder.amount.$invalid && submitted) ||
                                    v$.dataOrder.amount.$pending.$response
                                "
                                class="p-error"
                                >{{ v$.dataOrder.amount.required.$message.replace('Value', 'Amount') }}</small
                            >
                        </div>
                    </div>
                </div>

                <div class="col-12 row">
                    <div class="field mb-4">
                        <label for="amount" class="mb-2">Ghi chú</label>
                        <Textarea v-model="v$.dataOrder.note.$model" :autoResize="true" rows="5" />
                    </div>
                </div>

                <div class="">
                    <button type="submit" class="btn btn-primary">Lưu</button>&nbsp;
                    <button type="button" class="btn btn-secondary" @click="onClickCancel">Hủy</button>
                </div>
            </form>
        </div>
    </Dialog>
</template>

<script>
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { ListDevicesOtherDto } from '@/views/ListDeviceOther/ListDevicesOther.dto'
    import { HTTP } from '@/http-common'
    import { LocalStorage } from '@/helper/local-storage.helper'

    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        name: 'AddDevicesOther',
        props: ['isOpenDialog', 'branchs', 'devices', 'orderSelected'],
        data() {
            return {
                submitted: false,
                dataOrder: new ListDevicesOtherDto(),
            }
        },
        beforeUpdate() {
            if (this.orderSelected.idOrder) {
                this.v$.dataOrder.idBranch.$model = this.orderSelected.idBranch
                this.v$.dataOrder.idDevice.$model = this.orderSelected.idDevice
                this.v$.dataOrder.amount.$model = this.orderSelected.amount
                // this.v$.dataOrder.statusOrder.$model = this.orderSelected.statusOrder;
                // this.v$.dataOrder.statusDevice.$model = this.orderSelected.statusDevice;
                this.v$.dataOrder.note.$model = this.orderSelected.note
            }
        },
        validations() {
            return {
                dataOrder: {
                    idBranch: { required },
                    idDevice: { required },
                    amount: { required, minLength: 1 },
                    note: {},
                },
            }
        },
        methods: {
            onClickCancel() {
                this.resetForm()
                this.$emit('closeDialog')
            },
            saveData(isFormValid) {
                this.submitted = true
                if (isFormValid) {
                    if (this.orderSelected.idOrder) {
                        let userLogin = LocalStorage.jwtDecodeToken()
                        this.dataOrder.idOrder = this.orderSelected.idOrder
                        this.dataOrder.userUpdated = userLogin.Id
                        this.dataOrder.dateUpdated = new Date()
                        HTTP.put('Orders/updateOrder', this.dataOrder)
                            .then((res) => {
                                if (res.status === 200) {
                                    this.showSuccess()
                                    this.onClickCancel()
                                    this.$emit('reloadpage')
                                }
                            })
                            .catch((er) => {
                                this.showWarn()
                            })
                    } else {
                        let userLogin = LocalStorage.jwtDecodeToken()
                        this.dataOrder.userCreated = userLogin.Id
                        this.dataOrder.dateCreated = new Date()
                        this.dataOrder.userUpdated = userLogin.Id
                        this.dataOrder.dateUpdated = new Date()
                        HTTP.post('Orders/addOrders', this.dataOrder)
                            .then((res) => {
                                if (res.status === 200) {
                                    this.showSuccess()
                                    this.onClickCancel()
                                    this.$emit('reloadpage')
                                }
                            })
                            .catch((er) => {
                                this.showWarn()
                            })
                    }
                }
            },
            showSuccess() {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: 'Lưu thành công!',
                    life: 3000,
                })
            },
            showWarn() {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: 'Lưu không thành công !!!',
                    life: 3000,
                })
            },
            resetForm() {
                this.$refs.formAddEditOrder.reset()
                this.v$.$reset()
                const initialData = this.$options.data.call(this)
                Object.assign(this.$data, initialData)
            },
        },
    }
</script>

<style scoped>
    .input-height {
        height: 50px;
    }
</style>
