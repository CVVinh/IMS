<template>
    <Dialog
        header="Thêm website vào danh sách bị chặn"
        :maximizable="true"
        :closable="false"
        position="top"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '50vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container">
            <form ref="formAddProject" @submit.prevent="() => handleSubmit(!v$.$invalid)" class="p-fluid">
                <div class="input-layout w-100">
                    <label
                        class="mb-2"
                        for="BlockingWeb"
                        :class="{ 'p-error': v$.blockingWeb.linkWeb.$invalid && submitted }"
                    >
                        Đường dẫn trang web
                        <span style="color: red">*</span>
                    </label>
                    <vue3-tags-input
                        id="BlockingWeb"
                        :tags="tags"
                        class="input form-control"
                        placeholder="Nhập địa chỉ website bị chặn..."
                        @on-tags-changed="handleChangeTag"
                    />
                    <small class="p-error" v-if="v$.blockingWeb.linkWeb.required.$invalid && isSubmit">
                        {{ v$.blockingWeb.linkWeb.required.$message.replace('Value', 'Đường dẫn web') }}
                    </small>
                    <div class="mt-2">
                        <small style="font-style: italic">
                            <span style="color: red">*</span>
                            Lưu ý: Mỗi từ khóa cách nhau bằng dấu cách 'space'
                        </small>
                    </div>
                </div>
                <div class="row mb-4">
                    <div class="col col-md-12 col-24">
                        <div class="d-flex justify-content-end mt-3">
                            <div>
                                <Button
                                    label="Thêm"
                                    class="p-button-sm me-1"
                                    type="submit"
                                    icon="pi pi-check"
                                    autofocus
                                />
                            </div>
                            &emsp;
                            <div>
                                <Button
                                    label="Hủy"
                                    icon="pi pi-times"
                                    class="p-button-sm p-button-secondary"
                                    @click="closeDialog()"
                                />
                            </div>
                        </div>
                    </div>
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import jwtDecode from 'jwt-decode'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { BlockingWebDto } from '@/views/EquipmentAndDevices/BlockingWeb.dto'
    import { HTTP } from '@/http-common'
    import { BlockingWebService } from '@/service/blockingweb.service'

    export default {
        props: ['isOpen'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                userRequest: jwtDecode(localStorage.getItem('token')),
                blockingWeb: new BlockingWebDto(),
                isSubmit: false,
                submited: false,
                tags: [],
            }
        },
        beforeUpdate() {},
        methods: {
            handleSubmit() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    this.handlerAddBlockingWeb()
                }
            },
            handlerAddBlockingWeb() {
                const addBlocking = {
                    arrayBlockWeb: this.blockingWeb.linkWeb,
                    userCreate: this.userRequest.Id,
                }
                if (addBlocking) {
                    BlockingWebService.handlerRequireBlockingWeb(addBlocking)
                        .then((res) => {
                            if (res.status == 200) {
                                this.closeDialog()
                                this.resetForm()
                                this.successMessage('Thêm thành công các trang web bị chặn')
                            } else {
                                this.closeDialog()
                                this.WarningMessage('Có sự cố gì xảy ra đối với hệ thống !')
                            }
                        })
                        .catch(() => {
                            this.WarningMessage('Có sự cố gì xảy ra đối với hệ thống !')
                        })
                }
            },
            resetForm() {
                this.isSubmit = false
                this.blockingWeb.linkWeb = null
            },
            handleChangeTag(tags) {
                this.blockingWeb.linkWeb = tags
            },
            successMessage(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            WarningMessage(mess) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: mess,
                    life: 2000,
                })
            },
            closeDialog() {
                this.$emit('closeDialogBlocking')
                this.resetForm()
            },
        },
        validations() {
            return {
                blockingWeb: {
                    linkWeb: { required },
                },
            }
        },
    }
</script>
<style>
    .v3ti {
        height: 250px !important;
        display: block;
    }
</style>
