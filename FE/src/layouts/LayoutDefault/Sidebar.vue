<template>
    <Toast />
    <nav id="sidebarMenu" class="collapse d-lg-block sidebar collapse bg-white">
        <div class="position-sticky">
            <div class="list-group list-group-flush mx-3 m-t-5" id="NavItems">
                <!-- Tổng quan -->
                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex"
                    data-bs-toggle="collapse"
                    data-bs-target="#overview-collapse"
                    aria-expanded="false"
                    :class="{ 'sidebar-top-level-items': isActive1 }"
                    @click="onActive(1)"
                    style="cursor: pointer"
                >
                    <a style="width: 80%">
                        <i class="bx bx-notepad"></i>
                        <span>Tổng quan</span>
                    </a>
                </div>
                <div class="collapse" id="overview-collapse">
                    <ul class="dropdownList btn-toggle-nav list-unstyled">
                        <li class="list-group-item-action mt-1">
                            <router-link
                                :to="{ name: 'project', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag1'"
                                :class="{ 'active-nav-item': activeTag === 'tag1' }"
                            >
                                <i class="bx bx-notepad"></i>
                                <span>Dự án</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'users', params: {} }"
                                class="py-2 ripple dropdown-item"
                                @click="activeTag = 'tag2'"
                                :class="{ 'active-nav-item': activeTag === 'tag2' }"
                            >
                                <i class="bx bx-user"></i><span>Người dùng</span>
                            </router-link>
                        </li>
                    </ul>
                </div>
                <!-- /Tổng quan -->
                <!-- Thiết bị -->
                <router-link
                    :to="{ name: 'Equipment-Device', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag4'"
                    :class="{ 'active-nav-item': activeTag === 'tag4' }"
                    v-if="this.token ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                >
                    <i class="bx bx-devices"></i>
                    <span>Thiết bị</span>
                </router-link>
                <!-- /Thiết bị -->
                <!-- Nghỉ phép -->

                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex"
                    data-bs-toggle="collapse"
                    data-bs-target="#settings-collapse-leaveOff"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 3px"
                    :class="{ 'sidebar-top-level-items': isActiveLeaveOff }"
                    @click="onActive(4)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-cog"></i>
                        <span>Nghỉ phép</span>
                    </a>
                </div>
                <div class="collapse" id="settings-collapse-leaveOff">
                    <ul class="dropdownList btn-toggle-nav list-unstyled">
                        <li
                            class="list-group-item-action"
                            v-if="this.showLink.leaveoffStaff"
                        >
                            <router-link
                                :to="{ name: 'LeaveOffRegisterlists', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag20'"
                                :class="{ 'active-nav-item': activeTag === 'tag20' }"
                            >
                                <i class="bx bx-log-out-circle"></i>
                                <span>Đăng ký nghỉ phép</span>
                            </router-link>
                        </li>
                        <li
                            class="list-group-item-action"
                            v-if="this.showLink.leaveoffAdminPm"
                        >
                            <router-link
                                :to="{ name: 'Acceptregisterlists', params: {} }"
                                class="py-2 ripple list-group-item-action"
                                @click="activeTag = 'tag21'"
                                :class="{ 'active-nav-item': activeTag === 'tag21' }"
                            >
                                <i class="bx bx-log-out-circle"></i>
                                <span>Xác nhận nghỉ phép</span>
                            </router-link>
                        </li>
                        <li
                            class="list-group-item-action"
                            v-if="this.showLink.leaveoffSample"
                        >
                        <router-link
                            :to="{ name: 'Leaveoff', params: {} }"
                            class="py-2 ripple list-group-item-action"
                            @click="activeTag = 'tag22'"
                            :class="{ 'active-nav-item': activeTag === 'tag22' }"
                        >
                            <i class="bx bx-log-out-circle"></i>
                            <span>Tổng hợp nghỉ phép</span>
                        </router-link>
                         </li>
                    </ul>
                </div>
                <!-- /Nghỉ phép -->
                <!-- Tăng ca -->
                <router-link
                    :to="{ name: 'otsSub', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag6'"
                    :class="{ 'active-nav-item': activeTag === 'tag6' }"
                >
                    <i class="bx bx-time-five"></i>
                    <span>Tăng ca</span>
                </router-link>
                <!-- /Tăng ca -->
                <!-- Đánh giá -->
                <router-link
                    :to="{ name: 'reviews', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag16'"
                    :class="{ 'active-nav-item': activeTag === 'tag16' }"
                >
                    <i class="bx bx-glasses-alt"></i>
                    <span>Đánh giá</span>
                </router-link>
                <!-- /Đánh giá -->
                <!-- Công tác -->
                <!-- <router-link
                    :to="{ name: 'remotes', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag7'"
                    :class="{ 'active-nav-item': activeTag === 'tag7' }"
                >
                    <i class="bx bx-notepad"></i>
                    <span>Công tác</span>
                </router-link> -->
                <!-- /Công tác -->
                <!-- Thu chi -->
                <router-link
                    :to="{ name: 'Paid', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag8'"
                    :class="{ 'active-nav-item': activeTag === 'tag8' }"
                >
                    <i class="bx bx-wallet"></i>
                    <span>Thu chi</span>
                </router-link>
                <!-- /Thu chi -->
                <!-- Quy định -->
                <router-link
                    :to="{ name: 'ruleinfo', params: {} }"
                    class="py-2 ripple list-group-item-action"
                    @click="activeTag = 'tag9'"
                    :class="{ 'active-nav-item': activeTag === 'tag9' }"
                >
                    <i class="bx bxs-notepad"></i>
                    <span>Quy định</span>
                </router-link>
                <!-- /Quy định -->
                <!--  Báo cáo-->
                <!-- <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex"
                    data-bs-toggle="collapse"
                    data-bs-target="#report-collapse"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 5px"
                    :class="{ 'sidebar-top-level-items': isActive2 }"
                    @click="onActive(2)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-notepad"></i>
                        <span>Báo cáo</span>
                    </a>
                </div>
                <div class="collapse" id="report-collapse">
                    <ul class="dropdownList btn-toggle-nav list-unstyled">
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'listprojects', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag10'"
                                :class="{ 'active-nav-item': activeTag === 'tag10' }"
                            >
                                <i class="bx bx-spreadsheet"></i>
                                <span>Bảng thời gian</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'projectDetails', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag11'"
                                :class="{ 'active-nav-item': activeTag === 'tag11' }"
                            >
                                <i class="bx bx-notepad"></i>
                                <span>Chi tiết dự án</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'TaskReport', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag12'"
                                :class="{ 'active-nav-item': activeTag === 'tag12' }"
                            >
                                <i class="bx bxs-report"></i><span>Báo cáo công việc</span>
                            </router-link>
                        </li>
                    </ul>
                </div> -->
                <!--  /Báo cáo-->
                <!-- Thiết lập -->
                <div
                    class="py-2 ripple sidebar_after list-group-item-action d-flex"
                    data-bs-toggle="collapse"
                    data-bs-target="#settings-collapse"
                    aria-expanded="true"
                    style="cursor: pointer; margin-bottom: 3px"
                    :class="{ 'sidebar-top-level-items': isActive3 }"
                    @click="onActive(3)"
                >
                    <a style="width: 80%">
                        <i class="bx bx-cog"></i>
                        <span>Thiết lập</span>
                    </a>
                </div>
                <div class="collapse" id="settings-collapse">
                    <ul class="dropdownList btn-toggle-nav list-unstyled">
                        <li
                            class="list-group-item-action"
                            v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                        >
                            <router-link
                                :to="{ name: 'groupsSubs', params: {} }"
                                class="py-2 ripple dropdown-item"
                                @click="activeTag = 'tag3'"
                                :class="{ 'active-nav-item': activeTag === 'tag3' }"
                            >
                                <i class="bx bxs-group"></i>
                                <span>Nhóm quyền</span>
                            </router-link>
                        </li>
                        
                        <li
                            class="list-group-item-action"
                            v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                        >
                            <router-link
                                :to="{ name: 'menu', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag13'"
                                :class="{ 'active-nav-item': activeTag === 'tag13' }"
                            >
                                <i class="bx bx-list-ul"></i>
                                <span>Menu</span>
                            </router-link>
                        </li>

                        <li
                            class="list-group-item-action"
                            v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                        >
                            <router-link
                                :to="{ name: 'modulesSub', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag14'"
                                :class="{ 'active-nav-item': activeTag === 'tag14' }"
                            >
                                <i class="bx bxs-component"></i>
                                <span>Chức năng</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'PermissionSub', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag15'"
                                :class="{ 'active-nav-item': activeTag === 'tag15' }"
                                v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                            >
                                <i class="bx bx-badge-check"></i>
                                <span>Phân quyền</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'permissionUserGroupSub', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag16'"
                                :class="{ 'active-nav-item': activeTag === 'tag16' }"
                                v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                            >
                                <i class="bx bx-badge-check"></i>
                                <span>module_group</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'Action-list', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag17'"
                                :class="{ 'active-nav-item': activeTag === 'tag17' }"
                                v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                            >
                                <i class="bx bx-badge-check"></i>
                                <span>QL Thao tác</span>
                            </router-link>
                        </li>
                        <li class="list-group-item-action">
                            <router-link
                                :to="{ name: 'perrmission-action-module', params: {} }"
                                class="py-2 ripple dropdown-item"
                                aria-current="true"
                                @click="activeTag = 'tag18'"
                                :class="{ 'active-nav-item': activeTag === 'tag18' }"
                                v-if="this.token !== null ? (Number(this.token.IdGroup) === 1 ? true : false) : false"
                            >
                                <i class="bx bx-badge-check"></i>
                                <span>QL Action_Module</span>
                            </router-link>
                        </li>

                        
                    </ul>
                </div>
                <!-- /Thiết lập -->
                <!-- <a class="py-2 ripple list-group-item-action"><i class="bx bx-note"></i><span>Thông tin</span></a> -->
                <!-- <a class="py-2 ripple list-group-item-action"
                    ><i class="bx bxs-credit-card"></i>
                    <span>Chi tiêu</span>
                </a> -->
            </div>
        </div>
    </nav>
    <section class="home">
        <div class="text">
            <slot> </slot>
        </div>
    </section>
