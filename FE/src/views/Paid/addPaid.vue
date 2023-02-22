<template>
    <Dialog
        header="Thêm thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <form enctype="multipart/form-data">
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
                            :showIcon="true" />
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
            <!-- <FileUpload
                accept="image/*"
                :maxFileSize="1000000"
                :showUploadButton="true"
                :multiple="true"
                @upload="(e) => onTemplatedUpload(e)"
                id="images"
            >
                <template #empty>
                    <p>Drag and drop files to here to upload.</p>
                </template>
            </FileUpload> -->
            <div class="container">
                <input type="file" multiple @change="onFileChange" ref="fileupload" accept="image/*"/><br /><br />

                <div class="jumbotron">
                    <div class="row">
                        <div v-for="(item, index) in images" :key="index">
                            <div class="col-md-4" :id="index">
                                <button type="button" @click="removeImage(index)">&times;</button>
                                <img class="preview img-thumbnail" v-bind:ref="'image' + parseInt(index)" />
                                {{ item.name }}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <!-- <input type="file" ref="files" multiple="multiple" /> -->
        </form>

        <template #footer>
            <button class="btn btn-primary" @click="handleSubmit">Lưu</button>
            <button class="btn btn-secondary" @click="closeModal">Huỷ</button>
        </template>
    </Dialog>
</template>

<script>
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwt_decode from 'jwt-decode'
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
                    paidPerson: '',
                    isPaid: false,
                    paidDate: null,
                    paidImage: [],
                },
                projectArr: [],
                isPaidArr: [
                    { isPaid: false, name: 'Chưa Thanh Toán' },
                    { isPaid: true, name: 'Đã Thanh Toán' },
                ],
                isSubmit: false,
                token: null,
                currentUser: null,
                images: [],
                files: [],
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

        props: ['status', 'optionmodule'],

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

            onFileChange(e) {
                this.images = [];
                console.log("cos qua");
                let selectedFiles = e.target.files

                for (var i = 0; i < selectedFiles.length; i++) {
                    this.images.push(selectedFiles[i])
                }
                for (let i = 0; i < this.images.length; i++) {
                    let reader = new FileReader()
                    reader.addEventListener(
                        'load',
                        function () {
                            this.$refs['image' + parseInt(i)][0].src = reader.result
                        }.bind(this),
                        false,
                    ) //add event listener
                    reader.readAsDataURL(this.images[i])
                }
                //this.Datasend.paidImage = this.images;
            },

            // removeImage(index) {
            //     this.images.splice(index, 1)
            //     let imagesRefs = this.$refs
                
            //     Object.keys(imagesRefs).forEach((key) => {
            //         let refIndex = key.slice(-1) // 1; index: 0

            //         console.log("index: "+ index + " ;refIndex: "+ refIndex)
                    
            //         if (refIndex > index) {
            //             console.log("key: "+ key)
            //             imagesRefs[key][0].src = imagesRefs['image' + (refIndex - 1)][0].src
            //         }
            //     })

            //     console.log("imagesRefs: "+ JSON.stringify(imagesRefs) )
            // },

            removeImage(index) {

                this.images.splice(index, 1)
                if(this.images.length == 0){
                    this.$refs.fileupload.value = null;
                }
                var imagesRefs = this.$refs

                // Object.keys(imagesRefs).forEach((key) => {
                //     console.log("key: "+ key )

                //     let refIndex = key.slice(-1) // 1; index: 0

                //     console.log("index: "+ index + " ;refIndex: "+ refIndex)
                    
                //     if (refIndex >= index) {
                //         console.log("key: "+ key)
                //         console.log('image' + (refIndex - 1))
                //         imagesRefs[key][0].src = imagesRefs['image' + (refIndex - 1)][0].src
                //     }
                // })

                console.log("this.images: "+ JSON.stringify(this.images) )
                console.log("imagesRefs: "+ JSON.stringify(imagesRefs) )
            },


            async CallApi(fromData) {
                try {
                    const res =  await HTTP_LOCAL.post(`Paid`, fromData)

                    switch (res.status) {
                        case HttpStatus.OK:
                            this.clearform()
                            this.showSuccess('Thêm thành công!');
                            this.$emit('reloadpage')
                            break
                        case HttpStatus.UNAUTHORIZED:
                        case HttpStatus.FORBIDDEN:
                            this.showError2('Không có quyền thực hiện thao tác này!')
                            break
                        default:
                            this.showError('Lưu lỗi!')
                            
                    }
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.showError('Kiểm tra kết nối!')
                            break
                        case 'ERR_BAD_REQUEST':
                            this.showError(error.response.data)
                            break
                        default:
                    }
                }
            },

            async AddPaid() {
                var token = localStorage.getItem('token')
                this.currentUser = jwt_decode(token)

                const formData = new FormData()
                formData.append('PaidPerson', this.currentUser.Id)
                formData.append('PaidDate', DateHelper.formatDateTime(this.Datasend.paidDate))
                formData.append('ProjectId', this.Datasend.projectId)
                formData.append('CustomerName', this.Datasend.customerName)
                formData.append('AmountPaid', this.Datasend.amountPaid)
                formData.append('PaidReason', this.Datasend.paidReason)
                formData.append('IsPaid', this.Datasend.isPaid)
                
                
                // this.images.forEach((item) => {
                //     formData.append('paidImage', item)
                // })

                //console.log(this.Datasend.paidImage)
                await this.CallApi(formData)
                
            },

            async handleSubmit() {
                try{
                    this.isSubmit = true
                    if (!this.v$.$invalid) {
                        await this.AddPaid();
                        this.closeModal()
                    }
                }
                catch (err) {
                    console.log(err)
                    this.showError(error.response.data)
                }
            },

            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },

            showInfo(message) {
                this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 })
            },
            
        },

        beforeUpdate() {
            this.projectArr = this.optionmodule
        },
    }
</script>

<style>
    .Menu__form {
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
    .p-dialog-footer {
        display: flex;
    }
    /* .p-fileupload.p-fileupload-advanced.p-component {
        padding-right: 10px;
        padding-left: 10px;
    } */
    .preview {
        display: flex;
        justify-content: center;
        align-items: center;
        height: 100px;
        width: 100px;
    }
</style>
