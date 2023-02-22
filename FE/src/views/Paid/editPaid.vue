<template>
    
    <Dialog
        header="Cập nhật thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
    <form enctype="multipart/form-data" >
        <div class="Menu__form">
            <div class="Menu__form--items items-left">
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.customerName.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Tên khách hàng<span style="color: red">*</span></label
                    >
                    <InputText
                        type="text"
                        v-model="v$.Datasend.customerName.$model"
                        placeholder="Nhập tên khách hàng"
                    />
                    <small class="p-error" v-if="v$.Datasend.customerName.required.$invalid && isSubmit">{{
                        v$.Datasend.customerName.required.$message.replace('Value', 'Customer Name')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.amountPaid.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Mức chi<span style="color: red">*</span></label
                    >

                    <InputNumber v-model="v$.Datasend.amountPaid.$model" placeholder="Nhập mức chi" min="0" />
                    <small class="p-error" v-if="v$.Datasend.amountPaid.required.$invalid && isSubmit">{{
                        v$.Datasend.amountPaid.required.$message.replace('Value', 'Amount Paid')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label>Trạng thái<span style="color: red">*</span></label>
                    <Dropdown
                        class="inputdrop"
                        v-model="Datasend.isPaid"
                        :options="isPaidArr"
                        optionLabel="name"
                        optionValue="isPaid"
                        placeholder="Chọn trạng thái"
                    />
                </div>
            </div>
            <div class="Menu__form--items items-right">
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.paidReason.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Lý do chi trả<span style="color: red">*</span></label
                    >

                    <InputText type="text" v-model="v$.Datasend.paidReason.$model" placeholder="Lý do chi" />
                    <small class="p-error" v-if="v$.Datasend.paidReason.required.$invalid && isSubmit">{{
                        v$.Datasend.paidReason.required.$message.replace('Value', 'Paid Reason')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.paidDate.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Ngày chi<span style="color: red">*</span></label
                    >

                    <Calendar
                        v-model="Datasend.paidDate"
                        dateFormat="yy-mm-dd"
                        view="date"
                        placeholder="Chọn ngày chi"
                        :showIcon="true"
                    />
                </div>
                <div class="Menu__form--items-content">
                    <label>Dự án<span style="color: red">*</span></label>
                    <Dropdown
                        class="inputdrop"
                        v-model="Datasend.projectId"
                        :options="projectArr"
                        optionLabel="name"
                        optionValue="id"
                        placeholder="Chọn dự án"
                    />
                </div>
            </div>
            <!-- <Image v-bind:src="Datasend.paidImage.imagePath" alt="Image" width="50" preview /> -->
            <!-- <Image alt="Image" width="50" preview /> -->
        </div>
    </form>
        <template #footer>
            <Button label="Lưu" icon="pi pi-check" autofocus @click="handleSubmit" />
            <Button label="Hủy" icon="pi pi-times" class="p-button-text" @click="closeModal" />
        </template>
    </Dialog>
</template>

<script>
    import jwt_decode from 'jwt-decode'
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HttpStatus } from '@/config/app.config'

    export default {
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                Datasend: {
                    projectId: '',
                    customerName: '',
                    amountPaid: '',
                    paidReason: '',
                    paidPerson: 0,
                    isPaid: false,
                    paidDate: null,
                    token: null,
                    paidImage: null,
                },
                projectArr: [],
                isPaidArr: [
                    { isPaid: false, name: 'Chưa Thanh Toán' },
                    { isPaid: true, name: 'Đã Thanh Toán' },
                ],
                isSubmit: false,
            }
        },
        validations() {
            return {
                Datasend: {
                    projectId: { required },
                    customerName: { required },
                    amountPaid: { required },
                    paidReason: { required },
                    paidDate: { required },
                },
            }
        },
        props: ['status', 'optionmodule', 'dataedit'],
        methods: {
            closeModal() {
                this.$emit('closemodal')
            },

            clearform() {
                this.Datasend.projectId = ''
                this.Datasend.customerName = ''
                this.Datasend.amountPaid = ''
                this.Datasend.paidReason = ''
                this.isSubmit = false
            },
            
            async CallApi(fromData) {
                try {
                    const res = await HTTP_LOCAL.put(`Paid/${this.Datasend.id}`, fromData)

                    switch (res.status) {
                        case HttpStatus.OK:
                            this.clearform()
                            this.$emit('reloadpage')
                            this.showSuccess2('Cập nhật thành công!');
                            break
                        case HttpStatus.UNAUTHORIZED:
                        case HttpStatus.FORBIDDEN:
                            this.showError2('Không có quyền thực hiện thao tác này!')
                            break
                        default:
                            this.showError2('Lưu lỗi!')
                            
                    }
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showError2('Kiểm tra kết nối!')
                            break
                        case 'ERR_BAD_REQUEST':
                            this.showError2(error.response.data)
                            break
                        default:
                    }
                }
            },

            async handleSubmit() {
                try{
                    this.isSubmit = true
                    if (!this.v$.$invalid) {
                        await this.EditPaid();
                        this.closeModal()
                    }
                }
                catch (err) {
                    console.log(err)
                    this.showError2(error.response.data)
                }
            },

            async EditPaid() {
                this.token = LocalStorage.jwtDecodeToken()
                try {
                    
                    const formData = new FormData()
                    formData.append('PaidPerson', this.Datasend.user.id)
                    formData.append('PaidDate', DateHelper.formatDateTime(this.Datasend.paidDate))
                    formData.append('ProjectId', this.Datasend.projectId)
                    formData.append('CustomerName', this.Datasend.customerName)
                    formData.append('AmountPaid', this.Datasend.amountPaid)
                    formData.append('PaidReason', this.Datasend.paidReason)
                    formData.append('IsPaid', this.Datasend.isPaid)
                    formData.append('paidImage', this.Datasend.paidImages)

                    await this.CallApi(formData);

                } catch (err) {
                    console.log(err)
                }
            },

            showSuccess2(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },
            showError2(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },
        },

        beforeUpdate() {
            if (this.dataedit != null) this.Datasend = this.dataedit;
            this.Datasend.paidDate = DateHelper.formatDate(this.Datasend.paidDate)

            this.optionmodule.map((ele) => {
                // this.Datasend.paidDate = DateHelper.formatDate(ele)
                this.projectArr.push(ele)
            })
        },
    }
</script>

<style scoped>
    .Menu__form {
        display: flex;
    }
    .Menu__form--items {
        width: 50%;
        padding: 10px;
        display: flex;
        flex-direction: column;
    }
    .Menu__form--items-content {
        width: 100%;
        display: flex;
        flex-direction: column;
        justify-content: center;
    }
    .Menu-form-items-content {
        display: flex;
        justify-content: space-around;
    }

    .country-item-value {
        display: flex;
        height: 30px;
    }
</style>
