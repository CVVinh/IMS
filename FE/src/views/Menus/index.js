import ViewMenuVue from '@/views/Menus/viewsMenu/ViewMenus.vue'
import ViewSubmenuVue from '@/views/Menus/viewsMenu/ViewSubmenu.vue'
export const Module = [
    { id: '1', name: 'project', des: '' },
    { id: '2', name: 'remotes', des: '' },
    { id: '3', name: 'ots', des: '' },
    { id: '4', name: 'users', des: '' },
    { id: '5', name: 'equipments', des: '' },
    { id: '6', name: 'wikis', des: '' },
    { id: '7', name: 'leaveOff', des: '' },
    { id: '8', name: 'toSpend', des: '' },
    { id: '9', name: 'modules', des: '' },
    { id: '10', name: 'groups', des: '' },
    { id: '11', name: 'menu', des: '' },
]
export const color = [
    'bg-primary',
    'bg-warning',
    'bg-info',
    'background-color-cyan-600',
    'background-color-yellow-800',
    'background-color-green-600',
    'background-color-blue-gray-500',
]

export const MenuDemo = [
    {
        id: 'p1',
        Title: 'Project',
        Icon: 'bx bx-notepad',
        Controller: 'project',
        BackgroundColor: 'bg-primary',
        idModule: '1',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: 'p1',
                Title: 'Project',
                Icon: 'bx bx-notepad',
                Controller: 'project',
                BackgroundColor: 'bg-primary',
                idModule: '1',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 1,
            },
            {
                id: '1p1',
                Title: 'Add Task',
                Icon: 'bx bxs-layer-plus',
                View: ViewSubmenuVue,
                Controller: 'addtask',
                Parent: 'p1',
                idModule: '1',
            },
            {
                id: '2p1',
                Title: 'OTs',
                Icon: 'bx bx-time-five',
                View: ViewSubmenuVue,
                Controller: 'ots',
                Parent: 'p1',
                idModule: '1',
            },
        ],
    },
    {
        id: 'p2',
        Title: 'Remotes',
        Icon: 'bx bx-notepad',
        Controller: 'remotes',
        BackgroundColor: 'bg-warning',
        idModule: '2',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: '1p2',
                Title: 'Remotes01',
                Icon: 'bx bx-podcast',
                View: ViewSubmenuVue,
                Controller: '',
                Parent: 'p2',
                idModule: '2',
            },
        ],
    },
    {
        id: 'p3',
        Title: 'OTs',
        Icon: 'bx bx-time-five',
        Controller: 'ots',
        BackgroundColor: 'bg-info',
        idModule: '3',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: '1p3',
                Title: 'Project01',
                Icon: 'bx bx-notepad',
                View: ViewSubmenuVue,
                Controller: 'ots/project1',
                Parent: 'p3',
                idModule: '3',
            },
        ],
    },
    {
        id: 'p4',
        Title: 'Users',
        Icon: 'bx bx-user',
        Controller: 'users',
        BackgroundColor: 'background-color-cyan-600',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        idModule: '4',
        submenu: [
            {
                id: 'p4',
                Title: 'Users',
                Icon: 'bx bx-user',
                Controller: 'users',
                BackgroundColor: 'background-color-cyan-600',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 0,
                idModule: '4',
            },
            {
                id: '1p4',
                Title: 'Groups',
                Icon: 'bx bxs-group',
                View: ViewSubmenuVue,
                Controller: 'groups',
                Parent: 'p4',
                idModule: '4',
            },
            {
                id: '2p4',
                Title: 'Roles',
                Icon: 'bx bxs-lock',
                View: ViewSubmenuVue,
                Controller: 'roles',
                Parent: 'p4',
                idModule: '4',
            },
        ],
    },
    {
        id: 'p5',
        Title: 'Equipment',
        Icon: 'bx bx-devices',
        Controller: 'devices',
        idModule: '5',
        BackgroundColor: 'background-color-yellow-800',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: '2p5',
                Title: 'Devices',
                Icon: 'bx bx-devices',
                Controller: 'devices',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 'p5',
                idModule: '5',
            },
            {
                id: '1p5',
                Title: 'Handover',
                Icon: 'bx bxs-hand',
                Controller: 'handover',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 'p5',
                idModule: '5',
            },
            {
                id: '2p5',
                Title: 'Order',
                Icon: 'bx bxs-cart-add',
                Controller: 'listdevice',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 'p5',
                idModule: '5',
            },
        ],
    },
    {
        id: 'p6',
        Title: 'Wikis',
        Icon: 'bx bx-note',
        Controller: 'wikis',
        BackgroundColor: 'background-color-green-600',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        idModule: '6',
        submenu: [],
    },
    {
        id: 'p7',
        Title: 'Leave Off',
        Icon: 'bx bx-log-out-circle',
        Controller: '',
        BackgroundColor: 'background-color-blue-gray-500',
        View: ViewMenuVue, //props: [title, icon, controller]
        idModule: '7',
        Parent: 0,
        submenu: [],
    },
    {
        id: 'p9',
        Title: 'To Spend',
        Icon: '',
        Controller: '',
        BackgroundColor: 'background-color-green-600',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        idModule: '8',
        submenu: [],
    },
    {
        id: 'p10',
        Title: 'Module',
        Icon: 'bx bxs-component',
        Controller: 'modules',
        BackgroundColor: 'bg-primary',
        idModule: '10',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
    },
    {
        id: 'p11',
        Title: 'Groups',
        Icon: 'bx bxs-group',
        Controller: 'groups',
        BackgroundColor: 'bg-primary',
        idModule: '8',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: '1p4',
                Title: 'Groups',
                Icon: 'bx bxs-group',
                View: ViewSubmenuVue,
                Controller: 'groups',
                Parent: 'p4',
                idModule: '8',
            },
            {
                id: 'p4',
                Title: 'Users',
                Icon: 'bx bx-user',
                Controller: 'users',
                BackgroundColor: 'background-color-cyan-600',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 0,
                idModule: '8',
            },

            {
                id: '2p4',
                Title: 'Roles',
                Icon: 'bx bxs-lock',
                View: ViewSubmenuVue,
                Controller: 'roles',
                Parent: 'p4',
                idModule: '8',
            },
        ],
    },
    {
        id: 'p11',
        Title: 'Menu',
        Icon: 'bx bx-list-ul',
        Controller: 'menu',
        BackgroundColor: 'bg-warning',
        idModule: '8',
        View: ViewMenuVue, //props: [title, icon, controller]
        Parent: 0,
        submenu: [
            {
                id: '1p4',
                Title: 'Groups',
                Icon: 'bx bxs-group',
                View: ViewSubmenuVue,
                Controller: 'groups',
                Parent: 'p4',
                idModule: '8',
            },
            {
                id: 'p4',
                Title: 'Users',
                Icon: 'bx bx-user',
                Controller: 'users',
                BackgroundColor: 'background-color-cyan-600',
                View: ViewMenuVue, //props: [title, icon, controller]
                Parent: 0,
                idModule: '8',
            },

            {
                id: '2p4',
                Title: 'Roles',
                Icon: 'bx bxs-lock',
                View: ViewSubmenuVue,
                Controller: 'roles',
                Parent: 'p4',
                idModule: '8',
            },
        ],
    },

    {
        id: 'p12',
        Title: 'Role',
        Icon: 'bx bx-pen',
        Controller: 'roles',
        BackgroundColor: 'bg-info',
        idModule: '12',
        View: ViewMenuVue,
        Parent: 0,
    },
]

///permisionGroup
//permisionUser
