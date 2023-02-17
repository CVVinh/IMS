<template>
<!-- <ConfirmDialog></ConfirmDialog> -->
<div>
    <Toast position="top-right" />
</div>

<div class="page-wrap borderless">
    <div class="login-page-broadcast">

    </div>
    <div class="container navless-container">
        <div class="content">
            <div class="flash-container flash-container-page sticky" data-qa-selector="flash_container">
            </div>
            <div class="mt-3">
                <div class="col-sm-12 gl-text-center">
                    <img alt="VHEC" class="gl-w-10 js-lazy-loaded" src="@/assets/vhec.png" loading="lazy" data-qa_selector="js_lazy_loaded_content">
                    <h1 class="mb-3 gl-font-size-h2">
                        Internal Management System
                    </h1>
                </div>
            </div>
            <div class="mb-3">
                <div class="gl-w-half gl-xs-w-full gl-ml-auto gl-mr-auto bar">
                    <div id="signin-container">
                        <div class="tab-content">
                            <div class="login-box tab-pane active" id="login-pane" role="tabpanel">
                                <div class="login-body">
                                    <form @submit.prevent="handleSubmit(!v$.$invalid)" class="new_user gl-show-field-errors js-arkose-labs-form" id="new_user" aria-live="assertive" data-testid="sign-in-form" accept-charset="UTF-8" method="post"><input type="hidden" name="authenticity_token" value="OW2mVZEycfgMJTe+myG8WKJBIHyF1droRCxwN1VgqDGFuFZxLOA8um7KeUwDLJ30oOXK4TApzBXOH8xApaldMg==" autocomplete="off">
                                        <div class="form-group gl-px-5 gl-pt-5">
                                            <label for="username" :class="{ 'p-error': v$.username.$invalid && submitted }" class="label-bold gl-mb-1">Username or
                                                email</label>

                                            <input id="username" v-model="v$.username.$model" :class="{ 'p-invalid': v$.username.$invalid && submitted }" class="form-control gl-form-input top js-username-field" autofocus="autofocus" autocapitalize="off" autocorrect="off" required="required" title="This field is required." data-qa-selector="login_field" data-testid="username-field" type="text" name="user[login]">
                                            <small v-if="(v$.username.$invalid && submitted) || v$.username.$pending.$response" class="p-error">{{
                                                        v$.username.required.$message.replace('Value',
                                                            'Username')
                                                    }}</small>

                                        </div>

                                        <div class="form-group gl-px-5">
                                            <label for="password" :class="{ 'p-error': v$.password.$invalid && submitted }" class="label-bold gl-mb-1">Password</label>
                                            <input id="password" v-model.trim="v$.password.$model" :class="{ 'p-invalid': v$.password.$invalid && submitted }" inputStyle="width : 100%" :feedback="false" toggleMask class="form-control gl-form-input bottom" autocomplete="current-password" required="required" title="This field is required." data-qa-selector="password_field" data-testid="password-field" type="password" name="user[password]">
                                            <span v-if="v$.password.$error && submitted">
                                                <span id="password-error" v-for="(error, index) of v$.password.$errors" :key="index">
                                                    <small class="p-error"> {{ error.$message }} </small><br />
                                                </span>
                                            </span>
                                            <small v-else-if="(v$.password.$invalid && submitted) || v$.password.$pending.$response" class="p-error">{{
    v$.password.required.$message.replace('Value', 'Password')
                                                    }}</small>
                                        </div>
                                        <div class="gl-px-5">
                                            <div class="gl-display-inline-block">
                                                <div class="gl-form-checkbox custom-control custom-checkbox">
                                                    <input name="user[remember_me]" type="hidden" value="0" autocomplete="off"><input class="custom-control-input" type="checkbox" value="1" name="user[remember_me]" id="user_remember_me">
                                                    <label class="custom-control-label" for="user_remember_me"><span>Remember me</span></label>
                                                </div>

                                            </div>
                                            <div class="gl-float-right">
                                                <a href="/forgotpass">Forgot your password?</a>
                                            </div>
                                        </div>
                                        <div></div>
                                        <div class="submit-container move-submit-down gl-px-5">
                                            <button name="button" type="submit" class="gl-button btn btn-block btn-confirm js-sign-in-button" data-qa-selector="sign_in_button" data-testid="sign-in-button">Sign
                                                in</button>
                                        </div>
                                    </form>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</div>
