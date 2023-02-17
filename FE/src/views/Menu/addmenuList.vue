<template>
    <Dialog :visible="status" :closable="false" modal="true">
        <template #header>
            <h3>Add menu</h3>
        </template>

        <div class="Menu__form">
            <div class="Menu__form--items items-left">
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.idModule.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Module<span style="color: red">*</span></label
                    >
                    <Dropdown
                        class="inputdrop"
                        @load="GetParenByModule"
                        @change="GetParenByModule"
                        v-model="v$.Datasend.idModule.$model"
                        :options="optionnew"
                        optionLabel="nameModule"
                        optionValue="id"
                        placeholder="Select a module"
                    />
                    <small class="p-error" v-if="v$.Datasend.idModule.required.$invalid && isSubmit">{{
                        v$.Datasend.idModule.required.$message.replace('Value', 'Title')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.title.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Title<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.title.$model" placeholder="Input title" />
                    <small class="p-error" v-if="v$.Datasend.title.required.$invalid && isSubmit">{{
                        v$.Datasend.title.required.$message.replace('Value', 'Title')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.icon.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Icon<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.icon.$model" placeholder="Input class Icon" />
                    <small class="p-error" v-if="v$.Datasend.icon.required.$invalid && isSubmit">{{
                        v$.Datasend.icon.required.$message.replace('Value', 'icon')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.view.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >View<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.view.$model" placeholder="Input View" />
                    <small class="p-error" v-if="v$.Datasend.view.required.$invalid && isSubmit">{{
                        v$.Datasend.view.required.$message.replace('Value', 'View')
                    }}</small>
                </div>
            </div>
            <div class="Menu__form--items items-right">
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.controller.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Controller<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.controller.$model" placeholder="Input Controller" />
                    <small class="p-error" v-if="v$.Datasend.controller.required.$invalid && isSubmit">{{
                        v$.Datasend.controller.required.$message.replace('Value', 'Controller')
                    }}</small>
                </div>
                <div class="Menu__form--items-content">
                    <label
                        :class="{
                            'p-error': v$.Datasend.action.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Action<span style="color: red">*</span></label
                    >
                    <InputText type="text" v-model="v$.Datasend.action.$model" placeholder="Input action" />
                    <small class="p-error" v-if="v$.Datasend.action.required.$invalid && isSubmit">{{
                        v$.Datasend.action.required.$message.replace('Value', 'Action')
                    }}</small>
                </div>

                <div class="Menu__form--items-content">
                    <h5>Parent</h5>
                    <Dropdown
                        class="inputdrop"
                        v-model="Datasend.parent"
                        :options="parentArr"
                        optionLabel="title"
                        optionValue="id"
                        placeholder="Select a parent"
                    />
                </div>
            </div>
        </div>

        <template #footer>
            <Button label="Add" icon="pi pi-check" autofocus @click="AddMenu" />
            <Button label="Close" icon="pi pi-times" class="p-button-text" @click="closeModal" />
        </template>
    </Dialog>
</template>

<script>
    import { HTTP } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    export default {
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                Datasend: {
                    idModule: '',
                    title: '',
                    icon: '',
                    view: '',
                    controller: '',
                    action: '',
                    parent: 0,
                },
                selectedCountry: null,
                parentArr: [],
                isSubmit: false,
                optionnew: [],
            }
        },
        validations() {
            return {
                Datasend: {
                    idModule: { required },
                    title: { required },
                    icon: { required },
                    view: { required },
                    controller: { required },
                    action: { required },
                    parent: 0,
                },
            }
        },
        props: ['status', 'optionmodule'],
        methods: {
            closeModal() {
                this.$emit('closemodal')
            },
            GetParenByModule() {
                this.Datasend.parent = 0
                HTTP.get(`Menu/getlistMenubyIdModoule/${this.Datasend.idModule}`).then((res) => {
                    this.parentArr = res.data
                })
            },
            clearform() {
                this.Datasend.idModule = ''
                this.Datasend.title = ''
                this.Datasend.icon = ''
                this.Datasend.view = ''
                this.Datasend.controller = ''
                this.Datasend.action = ''
                this.Datasend.parent = 0
                this.isSubmit = false
            },
            AddMenu() {
                this.isSubmit = true
                if (!this.v$.$invalid) {
                    HTTP.post('Menu/addMenu', this.Datasend).then((res) => {
                        if (res.data === 'Sucess') {
                            this.clearform()
                            this.$emit('success')
                            this.closeModal()
                        }
                        this.$emit('reloadpage')
                    })
                } else {
                    this.$emit('failed')
                }
            },
        },

        beforeUpdate() {
            this.optionmodule.map((ele) => {
                if (ele.isDeleted == 0) {
                    this.optionnew.push(ele)
                }
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
