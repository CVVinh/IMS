import { HTTP } from '@/http-common'
import { ApiApplication } from '../config/app.config'

export class DeviceService {
    public static getAllEquipmentDevice() {
        return HTTP.get(ApiApplication.EQUIPMENT_DEVICE.GET_ALL_INFO_DEVICE)
    }
    public static searchDeviceByName(name: string) {
        return HTTP.get(ApiApplication.EQUIPMENT_DEVICE.SEARCH_DEVICE_BY_NAME(name))
    }
    public static searchDeviceByOperatingSystem(operatingSystem: any) {
        return HTTP.get(ApiApplication.EQUIPMENT_DEVICE.SEARCH_DEVICE_BY_OPERATINGSYSTEM(operatingSystem))
    }
}
