<template>
    <Dialog
        header="Thêm thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
    >
        <form enctype="multipart/form-data" class="container">
            <div class="Menu__form ">
                <div class="Menu__form--items items-left">
                    <div class="Menu__form--items-content">
                        <label
                            :class="{
                                'p-error': v$.Datasend.customerName.required.$invalid && isSubmit,
                                'input-title': true,
                            }"
                            >Tên khách hàng<span style="color: red">*</span></label>

                        <!-- <InputText type="text" v-model="v$.Datasend.customerName.$model" placeholder="Nhập tên khách hàng"/> -->

                        <Dropdown
                            class="inputdrop"
                            v-model="Datasend.customerName"
                            :options="customerArray"
                            optionLabel="fullName"
                            optionValue="id"
                            placeholder="Chọn khách hàng"
                        />
                        <small class="p-error" v-if="v$.Datasend.customerName.required.$invalid && isSubmit">{{
                            v$.Datasend.customerName.required.$message.replace('Value', 'Customer Name')
                        }}</small>
                    </div>

                    <div class="Menu__form--items-content">
                        <label>Dự án</label>
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
                <div class="Menu__form--items items-right">
                    <div class="Menu__form--items-content">
                        <label
                            :class="{
                                'p-error': v$.Datasend.paidReason.required.$invalid && isSubmit,
                                'input-title': true,
                            }"
                            >Lý do chi trả<span style="color: red">*</span></label>

                        <!-- <InputText type="text" v-model="v$.Datasend.paidReason.$model" placeholder="Lý do chi" /> -->
                        <Dropdown
                            class="inputdrop"
                            v-model="Datasend.paidReason"
                            :options="paidReasonArray"
                            optionLabel="name"
                            optionValue="id"
                            placeholder="Lý do chi"
                        />
                        <small class="p-error" v-if="v$.Datasend.paidReason.required.$invalid && isSubmit">{{
                            v$.Datasend.paidReason.required.$message.replace('Value', 'Paid Reason')
                        }}</small>
                    </div>
                    
                    <div class="Menu__form--items-content">
                        <label
                            :class="{
                                'p-error': v$.Datasend.amountPaid.required.$invalid && isSubmit,
                                'input-title': true,
                            }"
                            >Mức chi (VND)<span style="color: red">*</span></label
                        >
                        <InputNumber v-model="v$.Datasend.amountPaid.$model" :min=0 mode="decimal"/>
                        <small class="p-error" v-if="v$.Datasend.amountPaid.required.$invalid && isSubmit">{{
                            v$.Datasend.amountPaid.required.$message.replace('Value', 'Amount Paid')
                        }}</small>
                    </div>
                </div>
            </div>
            
            <div class="flex justify-content-center container">
                <label for="note">Nội dung lý do chi</label>
                <Textarea
                    id="note"
                    v-model="Datasend.contentReason"
                    :autoResize="true"
                    rows="5"
                    cols="30"
                    style="width: 100%"
                />
            </div>

            <div class="flex justify-content-center container mt-3">
                <h6>Thêm ảnh</h6>
                <div class="input_file">
                    <input type="file" multiple @change="onFileChange($event)" ref="fileupload" accept="image/*"/>
                </div>

                <div class="jumbotron p-fluid mt-3 content_box" v-if="isHaveImg">
                    <div class="row">
                        <div class="col-md-3 container_img" v-for="(item, index) in images" :key="index" :id="index">
                            <div class="image_item">
                                <img class="preview img-thumbnail img_show" v-bind:ref="'image' + parseInt(index)" />{{ item.name }}
                                <div class="middle_img">
                                    <button type="button" @click="removeImage(index)" class="button_del_img">&times;</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </form>

        <template #footer>
            <Button label="Lưu" icon="pi pi-check" class="p-button-primary p-button-icon" @click="handleSubmit"> </Button>
            <Button label="Huỷ" icon="pi pi-times" class="p-button-secondary p-button-icon" @click="closeModal"></Button>
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
                    projectId: 0,
                    customerName: '',
                    amountPaid: null,
                    paidReason: '',
                    paidPerson: '',
                    contentReason: '',
                    isPaid: false,
                    paidDate: null,
                    paidImage: [],
                },
                projectArr: [],
                customerArray: [],
                paidReasonArray: [],

                isSubmit: false,
                token: null,
                currentUser: null,
                images: [],
                isHaveImg: false,
            }
        },
        validations() {
            return {
                Datasend: {
                    customerName: { required },
                    amountPaid: { required },
                    paidReason: { required },
                },
            }
        },

        props: ['status', 'optionmodule', 'customerArr', 'paidReasonArr'],

        methods: {
            closeModal() {
                this.isHaveImg = false;
                this.$emit('closemodal');
            },

            clearform() {
                this.Datasend.projectId = 0;
                this.Datasend.customerName = '';
                this.Datasend.amountPaid = null;
                this.Datasend.paidReason = '';
                this.Datasend.contentReason = '';
                this.isSubmit = false;
                this.paidImage = [];
            },

            onFileChange(event) {
                this.images = [];
                this.isHaveImg = true;
                const selectedFiles = event.target.files

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
            },

            // xin dung xoa nhung dong code comment nay
            // removeImage(index) {
            //     this.images.splice(index, 1)
            //     let imagesRefs = this.$refs
            //     Object.keys(imagesRefs).forEach((key) => {
            //         let refIndex = key.slice(-1) // 1; index: 0
            //         if (refIndex > index) {
            //             imagesRefs[key][0].src = imagesRefs['image' + (refIndex - 1)][0].src
            //         }
            //     })
            // },

            removeImage(index) {
                this.images.splice(index, 1)
                if(this.images.length == 0){
                    this.$refs.fileupload.value = null;
                    this.isHaveImg = false;
                }
                let imagesRefs = this.$refs
                Object.keys(imagesRefs).forEach((key) => {
                    let refIndex = key.slice(-1) 
                    if (key.includes("image")) {
                        if (parseInt(refIndex) > parseInt(index) && imagesRefs[key][0]) {
                            imagesRefs['image' + (refIndex - 1)][0].src = imagesRefs[key][0].src;
                        }
                    }
                });
            },

            async CallApi(fromData) {
                try {
                    const res =  await HTTP.post(`Paid`, fromData)

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
                            console.log(error.response.data)
                            break
                        default:
                    }
                }
            },

            async AddPaid() {
                var token = localStorage.getItem('token')
                this.currentUser = jwt_decode(token)

                const formData = new FormData();
                formData.append('PaidPerson', this.currentUser.Id);
                formData.append('ProjectId', this.Datasend.projectId);
                formData.append('CustomerName', this.Datasend.customerName);
                formData.append('AmountPaid', this.Datasend.amountPaid);
                formData.append('PaidReason', this.Datasend.paidReason);
                formData.append('ContentReason', this.Datasend.contentReason);
                
                this.images.forEach((item) => {
                    formData.append('paidImage', item)
                })
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
                }
            },

            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 });
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 });
            },

            showInfo(message) {
                this.$toast.add({ severity: 'info', summary: 'Thông báo', detail: message, life: 3000 });
            },
            
        },

        beforeUpdate() {
            this.projectArr = this.optionmodule;
            this.customerArray = this.customerArr;
            this.paidReasonArray = this.paidReasonArr;
        },
    }
