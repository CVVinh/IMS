<template>
    <layout-none-dynamic>
        <div>
            <Toast position="top-right" />
        </div>

        <body id="particles-js"></body>
        <div class="animated bounceInDown">
            <div class="container">
                <form name="form1" class="box" @submit.prevent="checkStuff()">
                    <h3>Quên mật khẩu</h3>
                    <br />
                    <InputText
                        type="text"
                        name="email"
                        v-model="username"
                        placeholder="Tên người dùng"
                        autocomplete="off"
                        style="color: #333"
                    />
                    <!-- <input type="text" name="email" v-model="username" placeholder="Usercode" autocomplete="off"> -->
                    <div class="row">
                        <div class="col-6">
                            <input type="submit" value="Xác nhận" class="btn1" />
                        </div>

                        <div class="col-6">
                            <input type="submit" value="Hủy" class="btn2" @click.prevent="backToLogin()" />
                        </div>
                    </div>
                </form>
            </div>
        </div>

        <Dialog modal="true" :closable="false" :visible="displayBasic" :style="{ opacity: 1 }">
            <ProgressSpinner />
        </Dialog>
        <Dialog modal="true" :closable="false" :visible="isDialogMessage" header="Success" :draggable="false">
            {{ message }}
            <template #footer>
                <Button label="OK" icon="pi pi-check" @click="this.backToLogin()" />
            </template>
        </Dialog>
    </layout-none-dynamic>
</template>

<script>
    import { HTTP } from '@/http-common'
    import { ToastSeverity } from 'primevue/api'
    const newLocal = this
    export default {
        name: 'forgotpass',
        data() {
            return {
                displayBasic: false,
                username: null,
                message: '',
                isDialogMessage: false,
            }
        },
        methods: {
            openBasic() {
                this.displayBasic = true
            },
            closeBasic() {
                this.displayBasic = false
            },
            async checkStuff() {
                if (this.username == null) {
                    this.$toast.add({
                        severity: ToastSeverity.WARN,
                        summary: 'Cảnh báo',
                        detail: 'Tên người dùng là bắt buộc !',
                        life: 3000,
                    })
                } else {
                    await this.Forgot()
                }
            },
            async Forgot() {
                const CallApi = async () => {
                    try {
                        this.openBasic()
                        const response = await HTTP.post('/Users/ResetPass', this.username)
                        if (response.data) {
                            this.username = null
                            this.message = response.data.message
                            this.isDialogMessage = true
                            this.closeBasic()
                        }
                    } catch (error) {
                        this.closeBasic()
                        switch (error.code) {
                            case 'ERR_NETWORK':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: 'Lỗi',
                                    detail: 'Kiểm tra kết nối !',
                                    life: 3000,
                                })
                                break
                            case 'ERR_BAD_REQUEST':
                                this.$toast.add({
                                    severity: ToastSeverity.ERROR,
                                    summary: 'Lỗi',
                                    detail: error.response.data,
                                    life: 3000,
                                })
                                break
                        }
                    }
                }
                await CallApi()
            },
            backToLogin() {
                this.isDialogMessage = false
                this.$router.push('/login')
            },
        },
    }
</script>

