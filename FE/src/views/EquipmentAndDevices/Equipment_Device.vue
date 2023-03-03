<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4 mt-3">
            <Card class="border-1">
                <template #header>
                    <div class="card card-body" style="background-color: #607d8b">
                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <h5 style="color: White">Thiết bị</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-end">
                            <div class="d-flex justify-content-start">
                                <div class="input-text">
                                    <InputText
                                        style="width: 100%"
                                        type="text"
                                        v-model="name"
                                        placeholder="Nhập tên nhân viên..."
                                    />
                                </div>
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button p-button-outlined ms-2"
                                    @click="handlerReload()"
                                />
                                <!-- <div class="ms-2">
                                    <Dropdown
                                        v-model="operatingSystem"
                                        :options="arrOperatingSystem"
                                        placeholder="Chọn hệ điều hành"
                                        optionLabel="name"
                                        optionValue="value"
                                    />
                                </div> -->
                                <!-- <Button
                                    type="button"
                                    icon="pi pi-search"
                                    class="p-button p-button-info ms-2"
                                    label="Search"
                                    @click="handlerSearch()"
                                /> -->
                            </div>
                        </div>
                    </div>
                </template>
                <template #emty>No data</template>
                <template #content>
                    <DataTable
                        :value="dataEquipment"
                        :rows="5"
                        ref="dt"
                        :paginator="true"
                        :loading="loading"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                        :rowsPerPageOptions="[5, 10, 15, 30]"
                        currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                        :globalFilterFields="['#', 'id', 'userName', 'name', 'updateAt']"
                    >
                        <Column field="#" header="No.">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column field="userName" header="Mã nhân viên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.userName }}
                            </template>
                        </Column>
                        <Column field="name" header="Tên nhân viên" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="mechineName" header="Tên thiết bị" :sortable="true">
                            <template #body="{ data }">
                                {{ data.infoDevice.mechineName }}
                            </template>
                        </Column>
                        <Column field="updateAt" header="Thời gian cập nhật">
                            <template #body="{ data }">
                                {{ formartDate(data.updateAt) }}
                            </template>
                        </Column>

                        <Column header="Thực thi" class="d-flex justify-content-center">
                            <template #body="{ data }">
                                <div class="d-flex justify-content-center">
                                    <Button
                                        @click="handlerDetailsDevice(data)"
                                        class="p-button-sm mt-1 me-2 p-button-info"
                                        icon="pi pi-eye"
                                    />
                                    <Button
                                        @click="handlerRequesDevice(data)"
                                        class="p-button-sm p-button-success mt-1 me-2"
                                        icon="pi pi-send"
                                    />
                                </div>
                            </template>
                        </Column>
                    </DataTable>
                </template>
            </Card>
        </div>
        <DialogDetailDevice
            :isOpen="this.isOpenDialogDevice"
            :selectedDevice="{ ...deviceDetail }"
            @closeDialogDevice="closeDialogDetailDevice()"
        />
    </LayoutDefaultDynamic>
