<template>
    <layout-none-dynamic>
        <div class="page-404">
            <div class="spinner-loading" v-if="loading">
                <ProgressSpinner style="width:50px; height:50px" strokeWidth="3" animationDuration=".6s" />
            </div>
            <div class="container">
                <div class="row justify-content-between align-items-center mt-5">
                    <div class="col-lg-6">
                        <div class="img-area">
                            <img src="./../assets/404.png" alt="image">
                        </div>
                    </div>
                    <div class="col-lg-5">
                        <div class="section-text">
                            <h1 class="title">Page Not Found</h1>
                            <p class="message__title">
                                We're sorry, the page you were looking for isn't found here. The link you followed
                                may
                                either be broken or no longer exists. Please try again, or take a look at our.
                            </p>
                        </div>
                        <div class="btn-login">
                            <Button label="GO TO HOME" class="p-button-rounded" @click="goToHome()" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </layout-none-dynamic>
</template>

<script>
export default {
    data() {
        return {
            loading: false,
        };
    },
    methods: {
        goToHome() {
            this.$router.push("/");
        }
    }
};
</script>

<style lang="scss">
.page-404 {
    height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    .message__title {
        font-size: 26px;
    }

    .btn-login {
        Button {
            font-size: 26px;
        }
    }

    .p-progress-spinner {
        display: flex;
        position: fixed;
        top: 50%;
        bottom: 0;
        left: 0;
        right: 0;
    }

    .spinner-loading {
        display: flex;
        position: fixed;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        background: white;
        z-index: 9999999;
        opacity: 0.5;
    }
}
</style>
