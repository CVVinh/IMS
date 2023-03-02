<template>
    <Dialog
        :visible="statusopen"
        :closable="false"
        :modal="true"
        :maximizable="true"
        :dismissableMask="true"
        :show="resetForm()"
        :breakpoints="{'960px': '75vw', '640px': '100vw'}" 
        :style="{width: '50vw'}"
    >
        <Toast position="top-right" />

        <template #header>
            <div class="container">
                <div class="row">
                    <div class="col">
                        <h3>{{ title }}</h3>
                    </div>
                </div>
                
            </div>
        </template>

        <div class="container detail_content">
            <div class="row detail_content_box">
                <div class="col-4"><b><i class="pi pi-calendar-plus"></i> Ngày tạo:</b> {{ dateCreated }}</div>
                <div class="col-4"><b><i class="pi pi-calendar-minus"></i> Ngày áp dụng:</b> {{ applyDay }}</div>
                <div class="col-4"><b><i class="pi pi-calendar-times"></i> Ngày hết hạn:</b> {{ expiredDay }}</div>
            </div>
            <div class="row detail_content_box">
                <div class="col">
                    <p v-html="content"></p>
                </div>
            </div>
        </div>

        <template #footer>
            <Button label="Hủy" icon="pi pi-times" class="p-button-secondary" @click="$emit('closeDetailt')" autofocus/>
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
        name: 'RuleInfoDetail',
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

    .detail_content {
        // display: flex;
        // justify-content: space-between;
        padding: 20px 40px 10px 40px;
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;
    }

    .detail_content_box {
        display: flex;
        padding: 20px;
        border-radius: 15px;
        margin-bottom: 18px;
        box-shadow: rgba(6, 24, 44, 0.5) 0px 0px 5px 2px ;
    }

</style>
