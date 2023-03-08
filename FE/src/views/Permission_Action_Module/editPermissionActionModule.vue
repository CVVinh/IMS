<template>
    <div class="d-inline">
        <Edit @click="openModal" :disabled="checkEdit" />
        <Dialog
            header="Sửa thao tác"
            :visible="displayModal"
            :breakpoints="{ '1500px': '45vw', '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw', maxWidth: '400px' }"
            :modal="true"
            :maximizable="true"
            :closable="false"
            @hide="handleHide"
            @show="handleLoad"
        >
            <div>
                <div class="field mb-3">
                    <label for="moduleName">Tên chức năng thay đổi thao tác *</label>      
                    <Dropdown 
                    disabled
                    v-model="idModule" :options="listModule" optionLabel="nameModule" optionValue="id"
                    placeholder="Select a module" 
                    id="moduleName"
                    style="width: 100%"
                    :class="v$.idModule.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />  
                    <small v-if="v$.idModule.$invalid && isSubmited" id="moduleName-help" class="p-error">
                        Chọn chức năng.
                    </small>
                </div>

                <div class="field">
                    <label for="note">Chọn thao tác cho chức năng*</label>
                    
                    <Dropdown 
                       v-model="idAction" :options="listAction" optionLabel="name" optionValue="id"
                        placeholder="Select multi action" 
                        id="moduleName"
                        style="width: 100%"
                        :class="v$.idAction.$invalid && isSubmited ? 'p-invalid' : ' '"
                    />  
                    
                    
                    <small v-if="v$.idAction.$invalid && isSubmited" id="note-help" class="p-error">
                           Chọn thao tác
                    </small>
                </div>
            </div>
            <template #footer>
                <Button label="Lưu" icon="pi pi-check" @click="handleSave(v$.$invalid)" autofocus />
                <Button label="Đóng" icon="pi pi-times" @click="closeModal" class="p-button-secondary" />
            </template>
        </Dialog>
    </div>
</template>

<script>
    import Edit from '../../components/buttons/Edit.vue'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { HTTP } from '@/http-common'
import { LocalStorage } from '@/helper/local-storage.helper'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        props: {
            checkEdit: Boolean,
            action: Object,
        },
        data() {
            return {
                isSubmited: false,

                idModule: null,
                idAction:[],
                listModule:[],
                listAction:[],

                displayModal: false,
                waiting: false,
            }
        },
        validations() {
            return {
                idModule: { required },
                idAction: { required },
               
            }
        },
        async mounted() {
          this.token = await  LocalStorage.jwtDecodeToken();
            await this.getListSelect()
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
            getListSelect(){
                HTTP.get("ActionModule/getAllActionModule")
                .then(res=>{
                    this.listAction = res.data._Data
                })
                .catch(err=>console.log(err))

                HTTP.get("Modules/getListModule")
                .then(res=>{
                  
                    this.listModule = res.data._Data
                })
                .catch(err=>console.log(err))
            },
            handleLoad() {
                this.idModule = this.$props.action.moduleId
                this.idAction = this.$props.action.actionModuleId
                this.editAction = {
                    name: this.$props.action.actionModule.name,
                    userUpdated: this.token.Id,
                    description : this.$props.action.actionModule.description
                }
            },
            handleUpdate() {
             
             
                    var newObject = {
                        "moduleId": this.idModule,
                        "actionModuleId": this.idAction,
                        "userCreated": this.token.Id
                    }

                HTTP.put(`PermissionActionModule/updateUPermissionActionModule/${this.$props.action.moduleId}/${this.$props.action.actionModuleId}`, newObject)
                    .then((res) => {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Chỉnh sửa thành công!',
                            life: 3000,
                        })
                        this.closeModal()
                        this.$emit('render')
                    })
                    .catch((err) => {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Chỉnh sửa lỗi!',
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
