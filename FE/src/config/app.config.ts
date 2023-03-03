export const HttpStatus = {
    OK: 200,
    CREATED: 201,
    BAD_REQUEST: 400,
    UNAUTHORIZED: 401,
    FORBIDDEN: 403,
    NOT_FOUND: 404,
    INTERNAL_SERVER_ERROR: 500,
    NOT_IMPLEMENTED: 501,
    BAD_GATEWAY: 502,
    SERVICE_UNAVAILABLE: 503,
    GATEWAY_TIMEOUT: 504,
    HTTP_VERSION_NOT_SUPPORTED: 505,
}

export const ApiApplication = {
    PROJECT: {
        GET_ALL: 'Project/getAllProject',
        UPDATED: 'Project/updateProject',
        CREATE: 'Project/addProject',
        DELETE: 'Project/DeleteProject',
    },

    USER: {
        GET_ALL: 'Users/getAll',
        GET_BY_CODE: 'Users/getUserByUserCode/',
        GET_INFO: 'Users/getInfo',
    },
    PERMISSION_GROUP_MENU: {
        GET_PERMISSION_GROUP: 'Permission_Groups/decentralization_Group',
        GET_BY_USER_GROUP: 'Permission_Groups/getPermissionGroup_By_IdGroup',
    },
    PERMISSION_USER_MENU: {
        GET_ALL: 'Group/getListGroup',
        GET_USER_BY_GROUP: 'Group/getUserByGroup',
        GET_USER_MENU: 'Permission_Use_Menus/getPermission_Use_Menu_ByIdUser',
        ADD_BY_USER: 'Permission_Use_Menus/addPermissionByUser',
        ADD_BY_GROUP: 'Permission_Use_Menus/addPermissionByGroup',
    },
    MODULE: {
        GET_ALL: 'Modules/getListModule',
    },
    RULEFOR: {
        GET_ALL: 'ReluFor/getListRelue',
        CREATE: 'ReluFor',
        UPDATE: (id: number) => `ReluFor/${id}`,
        DELETE: (id: number) => `ReluFor/${id}`,
    },
    EQUIPMENT_DEVICE: {
        GET_ALL_INFO_DEVICE: 'InfoDevices/GetAllDeviceListWithApplication',
        SEARCH_DEVICE_BY_NAME: (name: string) => `InfoDevices/FindWithUserName/${name}`,
        SEARCH_DEVICE_BY_OPERATINGSYSTEM: (operatingSystem: any) =>
            `InfoDevices/FindWithOperatingSystem/${operatingSystem}`,
    },
    NOTIFICATION: {
        GET_ALL_NOTIFICATION: 'Notification/GetAllListNotification',
        IS_WATCH_NOTIFICATION: (id: number) => `Notification/WatchNotification/${id}`,
    },
}
