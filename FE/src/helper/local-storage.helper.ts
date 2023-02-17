// @ts-ignore
import jwtDecode from "jwt-decode"

export const LocalStorageKey = {
    EMAIL: 'email',
    USER_NAME: 'username',
    TOKEN: 'token',
    USER_INFO: 'userInfo'
};

export class LocalStorage {
    public static jwtDecodeToken() {
        return jwtDecode(localStorage.getItem(LocalStorageKey.TOKEN))
    }
}
