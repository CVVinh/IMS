<template>
    <Dialog
        header="Detail Device"
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
                <h6 class="text-center">Hardware information</h6>
                <div class="m-2">
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName"> Mechine name: </lable>
                        <div class="ms-3 mb-1" id="mechineName">
                            {{ infoDevice.mechineName }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName"> User name: </lable>
                        <div class="ms-3 mb-1" id="mechineName">
                            {{ infoDevice.userName }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName"> Operating system: </lable>
                        <div class="ms-3 mb-1" id="mechineName">
                            {{ infoDevice.operatingSystem }}
                        </div>
                    </div>
                    <div class="mb-2 border-bottom">
                        <lable class="font-w-500" for="mechineName"> System type: </lable>
                        <div class="ms-3 mb-1" id="mechineName">
                            {{ infoDevice.systemType }}
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-8" style="padding: 0px 25px">
                <h6 class="text-center">Software information</h6>
                <div class="mt-2">
                    <div class="mb-2">
                        <Export label="Export" @click="exportCSV($event)" />
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
                                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                                :globalFilterFields="['#', 'id', 'DisplayName', 'path']"
                            >
                                <Column field="#" header="N.o">
                                    <template #body="{ index }">
                                        {{ index + 1 }}
                                    </template>
                                </Column>
                                <Column field="DisplayName" header="Software name" :sortable="true">
                                    <template #body="{ data }">
                                        {{ data.DisplayName }}
                                    </template>
                                </Column>
                                <Column field="path" header="Destination">
                                    <template #body="{ data }">
                                        {{ data.path }}
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
                label="Cancel"
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
