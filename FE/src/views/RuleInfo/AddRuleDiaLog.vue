<template>
    <Dialog
        :visible="statusopen"
        :closable="false"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
        header="Thêm"
    >
        <div>
            <Toast position="top-right" />
        </div>

        <div class="form-demo">
            <div class="flex justify-content-center container">
                <div class="p-fluid form-addrule">
                    <form class="form-addproject" @submit.prevent="handleSubmit()" enctype="multipart/form-data">
                        <div class="row mb-4">
                            <div class="col col-md-12 col-24">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.title.$error }">
                                    <InputText
                                        id=" title"
                                        v-model="v$.title.$model"
                                        :class="{ 'p-invalid': v$.title.$invalid && submitted }"
                                        autocomplete="false"
                                    />
                                    <label for="title" :class="{ 'p-error': v$.title.$invalid && submitted }"
                                        >Tiêu đề*</label
                                    >
                                </div>
                                <span v-if="v$.title.$error && submitted">
                                    <span id="title-error" v-for="(error, index) of v$.title.$errors" :key="index">
                                        <small class="p-error">{{ error.$message }}</small>
                                    </span>
                                </span>
                                <small
                                    v-else-if="(v$.title.$invalid && submitted) || v$.title.$pending.$response"
                                    class="p-error"
                                    >{{ v$.title.required.$message.replace('Value', 'Title') }}
                                </small>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col col-md-6 col-12">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.applyDay.$error }">
                                    <Calendar
                                        id="applyDay"
                                        v-model="v$.applyDay.$model"
                                        :showIcon="true"
                                        :class="{ 'p-invalid': v$.applyDay.$invalid && submitted }"
                                    />
                                    <label for="applyDay" :class="{ 'p-error': v$.applyDay.$invalid && submitted }"
                                        >Ngày áp dụng*</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.applyDay.$invalid && submitted) || v$.applyDay.$pending.$response"
                                    class="p-error"
                                    >{{ v$.applyDay.required.$message.replace('Value', 'Apply Day') }}</small
                                >
                            </div>

                            <div class="col col-md-6 col-12">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.expiredDay.$error }">
                                    <Calendar
                                        id="expiredDay"
                                        v-model="v$.expiredDay.$model"
                                        :showIcon="true"
                                        :class="{ 'p-invalid': v$.expiredDay.$invalid && submitted }"
                                    />
                                    <label for="expiredDay" :class="{ 'p-error': v$.expiredDay.$invalid && submitted }"
                                        >Ngày hết hạn *</label
                                    >
                                </div>
                                <small
                                    v-if="(v$.expiredDay.$invalid && submitted) || v$.expiredDay.$pending.$response"
                                    class="p-error"
                                    >{{ v$.expiredDay.required.$message.replace('Value', 'Expire Day') }}</small
                                >
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col col-md-12 col-24">
                                <div class="input_file">
                                    <input
                                        type="file"
                                        @change="uploadFile($event)"
                                        accept=".doc, xls, .xlsx, .pdf, .ppt, .pptx, .txt"
                                    />
                                </div>
                            </div>
                        </div>

                        <div class="row">
                            <div class="col col-md-12 col-24">
                                <div class="p-float-label" :class="{ 'form-group--error': v$.content.$error }">
                                    <h6>Nội dung</h6>
                                    <Editor
                                        editorStyle="height: 400px"
                                        id=" content"
                                        v-model="v$.content.$model"
                                        :class="{ 'p-invalid': v$.content.$invalid && submitted }"
                                    />
                                </div>

                                <small
                                    v-if="(v$.content.$invalid && submitted) || v$.content.$pending.$response"
                                    class="p-error"
                                    >{{ v$.content.required.$message.replace('Value', 'Content') }}
                                </small>
                            </div>
                        </div>

                        <div class="row mb-4">
                            <div class="col col-md-12 col-24">
                                <div class="d-flex justify-content-end mt-3">
                                    <div>
                                        <Button type="submit" label="Lưu" icon="pi pi-check" />
                                    </div>
                                    &emsp;
                                    <div>
                                        <Button
                                            label="Hủy"
                                            icon="pi pi-times"
                                            class="p-button-secondary"
                                            @click="closeAdd()"
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </form>
                </div>
            </div>
        </div>
    </Dialog>
</template>

<script>
    import { required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'

    export default {
        props: ['statusopen'],

        setup: () => ({
            v$: useVuelidate(),
        }),

        data() {
            return {
                title: null,
                applyDay: new Date(),
                expiredDay: new Date(),
                idUser: localStorage.getItem('username'),
                formFile: null,
                content: null,

                submitted: false,
            }
        },

        validations() {
            return {
                title: {
                    required,
                },
                applyDay: {
                    required,
                },
                expiredDay: {
                    required,
                },
                content: {
                    required,
                },
            }
        },

        methods: {
            onUploadFile() {
                this.$toast.add({ severity: 'info', summary: 'Success', detail: 'File Uploaded', life: 3000 })
            },

            closeAdd() {
                this.$emit('closeAdd')
            },

            handleSubmit() {
                this.submitted = true
                if (this.v$.$invalid === false) {
                    this.submit()
                    this.closeAdd()
                }
            },

            resetForm() {
                this.title = null
                this.applyDay = new Date()
                this.expiredDay = new Date()
                this.idUser = localStorage.getItem('username')
                this.content = null
                this.content = null
            },

            async CallApi(fromData) {
                try {
                    const res = await HTTP.post('/Rules', fromData)

                    switch (res.status) {
                        case HttpStatus.OK:
                            this.resetForm()
                            this.$emit('reloadpage')
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Thành công',
                                detail: 'Thêm mới thành công!',
                                life: 3000,
                            })
                            break
                        case HttpStatus.UNAUTHORIZED:
                        case HttpStatus.FORBIDDEN:
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Không có quyền thực hiện thao tác này!',
                                life: 2000,
                            })
                            break
                        default:
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Lưu lỗi ',
                                life: 3000,
                            })
                    }
                } catch (error) {
                    switch (error.code) {
                        case 'ERR_NETWORK':
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Kiểm tra kết nối !',
                                life: 3000,
                            })
                            break
                        case 'ERR_BAD_REQUEST':
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: error.response.data,
                                life: 3000,
                            })
                            break
                        default:
                    }
                }
            },

            async submit() {
                var token = localStorage.getItem('token')
                var decode = jwt_decode(token)

                const fromData = new FormData()
                fromData.append('title', this.title)
                fromData.append('applyDay', DateHelper.formatDateTime(this.applyDay))
                fromData.append('expiredDay', DateHelper.formatDateTime(this.expiredDay))
                fromData.append('content', this.content)
                fromData.append('idUser', parseInt(decode.Id))
                fromData.append('formFile', this.formFile)

                await this.CallApi(fromData)
            },

            uploadFile(event) {
                const file = event.target.files[0]
                this.formFile = file
            },
        },
    }
</script>

<style lang="scss" scope>
    .form-demo {
        min-width: 900px;
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-items: center;
        margin-top: 2rem;
    }

    .input {
        width: 100%;
    }

    .form-addrule {
        width: 100%;
    }

    .p-fileupload {
        margin-left: 30px;
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
</style>