<style scoped>
    @import url('https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400');

    body,
    html {
        font-family: 'Source Sans Pro', sans-serif;
        background-image: url(https://wallpapercave.com/wp/wp2053403.jpg);
        background-repeat: no-repeat;
        background-size: cover;
        padding: 0;
        margin: 0;
    }

    #particles-js {
        position: absolute;
        width: 100%;
        height: 100%;
    }

    .container {
        margin-top: 10rem;
        top: 120px;
        left: 50%;
        position: absolute;
        text-align: left;
        transform: translateX(-50%);
        background-color: rgb(255, 255, 255);
        border-radius: 9px;
        border-top: 10px solid #007cc2;
        border-bottom: 10px solid #007cc2;
        width: 400px;
        height: 350px;
        box-shadow: 1px 1px 108.8px 19.2px rgb(25, 31, 53);
    }

    .box h3 {
        font-family: 'Source Sans Pro', sans-serif;
        color: #007cc2;
        font-size: 30px;
        margin-top: 50px;
        text-align: center;
    }

    .box h3 span {
        color: #dfdeee;
        font-weight: lighter;
    }

    .box h5 {
        font-family: 'Source Sans Pro', sans-serif;
        font-size: 13px;
        color: #a1a4ad;
        letter-spacing: 1.5px;
        margin-top: -15px;
        margin-bottom: 70px;
    }

    .box input[type='text'],
    .box input[type='password'] {
        display: block;
        margin: 20px auto;
        background: #ffffff;
        border-radius: 5px;
        padding: 14px 10px;
        width: 320px;
        outline: none;
        color: #d6d6d6;
        -webkit-transition: all 0.2s ease-out;
        -moz-transition: all 0.2s ease-out;
        -ms-transition: all 0.2s ease-out;
        -o-transition: all 0.2s ease-out;
        transition: all 0.2s ease-out;
    }

    ::-webkit-input-placeholder {
        color: #565f79;
    }

    .box input[type='text']:focus,
    .box input[type='password']:focus {
        border: 1px solid #79a6fe;
    }

    a {
        color: #5c7fda;
        text-decoration: none;
    }

    a:hover {
        text-decoration: underline;
    }

    label input[type='checkbox'] {
        display: none;
        /* hide the default checkbox */
    }

    /* style the artificial checkbox */
    label span {
        height: 13px;
        width: 13px;
        border: 2px solid #464d64;
        border-radius: 2px;
        display: inline-block;
        position: relative;
        cursor: pointer;
        left: 7.5%;
    }

    .btn1 {
        border: 0;
        background: #007cc2;
        color: #dfdeee;
        border-radius: 100px;
        width: 155px;
        height: 49px;
        font-size: 16px;
        position: absolute;
        top: 70%;
        left: 10%;
        transition: 0.3s;
        cursor: pointer;
        font-weight: bold;
    }

    .btn2 {
        border: 0;
        background: #007cc2;
        color: #dfdeee;
        border-radius: 100px;
        width: 155px;
        height: 49px;
        font-size: 16px;
        position: absolute;
        top: 70%;
        right: 10%;
        transition: 0.3s;
        cursor: pointer;
        font-weight: bold;
    }

    .rmb {
        position: absolute;
        margin-left: -24%;
        margin-top: 0px;
        color: #dfdeee;
        font-size: 13px;
    }

    .forgetpass {
        position: relative;
        float: right;
        right: 28px;
    }

    .dnthave {
        position: absolute;
        top: 92%;
        left: 24%;
    }

    [type='checkbox']:checked + span:before {
        /* <-- style its checked state */
        font-family: FontAwesome;
        font-size: 16px;
        content: '\f00c';
        position: absolute;
        top: -4px;
        color: #896cec;
        left: -1px;
        width: 13px;
    }

    .typcn {
        position: absolute;
        left: 339px;
        top: 282px;
        color: #3b476b;
        font-size: 22px;
        cursor: pointer;
    }

    .typcn.active {
        color: #7f60eb;
    }

    .error {
        background: #ff3333;
        text-align: center;
        width: 337px;
        height: 20px;
        padding: 2px;
        border: 0;
        border-radius: 5px;
        margin: 10px auto 10px;
        position: absolute;
        top: 31%;
        left: 7.2%;
        color: white;
        display: none;
    }

    .footer {
        position: relative;
        left: 0;
        bottom: 0;
        top: 605px;
        width: 100%;
        color: #78797d;
        font-size: 14px;
        text-align: center;
    }

    .footer .fa {
        color: #7f5feb;
    }

    .div-forgotpassword {
        display: flex;
        width: 60%;
        margin-top: 10px;
    }

    .label-forgotpassword {
        display: flex;
        width: 50%;
        height: 3rem;
        font-size: 28px;
    }

    .input-forgotpassword {
        display: flex;
        width: 70%;
        height: 3rem;
        font-size: 28px;
    }

    .div-forgotpassword {
        display: flex;
        width: 60%;
        margin-top: 25px;
        justify-content: flex-end;
    }
</style>
