<template>
    <header class="navbar navbar-gitlab navbar-expand-sm js-navbar height-header-content">
        <div class="container-fluid height-header-content">
            <div class="header-content js-header-content height-header-content">
                <div
                    class="title-container hide-when-top-nav-responsive-open gl-transition-medium gl-display-flex gl-align-items-stretch gl-pt-0 gl-mr-3"
                >
                    <div class="title">
                        <span class="gl-sr-only">VHEC</span>
                        <a id="logo" @click="clickDashboard()">
                            <img
                                src="../../assets/logoReview.png"
                                style="width: 100%; height: 100%; border-radius: 5px"
                            />
                        </a>
                    </div>
                    <div class="gl-display-flex gl-align-items-center"></div>
                    <div class="gl-display-none gl-sm-display-block">
                        <ul class="list-unstyled nav navbar-sub-nav" id="js-top-nav">
                            <li class="dropdown header-projects qa-projects-dropdown ms-2">
                                <a
                                    class="header-new-dropdown-toggle has-tooltip gl-display-flex dropdown-toggle"
                                    type="button"
                                    :class="{ 'active-dropdown-btn': objChange.dropdown.Project.open }"
                                    :ref="(e) => (objChange.dropdown.Project.Target = e)"
                                    @click="HandleDropdown('Project')"
                                >
                                    Dự án
                                </a>
                                <div
                                    :if="objChange.dropdown.Project.open"
                                    class="dropdown-menu frequent-items-dropdown-menu dropdown-content-top"
                                    :class="{ 'show-dropdown': objChange.dropdown.Project.open == true }"
                                >
                                    <div class="frequent-items-dropdown-container">
                                        <div class="frequent-items-dropdown-sidebar qa-projects-dropdown-sidebar">
                                            <ul>
                                                <li>
                                                    <a class="qa-your-projects-link" href="#">
                                                        <i
                                                            class="fa-solid fa-house-user me-3"
                                                            style="font-size: 20px"
                                                        ></i>
                                                        Dự án
                                                        <span>(IMS)</span>
                                                    </a>
                                                    <div class="mt-1 project-options-task">
                                                        <div>
                                                            <a class="btn"> Thêm dự án </a>
                                                        </div>
                                                        <div>
                                                            <a class="btn"> Dự án</a>
                                                        </div>
                                                        <div>
                                                            <a class="btn"> Bảng dự án</a>
                                                        </div>
                                                    </div>
                                                </li>
                                                <li class="divider"></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </li>
                            <li class="dropdown header-projects qa-projects-dropdown ms-2">
                                <a
                                    class="header-new-dropdown-toggle has-tooltip gl-display-flex dropdown-toggle"
                                    type="button"
                                    :class="{ 'active-dropdown-btn': objChange.dropdown.Options.open }"
                                    :ref="(e) => (objChange.dropdown.Options.Target = e)"
                                    @click="HandleDropdown('Options')"
                                >
                                    Tùy chọn
                                </a>
                                <div
                                    :if="objChange.dropdown.Options.open"
                                    class="dropdown-menu frequent-items-dropdown-menu dropdown-content-top"
                                    :class="{ 'show-dropdown': objChange.dropdown.Options.open == true }"
                                >
                                    <div class="frequent-items-dropdown-container">
                                        <div class="frequent-items-dropdown-sidebar qa-projects-dropdown-sidebar">
                                            <ul>
                                                <li class="cursor-pointer">
                                                    <a class="qa-your-projects-link" @click="openDialogProject()"
                                                        >Thêm dự án
                                                    </a>
                                                </li>
                                                <li class="cursor-pointer">
                                                    <a class="qa-your-projects-link" @click="openDialogUser()"
                                                        >Thêm người dùng
                                                    </a>
                                                </li>
                                                <li class="cursor-pointer">
                                                    <a @click="clickAddTask()" class="qa-your-projects-link"
                                                        >Thêm công việc</a
                                                    >
                                                </li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </li>
                            <li class="dropdown header-projects qa-projects-dropdown ms-2">
                                <a
                                    class="header-new-dropdown-toggle has-tooltip gl-display-flex"
                                    type="button"
                                    :class="{ 'active-dropdown-btn': objChange.dropdown.Wikis.open }"
                                    :ref="(e) => (objChange.dropdown.Wikis.Target = e)"
                                    @click="HandleDropdown('Wikis')"
                                >
                                    Thông tin
                                </a>
                            </li>
                        </ul>
                    </div>
                </div>
                <div
                    class="navbar-collapse gl-transition-medium collapse gl-mr-auto global-search-container hide-when-top-nav-responsive-open ms-2"
                >
                    <ul class="nav navbar-nav gl-w-full gl-align-items-center">
                        <li class="nav-item header-search-new gl-display-none gl-lg-display-block gl-w-full">
                            <div class="header-search is-not-active gl-relative gl-w-full" id="js-header-search">
                                <form>
                                    <div class="gl-search-box-by-type">
                                        <i
                                            class="fa-sharp fa-solid fa-magnifying-glass s16 gl-search-box-by-type-search-icon gl-icon"
                                        ></i>
                                        <input
                                            autocomplete="off"
                                            class="form-control gl-form-input gl-search-box-by-type-input"
                                            id="search"
                                            :class="{ 'active-dropdown-btn': objChange.dropdown.Search.open }"
                                            :ref="(e) => (objChange.dropdown.Search.Target = e)"
                                            name="search"
                                            @click="HandleDropdown('Search')"
                                            placeholder="Search"
                                            type="text"
                                        />
                                    </div>
                                    <kbd
                                        class="gl-absolute gl-right-3 gl-top-0 keyboard-shortcut-helper gl-z-index-1 has-tooltip"
                                        title="Use the shortcut key &lt;kbd&gt;/&lt;/kbd&gt; to start a search"
                                    >
                                        /
                                    </kbd>
                                </form>
                            </div>
                        </li>
                        <li class="nav-item d-none d-sm-inline-block d-lg-none">
                            <a title="Search" aria-label="Search" href="#">
                                <i class="fa-sharp fa-solid fa-magnifying-glass s16"></i>
                            </a>
                        </li>
                    </ul>
                </div>
                <div class="navbar-collapse gl-transition-medium collapse" :class="{ isToggle: objChange.isToggle }">
                    <ul class="nav navbar-nav gl-w-full gl-align-items-center gl-justify-content-end">
                        <!--
                        <li class="header-new dropdown gl-display-none gl-sm-display-block gl-text-right">
                            <a
                                class="header-new-dropdown-toggle has-tooltip gl-display-flex dropdown-toggle"
                                type="button"
                                :class="{ 'active-dropdown-btn': objChange.dropdown.Create.open }"
                                :ref="(e) => (objChange.dropdown.Create.Target = e)"
                                @click="HandleDropdown('Create')"
                            >
                                <i class="fa-solid fa-square-plus s16" style="font-size: 16px"></i>
                            </a>

                            <div
                                :if="objChange.dropdown.Create.open"
                                class="dropdown-menu dropdown-menu-right"
                                :class="{ 'show-dropdown': objChange.dropdown.Create.open === true }"
                            >
                                <ul>
                                    <li><h6 class="dropdown-header">This project</h6></li>
                                    <li><a class="dropdown-item" href="#">New issue</a></li>
                                    <li><a class="dropdown-item" href="#">New merge request</a></li>
                                    <li><a class="dropdown-item" href="#">New snippet</a></li>
                                    <li class="divider"></li>
                                    <li><h6 class="dropdown-header">GitLab</h6></li>
                                    <li><a class="dropdown-item" href="#">New project/repository</a></li>
                                    <li><a class="dropdown-item" href="#">New group</a></li>
                                    <li><a class="dropdown-item" href="#">New snippet</a></li>
                                </ul>
                            </div>
                        </li>

                        <li class="user-counter">
                            <a title="Issues" class="dashboard-shortcuts-issues js-prefetch-document" href="#">
                                <i class="fa-solid fa-box-tissue" style="font-size: 16px"> </i>
                                <span
                                    aria-label="0 assigned issues"
                                    class="gl-badge badge badge-pill badge-success sm gl-ml-n2 gl-display-none"
                                    >0
                                </span>
                            </a>
                        </li>

                        <li class="user-counter dropdown">
                            <a
                                class="dashboard-shortcuts-merge_requests dropdown-toggle"
                                href="#"
                                title="Merge requests"
                                :class="{ 'active-dropdown-btn': objChange.dropdown.Merge.open }"
                                :ref="(e) => (objChange.dropdown.Merge.Target = e)"
                                @click="HandleDropdown('Merge')"
                            >
                                <i class="fa-solid fa-code-pull-request" style="font-size: 16px"> </i>
                                <span
                                    class="gl-badge badge badge-pill badge-warning sm js-merge-requests-count gl-ml-n2 gl-display-none"
                                    >0
                                </span>
                            </a>
                            <div
                                :if="objChange.dropdown.Merge.open"
                                class="dropdown-menu dropdown-menu-right dropdown-content-top"
                                :class="{ 'show-dropdown': objChange.dropdown.Merge.open === true }"
                            >
                                <ul>
                                    <li class="dropdown-header">Merge requests</li>
                                    <li>
                                        <a class="d-flex dropdown-item" href="#"
                                            >Assigned to you <span class="gl-badge-count">0</span></a
                                        >
                                    </li>
                                    <li>
                                        <a class="d-flex dropdown-item" href="#"
                                            >Review requests for you <span class="gl-badge-count">0</span></a
                                        >
                                    </li>
                                </ul>
                            </div>
                        </li>

                        <li class="user-counter">
                            <a title="To-Do List" class="shortcuts-todos" href="#">
                                <i class="fa-solid fa-square-check" style="font-size: 16px"></i>
                                <span class="gl-badge badge badge-pill badge-info sm js-todos-count gl-ml-n2 hidden"
                                    >0
                                </span></a
                            >
                        </li>

                        <li class="nav-item header-help dropdown d-none d-md-block">
                            <a
                                class="header-help-dropdown-toggle dropdown-toggle gl-relative"
                                data-toggle="dropdown"
                                :class="{ 'active-dropdown-btn': objChange.dropdown.Help.open }"
                                :ref="(e) => (objChange.dropdown.Help.Target = e)"
                                @click="HandleDropdown('Help')"
                            >
                                <span class="gl-sr-only"> Help </span>
                                <i class="fa-solid fa-circle-question" style="font-size: 16px"></i>
                                <span class="notification-dot rounded-circle gl-absolute"></span>
                            </a>
                            <div
                                :if="objChange.dropdown.Help.open"
                                class="dropdown-menu dropdown-menu-right dropdown-content-top"
                                :class="{ 'show-dropdown': objChange.dropdown.Help.open === true }"
                            >
                                <ul>
                                    <li>
                                        <button class="d-flex" type="button">
                                            What&#39;s new
                                            <span class="gl-badge-count">0</span>
                                        </button>
                                    </li>

                                    <li>
                                        <a href="#">Help</a>
                                    </li>
                                    <li>
                                        <a href="#">Support</a>
                                    </li>
                                    <li>
                                        <a target="_blank" class="text-nowrap" href="#">Community forum</a>
                                    </li>
                                    <li>
                                        <button class="d-flex" type="button">
                                            Keyboard shortcuts
                                            <kbd aria-hidden="true" class="gl-badge-count">?</kbd>
                                        </button>
                                    </li>
                                    <li class="divider"></li>
                                    <li>
                                        <a href="#">Submit feedback</a>
                                    </li>
                                    <li>
                                        <a target="_blank" class="text-nowrap" href="#">Contribute to GitLab </a>
                                    </li>

                                    <li>
                                        <a href="">Switch to GitLab Next</a>
                                    </li>
                                </ul>
                            </div>
                        </li>
                        -->
                        <li class="nav-item header-user js-nav-user-dropdown dropdown">
                            <a
                                class="header-user-dropdown-toggle dropdown-toggle"
                                href="#"
                                :class="{ 'active-dropdown-btn': objChange.dropdown.User.open }"
                                :ref="(e) => (objChange.dropdown.User.Target = e)"
                                @click="HandleDropdown('User')"
                            >
                                <img
                                    class="gl-avatar gl-avatar-s24 header-user-avatar gl-avatar-circle"
                                    height="24"
                                    width="24"
                                    loading="lazy"
                                    src="https://upload.wikimedia.org/wikipedia/commons/9/99/Sample_User_Icon.png"
                                />
                            </a>
                            <div
                                :if="objChange.dropdown.User.open"
                                class="dropdown-menu dropdown-menu-right dropdown-content-top"
                                :class="{ 'show-dropdown': objChange.dropdown.User.open === true }"
                            >
                                <ul>
                                    <li class="current-user">
                                        <a class="gl-line-height-20!" href="#">
                                            <div class="gl-font-weight-bold">{{ userName }}</div>
                                        </a>
                                    </li>
                                    <li class="divider"></li>
                                    <li v-for="(item, index) in moreMenuItemsUser" :key="index">
                                        <router-link :to="item.path"> {{ item.label }}</router-link>
                                    </li>
                                    <li class="divider d-md-none"></li>
                                    <li v-for="(item, index) in moreMenuItemsSupport" :key="index" class="d-md-none">
                                        <router-link :to="item.path" :target="item.target" :class="item.class">
                                            {{ item.label }}</router-link
                                        >
                                    </li>
                                    <li class="divider"></li>
                                    <li class="cursor-pointer">
                                        <a class="sign-out-link" @click="() => HandlerLogout()">Đăng xuất</a>
                                    </li>
                                </ul>
                            </div>
                        </li>
                    </ul>
                </div>
                <button
                    class="navbar-toggler d-block d-sm-none gl-border-none!"
                    type="button"
                    @click="() => HandleToggle()"
                >
                    <span class="sr-only">Toggle navigation</span>
                    <span class="more-icon gl-px-3 gl-font-sm gl-font-weight-bold" v-if="objChange.isToggle === false">
                        <span class="gl-pr-2">Menu</span>
                        <i class="fa-solid fa-bars" style="font-size: 16px"></i>
                    </span>
                    <span v-else class="more-icon gl-px-3 gl-font-sm gl-font-weight-bold">
                        <i class="fa-sharp fa-solid fa-xmark" style="font-size: 16px"></i>
                    </span>
                </button>
            </div>
        </div>
    </header>
    <DialogAddEdit
        :isOpenDialog="DialogProject.isOpenProject"
        :projectSelected="{ ...DialogProject.projectSelected }"
        @closeDialog="closeDialogProject()"
        :user="DialogProject.user"
        :leader="DialogProject.leader"
    />
    <AddUserDiaLog
        :statusopen="DialogUser.isOpenUser"
        @closeAdd="closeDialogUser()"
        :roleoption="DialogUser.listRole"
    />