</template>
<script>
    import dayjs from 'dayjs'
    import DialogDetailDevice from './DialogDetailDevice.vue'
    import { DeviceService } from '@/service/device.service'
    import { HTTP, GET_USER_BY_ID } from '@/http-common'
    import { OperatingSystem } from './OperatingSystem'
    export default {
        async created() {
            await this.getAllDevice()
        },
        data() {
            return {
                loading: true,
                dataEquipment: [
                    // {
                    //     id: 1,
                    //     name: 'Nguyễn Nhựt Thanh',
                    //     updateAt: new Date(),
                    //     infoDevice: {
                    //         mechineName: 'THANHNGUYEN',
                    //         userName: 'thanhNT',
                    //         operatingSystem: 'Microsoft Windown NT 10.10.2',
                    //         systemType: '64 bit operating system, x64-based processor',
                    //     },
                    //     infoInstallSoftware: [
                    //         {
                    //             id: 1,
                    //             DisplayName: 'Visula Code',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 2,
                    //             DisplayName: 'Visula Code 1',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 3,
                    //             DisplayName: 'Visula Code 2',
                    //             path: 'C:/path/hshs',
                    //         },
                    //     ],
                    // },
                    // {
                    //     id: 1,
                    //     name: 'Võ Thanh Phong',
                    //     updateAt: new Date(),
                    //     infoDevice: {
                    //         mechineName: 'PHONGVO',
                    //         userName: 'phongvo',
                    //         operatingSystem: 'Microsoft Windown NT 10.10.2',
                    //         systemType: '64 bit operating system, x64-based processor',
                    //     },
                    //     infoInstallSoftware: [
                    //         {
                    //             id: 1,
                    //             DisplayName: 'Visula Code',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 2,
                    //             DisplayName: 'Visula Code 1',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 3,
                    //             DisplayName: 'Visula Code 2',
                    //             path: 'C:/path/hshs',
                    //         },
                    //     ],
                    // },
                    // {
                    //     id: 1,
                    //     name: 'Trần Lăng Khoa',
                    //     updateAt: new Date(),
                    //     infoDevice: {
                    //         mechineName: 'TRANLANGKHOA',
                    //         userName: 'langkhoa',
                    //         operatingSystem: 'Microsoft Windown NT 10.10.2',
                    //         systemType: '64 bit operating system, x64-based processor',
                    //     },
                    //     infoInstallSoftware: [
                    //         {
                    //             id: 1,
                    //             DisplayName: 'Visula Code',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 2,
                    //             DisplayName: 'Visula Code 1',
                    //             path: 'C:/path/hshs',
                    //         },
                    //         {
                    //             id: 3,
                    //             DisplayName: 'Visula Code 2',
                    //             path: 'C:/path/hshs',
                    //         },
                    //     ],
                    // },
                ],
                isOpenDialogDevice: false,
                deviceDetail: [],
                name: null,
                operatingSystem: 19000,
                arrOperatingSystem: OperatingSystem,
            }
        },
        watch: {
            name: {
                handler: async function Change(newText) {
                    console.log(typeof newText)
                    await this.handlerSearchByName()
                },
            },
            deep: true,
        },
        methods: {
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            getUserById(id) {
                return HTTP.get(GET_USER_BY_ID(id)).then((res) => res.data)
            },
            async handlerLoadData() {
                for (let i = 0; i < this.dataEquipment.length; i++) {
                    const user = await this.getUserById(Number(this.dataEquipment[i].idUser))
                    this.dataEquipment[i].name = user.fullName
                    this.dataEquipment[i].user = user
                    this.dataEquipment[i].userName = user.userCode
                    this.dataEquipment[i].infoDevice.userName = user.userCode
                }
            },
            async getAllDevice() {
                await DeviceService.getAllEquipmentDevice()
                    .then((res) => {
                        console.log(res.data._Data)
                        res.data._Data.forEach((el) => {
                            this.dataEquipment.push({
                                name: null,
                                userName: null,
                                updateAt: el.deviceInfo.updateAt,
                                idUser: el.deviceInfo.userId,
                                user: null,
                                infoDeviceEmty: el.deviceInfo,
                                infoDevice: {
                                    mechineName: el.deviceInfo.deviceName,
                                    userName: null,
                                    operatingSystem: el.deviceInfo.operatingSystem,
                                    systemType: el.deviceInfo.systemType,
                                    cpuName: el.deviceInfo.cpuName,
                                    deviceTotalRamSize: el.deviceInfo.deviceTotalRamSize,
                                    mainName: el.deviceInfo.mainName,
                                    diskDriveTotalSize: el.deviceInfo.diskDriveTotalSize,
                                },
                                infoInstallSoftware: el.applications,
                            })
                        })
                    })
                    .catch((err) => console.log(err))
                await this.handlerLoadData()
                this.loading = false
            },
            handlerRequesDevice() {
                console.log('vô reqeust device')
            },
            async handlerSearchByName() {
                this.loading = true
                await DeviceService.searchDeviceByName(this.name)
                    .then((res) => {
                        console.log(res.data)
                        this.dataEquipment = []
                        res.data._Data.forEach((el) => {
                            this.dataEquipment.push({
                                name: null,
                                userName: null,
                                updateAt: el.deviceInfo.updateAt,
                                idUser: el.deviceInfo.userId,
                                user: null,
                                infoDeviceEmty: el.deviceInfo,
                                infoDevice: {
                                    mechineName: el.deviceInfo.deviceName,
                                    userName: null,
                                    operatingSystem: el.deviceInfo.operatingSystem,
                                    systemType: el.deviceInfo.systemType,
                                },
                                infoInstallSoftware: el.applications,
                            })
                        })
                    })
                    .catch(() => {
                        this.dataEquipment = []
                        this.getAllDevice()
                        this.loading = false
                    })
                await this.handlerLoadData()
                this.loading = false
            },
            handlerDetailsDevice(data) {
                this.isOpenDialogDevice = true
                this.deviceDetail = { ...data }
                console.log(data)
            },
            closeDialogDetailDevice() {
                this.isOpenDialogDevice = false
                this.deviceDetail = []
            },
            async handlerReload() {
                this.loading = true
                this.dataEquipment = []
                this.name = null
                this.operatingSystem = 19000
                await this.getAllDevice()
            },
        },
        components: {
            DialogDetailDevice,
        },
    }
</script>
<style>
    .p-card-header {
        padding: 10px;
    }
    .p-card-body {
        padding: 10px !important;
    }
    .p-card .p-card-content {
        padding: 0px 0px 1.25rem 0px;
    }
</style>