</template>

<script>
    import { HTTP, GET_GROUP_BY_ID } from '@/http-common'
    import { Module } from '@/views/Menus'
    import jwt_decode from 'jwt-decode'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    export default {
        name: 'home',
        data() {
            return {
                moduleCurrent: '',
                menus: ['this is item default'],
                modules: [],
                idModule: '',
                submenu: [],
                isLoading: true,
                module: [],
                isActive1: false,
                isActive2: false,
                isActive3: false,
                isActiveLeaveOff : false,
                dataGrounp: [],
                path: null,
                token: LocalStorage.jwtDecodeToken(),
                activeTag: null,
                dopdowntag: null,
                showLink : {
                    leaveoffAdminPm : false,
                    leaveoffStaff : false,
                    leaveoffSample : false,
                }
            }

        },
        async created() {
            await this.getRole()
        },
        mounted() {
            this.checkGroup();
            this.activeTag = localStorage.getItem('activeTag')
        },
        watch: {
            activeTag(newValue) {
                localStorage.setItem('activeTag', newValue)
            },
        },
        methods: {
            onActive(dropdownNumber) {
                if (dropdownNumber === 1) {
                    this.isActive1 = !this.isActive1
                } else if (dropdownNumber === 2) {
                    this.isActive2 = !this.isActive2
                } else if (dropdownNumber === 3) {
                    this.isActive3 = !this.isActive3
                } else if (dropdownNumber === 4){
                    this.isActiveLeaveOff = !this.isActiveLeaveOff
                }
            },
            checkRole(id) {
                return Number(id)
            },
            async getRole() {
                HTTP.get(GET_GROUP_BY_ID(Number(this.token.IdGroup))).then((res) => {
                    this.dataGrounp = res.data
                })
            },
            getListMenuByIdModule(id) {
                HTTP.get(`Menu/getMenuByModule?moduleId=${id}`).then((res) => {
                    if (res.status == 200) {
                        this.menus = res.data
                        const temp = []
                        this.menus.forEach((menu) => {
                            if (menu.parent != 0 && menu.isDeleted != 1) {
                                temp.push(menu)
                            }
                        })
                        this.menus = temp
                    }
                })
            },
            getListModule() {
                HTTP.get('Modules/getListModule').then((res) => {
                    if (res.status == 200) {
                        this.modules = res.data

                        this.modules.forEach((module) => {
                            if (module.nameModule == this.moduleCurrent && module.isDeleted == 0) {
                                this.module.push(module)
                            }
                        })
                        if (this.module.length > 0) {
                            this.getListMenuByIdModule(this.module[0].id)
                        } else {
                            this.menus = []
                        }
                        this.isLoading = false
                    }
                })
            },
            Logout() {
                const CallApi = async () => {
                    try {
                        const access_token = localStorage.getItem('token')
                        const response = await HTTP.post(
                            'Users/Logout',
                            JSON.stringify(localStorage.getItem('username')),
                        )
                        if (response.status == 200) {
                            localStorage.removeItem('username')
                            localStorage.removeItem('token')
                            this.$router.push('/login')
                        }
                    } catch (error) {
                        switch (error.response.status) {
                            case 403:
                                break
                        }
                    }
                }
                CallApi()
            },
            checkGroup(){
                var listGroupUser = Object.values(this.token.listGroup);
                if(listGroupUser.includes("1") || listGroupUser.includes("5")) {
                    this.showLink.leaveoffAdminPm = true;
                }
                if(listGroupUser.includes("2") ) {
                    this.showLink.leaveoffSample = true;
                }
                if(listGroupUser.includes("3") || listGroupUser.includes("4")) {
                    this.showLink.leaveoffStaff = true;
                }
            },
            confirmLogout() {
                this.$confirm.require({
                    message: 'Do you want to logout?',
                    header: 'Logout Confirmation',
                    icon: 'pi pi-info-circle',
                    accept: () => {
                        this.Logout()
                    },
                    reject: () => {
                        return
                    },
                })
            },
        },
    }
