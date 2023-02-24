<template>
    <Dialog
        header="Chi tiết thiết bị"
        :maximizable="true"
        :closable="false"
        position="top"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '90vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="row">
            <div class="col-md-4 border-end" style="padding: 0px 25px">
                <h6 class="text-center">Thông tin phần cứng</h6>
                <div class="m-2">
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName"> Tên máy: </lable>
                        <div class="ms-3 mb-1" id="mechineName">
                            {{ infoDevice.mechineName }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName1"> Tên tài khoản: </lable>
                        <div class="ms-3 mb-1" id="mechineName1">
                            {{ infoDevice.userName }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName2"> Hệ điều hành: </lable>
                        <div class="ms-3 mb-1" id="mechineName2">
                            {{ infoDevice.operatingSystem }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName3"> Loại hệ thống: </lable>
                        <div class="ms-3 mb-1" id="mechineName3">
                            {{ infoDevice.systemType }}
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-8" style="padding: 0px 25px">
                <h6 class="text-center">Thông tin phần mềm</h6>
                <div class="mt-2">
                    <div class="mb-2">
                        <Export label="Xuất Excel" @click="exportCSV($event)" />
                    </div>
                    <Card class="border-1">
                        <template #content>
                            <DataTable
                                :value="arraySoftware"
                                :rows="5"
                                ref="dt"
                                :paginator="true"
                                :loading="loading"
                                showGridlines="true"
                                responsiveLayout="scroll"
                                :exportFilename="
                                    'List_Device_Of_' + infoDevice.userName + '_Summary_Report_' + new Date()
                                "
                                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                                :rowsPerPageOptions="[5, 10, 15, 30]"
                                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                            >
                                <Column field="#" header="No.">
                                    <template #body="{ index }">
                                        {{ index + 1 }}
                                    </template>
                                </Column>
                                <Column
                                    field="applicationName"
                                    header="Tên phần mềm"
                                    exportHeader="Tên phần mềm"
                                    :sortable="true"
                                >
                                    <template #body="{ data }">
                                        {{ data.applicationName }}
                                    </template>
                                </Column>
                                <Column field="applicationLocation" header="Đường dẫn" exportHeader="Đường dẫn">
                                    <template #body="{ data }">
                                        {{ data.applicationLocation }}
                                    </template>
                                </Column>
                            </DataTable>
                        </template>
                    </Card>
                </div>
            </div>
        </div>
        <template #footer>
            <Button
                label="Hủy"
                class="p-button-sm p-button-secondary p-button-outlined"
                icon="pi pi-times"
                @click="closeDialog()"
                autofocus
            />
        </template>
    </Dialog>
</template>
<script>
    import Export from '../../components/buttons/Export.vue'
    export default {
        props: ['isOpen', 'selectedDevice'],
        data() {
            return {
                detailDevice: null,
                arraySoftware: null,
                infoDevice: null,
                loading: false,
            }
        },
        beforeUpdate() {
            this.getDetailsDevice()
        },
        methods: {
            getDetailsDevice() {
                this.detailDevice = null
                this.arraySoftware = null
                this.infoDevice = null
                this.detailDevice = this.selectedDevice
                this.infoDevice = this.selectedDevice.infoDevice
                this.arraySoftware = this.selectedDevice.infoInstallSoftware
            },
            closeDialog() {
                this.$emit('closeDialogDevice')
            },
            exportCSV() {
                this.$refs.dt.exportCSV()
            },
        },
        components: {
            Export,
        },
    }
</script>
<style>
    .p-dialog-content {
        padding: 0px 25px;
    }
    .font-w-500 {
        font-weight: 500;
    }
</style>
