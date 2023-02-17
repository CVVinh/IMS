<template>
    <!-- <header class="wrapper"> -->
    <Menubar :model="navItems">
        <template #start>

        </template>
        <template #item="{ item }">
            <router-link :class="{ 'p-disabled': item.disabled }" class="menuitem-link" :to="item.to"
                active-class="active">
                <i v-if="item.icon" class="menuitem-link-icon" :class="item.icon"></i>
                {{ item.label }}
            </router-link>
        </template>

        <template #end>
            <div class="sreach">
                <span class="p-input-icon-left">
                    <i class="pi pi-search" />
                    <InputText type="text" v-model="value3" placeholder="Search" />
                </span>
            </div>
            <div class="login">
                <div id="profile" class="profile-wrapper">
                    <div class="profile" @click="toggleMenu">
                        <i class="user-icon fas fa-user-circle"></i>
                        <span>{{ user.name }}</span>
                        <i class="down-icon pi pi-angle-down"></i>
                    </div>
                </div>
            </div>
            <Menu class="more-menu" ref="moreMenu" :model="moreMenuItems" :popup="true" appendTo="#profile"> </Menu>
        </template>
    </Menubar>
    <!-- </header> -->
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import Menubar from 'primevue/menubar'
import Menu from 'primevue/menu'
const router = useRouter()
const navItems = ref([
    {
        label: 'Dashboard',
        to: '/homePage',
    },
    {
        label: 'Project',
        to: '/layoutDefault',
    },
    {
        label: 'User',
        to: '/',
    },
])

const moreMenuItems = ref([
    {
        label: 'Account',
        items: [
            {
                label: 'Profile Settings',
                command: () => {
                    router.push('/home/profile')
                },
            },
            {
                label: 'ChangePassword',
                command: () => {
                    router.push('/home/changepassword')
                },
            },
        ],
    },
    {
        separator: true,
    },
    {
        label: 'Sign out',
        command: () => {
            router.replace('/login')
            localStorage.clear()
        },
    },
])

const moreMenu = ref()
const user = ref({
    name: localStorage.getItem('name'),
})
function checkLogin() {
    if (user.value.name == null) {
        user.value.name = 'Login'
    }
}
checkLogin()
const toggleMenu = (e) => {
    moreMenu.value.toggle(e)
}
</script>

<style lang="scss" scoped>
.p-menubar {
    width: 100%;
    height: 100%;
    margin: 0 auto;
    display: flex;
    flex: 1;
    align-items: center;
    padding: 0;
    background-color: rgb(51, 179, 255);
    color: white;
    border: none;
    border-radius: unset;
    padding-left: 20px;
    padding-right: 20px;
    // box-shadow: 0 2px 2px 0 rgb(0, 0, 0, 0.8);

    &:deep(.p-menubar-start),
    &:deep(.p-menubar-end),
    &:deep(.p-menubar-root-list),
    &:deep(.p-menuitem) {
        height: 100%;
    }

    &:deep(.p-menubar-start) {
        padding: 0 20px;
        display: flex;
        align-items: center;

        // .logo {
        //     height: 100%;
        //     display: flex;
        //     align-items: center;
        //     // color: var(--text-color-tertiary);

        //     img {
        //         height: 100%;

        //     }
        // }
    }

    &:deep(.p-menubar-root-list) {
        // flex: 1;
        font-weight: 500;

        .p-menuitem {
            // min-width: 110px;
            display: flex;
            align-items: center;

            .menuitem-link {
                height: 100%;
                padding: 0 12px;
                display: flex;
                column-gap: 15px;
                align-items: center;
                color: white;
                text-decoration: none;
                position: relative;

                &:hover {
                    background-color: var(--color-black-light-1);
                }
            }
        }
    }

    &:deep(.p-menubar-end) {
        flex: 1;
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding-left: 10px;
        padding-right: 20px;

        .sreach {
            padding: 2px;

            .p-inputtext {
                height: 30px;
                border-radius: 20px;
            }
        }

        .profile-wrapper {
            height: 100%;
            position: relative;

            .more-menu {
                top: -15px !important;
                right: 0 !important;
                bottom: unset !important;
                left: unset !important;
                margin-top: 60px;
            }
        }

        .profile {
            height: 100%;
            padding: 0 10px;
            display: flex;
            align-items: center;
            column-gap: 10px;
            font-weight: 500;
            cursor: pointer;
            user-select: none;

            // &:hover {
            //     color: var(--color-orange);
            //     background-color: var(--color-yellow);
            // }

            &>.user-icon {
                font-size: 26px;
            }

            &>.down-icon {
                font-weight: 700;
            }
        }
    }
}

.active {
    color: var(--color-secondary);
}

.active:after {
    content: '';
    width: 100%;
    position: absolute;
    bottom: 0;
    left: 0;
    border-bottom: 3px solid var(--color-black-light-1);
}
</style>