</script>

<style>
    .active-nav-item {
        color: #0d6efd !important;
        text-decoration: none;
        background: rgba(0, 0, 0, 0.04);
    }
    .sidebar_after::after {
        content: '\f0d7';
        font-family: FontAwesome;
        font-style: normal;
        font-weight: normal;
        text-decoration: inherit;
        justify-content: end;
        width: 20%;
        display: flex;
        color: rgb(149 147 147);
        margin-right: 13px;
    }
    .m-t--4 {
        margin-top: 0.5rem !important;
    }
    .home {
        margin-top: 57px;
        margin-left: 212px;
    }

    /* Sidebar */
    .sidebar {
        position: fixed;
        top: -15px;
        bottom: 0;
        left: 0;
        padding: 58px 0 0;
        /* Height of navbar */
        border-radius: 2px;
        box-shadow: 0 0 3px 0 rgba(0, 0, 0, 0.12), 0 2px 3px 0 rgba(0, 0, 0, 0.22);
        z-index: 99;
        min-width: 215px;
    }

    @media (max-width: 991.98px) {
        .sidebar {
            width: 100%;
        }
    }

    .sidebar .active {
        border-radius: 5px;
        box-shadow: 0 2px 5px 0 rgb(0 0 0 / 16%), 0 2px 10px 0 rgb(0 0 0 / 12%);
    }

    .sidebar-sticky {
        position: relative;
        top: 0;
        height: calc(100vh - 48px);
        padding-top: 0.5rem;
        overflow-x: hidden;
        overflow-y: auto;
        /* Scrollable contents if viewport is shorter than content. */
    }

    .py-2 {
        text-decoration: none;
        color: black;
    }

    .py-2 i {
        margin-right: 5px;
        margin-left: 5px;
    }

    .dropdown-item {
        padding: 0.25rem 0.5rem !important;
    }

    .dropdownList li a span {
        margin-left: 15px;
    }

    .dropdownList li a i {
        margin-left: 15px;
    }

    .dropdownList li {
        margin-bottom: 7px;
        height: 25px;
        display: flex;
        align-items: center;
    }

    .list-group-item-action:focus,
    .list-group-item-action:hover {
        z-index: 1;
        color: #0d6efd;
        text-decoration: none;
        background: rgba(0, 0, 0, 0.04);
    }

    .list-group-item-action:active {
        background: rgba(0, 0, 0, 0.04);
    }

    .sidebar-top-level-items {
        background: rgba(0, 0, 0, 0.04);
    }

    .dropdown-item.active,
    .dropdown-item:active {
        color: #0d6efd;
        text-decoration: none;
        background: rgba(0, 0, 0, 0.04);
    }
</style>
