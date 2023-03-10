import { createRouter, createWebHistory } from 'vue-router'
import jwt_decode from 'jwt-decode'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: () => import('@/views/Home.vue'),
        },
        /* #region Project */
        {
            path: '/project',
            name: 'project',
            meta: {
                title: 'IMS - Project',
            },
            component: () => import('@/views/Project/Project.vue'),
        },
        {
            path: '/project/detailproject/:id',
            name: 'detailproject',
            meta: {
                title: 'IMS - Project',
            },
            props: true,
            component: () => import('@/views/Project/DetailProject.vue'),
        },
        {
            path: '/project/project',
            name: 'projectSub',
            meta: {
                title: 'IMS - Project',
            },
            component: () => import('@/views/Project/Project.vue'),
        },
        {
            path: '/project/addtask',
            name: 'addtaskSub',
            meta: {
                title: 'IMS - Tasks',
            },
            component: () => import('@/views/Tasks/AddTasks.vue'),
        },
        {
            path: '/ots',
            name: 'otsSub',
            meta: {
                title: 'IMS - Ots',
            },
            component: () => import('@/views/Overtime/OTs.vue'),
        },
        // {
        //     path: '/project/add',
        //     name: 'addproject',
        //     meta: {
        //         title: 'IMS - Project',
        //     },
        //     component: () => import('@/views/Project/AddProject.vue'),
        // },
        {
            path: '/project/edit/:id',
            name: 'editProject',
            meta: {
                title: 'IMS - Project',
            },
            props: true,
            component: () => import('@/views/Project/EditProject.vue'),
        },
        /* #endregion */
        /* #region Users */
        {
            path: '/users',
            name: 'users',
            meta: {
                title: 'IMS - Users',
            },
            component: () => import('@/views/Users/Users.vue'),
        },

        {
            path: '/edituser/:id',
            name: 'edituser',
            props: true,
            component: () => import('@/views/Users/EditUser.vue'),
        },
        {
            path: '/users/users',
            name: 'usersSub',
            meta: {
                title: 'IMS - Users',
            },
            component: () => import('@/views/Users/Users.vue'),
        },
        {
            path: '/users/groups',
            name: 'groupsSub',
            meta: {
                title: 'IMS - Groups',
            },
            component: () => import('@/views/Groups/Groups.vue'),
        },
        {
            path: '/permissionUserMenu',
            name: 'PermissionSub',
            meta: {
                title: 'IMS - Permission UserMenu',
            },
            component: () => import('@/views/PermissionUserMenu/PermissionUserMenu.vue'),
        },

        {
            path: '/adduser',
            name: 'adduser',
            meta: {
                title: 'IMS - Users',
            },
            component: () => import('@/views/Users/AddUser.vue'),
        },
        {
            path: '/users/permissionUserGroup',
            name: 'permissionUserGroupSub',
            meta: {
                title: 'IMS - Permission UserGroup',
            },
            component: () => import('@/views/permissionUserGroup/permissionUserGroup.vue'),
        },
        /* #endregion */
        /* #region ot-remote */

        {
            path: '/ots',
            name: 'ots',
            meta: {
                title: 'IMS - Ots',
            },
            component: () => import('@/views/Overtime/OTs.vue'),
        },
        {
            path: '/ots/addOT',
            name: 'addOT',
            meta: {
                title: 'IMS - Ots',
            },
            component: () => import('@/views/Overtime/AddOT.vue'),
        },
        {
            path: '/ots/edit/:id',
            name: 'editOT',
            meta: {
                title: 'IMS - Ots',
            },
            props: true,
            component: () => import('@/views/Overtime/AddOT.vue'),
        },
        {
            path: '/ots/accept/:type',
            name: 'acceptOT',
            meta: {
                title: 'IMS - Ots',
            },
            props: true,
            component: () => import('@/views/Overtime/AcceptOT.vue'),
        },
        {
            path: '/remotes',
            name: 'remotes',
            meta: {
                title: 'IMS - Remotes',
            },
            component: () => import('@/views/Remote/Remotes.vue'),
        },
        {
            path: '/remotes/addRemote',
            name: 'addRemote',
            meta: {
                title: 'IMS - Remotes',
            },
            component: () => import('@/views/Remote/AddRemote.vue'),
        },
        {
            path: '/remotes/edit/:id',
            name: 'editRemote',
            meta: {
                title: 'IMS - Remotes',
            },
            props: true,
            component: () => import('@/views/Remote/AddRemote.vue'),
        },
        /* #endregion */
        /* #region Password */
        {
            path: '/changepass',
            name: 'changepass',
            meta: {
                title: 'IMS - Change Password',
            },
            component: () => import('@/views/ChangePass.vue'),
        },
        {
            path: '/forgotpass',
            name: 'forgotpass',
            meta: {
                title: 'IMS - Forgot Password',
            },
            component: () => import('@/views/ForgotPassword.vue'),
        },
        /* #endregion */
        /* #region Tasks */
        {
            path: '/addtask/',
            name: 'addtask',
            meta: {
                title: 'IMS - Tasks',
            },
            component: () => import('@/views/Tasks/AddTasks.vue'),
        },
        {
            path: '/tasks',
            name: 'tasks',
            meta: {
                title: 'IMS - Tasks',
            },
            component: () => import('@/views/Tasks/Tasks.vue'),
        },
        {
            path: '/edittask',
            name: 'edittask',
            meta: {
                title: 'IMS - Tasks',
            },
            component: () => import('@/views/Tasks/EditTasks.vue'),
        },
        /* #endregion */
        /* #region Others */
        {
            path: '/layoutDefault',
            name: 'layoutDefault',
            component: () => import('@/views/layoutDefault.vue'),
        },
        {
            path: '/homePage',
            name: 'homePage',
            meta: {
                title: 'IMS - Home Page',
            },
            component: () => import('@/views/HomePage.vue'),
        },
        {
            path: '/login',
            name: 'login',
            meta: {
                title: 'IMS - Login',
            },
            component: () => import('@/views/UserLogin.vue'),
        },
        {
            path: '/:catch(.*)',
            meta: {
                title: 'IMS - Not Found',
            },
            redirect: '/notfound',
        },
        {
            path: '/notfound',
            name: 'notfound',
            meta: {
                title: 'IMS - Not Found',
            },
            component: () => import('@/views/NotFound.vue'),
        },
        {
            path: '/roles',
            name: 'roles',
            meta: {
                title: 'IMS - Roles',
            },
            component: () => import('@/views/Roles.vue'),
        },
        {
            path: '/listdevice',
            name: 'listdevice',
            meta: {
                title: 'IMS - Orders',
            },
            component: () => import('@/views/ListDeviceOther/ListDeviceOther.vue'),
        },
        {
            path: '/handover',
            name: 'handover',
            meta: {
                title: 'IMS - Handover',
            },
            component: () => import('@/views/Equipment/Handover.vue'),
        },
        {
            path: '/addusernew',
            name: 'addusernew',

            component: () => import('@/views/Users/AddUsernew.vue'),
        },
        {
            path: '/profile',
            name: 'profile',
            component: () => import('@/views/Profile/Profile.vue'),
        },
        /* #endregion */
        /* #region Equipment */
        {
            path: '/equipments',
            name: 'equipments',
            meta: {
                title: 'IMS - Devices',
            },
            component: () => import('@/views/Devices/Devices.vue'),
        },
        {
            path: '/equipments/devices',
            name: 'devices',
            meta: {
                title: 'IMS - Devices',
            },
            component: () => import('@/views/Devices/Devices.vue'),
        },
        {
            path: '/equipments/handover',
            name: 'handover',
            meta: {
                title: 'IMS - Handover',
            },
            component: () => import('@/views/Equipment/Handover.vue'),
        },
        {
            path: '/equipments/listdevice',
            name: 'listdevice',
            meta: {
                title: 'IMS - Orders',
            },
            component: () => import('@/views/ListDeviceOther/ListDeviceOther.vue'),
        },
        /* #endregion */
        /* #region menu */

        {
            path: '/menu',
            name: 'menu',
            meta: {
                title: 'IMS - Menu',
            },
            component: () => import('@/views/Menu/menuList.vue'),
        },

        {
            path: '/menu/menu',
            name: 'menuSub',
            meta: {
                title: 'IMS - Menu',
            },
            component: () => import('@/views/Menu/menuList.vue'),
        },
        {
            path: '/menu/modules',
            name: 'modulesSub',
            meta: {
                title: 'IMS - Modules',
            },
            component: () => import('@/views/Module/Modules.vue'),
        },
        /* #endregion */
        /* #region Group */
        {
            path: '/groups',
            name: 'groups',
            meta: {
                title: 'IMS - Groups',
            },
            component: () => import('@/views/Groups/Groups.vue'),
        },
        {
            path: '/groups',
            name: 'groupsSubs',
            meta: {
                title: 'IMS - Groups',
            },
            component: () => import('@/views/Groups/Groups.vue'),
        },
        {
            path: '/groups/users',
            name: 'usersSubs',
            meta: {
                title: 'IMS - Users',
            },
            component: () => import('@/views/Users/Users.vue'),
        },
        {
            path: '/groups/permissionUserMenu',
            name: 'PermissionSubs',
            meta: {
                title: 'IMS - Permission UserMenu',
            },
            component: () => import('@/views/PermissionUserMenu/PermissionUserMenu.vue'),
        },
        {
            path: '/groups/permissionUserGroup',
            name: 'permissionUserGroupSubs',
            meta: {
                title: 'IMS - Permission UserGroup',
            },
            component: () => import('@/views/permissionUserGroup/permissionUserGroup.vue'),
        },
        /* #endregion */
        /* #region Module */
        {
            path: '/modules',
            name: 'modules',
            meta: {
                title: 'IMS - Modules',
            },
            component: () => import('@/views/Module/Modules.vue'),
        },
        {
            path: '/modules/modules',
            name: 'modulesSubs',
            meta: {
                title: 'IMS - Modules',
            },
            component: () => import('@/views/Module/Modules.vue'),
        },
        {
            path: '/modules/menu',
            name: 'menuSubs',
            meta: {
                title: 'IMS - Menu',
            },
            component: () => import('@/views/Menu/menuList.vue'),
        },
        /* #endregion */
        /* #region Permission */
        {
            path: '/permissionUserMenu',
            name: 'Permission',
            meta: {
                title: 'IMS - Permission UserMenu',
            },
            component: () => import('@/views/PermissionUserMenu/PermissionUserMenu.vue'),
        },
        {
            path: '/permissionUserGroup',
            name: 'permissionUserGroup',
            meta: {
                title: 'IMS - Permission UserGroup',
            },
            component: () => import('@/views/permissionUserGroup/permissionUserGroup.vue'),
        },
        {
            path: '/Permission/permissionUserMenu',
            name: 'PermissionSubsssss',
            meta: {
                title: 'IMS - Permission UserMenu',
            },
            component: () => import('@/views/PermissionUserMenu/PermissionUserMenu.vue'),
        },
        {
            path: '/Permission/permissionUserGroup',
            name: 'PermissionSubssss',
            meta: {
                title: 'IMS - Permission UserGroup',
            },
            component: () => import('@/views/permissionUserGroup/permissionUserGroup.vue'),
        },
        {
            path: '/Permission/roles',
            name: 'rolesSub',
            meta: {
                title: 'IMS - Roles',
            },
            component: () => import('@/views/Roles.vue'),
        },
        {
            path: '/projects',
            name: 'listprojects',
            meta: {
                title: 'IMS - List Project',
            },
            component: () => import('@/views/ListProject/ListProject.vue'),
        },
        {
            path: '/projects/tasks/:id',
            name: 'ProjectTasks',
            meta: {
                title: 'IMS - Issue',
            },
            props: true,
            component: () => import('@/views/IssueProject/DetailsProjectIssue.vue'),
        },
        {
            path: '/projectDetails',
            name: 'projectDetails',
            meta: {
                title: 'IMS - Project Details',
            },
            component: () => import('@/views/ProjectDetails/projectDetails.vue'),
        },
        {
            path: '/report/task',
            name: 'TaskReport',
            meta: {
                title: 'IMS - Report',
            },
            props: true,
            component: () => import('@/views/TaskReport/GetListTaskOfMenberInProjects.vue'),
        },
        {
            path: '/leaveOffs/acceptregisterlists',
            name: 'Acceptregisterlists',
            meta: {
                title: 'IMS - Leave Off',
            },
            props: true,
            component: () => import('@/views/LeaveOff/LeaveRegisterList.vue'),
        },
        {
            path: '/leaveOffs/summary',
            name: 'Leaveoff',
            meta: {
                title: 'IMS - Leave Off',
            },
            props: true,
            component: () => import('@/views/LeaveOff/LeaveOff_Info.vue'),
        },
        {
            path: '/leaveOffs/Registerlists',
            name: 'LeaveOffRegisterlists',
            meta: {
                title: 'IMS - Leave Off Register',
            },
            props: true,
            component: () => import('@/views/LeaveOff/LeaveOffRegister.vue'),
        },
        {
            path: '/rules',
            name: 'ruleinfo',
            meta: {
                title: 'IMS - Rule Info',
            },
            component: () => import('@/views/RuleInfo/Rule.vue'),
        },
        {
            path: '/paids',
            name: 'Paid',
            meta: {
                title: 'IMS - Paid',
            },
            props: true,
            component: () => import('@/views/Paid/Paid.vue'),
        },
        {
            path: '/EquipmentDevice',
            name: 'Equipment-Device',
            meta: {
                title: 'IMS - Equipment',
            },
            props: true,
            component: () => import('@/views/EquipmentAndDevices/Equipment_Device.vue'),
        },
        {
            path: '/action',
            name: 'Action-list',
            meta: {
                title: 'Action-list',
            },
            props: true,
            component: () => import('@/views/Actions/Actions.vue'),
        },
        {
            path: '/permissionActionModule',
            name: 'perrmission-action-module',
            meta: {
                title: 'permission-action-module-list',
            },
            props: true,
            component: () => import('@/views/Permission_Action_Module/perrmissionActionModule.vue'),
        },
        {    
            path: '/reviews',
            name: 'reviews',
            meta: {
                title: 'IMS - Staff Review',
            },
            props: true,
            component: () => import('@/views/StaffReview/ListReview.vue'),
        },

        /* #endregion */
    ],
})

router.beforeEach(async (to, from, next) => {
    document.title = to.meta.title ?? 'IMS'
    if (to.fullPath == '/login' || to.fullPath == '/forgotpass') {
        next()
    } else {
        if (!localStorage.getItem('username') || !localStorage.getItem('token')) {
            next('/login')
        } else {
            const decodeToken = jwt_decode(localStorage.getItem('token'))
            if (decodeToken.exp < Math.floor(new Date().getTime() / 1000)) {
                localStorage.clear()
                next('/login')
            } else {
                next()
            }
        }
    }
})

export default router
