<template>
    <Dialog
        :visible="statusopen"
        :closable="false"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
        :show="resetForm()"
    >
        <div>
            <Toast position="top-right" />
        </div>

        <template #header>
            <div class="flex justify-content-center container">
                <div class="p-fluid">
                    <div class="row">
                        <h3>{{ title }}</h3>
                    </div>
                    <div class="row">
                        <div class="col-4">Ngày tạo: {{ dateCreated }}</div>
                        <div class="col-4">Ngày áp dụng : {{ applyDay }}</div>
                        <div class="col-4">Ngày hết hạn : {{ expiredDay }}</div>
                    </div>
                </div>
            </div>
        </template>

        <div class="form-demo">
            <div class="flex justify-content-center container">
                <div class="p-fluid form-detailtrule">
                    <p v-html="content"></p>
                </div>
            </div>
        </div>

        <template #footer>
            <Button label="Hủy" icon="pi pi-times" class="p-button-secondary" @click="$emit('closeDetailt')" />
        </template>
    </Dialog>
</template>

<script>
    import { required, alphaNum, numeric, between, minLength, maxLength } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import jwt_decode from 'jwt-decode'
    import { HttpStatus } from '@/config/app.config'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'

    export default {
        props: ['statusopen', 'dataRuleById'],

        data() {
            return {
                title: null,
                applyDay: new Date(),
                expiredDay: new Date(),
                content: null,
                decode: LocalStorage.jwtDecodeToken(),
                userUpdated: localStorage.getItem('username'),
                dateCreated: null,
                formFile: null,
                deleteMesg: true,

                submitted: false,
            }
        },

        methods: {
            resetForm() {
                this.title = this.dataRuleById ? this.dataRuleById.title : null
                this.applyDay = this.dataRuleById ? this.dataRuleById.applyDay : new Date()
                this.expiredDay = this.dataRuleById ? this.dataRuleById.expiredDay : new Date()
                this.dateCreated = this.dataRuleById ? this.dataRuleById.dateCreated : new Date()
                this.content = this.dataRuleById ? this.dataRuleById.content : null
                this.userUpdated = this.dataRuleById ? this.dataRuleById.userCreated : this.decode.Id
                this.formFile = this.dataRuleById ? this.dataRuleById.pathFile : null
                this.dataRuleById ? (this.deleteMesg = true) : (this.deleteMesg = false)

                //console.log("dataRuleById: "+ JSON.stringify(this.dataRuleById))
            },
        },

        mounted() {},
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

    .form-detailtrule {
        width: 100%;
        height: 100%;
    }
</style>
