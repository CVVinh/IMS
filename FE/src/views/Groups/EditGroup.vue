<template>
    <Dialog
        header="Edit Group"
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
                    <h1>Edit Group</h1>
                </div>
            </div>
            <form class="form-addproject" @submit.prevent="submitGroup()">
                <div class="input-layout">
                    <label
                        :class="{
                            'input-title': true,
                        }"
                        >idGroup: <span>{{ this.group.id }}</span></label
                    >
                </div>
                <div class="input-layout">
                    <label
                        :class="{
                            'p-error': v$.group.nameGroup.required.$invalid && isSubmit,
                            'input-title': true,
                        }"
                        >Name Group<span style="color: red">*</span></label
                    >
                    <InputText v-model="v$.group.nameGroup.$model" class="input" />
                    <small class="p-error" v-if="v$.group.nameGroup.required.$invalid && isSubmit">{{
                        v$.group.nameGroup.required.$message.replace('Value', 'Name group')
                    }}</small>
                </div>

                <div class="input-layout">
                    <label class="input-title">Desc </label>
                    <Textarea v-model="this.group.discription" class="input" rows="5" />
                </div>
                <div class="group-button">
                    <div>
                        <Button label="Save" type="submit" icon="pi pi-check" />{{ ' ' }}
                        <Button label="Cancel" class="p-button-secondary" v-on:click="closeDialog()" />
                    </div>
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
        props: ['group', 'isOpen'],
        setup: () => ({ v$: useVuelidate() }),
        data() {
            return {
                isSubmit: false,
                submited: true,
                allGroups: [],
                user: '',
            }
        },
        methods: {
            submitGroup() {
                this.isSubmit = true
                if (!this.v$.$invalid && this.user) {
                    HTTP.put('Group/updateGroup', this.group)
                        .then((res) => {
                            if (res.status == 200) {
                                this.$toast.add({
                                    severity: 'success',
                                    summary: 'Successfull',
                                    detail: 'Edit group successfull!',
                                    life: 3000,
                                })
                                this.$emit('setChange')
                                this.closeDialog()
                            }
                        })
                        .catch((err) => {
                            if (err.data) {
                                this.$toast.add({
                                    severity: 'error',
                                    summary: 'Error',
                                    detail: 'Edit group fail!',
                                    life: 3000,
                                })
                            } else {
                                
                            }
                        })
                }
            },

            closeDialog() {
                this.resetForm()
                this.$emit('closeDialog')
            },

            resetForm() {
                this.isSubmit = false
            },
        },

        mounted() {
            this.user = jwtDecode(localStorage.getItem('token'))
        },
        validations() {
            return {
                group: {
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
    .group-button {
        width: calc(100% - 36px);
        margin: 24px 18px 24px 18px;
        display: flex;
        justify-items: center;
        align-items: center;
        flex-direction: column;
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
</style>
<style>
    .p-dialog-header-close-icon {
        z-index: 2000;
    }
</style>