</template>

<script>
import {
    HTTP,
    reLoadHTTP
} from '@/http-common'
import {
    ToastSeverity
} from 'primevue/api'
import {
    useVuelidate
} from '@vuelidate/core'
import {
    required
} from '@vuelidate/validators'
import LayoutDefaultDynamic from '../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
import LayoutNoneDynamic from '../layouts/LayoutNone/LayoutNoneDynamic.vue'
import jwtDecode from 'jwt-decode';

export default {
    inheritAttrs: false,
    setup: () => ({
        v$: useVuelidate(),
    }),
    name: 'login',
    data() {
        return {
            username: null,
            password: null,
            submitted: false,
            wait: false,
        }
    },
    beforeCreate() {
        if (localStorage.getItem('username') && localStorage.getItem('token')) {
            this.$router.push('/')
        }
    },
    validations() {
        return {
            username: {
                required,
            },
            password: {
                required,
            },
        }
    },
    methods: {
        handleSubmit(isFormValid) {
            this.submitted = true
            if (!isFormValid) {
                return
            }
            this.Login()
        },
        Login() {
            const data = {
                username: this.username,
                password: this.password,
            }
            const CallApi = async () => {
                try {
                    this.wait = true
                    const response = await HTTP.post('Users/Login', data)
                    if (response.data.success) {
                        localStorage.setItem('username', response.data.username)
                        localStorage.setItem('token', response.data.token)
                        localStorage.setItem('IdGroup', jwtDecode(response.data.token).IdGroup)
                        reLoadHTTP()
                        this.$router.push('/')
                    }
                } catch (error) {
                    this.wait = false
                    if (this.wait == false) {
                        switch (error.code) {
                            case 'ERR_NETWORK':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: 'Error!',
                                    detail: 'Check your connect !',
                                    life: 3000,
                                })
                                break
                            case 'ERR_BAD_REQUEST':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: 'Error!',
                                    detail: error.response.data,
                                    life: 3000,
                                })
                                break
                        }
                    }
                }
            }
            CallApi()
        },
    },
    components: {
        LayoutDefaultDynamic,
        LayoutNoneDynamic
    },
}
</script>

<style lang="scss" scoped>
body {
    font-family: Sans-Serif;
}

.devise-layout-html body .page-wrap {
    min-height: 100%;
    position: relative;
}

.devise-layout-html body .login-page-broadcast {
    margin-top: 40px;
}

.devise-layout-html body .navless-container {
    padding: 0 15px 65px;
}

.devise-layout-html body .flash-container {
    padding-bottom: 65px;
}

.login-page .container {
    max-width: 960px;
}

.gl-mr-auto {
    margin-right: auto;
}

.gl-w-half {
    width: 40%;
}

.navless-container {
    margin-top: var(--header-height, 48px);
    padding-top: 32px;
}

.container {
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;
    margin-right: auto;
    margin-left: auto;
}

.container .content {
    margin: 0;
}

.login-page .flash-container {
    margin-bottom: 16px;
    position: relative;
    top: 8px;
}

.gl-text-center {
    text-align: center;
}

.gl-w-10 {
    width: 3.5rem;
}

.col-lg-12 {
    position: relative;
    width: 100%;
    padding-right: 15px;
    padding-left: 15px;
}

img {
    vertical-align: middle;
    border-style: none;
}

h1 {
    margin-bottom: 0.25rem;
    font-weight: 600;
    line-height: 1.2;
    color: #333238;
    font-size: 1.1875rem;
    margin-top: 20px;
}

.mb-3,
.my-3 {
    margin-bottom: 1rem !important;
}

.gl-ml-auto {
    margin-left: auto;
}