</template>

<script setup>
    import { ref, reactive } from 'vue'
    import { useRouter } from 'vue-router'
    import { HTTP } from '@/http-common'
    import { HttpStatus } from '@/config/app.config'
    import { ProjectDto } from '@/views/Project/Project.dto'
    import DialogAddEdit from '@/views/Project/DialogAddEdit.vue'
    import AddUserDiaLog from '@/views/Users/AddUserDiaLog.vue'
    const router = useRouter()

    let DialogProject = reactive({
        isOpenProject: false,
        users: [],
        leader: [],
        projectSelected: new ProjectDto(),
    })

    let DialogUser = reactive({
        isOpenUser: false,
        listRole: [],
    })

    let objChange = reactive({
        isToggle: false,
        dropdown: {
            Create: { open: false, Target: {} },
            Merge: { open: false, Target: {} },
            Help: { open: false, Target: {} },
            User: { open: false, Target: {} },
            Project: { open: false, Target: {} },
            Search: { open: false, Target: {} },
            Options: { open: false, Target: {} },
            Wikis: { open: false, Target: {} },
        },
    })

    function openDialogProject() {
        DialogProject.isOpenProject = true
        DialogProject.projectSelected = []
    }

    function closeDialogProject() {
        DialogProject.isOpenProject = false
        DialogProject.projectSelected = []
    }

    function openDialogUser() {
        DialogUser.isOpenUser = true
    }

    function closeDialogUser() {
        DialogUser.isOpenUser = false
    }

    const moreMenuItemsUser = ref([
        {
            label: 'Cập nhật tài khoản',
            path: '/profile',
        },
        {
            label: 'Thay đổi mật khẩu',
            path: '/changepass',
        },
    ])

    const moreMenuItemsSupport = ref([
        {
            label: 'Help',
            path: '#',
            target: '',
            class: '',
        },
        {
            label: 'Support',
            path: '#',
            target: '',
            class: '',
        },
        {
            label: 'Community forum',
            path: '#',
            target: '_blank',
            class: 'text-nowrap',
        },
        {
            label: 'Submit feedback',
            path: '#',
            target: '',
            class: '',
        },
        {
            label: 'Contribute to GitLab ',
            path: '#',
            target: '_blank',
            class: 'text-nowrap',
        },
    ])

    const user = ref({
        name: localStorage.getItem('name'),
    })
    const userName = localStorage.getItem('username').toLocaleLowerCase()
    const search = ''
    function checkLogin() {
        if (user.value.name == null) {
            user.value.name = 'Login'
        }
    }
    checkLogin()

    function clickAddTask() {
        router.push({ name: 'addtask' })
    }

    function clickDashboard() {
        router.replace('/')
    }

    const HandlerLogout = async () => {
        const response = await HTTP.post('Users/Logout', localStorage.getItem('username'))
        if (response.status === HttpStatus.OK) {
            localStorage.clear()
            window.location = '/login'
        }
    }

    const HandleToggle = () => {
        objChange.isToggle = !objChange.isToggle
    }

    function HandleDropdown(option) {
        const closeListerner = (e) => {
            if (CatchOutsideClick(e, option, objChange.dropdown[option])) {
                window.removeEventListener('click', closeListerner)
                objChange.dropdown[option].open = false
            }
        }
        window.addEventListener('click', closeListerner)
        objChange.dropdown[option].open = !objChange.dropdown[option].open
    }

    function CatchOutsideClick(event, option, dropdown) {
        if (dropdown.Target == event.target) return false
        if (objChange.dropdown[option].open && dropdown.Target != event.target) return true
    }
