<template>
    <Dialog :visible="status" :closable="false" modal="true">
        <template #header>
            <h3>Edit paid</h3>
        </template>

        <div class="Menu__form">
            <div class="Menu__form--items items-left">
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.customerName.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Customer Name<span style="color: red">*</span></label
                    >
                    <InputText
                        type="text"
                        v-model="v$.Datasend.customerName.$model"
                        placeholder="Input Customer Name"
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
                        >Amount Paid<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.amountPaid.$model" placeholder="Input Amount Paid" />
                    <small class="p-error" v-if="v$.Datasend.amountPaid.required.$invalid && isSubmit">{{
                        v$.Datasend.amountPaid.required.$message.replace('Value', 'Amount Paid')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <h5>Status</h5>
                    <Dropdown
                        class="inputdrop"
                        v-model="Datasend.isPaid"
                        :options="isPaidArr"
                        optionLabel="name"
                        optionValue="isPaid"
                        placeholder="Select a Status"
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
                        >Paid Reason<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.paidReason.$model" placeholder="Input Paid Reason" />
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
                        >Paid Date<span style="color: red">*</span></label
                    >
                    <Calendar v-model="Datasend.paidDate" dateFormat="yy-mm-dd" view="date" placeholder="Input Paid Date" />
                </div>
                <div class="Menu__form--items-content">
                    <h5>Project</h5>
                    <Dropdown
                        class="inputdrop"
                        v-model="Datasend.projectId"
                        :options="projectArr"
                        optionLabel="name"
                        optionValue="id"
                        placeholder="Select a project"
                    />
                </div>
            </div>
        </div>

        <template #footer>
            <Button label="Edit" icon="pi pi-check" autofocus @click="EditPaid" />
            <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeModal" />
        </template>
    </Dialog>
</template>

<script>
    import jwt_decode from 'jwt-decode'
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { DateHelper } from '@/helper/date.helper'
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
                    this.Datasend.paidDate = DateHelper.formatDate(this.Datasend.paidDate);

                })
            },

            EditPaid() {
                this.isSubmit = true
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)
                this.Datasend.paidPerson = decode.Id
                if (!this.v$.$invalid) {
                    HTTP_LOCAL.put('Paid?id=' + this.Datasend.id, this.Datasend).then((res) => {
                        this.showSuccess2()
                        this.closeModal()
                        this.$emit('reloadpage')
                        window.location.reload()
                    })
                } else {
                    //this.$emit('failed')
                    this.showError2()
                }
            },

            showSuccess2() {
                this.$toast.add({ severity: 'success', summary: 'Success Message', detail: 'Edit success', life: 3000 })
            },
            showError2() {
                this.$toast.add({ severity: 'error', summary: 'Error Message', detail: 'Edit Failed', life: 3000 })
            },
        },

        beforeUpdate() {
            if (this.dataedit != null)
             this.getPaidById(this.dataedit)

            this.optionmodule.map((ele) => {
                console.log(ele);
                // this.Datasend.paidDate = DateHelper.formatDate(ele)
                this.projectArr.push(ele)
            })
        },
    }
</script>

<style scoped>
    .Menu__form {
        width: 800px;
        height: 400px;
        display: flex;
    }
    .Menu__form--items {
        width: 50%;
        height: 100%;

        padding: 10px;
        display: flex;
        flex-direction: column;
    }
    .Menu__form--items-content {
        width: 100%;
        height: 90px;
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
