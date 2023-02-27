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

                <div class="detail__content-box" :style="[{ background: this.Datasend.isPaid ? 'green': 'orange'}]">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text detail__content-box-items-format-text">
                            <b >{{ this.Datasend.isPaid == true ? 'Đã thanh toán' : 'Chưa thanh toán' }}</b> 
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">
                    <div class="detail__content-box-items" >
                        <div class="detail__content-box-items-text">
                            <b><i class="pi pi-users p-button-icon"></i> Khách hàng:</b> {{ this.Datasend.customerName }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-notepad"></i> Dự án:</b> {{ this.project }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top">    
                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-wallet"></i> Mức chi:</b> {{ this.Datasend.amountPaid.toLocaleString('it-IT', { style: 'currency', currency: 'VND' }) }}
                        </div>
                    </div>

                    <div class="detail__content-box-items top">
                        <div class="detail__content-box-items-text">
                            <b><i class="bx bx-time-five"></i> Ngày chi:</b> {{ Datasend.paidDate }}
                        </div>
                    </div>
                </div>

                <div class="detail__content-box detail__content-box-top detail__content-box-size ">
                    <div class="detail__content-box-items">
                        <div class="detail__content-box-items-text">
                            <b><i class="p-confirm-dialog-icon pi pi-info-circle"></i> Lý do chi trả:</b> {{ Datasend.paidReason }}
                        </div>
                    </div>
                </div>
            </div>

            <div class="detail__content-box box-right ">
                <div v-if="Datasend.paidImages.length > 0" >
                    <Galleria :value="dataImgDetail" :responsiveOptions="responsiveOptions" :numVisible="5" :circular="true" :showItemNavigators="true" :showItemNavigatorsOnHover="true" >
                        <template #item="slotProps">
                            <img :src="slotProps.item.itemImageSrc" :alt="slotProps.item.alt" style="width: 100%" />
                        </template>
                        <template #thumbnail="slotProps">
                            <img :src="slotProps.item.thumbnailImageSrc" :alt="slotProps.item.alt" style="display: block;"/>
                        </template>
                    </Galleria>
                </div>
                <div v-else>
                    <h3>Không có hình ảnh để hiển thị</h3>
                </div>
            </div>
        </div>

        <template #footer>   
            <button v-if="Datasend.isPaid == false" class="btn btn-primary pi pi-check p-button-icon" @click="confirmPayment"> Thanh toán</button>        
            <button class="btn btn-secondary pi pi-times p-button-icon" @click="closeModal"> Huỷ</button>
        </template>
    </Dialog>
</template>

<script>
    import { GET_LIST_PAID, HTTP, HTTP_LOCAL } from '@/http-common'
    import { useVuelidate } from '@vuelidate/core'
    import { required } from '@vuelidate/validators'
    import { DateHelper } from '@/helper/date.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'

    export default {
        data() {
            return {
                Datasend: {
                    projectId: '',
                    customerName: '',
                    amountPaid: 0,
                    paidReason: '',
                    paidPerson: 0,
                    isPaid: false,
                    paidDate: null,
                    token: null,
                    paidImages: null,
                },
                project: null,
                dataImgDetail: [],
                responsiveOptions: [
                    {
                        breakpoint: '1024px',
                        numVisible: 8
                    },
                    {
                        breakpoint: '768px',
                        numVisible: 5
                    },
                    {
                        breakpoint: '560px',
                        numVisible: 3
                    }
                ]
            }
        },
        props: ['status', 'dataDetail'],
        methods: {
            closeModal() {
                this.imagesOld = [];
                this.images = [];
                this.$emit('closemodal')
            },

            confirmPayment () {
                this.$emit("confirmPayment", this.Datasend);
            },
           
            showSuccess(message) {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: message, life: 3000 })
            },
            showError(message) {
                this.$toast.add({ severity: 'error', summary: 'Lỗi', detail: message, life: 3000 })
            },
        },

        beforeUpdate() {
            this.dataImgDetail = [];
            if (this.dataDetail != null){
                this.Datasend = this.dataDetail;
                this.Datasend.paidDate = DateHelper.formatDate(this.Datasend.paidDate)

                if(this.Datasend.paidImages.length > 0){
                    this.Datasend.paidImages.forEach(item => {
                        var imgObj = {
                            "itemImageSrc": item.imagePath,
                            "thumbnailImageSrc": item.imagePath,
                            "alt": "Image "+item.imageId,
                        }
                        this.dataImgDetail.push(imgObj);
                    });
                }
                HTTP_LOCAL.get(`/Project/getById/${this.Datasend.projectId}`).then((res) => {
                    if(res.status == 200){
                        this.project = res.data.name;
                    }
                    else {
                        console.log("Lỗi lấy project của người dùng!");
                    }
                })
                .catch(err => {
                    console.log(err);
                })
            } 
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

        .detail__content-box-size {
            min-height: 150px;
        }

        .box-left {
            flex: 35%;
        }

        .box-right {
            flex: 65%;
            margin-left: 15px;
        }
    }

    @media (max-width: 500px) {
        .detail__content {
            flex-direction: column;

            .box-left, .box-right {
                flex: 100%;
            }
            .box-right  {
                margin-top: 10px;
            }
        }
    }

</style>