</script>
<script>
    import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'
    export default {
        data() {
            return { noti: [] }
        },
        created() {
            const connection = new HubConnectionBuilder()
                .withUrl('http://localhost:5001/NotificationHub')
                .configureLogging(LogLevel.Information)
                .build()
            connection.on('ReceiveLeaveOff', () => {
                this.toastSuccess('Thành Công !!!')
                this.getNotification()
            })
            connection.start()
        },
        methods: {
            getNotification() {
                HTTP.get('Notification').then((res) => {
                    this.noti = res.data
                    console.log(this.noti)
                })
            },
            toastSuccess(message) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: message,
                    life: 3000,
                })
            },
        },
    }
</script>
<style scoped>
    @import '../../styles/Css/header.css';
    .isToggle {
        display: flex !important;
        flex: 1 1 auto !important;
    }
    .show-dropdown {
        display: block !important;
    }
    .dropdown-menu-right {
        right: 0 !important;
        left: auto !important;
    }
    .dropdown-content-top {
        margin-top: 12px !important;
    }
    .gl-badge-count {
        border-radius: 10rem;
        margin-left: auto;
        padding-top: 0;
        padding-bottom: 0;
        background-color: #dcdcde;
        color: #535158;
        width: 15px;
        justify-content: center;
        display: inline-flex;
    }
    .active-dropdown-btn {
        color: black !important;
        background-color: #fff !important;
        pointer-events: none !important;
    }
    .cursor-pointer {
        cursor: pointer;
    }
    .project-options-task {
        padding: 10px 12px;
        display: flex;
        justify-content: space-between;
    }
    .height-header-content {
        height: 42px;
    }
</style>
