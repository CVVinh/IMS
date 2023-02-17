<template>
    <layout-fill-dynamic>
        <sideBar />
        <div class="spinner-loading" v-if="loading">
            <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="3" animationDuration=".6s" />
        </div>
        <Toast />
        <!-- <div class="padd">
            <div class="col-12 row">
                <div v-for="(menu, index) in Menus" class="col-xl-2 col-lg-4 col-md-6 p-2 item" @click="">
                    <router-link style="text-decoration: none; width: min-content" :to="`/${menu.controller}`">
                        <div :class="`card text-white ${this.color[index % 7]}`">
                            <div class="card-body">
                                <h2 class="text-center">{{ menu.title }}</h2>
                                <hr />
                                <div class="icon">
                                    <i :class="menu.icon"></i>
                                </div>
                            </div>
                        </div>
                    </router-link>
                </div>
            </div>
        </div> -->
    </layout-fill-dynamic>
</template>
<script>
    import sideBar from '@/layouts/LayoutDefault/Sidebar.vue'
    import { HTTP } from '@/http-common'
    import ViewMenusVue from './Menus/viewsMenu/ViewMenus.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    export default {
        name: 'home',
        data() {
            return {
                loading: true,
                Menus: [],
                color: [
                    'bg-primary',
                    'bg-warning',
                    'bg-info',
                    'background-color-cyan-600',
                    'background-color-yellow-800',
                    'background-color-green-600',
                    'background-color-blue-gray-500',
                ],
            }
        },
        components: {
            ViewMenusVue,
            sideBar,
        },
        methods: {
            getRoles() {
                HTTP.get('Group/getListGroup').then((res) => {
                    this.listRole = res.data
                })
            },
            addTask() {
                this.$router.push({ name: 'addtask' })
            },
            getListMenuParent() {
                HTTP.get('Menu/getListMenuParent').then((res) => {
                    if (res.status === 200) {
                        this.Menus = res.data
                        this.Menus.forEach((Menu, index) => {
                            if (Menu.isDeleted === 1) {
                                this.Menus.splice(index, 1)
                            }
                        })
                        this.loading = false
                    }
                })
            },
        },
        async mounted() {
            this.getListMenuParent()
            await this.getRoles()
            await UserRoleHelper.getUserRole(this.user, this.leader)
        },
    }
</script>
<style lang="scss" scoped>
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
    ::v-deep(.p-panel-header) {
        display: none;
    }
    ::v-deep(.p-panel-content) {
        border: none;
        background-color: var(--gray-300);
        &:hover {
            background: var(--yellow-100);
            .show-hover {
                display: block;
            }
            .hide-hover {
                display: none;
            }
            .icon-panel {
                color: var(--green-300);
            }
        }
    }
    .panel {
        box-shadow: 1px 5px 1px 10px var(--gray-100);
    }
    .panel-left-right {
        border: 1px solid var(--gray-400);
        background-color: var(--gray-300);
        cursor: pointer;
        padding: 0;
    }
    .panel-center {
        border-top: 1px solid var(--gray-400);
        border-bottom: 1px solid var(--gray-400);
        background-color: var(--gray-300);
        cursor: pointer;
        padding: 0;
    }
    .header-panel {
        margin-top: 10px;
    }
    .content-panel {
        margin-left: 10px;
    }
    .footer-panel {
        margin-left: 6px;
        margin-bottom: 20px;
        height: 30px;
    }
    .show-hover {
        display: none;
    }
    .hide-hover {
        display: block;
    }
    .icon-panel {
        font-size: 5rem;
        color: var(--gray-400);
    }

    .font-size-check {
        font-size: 1.5rem;
    }
    .panel-center:hover {
        background: var(--yellow-100);
        .show-hover {
            display: block;
        }
        .hide-hover {
            display: none;
        }
        .icon-panel {
            color: var(--green-300);
        }
    }
    .panel-left-right:hover {
        background: var(--yellow-100);
        .show-hover {
            display: block;
        }
        .hide-hover {
            display: none;
        }
        .icon-panel {
            color: var(--green-300);
        }
    }

    .card {
        height: 17rem;
        border-radius: 20px;
    }

    .card-body {
        height: 25rem;
        display: flex;
        flex-direction: column;
    }

    .icon {
        display: flex;
        flex: 1;
        justify-content: center;
        align-items: center;
    }

    .icon i {
        font-size: 8rem;
    }

    .padd {
        padding: 10px 140px;
        width: 100%;
        display: flex;
        flex-wrap: wrap;
        align-items: ali;
        justify-content: center;
        column-gap: 12px;
        row-gap: 12px;
    }

    .height-h2 {
        height: 34px;
    }

    .background-color-cyan-600 {
        background-color: var(--cyan-600);
    }

    .background-color-yellow-800 {
        background-color: var(--yellow-800);
    }

    .background-color-green-600 {
        background-color: var(--green-600);
    }

    .background-color-blue-gray-500 {
        background-color: var(--bluegray-500);
    }
    @media only screen and (max-width: 500px) {
        .padd {
            padding: 10px 60px;
        }
    }
</style>
