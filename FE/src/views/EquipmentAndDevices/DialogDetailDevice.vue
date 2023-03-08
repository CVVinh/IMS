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
                <h5 class="text-center">Thông tin phần cứng</h5>
                <fieldset class="border p-2 mt-2">
                    <legend class="float-none w-auto p-2-custom">Thông tin máy tính</legend>
                    <div class="m-2-custom">
                        <div class="mb-2 border-bottom">
                            <lable class="font-w-500" for="mechineName"> Tên máy: </lable>
                            <div class="ms-3 mb-1" id="mechineName">
                                {{ infoDevice.mechineName ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                        <div class="mb-2 border-bottom">
                            <lable class="font-w-500" for="mechineName1"> Tên tài khoản: </lable>
                            <div class="ms-3 mb-1" id="mechineName1">
                                {{ infoDevice.userName ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                        <div class="mb-2 border-bottom">
                            <lable class="font-w-500" for="mechineName2"> Hệ điều hành: </lable>
                            <div class="ms-3 mb-1" id="mechineName2">
                                {{ infoDevice.operatingSystem ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                        <div class="mb-2">
                            <lable class="font-w-500" for="mechineName3"> Loại hệ thống: </lable>
                            <div class="ms-3 mb-1" id="mechineName3">
                                {{ infoDevice.systemType ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                    </div>
                </fieldset>
                <fieldset class="border p-2 mt-2">
                    <legend class="float-none w-auto p-2-custom">Thông tin CPU</legend>
                    <div class="m-2-custom">
                        <div class="mb-2 border-bottom">
                            <lable class="font-w-500" for="mechineName4"> Tên CPU: </lable>
                            <div class="ms-3 mb-1" id="mechineName4">
                                {{ infoDevice.cpuName ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                        <div class="mb-2">
                            <lable class="font-w-500" for="mechineName7"> Tên Mainboard: </lable>
                            <div class="ms-3 mb-1" id="mechineName7">
                                {{ infoDevice.mainName ?? 'Đang cập nhật....' }}
                            </div>
                        </div>
                    </div>
                </fieldset>
                <fieldset class="border p-2 mt-2">
                    <legend class="float-none w-auto p-2-custom">Thông tin vùng nhớ</legend>
                    <div class="m-2-custom">
                        <div class="mb-2 border-bottom">
                            <lable class="font-w-500" for="mechineName5"> Tổng dung lượng Ổ cứng: </lable>
                            <div class="ms-3 mb-1" id="mechineName5">{{ infoDevice.diskDriveTotalSize ?? 0 }} GB</div>
                        </div>
                        <div class="mb-2">
                            <lable class="font-w-500" for="mechineName6"> Tổng dung lượng Ram: </lable>
                            <div class="ms-3 mb-1" id="mechineName6">{{ infoDevice.deviceTotalRamSize ?? 0 }} GB</div>
                        </div>
                    </div>
                </fieldset>
            </div>
            <div class="col-md-8" style="padding: 0px 25px">
                <h5 class="text-center">Thông tin phần mềm</h5>
                <div class="mt-2">
                    <div class="mb-2" style="padding: 10px 0px 0px 10px">
                        <Export label="Xuất Excel" @click="exportCSV($event)" />
                        <div style="float: right">
                            <Button
                                @click="handlerRequesDevice(data)"
                                class="p-button-sm p-button-primary mt-1 me-2"
                                icon="pi pi-send"
                                label="Gửi yêu cầu gỡ ứng dụng"
                            />
                        </div>
                    </div>
                    <ScrollPanel style="width: 100%; height: 36rem; padding: 0px 10px 0px 0px">
                        <Card class="border-1">
                            <template #content>
                                <DataTable
                                    :value="arraySoftware"
                                    :rows="300"
                                    ref="dt"
                                    :paginator="true"
                                    :loading="loading"
                                    showGridlines="true"
                                    responsiveLayout="scroll"
                                    :exportFilename="
                                        'List_Device_Of_' + infoDevice.userName + '_Summary_Report_' + new Date()
                                    "
                                    paginatorTemplate="CurrentPageReport "
                                    :rowsPerPageOptions="[300]"
                                    currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                                    :sortOrder="1"
                                    :sortField="'applicationName'"
                                    dataKey="applicationId"
                                    :rowHover="true"
                                    v-model:selection="selectedDeviceExits"
                                >
                                    <Column selectionMode="multiple" headerStyle="width: 2rem"></Column>
                                    <Column header="No.">
                                        <template #body="{ index }">
                                            {{ index + 1 }}
                                        </template>
                                    </Column>
                                    <Column field="applicationName" header="Tên phần mềm" exportHeader="Tên phần mềm">
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
                    </ScrollPanel>
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
    import { NotificationService } from '@/service/notification.service'
    import jwtDecode from 'jwt-decode'
    export default {
        props: ['isOpen', 'selectedDevice'],
        data() {
            return {
                detailDevice: null,
                arraySoftware: null,
                infoDevice: null,
                loading: false,
                selectedDeviceExits: [],
                userRequest: jwtDecode(localStorage.getItem('token')),
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
            handlerRequesDevice() {
                if (this.selectedDeviceExits.length > 0) {
                    var obj = {
                        requestUser: this.userRequest.Id,
                        usercode: this.infoDevice.userName,
                        message: `Ứng dụng ${this.selectedDeviceExits
                            .map((el) => el.applicationName)
                            .join(', ')} không được phép sử dụng, yêu cầu xóa ngay!`,
                        title: 'Yêu cầu xóa ứng dụng',
                    }
                    NotificationService.handlerRequireDelete(obj)
                        .then((res) => {
                            if (res.status == 200) {
                                this.successMessage(
                                    'Gửi phản hồi thành công đến người dùng có tài khoản ' +
                                        `'${this.infoDevice.userName ?? 'Ẩn danh'}'`,
                                )
                            } else {
                                this.WarningMessage('Có lỗi trong quá trình gửi yêu cầu !')
                            }
                        })
                        .catch(() => {
                            this.WarningMessage('Có lỗi trong quá trình gửi yêu cầu !')
                        })
                } else {
                    this.WarningMessage('Xin lỗi bạn chưa chọn ứng dụng để gửi phản hồi !')
                }
                this.selectedDeviceExits = []
            },
            successMessage(mess) {
                this.$toast.add({
                    severity: 'success',
                    summary: 'Thành công',
                    detail: mess,
                    life: 3000,
                })
            },
            WarningMessage(mess) {
                this.$toast.add({
                    severity: 'warn',
                    summary: 'Cảnh báo',
                    detail: mess,
                    life: 2000,
                })
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
    .m-2-custom {
        margin: 0px 0rem 0.5rem 0.5rem !important;
    }
    .p-2-custom {
        margin: 0px 0rem 0.5rem 0.5rem !important;
        font-size: 18px;
        font-weight: 500;
    }
    .p-card {
        box-shadow: none !important;
    }
    .p-dialog-content {
        padding: 0px 25px;
        overflow: unset;
    }
    .font-w-500 {
        font-weight: 500;
    }
    .p-fieldset-legend-text {
        font-size: 15px;
    }
    fieldset.scheduler-border {
        border: 1px groove #ddd !important;
        padding: 0 1.4em 1.4em 1.4em !important;
        margin: 0 0 1.5em 0 !important;
        -webkit-box-shadow: 0px 0px 0px 0px #000;
        box-shadow: 0px 0px 0px 0px #000;
    }
    legend.scheduler-border {
        width: inherit;
        padding: 0 10px;
        border-bottom: none;
    }
</style>
