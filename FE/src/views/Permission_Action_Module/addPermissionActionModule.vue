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
                    <label for="moduleName">Chọn chức năng thêm thao tác *</label>      
                    <Dropdown 
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
                    <label for="note">Chọn thao tác cho chức năng *</label>
                    <MultiSelect  
                       v-model="idAction" :options="listAction" optionLabel="name" optionValue="id"
                        placeholder="Select multi action" 
                        id="moduleName"
                        style="width: 100%"
                        :class="v$.idAction.$invalid && isSubmited ? 'p-invalid' : ' '" />
                    
                    <small v-if="v$.idAction.$invalid && isSubmited" id="note-help" class="p-error">
                           Chọn thao tác
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
                idModule: null,
                idAction:[],
                listModule:[],
                listAction:[],
                displayModal: false,
                waitingAdd: false,
                token : '',
            }
        },
        async mounted(){
            this.token = await LocalStorage.jwtDecodeToken()
            await this.getListSelect();
        },
        validations() {
            return {
                idModule : { required } ,
                idAction: { required },
            }
        },
        methods: {
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

                const dataSend = []
                this.idAction.map(ele=>{
                    var newObject = {
                        "moduleId": this.idModule,
                        "actionModuleId": ele,
                        "userCreated": this.token.Id
                    }
                    dataSend.push(newObject)
                })
                HTTP.post(`PermissionActionModule`, dataSend)
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
        watch:{
            // reset array action when Idmodule change
            idModule: function(oldvalue,newvalue){
               this.idAction = [] 
            }
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
