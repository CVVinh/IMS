<template>
    <layout-default-dynamic>
        <body>
            <div class="change-password">
                <div class="form-demo">
                    <div class="flex justify-content-center container">
                        <div class="card">
                            <form @submit.prevent="handleSubmit(!v$.$invalid)" class="p-fluit">
                                <h1 class="sr-only" style="text-align: center">Đổi mật khẩu</h1>
                                <div class="illustration">
                                    <img src="../assets/changepass1.png" width="150" height="150" />
                                </div>
                                <div class="field">
                                    <div class="p-float-label">
                                        <Password
                                            id="currentpass"
                                            v-model="v$.currentpass.$model"
                                            :class="{ 'p-invalid': v$.currentpass.$invalid && submitted }"
                                            class="input-size"
                                            inputStyle="width : 100%"
                                            :feedback="false"
                                            toggleMask
                                        >
                                        </Password>

                                        <label
                                            for="currentpass"
                                            :class="{ 'p-error': v$.currentpass.$invalid && submitted }"
                                            >Mật khẩu hiện tại *</label
                                        >
                                    </div>
                                    <small
                                        v-if="
                                            (v$.currentpass.$invalid && submitted) || v$.currentpass.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.currentpass.required.$message.replace('Value', 'Current password')
                                        }}</small
                                    >
                                </div>
                                <div class="field">
                                    <div class="p-float-label">
                                        <Password
                                            id="newpass"
                                            v-model.trim="v$.newpass.$model"
                                            :class="{ 'p-invalid': v$.newpass.$invalid && submitted }"
                                            inputStyle="width : 100%"
                                            toggleMask
                                        >
                                        </Password>
                                        <label for="newpass" :class="{ 'p-error': v$.newpass.$invalid && submitted }"
                                            >Mật khẩu mới *</label
                                        >
                                    </div>
                                    <span v-if="v$.newpass.$error && submitted">
                                        <span
                                            id="newpass-error"
                                            v-for="(error, index) of v$.newpass.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error"> {{ error.$message }} </small><br />
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="(v$.newpass.$invalid && submitted) || v$.newpass.$pending.$response"
                                        class="p-error"
                                        >{{ v$.newpass.required.$message.replace('Value', 'New password') }}</small
                                    >
                                </div>
                                <div class="field">
                                    <div class="p-float-label">
                                        <Password
                                            id="confirmpass"
                                            v-model.trim="v$.confirmpass.$model"
                                            :class="{ 'p-invalid': v$.confirmpass.$invalid && submitted }"
                                            inputStyle="width : 100%"
                                            toggleMask
                                            :feedback="false"
                                        >
                                        </Password>
                                        <label
                                            for="confirmpass"
                                            :class="{ 'p-error': v$.confirmpass.$invalid && submitted }"
                                            >Xác nhận mật khẩu *</label
                                        >
                                    </div>
                                    <span v-if="v$.confirmpass.$error && submitted">
                                        <span
                                            id="confirmpass-error"
                                            v-for="(error, index) of v$.confirmpass.$errors"
                                            :key="index"
                                        >
                                            <small class="p-error">
                                                {{ error.$message.replace('Value', 'Confirm password') }}
                                            </small>
                                            <br />
                                        </span>
                                    </span>
                                    <small
                                        v-else-if="
                                            (v$.confirmpass.$invalid && submitted) || v$.confirmpass.$pending.$response
                                        "
                                        class="p-error"
                                        >{{
                                            v$.confirmpass.required.$message.replace('Value', 'Confirm password')
                                        }}</small
                                    >
                                </div>
                                <div class="button-div">
                                    <button type="submit" class="btn btn-primary" style="margin: 10px">Hoàn tất</button>
                                    <button type="button" class="btn btn-secondary" @click="backToHome()">Hủy</button>
                                </div>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </body>
    </layout-default-dynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import { helpers, minLength, required, sameAs } from '@vuelidate/validators'
    import { useVuelidate } from '@vuelidate/core'
    import { ToastSeverity } from 'primevue/api'
    export default {
        setup: () => ({
            v$: useVuelidate(),
        }),
        name: 'changepass',
        data() {
            return {
                currentpass: '',
                newpass: '',
                confirmpass: '',
                submitted: false,
            }
        },
        validations() {
            return {
                currentpass: {
                    required,
                },
                newpass: {
                    required,
                    minLenght: minLength(8),
                },
                confirmpass: {
                    minLenght: minLength(8),
                    sameAsPassword: helpers.withMessage(
                        'Xác nhận mật khẩu phải giống mật khẩu mới',
                        sameAs(this.newpass),
                    ),
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
                this.Changepass()
            },
            Changepass() {
                const data = {
                    oldpassword: this.currentpass,
                    newpassword: this.newpass,
                }
                const CallApi = async () => {
                    try {
                        const res = await HTTP.put('Users/changePassword/' + localStorage.getItem('username'), data)
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: ToastSeverity.SUCCESS,
                                summary: res.statusText,
                                detail: res.data,
                                life: 3000,
                            })
                            localStorage.clear()
                            window.location = '/login'
                        }
                    } catch (error) {
                        switch (error.code) {
                            case 'ERR_NETWORK':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: error.message,
                                    detail: 'Kiểm tra kết nối !',
                                    life: 3000,
                                })
                                break
                            case 'ERR_BAD_REQUEST':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: error.response.statusText,
                                    detail: error.response.data,
                                    life: 3000,
                                })
                                break
                        }
                    }
                }
                CallApi()
            },
            backToHome() {
                this.$router.push('/')
            },
        },
    }
</script>

<style lang="scss" scoped>
    .change-password {
        width: 60%;
        margin: auto;
        text-align: center;
    }

    .illustration {
        padding-bottom: 1rem;
    }

    .form-demo {
        margin-top: 5%;

        .card {
            min-width: 450px;

            form {
                margin-top: 2rem;
            }

            .field {
                width: 80%;
                margin: auto;
                margin-bottom: 1.5rem;
                font-size: small;
                text-align: left;

                .p-float-label {
                    width: 100%;

                    .p-password {
                        width: 100%;
                    }

                    .p-input-icon-right {
                        width: 100%;
                    }
                }
            }
        }

        @media screen and (max-width: 960px) {
            .card {
                width: 80%;
            }
        }
    }
</style>
