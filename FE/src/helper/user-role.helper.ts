import { LocalStorageKey } from "./local-storage.helper";
import { UserService } from "../service/user.service";
import { HttpStatus } from "../config/app.config";
import storeRole from "./../stores/role";
import permissionAccess from "./../stores/permissionAccess";

export class UserRoleHelper {
    private static userInfo: []
    public static isAccess: boolean = false;

    public static async getUserRole(user, leader) {
        const dataValue = await UserService.getAllUser()
        if (dataValue.status === HttpStatus.OK) {
            dataValue.data.forEach((element) => {
                if (element.idGroup == "Users") {
                    return user.push(element);
                } else if (element.idGroup == "Leader") {
                    return leader.push(element);
                }
            })
        } else {
            user = [];
            leader = [];
        }
    }

    public static getUserInfo() {
        UserService.getUserInfo().then((res) => {
            if (res.data) {
                localStorage.setItem(LocalStorageKey.USER_INFO, JSON.stringify(res.data))
                return (this.userInfo = res.data)
            }
        })
    }

    public static async isAdmin() {
        await storeRole.actions.getKeyGroup();
        return storeRole.state.key === 'admin'
    }

    public static async isDirector() {
        return storeRole.state.key === 'director'
    }
    public static async isPm() {
        return storeRole.state.key === 'pm'
    }
    public static async isLeader() {
        return storeRole.state.key === 'leader'
    }
    public static async isAccountant() {
        return storeRole.state.key === 'accountant'
    }
    public static async isHr() {
        return storeRole.state.key === 'hr'
    }
    public static async isUsers() {
        return storeRole.state.key === 'users'
    }

    public static async isAccessModule(nameModule) {
        await permissionAccess.actions.getListAccessModuleByGroup()
        permissionAccess.state.listModule.forEach(item => {
            if (item.nameModule === nameModule) {
                this.isAccess = item.access
            }
            return this.isAccess;
        })
    }
}
