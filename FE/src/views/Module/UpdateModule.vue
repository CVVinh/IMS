<template>
    <div class="d-inline">
        <Edit @click="openModal" :disabled="checkEdit" />
        <Dialog
            header="Sửa chức năng"
            :visible="displayModal"
            :breakpoints="{ '1500px': '45vw', '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw', maxWidth: '400px' }"
            :modal="true"
            @hide="handleHide"
            @show="handleLoad"
        >
            <div>
                <div class="field mb-3">
                    <label for="moduleName">Tên chức năng*</label>
                    <InputText
                        v-model="editModule.nameModule"
                        id="moduleName"
                        type="text"
                        style="width: 100%"
                        :class="v$.editModule.nameModule.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />
                    <small v-if="v$.editModule.nameModule.$invalid && isSubmited" id="moduleName-help" class="p-error">
                        Nhập tên chức năng.
                    </small>
                </div>
                <div class="field">
                    <label for="note">Ghi chú *</label>
                    <Textarea
                        id="note"
                        v-model="editModule.note"
                        :autoResize="true"
                        rows="5"
                        cols="30"
                        style="width: 100%"
                        :class="v$.editModule.note.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />
                    <small v-if="v$.editModule.note.$invalid && isSubmited" id="note-help" class="p-error">
                        Nhập ghi chú.
                    </small>
                </div>
            </div>
            <template #footer>
                <Button label="Đóng" icon="pi pi-times" @click="closeModal" class="p-button-text" />
                <Button label="Lưu" icon="pi pi-check" @click="handleSave(v$.$invalid)" autofocus />
            </template>
        </Dialog>
    </div>
</template>

<script>
    import Edit from '../../components/buttons/Edit.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: {
            checkEdit: Boolean,
            module: Object,
        },
        data() {
            return {
                isSubmited: false,
                editModule: {
                    nameModule: '',
                    note: '',
                },
                displayModal: false,
                waiting: false,
            }
        },
        validations() {
            return {
                editModule: {
                    nameModule: { required },
                    note: { required },
                },
            }
        },
        mounted() {
            this.handleLoad()
        },
        methods: {
            openModal() {
                this.displayModal = true
            },
            closeModal() {
                this.isSubmited = false
                this.displayModal = false
            },
            handleLoad() {
                this.editModule = {
                    id: this.$props.module.id,
                    nameModule: this.$props.module.nameModule,
                    note: this.$props.module.note,
                    isDeleted: this.$props.module.isDeleted,
                }
            },
            handleUpdate() {
                HTTP.put(`Modules/updateMoudel`, this.editModule)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Sửa thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi ',
                            detail: 'Lỗi máy chủ',
                            life: 3000,
                        })
                    })
            },
            handleSave(isFormValid) {
                if (this.waiting) {
                    return
                } else {
                    this.waiting = true
                    setTimeout(() => {
                        this.waiting = false
                    }, 3000)
                }
                this.isSubmited = true
                if (isFormValid) {
                    return
                }
                this.handleUpdate()
            },
            handleHide() {
                this.closeModal()
            },
        },
        components: {
            Edit,
        },
    }
</script>

<style scoped>
    .field label {
        margin-bottom: 5px;
    }
</style>
