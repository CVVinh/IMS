export class LeaveOffDto {
    id: number
    createTime: any = new Date()
    startTime: any = new Date()
    endTime: string
    reasonNotAccept: string
    reason: string
    status: number
    idCompanyBranh: any = null
}
