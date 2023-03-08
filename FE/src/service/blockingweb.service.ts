import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class BlockingWebService {
    public static handlerRequireBlockingWeb(object: any) {
        return HTTP.post(ApiApplication.BLOCKINGWEB.ADD_BLOCKING_WEB, object)
    }
}
