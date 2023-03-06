<template>
    <Dialog
        header="Chi tiết thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
    >
        <div class="detail__content">
            <div class="detail__content-box box-left">
                <div
                    class="detail__content-box"
                    :style="[
                        {
                            background:
                                this.Datasend.isPaid && this.Datasend.isAccept
                                    ? 'green'
                                    : this.Datasend.isPaid && !this.Datasend.isAccept
                                    ? 'red'
                                    : 'orange',
                        },
                    ]"
                >
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text detail__content-box-items-format-text">
                            <b>{{
                                this.Datasend.isPaid && this.Datasend.isAccept
                                    ? 'Đã Thanh Toán'
                                    : this.Datasend.isPaid && !this.Datasend.isAccept
                                    ? 'Từ Chối Thanh Toán'
                                    : 'Chưa Thanh Toán'
                            }}</b>
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-id-card p-button-icon"></i> Người chi tiêu:</b>
                            {{ this.Datasend.paidNamePerson }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-user-edit"></i> Người xác nhận:</b> {{ this.Datasend.confirmNamePerson }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-users p-button-icon"></i> Khách hàng:</b>
                            {{ this.Datasend.customerFullName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-notepad"></i> Dự án:</b> {{ this.Datasend.nameProject }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-info-circle"></i> Lý do chi:</b>
                            {{ Datasend.paidNameReason }}
                        </div>
                    </div>
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-wallet"></i> Mức chi:</b> {{ this.Datasend.amountPaidName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-time-five"></i> Ngày Tạo:</b> {{ Datasend.createDate }}
                        </div>
                    </div>
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-time-five"></i> Ngày Chi:</b> {{ Datasend.paidDate }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top detail__content-box-size_content_reason">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-book"></i> Nội dung chi:</b>
                            {{ Datasend.contentReason }}
                        </div>
                    </div>
                </div>
            </div>

            <div class="detail__content-box box-right">
                <div v-if="Datasend.paidImages.length > 0">
                    <Galleria
                        :value="dataImgDetail"
                        :responsiveOptions="responsiveOptions"
                        :numVisible="3"
                        :circular="true"
                        :showItemNavigators="true"
                        :showItemNavigatorsOnHover="true"
                        :showIndicators="true"
                        :showIndicatorsOnItem="true"
                    >
                        <template #item="slotProps">
                            <img
                                :src="slotProps.item.itemImageSrc"
                                :alt="slotProps.item.alt"
                                style="width: 100%; display: block"
                            />
                        </template>
                        <template #thumbnail="slotProps">
                            <img
                                :src="slotProps.item.thumbnailImageSrc"
                                :alt="slotProps.item.alt"
                                style="display: block"
                            />
                        </template>
                    </Galleria>
                </div>
                <div v-else>
                    <img src="@/assets/noImage2.png" alt="anh" class="no_image" />
                </div>
            </div>
        </div>

        <template #footer>
            <Button
                v-if="Datasend.isPaid == false"
                label="Thanh toán"
                icon="pi pi-check"
                class="p-button-primary p-button-icon"
                @click="confirmPayment"
            ></Button>
            <Button
                label="Huỷ"
                icon="pi pi-times"
                class="p-button-secondary p-button p-button-icon p-component"
                @click="closeModal"
                enter="closeModal"
            ></Button>
        </template>
    </Dialog>
</template>

<script>
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL, GET_PROJECT_BY_ID, GET_USER_BY_ID, HTTP_API_GITLAB } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { onBeforeMount } from 'vue'

    export default {
        data() {
            return {
                Datasend: {
                    projectId: '',
                    customerName: '',
                    amountPaid: 0,
                    paidReason: '',
                    contentReason: '',
                    paidPerson: 0,
                    isPaid: false,
                    isAccept: false,
                    paidDate: null,
                    token: null,
                    paidImages: [],
                    paidNamePerson: null,
                    confirmNamePerson: null,
                    customerFullName: null,
                    paidNameReason: null,
                    nameProject: null,
                    amountPaidName: null,
                    createDate: null,
                },
                dataImgDetail: [],
                responsiveOptions: [
                    {
                        breakpoint: '1024px',
                        numVisible: 4,
                    },
                    {
                        breakpoint: '960px',
                        numVisible: 3,
                    },
                    {
                        breakpoint: '768px',
                        numVisible: 2,
                    },
                    {
                        breakpoint: '560px',
                        numVisible: 1,
                    },
                ],
            }
        },
        props: ['status', 'dataDetail'],
        methods: {
            showResponseApi(status, message = '') {
                switch (status) {
                    case 401:
                    case 403:
                        //this.showError('Bạn không có quyền thực hiện chức năng này!');
                        break

                    case 404:
                        this.showError('Lỗi! Load dữ liệu!')
                        break

                    default:
                        if (message != '') {
                            this.showError(message)
                        } else {
                            this.showError('Có lỗi trong quá trình thực hiện!')
                        }
                        break
                }
            },

            async getUsers(id) {
                return await HTTP.get(GET_USER_BY_ID(id))
                    .then((respone) => respone.data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getCustomerId(id) {
                return await HTTP.get(`Customer/GetById/${id}`)
                    .then((respone) => respone.data._Data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getPaidReasonId(id) {
                return await HTTP.get(`PaidReason/GetById/${id}`)
                    .then((respone) => respone.data._Data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            async getProjects(id) {
                return await HTTP.get(`Project/getProByIdDel/${id}`)
                    .then((respone) => respone.data)
                    .catch((error) => {
                        var message = error.response.data != '' ? error.response.data : error.response.statusText
                        this.showResponseApi(error.response.status, message)
                    })
            },

            closeModal() {
                this.imagesOld = []
                this.images = []
                this.$emit('closemodal')
            },

            confirmPayment() {
                this.$emit('confirmPayment', this.Datasend)
            },

            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },
        },

        async beforeUpdate() {
            //beforeUpdate
            this.dataImgDetail = []
            this.Datasend = []
            if (this.dataDetail != null) {
                this.Datasend = this.dataDetail

                if (this.Datasend.personConfirm != null) {
                    var confirmUser = await this.getUsers(parseInt(this.Datasend.personConfirm))
                    this.Datasend.confirmNamePerson = confirmUser.fullName
                } else {
                    this.Datasend.confirmNamePerson = ''
                }

                var paidUser = await this.getUsers(parseInt(this.Datasend.paidPerson))
                var customer = await this.getCustomerId(parseInt(this.Datasend.customerName))
                var paidReason = await this.getPaidReasonId(parseInt(this.Datasend.paidReason))

                this.Datasend.paidPerson = parseInt(this.Datasend.paidPerson)
                this.Datasend.paidNamePerson = paidUser.fullName

                this.Datasend.customerName = parseInt(this.Datasend.customerName)
                this.Datasend.customerFullName = customer.fullName

                this.Datasend.paidReason = parseInt(this.Datasend.paidReason)
                this.Datasend.paidNameReason = paidReason.name

                if (this.Datasend.paidDate === '0001-01-01T00:00:00' || this.Datasend.paidDate === '') {
                    this.Datasend.paidDate = ''
                } else {
                    this.Datasend.paidDate = DateHelper.formatDate(this.Datasend.paidDate)
                }

                if (this.Datasend.createDate === '0001-01-01T00:00:00' || this.Datasend.createDate === '') {
                    this.Datasend.createDate = ''
                } else {
                    this.Datasend.createDate = DateHelper.formatDate(this.Datasend.createDate)
                }

                this.Datasend.amountPaidName = this.Datasend.amountPaid.toLocaleString('it-IT', {
                    style: 'currency',
                    currency: 'VND',
                })

                if (this.Datasend.paidImages.length > 0) {
                    this.Datasend.paidImages.forEach((item) => {
                        var imgObj = {
                            itemImageSrc: item.imagePath,
                            thumbnailImageSrc: item.imagePath,
                            alt: 'Image ' + item.imageId,
                        }
                        this.dataImgDetail.push(imgObj)
                    })
                }

                if (this.Datasend.projectId != 0) {
                    var project = await this.getProjects(this.Datasend.projectId)
                    this.Datasend.nameProject = project.name
                } else {
                    this.Datasend.nameProject = ''
                }
            }
            console.log(this.Datasend)
        },
    }
</script>

<style scoped lang="scss">
    .detail__content {
        display: flex;
        justify-content: space-between;
        padding: 20px;
        margin-bottom: 20px;
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;

        .detail__content-box {
            display: flex;
            flex-direction: column;
            box-shadow: rgba(6, 24, 44, 0.4) 0px 0px 0px 2px, rgba(6, 24, 44, 0.65) 0px 4px 6px -1px,
                rgba(255, 255, 255, 0.08) 0px 1px 0px inset;
            padding: 10px;
            border-radius: 10px;

            .detail__content-box-items {
                width: 100%;

                .detail__content-box-items-text {
                    font-size: 18px;
                }

                .detail__content-box-items-format-text {
                    color: white;
                    text-align: center;
                }

                .top {
                    margin-top: 10px;
                }
            }
        }

        .detail__content-box-top {
            margin-top: 15px;
        }

        .detail__content-box-size_content_reason {
            min-height: 160px;
        }

        .box-left {
            flex: 35%;
        }

        .box-right {
            flex: 65%;
            margin-left: 15px;
        }
    }

    .no_image {
        border: 1px solid #ddd;
        border-radius: 15px;
    }

    @media (max-width: 500px) {
        .detail__content {
            flex-direction: column;

            .box-left,
            .box-right {
                flex: 100%;
            }
            .box-right {
                margin-top: 10px;
            }
        }
    }
</style>