.tab-content {
    overflow: visible;
}

.login-page .borderless .login-box,
.login-page .borderless .omniauth-container {
    box-shadow: none;
}

.tab-content>.active {
    display: block;
}

.login-page .login-box .login-body,
.login-page .omniauth-container .login-body {
    font-size: 13px;
}

input {
    border-radius: 0.25rem;
    color: #333238;
    background-color: #fff;
}

button,
input {
    overflow: visible;
}

.login-page .submit-container {
    margin-top: 16px;
}

input,
button,
select,
optgroup,
textarea {
    margin: 0;
    font-family: inherit;
    font-size: inherit;
    line-height: inherit;
}

.gl-pt-5 {
    padding-top: 1rem;
}

.gl-px-5 {
    padding-left: 1rem;
    padding-right: 1rem;
}

.form-group {
    margin-bottom: 1rem;
}

label.label-bold {
    font-weight: 600;
}

.gl-mb-1 {
    margin-bottom: 0.125rem;
}

.gl-show-field-errors .form-control:not(textarea) {
    height: 34px;
}

.gl-display-inline-block {
    display: inline-block;
}

.gl-form-checkbox {
    font-size: 0.875rem;
    line-height: 1rem;
    color: #333238;
}

.devise-layout-html {
    margin: 0;
    padding: 0;
    height: 100%;
}

.gl-form-input,
.gl-form-input.form-control {
    background-color: #fff;
    font-family: var(--default-regular-font, -apple-system), BlinkMacSystemFont, "Segoe UI", Roboto, "Noto Sans", Ubuntu, Cantarell, "Helvetica Neue", sans-serif, "Apple Color Emoji", "Segoe UI Emoji", "Segoe UI Symbol", "Noto Color Emoji";
    font-size: 0.875rem;
    line-height: 1rem;
    padding-top: 0.5rem;
    padding-bottom: 0.5rem;
    padding-left: 0.75rem;
    padding-right: 0.75rem;
    height: auto;
    color: #333238;
    box-shadow: inset 0 0 0 1px #89888d;
    border-style: none;
    appearance: none;
    -moz-appearance: none;
    -webkit-appearance: none;
}

.gl-show-field-errors .form-control:not(textarea) {
    height: 34px;
}

.gl-show-field-errors .gl-field-error-outline {
    border: 1px solid #dd2b0e;
}

.gl-button.gl-button.btn-confirm,
.gl-button.gl-button.btn-info,
.gl-button.gl-button.btn-block.btn-confirm,
.gl-button.gl-button.btn-block.btn-info {
    background-color: #1f75cb;
    box-shadow: inset 0 0 0 1px #1068bf;
}

.custom-control {
    position: relative;
    z-index: 1;
    display: block;
    min-height: 1.5rem;
    padding-left: 1.5rem;
    -webkit-print-color-adjust: exact;
    color-adjust: exact;
}

input {
    border-radius: 0.25rem;
    color: #333238;
    background-color: #fff;
}

input[type="checkbox"] {
    box-sizing: border-box;
    padding: 0;
}

.custom-control-input {
    position: absolute;
    left: 0;
    z-index: -1;
    width: 1rem;
    height: 1rem;
}

.btn-block {
    width: 100%;
    margin: 0;
    margin-bottom: 1rem;
    color: #fff;
    margin-top: 20px;
}

.gl-float-right {
    float: right;
}

a {
    color: #1f75cb;
    text-decoration: none;
    background-color: transparent;
    transition: background-color 100ms linear, color 100ms linear, border 100ms linear;
}

.hidden {
    display: none !important;
    visibility: hidden !important;
}

.login-page .submit-container {
    margin-top: 16px;
}

@media (max-width: 575.98px) {
    .container .content {
        margin-top: 20px;
    }

    .gl-xs-w-full {
        width: 100%;
    }

    .devise-layout-html body .flash-container {
        padding-bottom: 0;
    }
}

@media (max-width: 767.98px) {
    .tab-content {
        isolation: isolate;
    }
}
</style>
