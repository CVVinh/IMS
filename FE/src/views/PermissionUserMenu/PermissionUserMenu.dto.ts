export class PermissionUserMenuDto {
    idUser: number;
    idModule: number;
    modules: ModulesDto;
    permission: PermissionDto[];
}

export class PermissionDto {
    selectedAccess: boolean;
    selectedAdd: boolean;
    selectedEdit: boolean;
    selectedDelete: boolean;
    selectedExport: boolean;
    selectedAll: boolean;
}

export class ModulesDto {
    id: number;
    nameModule: string;
    note: string;
    isDeleted: boolean;
    access: boolean = false;
    add: boolean = false;
    edit: boolean = false;
    delete: boolean = false;
    export: boolean = false;
    all: boolean = false;
    permission: PermissionDto;
}

export class GroupDto {
    id: number;
    dateCreated: string;
    dateModified: string;
    discription: string;
    isDeleted: boolean;
    nameGroup: string;
    userCreated: string;
    userModified: string;
}