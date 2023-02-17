export class ListDevicesOtherDto {
    idOrder: number;
    idBranch: number;
    idDevice: number;
    amount: number;
    userCreated: number;
    dateCreated: string;
    userUpdated: number;
    dateUpdated: string;
    isDeleted: number;
    note: string;
    statusOrder: number;
    statusDevice: number;
}

export class Order {
    branchName: string;
    deviceName: string;
    lastName: string;
    firstName: string;
    orders1: Order1[];
}

export class Order1 {
    idOrder: number;
    idBranch: number;
    idDevice: number;
    amount: number;
    userUpdated: number;
    dateUpdated: string;
    userCreated: number;
    dateCreated: string;
    isDelete: number;
    note: string;
    statusOrder: number;
    statusDevice: number;
}