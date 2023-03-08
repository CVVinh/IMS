<template>
    <div class="d-inline">
        <Add label="Thêm" @click="openModal" />
        <Dialog
            header="Thêm thao tác"
            :visible="displayModal"
            :breakpoints="{ '1500px': '45vw', '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw', maxWidth: '400px' }"
            :modal="true"
            :maximizable="true"
            :closable="false"
            @hide="handleHide"
        >
            <div>
                <div class="field mb-3">
                    <label for="moduleName">Thêm thao tác *</label>
                    <InputText
                        v-model="newActions.nameAction"
                        id="moduleName"
                        type="text"
                        style="width: 100%"
                        :class="v$.newActions.nameAction.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />
                    <small v-if="v$.newActions.nameAction.$invalid && isSubmited" id="moduleName-help" class="p-error">
                        Nhập tên chức năng.
                    </small>
                </div>
                <div class="field">
                    <label for="note">Ghi chú *</label>
                    <Textarea
                        id="note"
                        v-model="newActions.description"
                        :autoResize="true"
                        rows="5"
                        cols="30"
                        style="width: 100%"
                        :class="v$.newActions.description.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />
                    <small v-if="v$.newActions.description.$invalid && isSubmited" id="note-help" class="p-error">
                        Nhập ghi chú.
                    </small>
                </div>
            </div>
            <template #footer>
                <Button label="Lưu" icon="pi pi-check" autofocus @click="handleSave(v$.$invalid)" />
                <Button label="Đóng" icon="pi pi-times" @click="closeModal" class="p-button-secondary" />
            </template>
        </Dialog>
    </div>
</template>

<script>
    import Add from '../../components/buttons/Add.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
import { LocalStorage } from '@/helper/local-storage.helper'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        data() {
            return {
                isSubmited: false,
                newActions: {
                    name: '',
                    description: '',
                },
                displayModal: false,
                waitingAdd: false,
                token : '',
            }
        },
        async mounted(){
            this.token = await LocalStorage.jwtDecodeToken()
        },
        validations() {
            return {
                newActions: {
                    nameAction: { required },
                    description: { required },
                },
            }
        },
        methods: {
            openModal() {
                this.displayModal = true
            },
            closeModal() {
                this.isSubmited = false
                this.newActions = {
                    name: '',
                    description: '',
                }
                this.displayModal = false
            },
            handleAdd() {

                const datasend = {
                    "name": this.newActions.nameAction,
                    "description": this.newActions.description,
                    "userCreated": this.token.Id
                }
                HTTP.post(`ActionModule/createActionModule`, datasend)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Thêm mới thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Thêm mới lỗi!',
                            life: 3000,
                        })
                    })
            },
            handleSave(isFormValid) {
                if (this.waitingAdd) {
                    return
                } else {
                    this.waitingAdd = true
                    setTimeout(() => {
                        this.waitingAdd = false
                    }, 3000)
                }
                this.isSubmited = true
                if (isFormValid) {
                    return
                }
                this.handleAdd()
            },
            handleHide() {
                this.closeModal()
            },
        },
        components: {
            Add,
        },
    }
</script>

<style scoped>
    .field label {
        margin-bottom: 5px;
    }
</style>
