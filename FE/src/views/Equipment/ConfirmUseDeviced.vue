<template>
    <div class="outside">
        <div class="container">
            <div class="header-outside">
                <div class="title">
                    <h1>Your Device Using</h1>
                </div>
            </div>
        </div>

        <div class="confirmAll"><Confirm label="Confirm all" @click="confirmAll()" /></div>
        <div class="table-outside">
            <DataTable :value="this.yourDevices" class="datatable">
                <Column field="hand1.idHandover" sortable header="Id Handover"></Column>
                <Column field="deviceName" sortable header="Name Device"></Column>
                <Column field="hand1.amount" sortable header="Amount"></Column>
                <Column field="hand1.dateCreated" sortable header="Date Created"></Column>
                <Column field="hand1.userCreated.userCode" sortable header="User Created"></Column>
                <Column field="status" sortable header="Status">
                    <template #body="slotProps">
                        <Button
                            v-if="slotProps.data.hand1.status == '1'"
                            label="Confirm"
                            class="p-button-primary"
                            v-on:click="confirm(slotProps.data.hand1.idHandover)"
                        />
                        <p class="note" v-if="slotProps.data.hand1.status == '2'">Confirmed</p>
                        <p class="note" v-if="slotProps.data.hand1.status != '1' && slotProps.data.hand1.status != '2'">
                            Cancel
                        </p>
                    </template>
                </Column>
            </DataTable>
        </div>
    </div>
</template>
<script>
    import { HTTP } from '@/http-common'
    import jwtDecode from 'jwt-decode'
    import ConfirmVue from '@/components/buttons/Confirm.vue'
    import Confirm from '../../components/buttons/Confirm.vue'
    export default {
        inject: ['dialogRef'],
        data() {
            return {
                formUseDevice: {
                    idDevice: null,
                    dateCreated: '',
                    userReceive: null,
                    userCreated: null,
                    amount: null,
                },
                yourDevices: [],
                user: '',
                users: '',
                handovers: [],
            }
        },
        methods: {
            confirmAll() {
                let id = []
                this.yourDevices.forEach((device) => {
                    if (device.hand1.status == '1') {
                        id.push(device.hand1.idHandover)
                    }
                })
                HTTP.put('Handovers/ConfirmHandover', id).then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: `Xác nhận ${id.length} Bàn giao thành công!`,
                            life: 3000,
                        })
                        this.getAllHandover()
                    }
                })
            },
            confirm(idHandover) {
                //PUT to useDevice
                HTTP.put('Handovers/ConfirmHandover', [idHandover]).then((res) => {
                    if (res.status == 200) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: `Xác nhận Bàn giao thành công!`,
                            life: 3000,
                        })
                        this.getAllHandover()
                    }
                })
            },
            closeDialog() {
                this.dialogRef.close()
            },
            getAllHandover() {
                HTTP.get('Handovers/getListHandover').then((res) => {
                    if (res.status == 200) {
                        this.handovers = res.data
                        this.yourDevices = []
                        if (this.handovers)
                            this.handovers.forEach((element) => {
                                if (element.hand1.userReceive == this.user.Id) {
                                    this.yourDevices.push(element)
                                }
                            })
                        this.getAllUser()
                    }
                })
            },
            getAllUser() {
                HTTP.get('Users/getAll').then((res) => {
                    if (res.status == 200) this.users = res.data

                    if (this.users)
                        this.users.forEach((user) => {
                            if (this.yourDevices)
                                this.yourDevices.forEach((device) => {
                                    if (device.hand1.userCreated == user.id) {
                                        device.hand1.userCreated = user
                                        return
                                    }
                                })
                        })
                })
            },
        },
        mounted() {
            this.getAllHandover()
            //GET ALL useDevice

            this.user = jwtDecode(localStorage.getItem('token'))
            //Get your device

            //GET ALL USER
        },
        components: {
            ConfirmVue,
            Confirm,
        },
    }
</script>
<style scoped>
    .container {
        z-index: 1;
        color: white;
        background-color: #33adff;
        display: flex;
        flex-direction: column;
        justify-content: center;
        position: sticky;
        top: 0px;
    }
    .header-outside {
        border-radius: 6px 6px 0px 0px;
        width: 100%;
        height: 64px;
        display: flex;
        justify-content: center;
        align-items: center;
    }
    .note {
        font-size: large;
        font-style: italic;
    }
    .outside {
        width: 100%;
        height: 100%;
    }
    .confirmAll {
        margin: 6px 0px 6px 0px;
        width: 100%;
        display: flex;
        justify-content: flex-end;
        position: sticky;
        right: 0px;
    }
</style>
