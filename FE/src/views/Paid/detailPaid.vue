<template>
    <Dialog
        header="Chi tiết thu chi"
        :visible="status"
        :closable="false"
        :maximizable="true"
        modal="true"
        :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
    >
        <div class="detail__content">
            <div class="detail__content-box box-left">

                <div class="detail__content-box-items" >
                    <div class="detail__content-box-items-text">
                        <b>Tên khách hàng:</b>{{ this.Datasend.customerName }}
                    </div>
                </div>

                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text">
                        <b>Mức chi:</b> {{ this.Datasend.amountPaid }}
                    </div>
                </div>

                <div class="detail__content-box-items top">
                    <div
                        class="detail__content-box-items-text"
                        :style="{ color: this.Datasend.isPaid ? 'green': 'orange'}"
                    >
                        <b>Trạng thái:</b>
                        {{
                            this.Datasend.isPaid == true ? 'Đã thanh toán' : 'Chưa thanh toán'
                        }}
                    </div>
                </div>
            </div>

            <div class="detail__content-box box-center">
                <div class="detail__content-box-items">
                    <div class="detail__content-box-items-text">
                        <b>Ngày chi:</b> {{ Datasend.paidDate }}
                    </div>
                </div>
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text">
                        <b>Dự án:</b> {{ this.project }}
                    </div>
                </div>
            </div> 

            <div class="detail__content-box box-right">
                <div class="detail__content-box-items">
                    <div class="detail__content-box-items-text">
                        <b>Lý do chi trả:</b> {{ Datasend.paidReason }}
                    </div>
                </div>
            </div>
        </div>

        <div class="content_box">

            <div v-if="Datasend.paidImages.length > 0" >
                <Galleria :value="dataImgDetail" :responsiveOptions="responsiveOptions" :numVisible="5">
                    <template #item="slotProps">
                        <img :src="slotProps.item.itemImageSrc" :alt="slotProps.item.alt" style="width: 100%" />
                    </template>
                    <template #thumbnail="slotProps">
                        <img :src="slotProps.item.thumbnailImageSrc" :alt="slotProps.item.alt" />
                    </template>
                </Galleria>
            </div>
            <div v-else>
                <h3>Không có hình ảnh để hiển thị</h3>
            </div>
        </div>

        <template #footer>           
            <button class="btn btn-secondary" @click="closeModal">Huỷ</button>
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
                    amountPaid: '',
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
                        numVisible: 5
                    },
                    {
                        breakpoint: '768px',
                        numVisible: 3
                    },
                    {
                        breakpoint: '560px',
                        numVisible: 1
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
                console.log(this.Datasend)
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

<style scoped>

    .detail__content {
        display: flex;
        justify-content: space-between;
        padding: 20px;
        margin-bottom: 20px;
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;
    }

    .detail__content-box {
        display: flex;
        flex-direction: column;
        box-shadow: rgba(6, 24, 44, 0.4) 0px 0px 0px 2px, rgba(6, 24, 44, 0.65) 0px 4px 6px -1px,
            rgba(255, 255, 255, 0.08) 0px 1px 0px inset;
        padding: 10px;
        border-radius: 10px;
    }

    .box-left {
        min-width: 300px;
        width: 32%;
    }
    .box-right {
        min-width: 300px;
        width: 32%;
        margin-left: 15px;
    }
    .box-center {
        min-width: 300px;
        width: 32%;
        margin-left: 15px;
    }

    .detail__content-box-items {
        display: flex;
        width: 100%;
        align-items: center;
    }
    .detail__content-box-items-text {
        font-size: 18px;
    }
    .top {
        margin-top: 10px;
    }
    .header {
        border: 1px solid black;
    }

    .content_box {
        box-shadow: -3px 3px 5px -3px #888888, 4px 5px 3px -4px #888888, 4px 5px 2px -5px #888888 inset;
        padding: 10px;
        border-radius: 10px;
    }

</style>
