import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class UserService {
    public static getAllUser() {
        return HTTP.get(ApiApplication.USER.GET_ALL)
    }

    public static getUserByCode(value) {
        return HTTP.get(ApiApplication.USER.GET_BY_CODE + value)
    }

    public static getUserInfo() {
        return HTTP.get(ApiApplication.USER.GET_INFO)
    }
}
