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
                                <h5 style="color: White">Equipment</h5>
                            </div>
                        </div>

                        <div class="d-flex align-items-center justify-content-between">
                            <div class="d-flex justify-content-start">
                                <div class="input-text">
                                    <InputText
                                        style="width: 100%"
                                        type="text"
                                        v-model="textFilter"
                                        placeholder="Enter employee name..."
                                    />
                                </div>
                                <Button
                                    type="button"
                                    icon="pi pi-search"
                                    class="p-button p-button-info ms-2"
                                    label="Search"
                                    @click="handlerSearch()"
                                />
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button p-button-outlined ms-2"
                                    @click="handlerReload()"
                                />
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
                        :globalFilterFields="['#', 'id', 'name', 'updateAt']"
                    >
                        <Column field="#" header="N.o">
                            <template #body="{ index }">
                                {{ index + 1 }}
                            </template>
                        </Column>
                        <Column field="name" header="Staff name" :sortable="true">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="updateAt" header="Update at">
                            <template #body="{ data }">
                                {{ formartDate(data.updateAt) }}
                            </template>
                        </Column>

                        <Column header="Action">
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

    export default {
        data() {
            return {
                loading: false,
                textFilter: null,
                dataEquipment: [
                    {
                        id: 1,
                        name: 'Nguyễn Nhựt Thanh',
                        updateAt: new Date(),
                        infoDevice: {
                            mechineName: 'THANHNGUYEN',
                            userName: 'thanhNT',
                            operatingSystem: 'Microsoft Windown NT 10.10.2',
                            systemType: '64 bit operating system, x64-based processor',
                        },
                        infoInstallSoftware: [
                            {
                                id: 1,
                                DisplayName: 'Visula Code',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 2,
                                DisplayName: 'Visula Code 1',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 3,
                                DisplayName: 'Visula Code 2',
                                path: 'C:/path/hshs',
                            },
                        ],
                    },
                    {
                        id: 1,
                        name: 'Võ Thanh Phong',
                        updateAt: new Date(),
                        infoDevice: {
                            mechineName: 'PHONGVO',
                            userName: 'phongvo',
                            operatingSystem: 'Microsoft Windown NT 10.10.2',
                            systemType: '64 bit operating system, x64-based processor',
                        },
                        infoInstallSoftware: [
                            {
                                id: 1,
                                DisplayName: 'Visula Code',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 2,
                                DisplayName: 'Visula Code 1',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 3,
                                DisplayName: 'Visula Code 2',
                                path: 'C:/path/hshs',
                            },
                        ],
                    },
                    {
                        id: 1,
                        name: 'Trần Lăng Khoa',
                        updateAt: new Date(),
                        infoDevice: {
                            mechineName: 'TRANLANGKHOA',
                            userName: 'langkhoa',
                            operatingSystem: 'Microsoft Windown NT 10.10.2',
                            systemType: '64 bit operating system, x64-based processor',
                        },
                        infoInstallSoftware: [
                            {
                                id: 1,
                                DisplayName: 'Visula Code',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 2,
                                DisplayName: 'Visula Code 1',
                                path: 'C:/path/hshs',
                            },
                            {
                                id: 3,
                                DisplayName: 'Visula Code 2',
                                path: 'C:/path/hshs',
                            },
                        ],
                    },
                ],
                isOpenDialogDevice: false,
                deviceDetail: [],
            }
        },
        watch: {
            textFilter: {
                handler: function Change(newText) {
                    console.log(newText)
                },
            },
            deep: true,
        },
        methods: {
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            handlerRequesDevice() {
                console.log('vô reqeust device')
            },
            handlerSearch() {
                console.log('search nef')
            },
            handlerDetailsDevice(data) {
                this.isOpenDialogDevice = true
                this.deviceDetail = { ...data }
                console.log(data)
                console.log(this.deviceDetail)
            },
            closeDialogDetailDevice() {
                this.isOpenDialogDevice = false
                this.deviceDetail = []
            },
            handlerReload() {
                console.log('reload o day')
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
