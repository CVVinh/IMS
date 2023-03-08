import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class NotificationService {
    public static getAllNotification() {
        return HTTP.get(ApiApplication.NOTIFICATION.GET_ALL_NOTIFICATION)
    }
    public static isWatchNotification(id: number) {
        return HTTP.put(ApiApplication.NOTIFICATION.IS_WATCH_NOTIFICATION(id))
    }
    public static handlerRequireDelete(object: any) {
        return HTTP.post(ApiApplication.NOTIFICATION.REQUIRE_DELETE_APPLICATION, object)
    }
}
