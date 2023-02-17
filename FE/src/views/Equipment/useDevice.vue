<template>
    <LayoutDefaultDynamic>
        <DynamicDialog />
        <Button label="Add Use Device" class="p-button-success" v-on:click="showAddUseDevice()" />
        <Button label="Your device" class="p-button-success" v-on:click="showYourDevice()" />
        <Button label="Edit Use Device" class="p-button-success" v-on:click="showEditUseDevice()" />
    </LayoutDefaultDynamic>
</template>
<script>
    import addUseDeviceVue from './addUseDevice.vue'
    import ConfirmUseDevicedVue from './ConfirmUseDeviced.vue'
    import EditUseDeviceVue from '@/views/Equipment/EditUseDevice.vue'
    export default {
        data() {
            return { dialogRef: this.$dialog }
        },
        methods: {
            showAddUseDevice() {
                this.$dialog.open(addUseDeviceVue, {
                    props: {
                        header: 'Add Handovers',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                    },
                })
            },
            showEditUseDevice() {
                this.$dialog.open(EditUseDeviceVue, {
                    props: {
                        header: 'Edit Use Device',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                    },
                    data: {
                        idHandover: '0',
                    },
                })
            },
            showYourDevice() {
                this.$dialog.open(ConfirmUseDevicedVue, {
                    props: {
                        header: 'Your Device Using',
                        maximizable: true,
                        closable: true,
                        position: 'center',
                        dismissableMask: true,
                    },
                })
            },
        },
    }
</script>
<style scoped>
    .outsideDialog {
        width: 1000px;
        height: 90vh;
    }
</style>
