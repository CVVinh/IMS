<template>
    <Dialog
        header="Thêm nhóm"
        :maximizable="true"
        :closable="false"
        position="center"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="form-layout">
            <div class="header-outside">
                <div class="title">
                    <h1>Thêm nhóm</h1>
                </div>
            </div>
            <form class="form-addproject" @submit.prevent="submitGroup()">
                <div class="input-layout">
                    <label
                        :class="{
                            'p-error': v$.formGroup.nameGroup.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Tên nhóm<span style="color: red">*</span></label
                    >
                    <InputText v-model="v$.formGroup.nameGroup.$model" class="input" />
                    <small class="p-error" v-if="v$.formGroup.nameGroup.required.$invalid && isSubmit">{{
                        v$.formGroup.nameGroup.required.$message.replace('Value', 'Name group')
                    }}</small>
                </div>

                <div class="input-layout">
                    <label class="input-title">Mô tả </label>
                    <Textarea v-model="this.formGroup.discription" class="input" rows="5" />
                </div>
                <div class="btn-right">
                    <Button label="Lưu" type="submit" icon="pi pi-check" />{{ ' ' }}
                    <Button label="Hủy" class="p-button-secondary" v-on:click="closeDialog()" />
                </div>
            </form>
        </div>
    </Dialog>
</template>
<script>
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import jwtDecode from 'jwt-decode'
    export default {
        props: ['isOpen'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                formGroup: {
                    nameGroup: null,
                    discription: '',
                },
                isSubmit: false,
                submited: false,
                user: '',
            }
        },
        methods: {
            submitGroup() {
                this.isSubmit = true
                if (!this.v$.$invalid && this.user) {
                    HTTP.post('Group/addGroup', { ...this.formGroup, userCreated: this.user.Id })
                        .then((res) => {
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Thành công',
                                    detail: 'Thêm mới thành công!',
                                    life: 3000,
                                })
                                this.submited = true
                                this.$emit('setChange')
                                this.closeDialog()
                            }
                        })
                        .catch((err) => {
                            if (err.response) {
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Lỗi',
                                    detail: (err.response.data = 'Ten nhom da ton tai'
                                        ? 'Tên nhóm đã tồn tại!'
                                        : err.response.data),
                                    life: 3000,
                                })
                            } else {
                            }
                        })
                }
            },

            closeDialog() {
                this.$emit('closeDialog')
            },

            resetForm() {
                this.isSubmit = false
                this.formGroup = {
                    nameGroup: null,
                    discription: '',
                }
            },
        },

        mounted() {
            this.user = jwtDecode(localStorage.getItem('token'))
        },
        validations() {
            return {
                formGroup: {
                    nameGroup: { required },
                },
            }
        },
    }
</script>
<style scoped>
    .form-layout {
        min-width: 600px;
        width: 100%;
        height: 100%;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-items: center;
    }
    .input {
        width: 100%;
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
    .btn-right {
        float: right;
        margin-top: 10px;
    }
</style>
<style>
    .p-dialog-header-close-icon {
        z-index: 2000;
    }
</style>