</script>

<style >
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
        margin-bottom: 10px;
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

    .input_file {
        border: 1px solid #e5e5e5;
        border-radius: 10px;
    }

    input[type='file']::file-selector-button {
        background-color: #7128fa;
        color: #fff;
        border: 0px;
        border-right: 1px solid #e5e5e5;
        padding: 10px 15px;
        margin-right: 20px;
        border-top-left-radius: 10px;
        border-bottom-left-radius: 10px;
        cursor: pointer;
    }

    input[type='file']::file-selector-button:hover {
        background-color: #591bcc;
        border: 0px;
        border-right: 1px solid #591bcc;
    }

    .content_box {
        box-shadow: -3px 3px 5px -3px #888888, 4px 5px 3px -4px #888888, 4px 5px 2px -5px #888888 inset;
        padding: 10px;
        border-radius: 10px;
    }

    .img_show:hover {
        cursor: pointer;
        box-shadow: 0 0 5px 2px rgba(0, 140, 186, 0.5);
    }


    .container_img {
        position: relative;
    }

    .image_item {
        opacity: 1;
        display: block;
        height: auto;
        transition: .5s ease;
        backface-visibility: hidden;
    }

    .middle_img {
        transition: .5s ease;
        opacity: 0;
        position: absolute;
        top: 38%;
        left: 50%;
        transform: translate(-50%, -50%);
        -ms-transform: translate(-50%, -50%);
        text-align: center;
    }

    .container_img:hover .image_item {
        opacity: 0.5;
    }

    .container_img:hover .middle_img {
        opacity: 1;
    }

    .button_del_img {
        background-color: #ddd;
        border: none;
        color: black;
        padding: 5px 10px;
        text-align: center;
        text-decoration: none;
        display: inline-block;
        margin: 4px 2px;
        cursor: pointer;
        border-radius: 16px;
        font-size: 16px;
    }

    .button_del_img:hover {
        color: white;
        background-color: red;
    }

</style>
