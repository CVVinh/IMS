<template>
    <Dialog
        header="Chi tiết xin nghỉ"
        :maximizable="true"
        :closable="false"
        position="top"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
        :style="{ width: '80vw' }"
        :dismissableMask="true"
        :modal="true"
        :visible="isOpen"
    >
        <div class="container" v-if="this.detailLeaveoff != null">
            <div class="card card-body">
                <div class="row">
                    <div class="col-md-3 border-end">
                        <div class="border-bottom mb-2">
                            <label> Ngày bắt đầu</label>
                            <h6>
                                {{ formartDate(this.detailLeaveoff.startTime) }}
                            </h6>
                        </div>
                        <div class="border-bottom mb-2">
                            <label> Ngày kết thúc </label>
                            <h6>
                                {{ formartDate(this.detailLeaveoff.endTime) }}
                            </h6>
                        </div>
                        <div>
                            <label> Trạng thái </label>
                            <h6 class="d-block" :class="checkStatus(this.detailLeaveoff.status).class">
                                {{ checkStatus(this.detailLeaveoff.status).title }}
                            </h6>
                        </div>
                    </div>
                    <div class="col-md-9">
                        <div :class="{ row: this.detailLeaveoff.status == 3 }">
                            <div :class="{ 'col-md-6': this.detailLeaveoff.status == 3 }">
                                <h6 class="mb-1">Lý do</h6>
                                <ScrollPanel style="width: 100%; height: 200px" class="custombar1">
                                    <p>
                                        {{ this.detailLeaveoff.reasons }}
                                    </p>
                                </ScrollPanel>
                            </div>
                            <div class="col-md-6" v-if="this.detailLeaveoff.status == 3">
                                <h6 class="mb-1">Lý do không cho phép nghỉ</h6>
                                <ScrollPanel style="width: 100%; height: 200px" class="custombar1">
                                    <p>
                                        {{ this.detailLeaveoff.reasonNotAccept }}
                                    </p>
                                </ScrollPanel>
                            </div>
                        </div>
                    </div>
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
    import { HTTP, GET_LEAVE_OFF_BY_ID } from '@/http-common'
    import dayjs from 'dayjs'
    export default {
        props: ['isOpen', 'selectedLeaveOff'],
        data() {
            return {
                detailLeaveoff: null,
                statusLeave: [
                    {
                        id: 1,
                        title: 'Waiting',
                        class: 'badge bg-warning',
                    },
                    {
                        id: 2,
                        title: 'Done',
                        class: 'badge bg-success',
                    },
                    {
                        id: 3,
                        title: 'Cancel',
                        class: 'badge bg-secondary ',
                    },
                ],
            }
        },
        beforeUpdate() {
            this.getLeaveOffById()
        },
        methods: {
            formartDate(date) {
                const fmDate = new Date(date)
                return dayjs(fmDate).format('YYYY/MM/DD-HH:mm:ss')
            },
            checkStatus(id) {
                var fillter = this.statusLeave.filter(function (el) {
                    return el.id == id
                })
                return Object.assign({}, fillter[0])
            },
            getLeaveOffById() {
                this.detailLeaveoff = null
                this.detailLeaveoff = this.selectedLeaveOff
            },
            closeDialog() {
                this.$emit('closeDialog')
            },
        },
    }
</script>
<style lang="scss" scoped>
    ::v-deep(.p-scrollpanel) {
        p {
            padding: 0.5rem;
            line-height: 1.5;
            margin: 0;
        }
        &.custombar1 {
            .p-scrollpanel-wrapper {
                border-right: 9px solid var(--surface-ground);
            }

            .p-scrollpanel-bar {
                background-color: #9fc8ee;
                opacity: 1;
                transition: background-color 0.2s;

                &:hover {
                    background-color: #007ad9;
                }
            }
        }
    }
</style>
