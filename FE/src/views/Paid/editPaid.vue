<template>
    <Dialog
        header="Sửa thu chi"
        :visible="status"
        :closable="false"
        modal="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
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

                    <InputText type="text" v-model="v$.Datasend.amountPaid.$model" placeholder="Nhập mức chi" />
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

                    <InputText type="text" v-model="v$.Datasend.paidReason.$model" placeholder="Nhập lý do " />
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
                        placeholder="Input Paid Date"
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
        </div>

        <template #footer>
            <Button label="Sửa" icon="pi pi-check" autofocus @click="EditPaid" />
            <Button label="Hoàn tất" icon="pi pi-times" class="p-button-text" @click="closeModal" />
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
                    token : null,
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
            async getPaidById(id) {
                HTTP_LOCAL.get(`Paid/GetById/${id}`).then((res) => {
                    this.Datasend = res.data._Data

                })
            },

            EditPaid() {

                this.token = LocalStorage.jwtDecodeToken();
                try {
                    this.isSubmit = true
                    // Admin Sample
                    if(Number(this.token.IdGroup) === 1 || Number(this.token.IdGroup) === 2){
                        this.Datasend.paidPerson = this.token.Id
                            if (!this.v$.$invalid) {
                            HTTP_LOCAL.put('Paid?id=' + this.Datasend.id, this.Datasend).then((res) => {
                                this.showSuccess2()
                                this.closeModal()
                                this.$emit('reloadpage')
                            })
                        } else {
                            //this.$emit('failed')
                            this.showError2()
                        }
                    }
                    // Cac roles con lai
                    if(Number(this.token.IdGroup) !== 1 && Number(this.token.IdGroup) !== 2)
                    {
                        this.Datasend.paidPerson = this.token.Id
                            if (!this.v$.$invalid) {
                            HTTP_LOCAL.put('Paid?id=' + this.Datasend.id, this.Datasend).then((res) => {
                                this.showSuccess2()
                                this.closeModal()
                                this.$emit('reloadpageother')
                            })
                        } else {
                            //this.$emit('failed')
                            this.showError2()
                        }
                    }   
                }catch(err){
                    console.log(err);
                }
 
            },

            showSuccess2() {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: 'Sửa thành công!', life: 3000 })
            },
            showError2() {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: 'Sửa Lỗi!', life: 3000 })
            },
        },

        beforeUpdate() {

            if (this.dataedit != null) this.getPaidById(this.dataedit)

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
