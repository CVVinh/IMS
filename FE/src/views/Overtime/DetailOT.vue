<template>
  <Dialog :visible="this.show" :closable="false" >
        <template #header>
            <h3>Chi tiết thông tin thẻ OT</h3>
        </template>
        <div class="detail__content">
            <div class="detail__content-box box-left">
                <div class="detail__content-box-items">
                    <div class="detail__content-box-items-text"><b>OT User:</b> {{ this.OTS ? this.OTS.nameUser : null  }}.</div> 
                </div>
                
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"> {{ this.OTS ? "(" +this.OTS.x.start + "h ->" + this.OTS.x.end +"h)" : null  }}</div> 
                </div>
              
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>Project:</b> {{ this.OTS ? this.OTS.name : null  }}</div> 
                </div>

                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>Create Date:</b> {{ this.OTS ? getFormattedDate(new Date(this.OTS.x.dateCreate)) : null  }}</div> 
                </div>

                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>Description:</b> {{ this.OTS ? this.OTS.x.description : null  }}</div> 
                </div>

                            
            </div>
            <div class="detail__content-box box-center">

                <div class="detail__content-box-items ">
                    <div class="detail__content-box-items-text"><b>OT Date:</b> {{  this.OTS ? getFormattedDate(new Date(this.OTS.x.date))  : null  }}</div> 
                </div>


                     
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>Lead:</b> {{ this.OTS ? this.OTS.nameLead : null  }}</div> 
                </div>
               
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>Type:</b> {{ this.OTS ? this.OTS.x.type : null  }}</div> 
                </div>
               
              
            </div>

            <div class="detail__content-box box-right">
                <div class="detail__content-box-items ">
                    <div class="detail__content-box-items-text"><b>OT Time:</b> {{ this.OTS ? this.OTS.x.realTime +  " giờ" : "" }}</div> 
                </div>
          
           
                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"><b>User Update:</b> {{ this.OTS ? this.OTS.nameUserUpdate : null  }}</div> 
                </div>

                <div class="detail__content-box-items top">
                    <div class="detail__content-box-items-text"
                    :style="{color : this.OTS.x.status === 0 ? 'blue' :
                                     this.OTS.x.status === 1 ? 'green' :
                                     this.OTS.x.status === 2 ? 'gray' :
                                     this.OTS.x.status === 3 ? 'red' :
                    ''}"
                    ><b>Status:</b> {{ this.OTS ? this.OTS.x.status === 0 ? "Waiting" 
                                     : this.OTS.x.status === 1 ? "Accepted"  
                                     : this.OTS.x.status === 2 ? "Denied"
                                     : this.OTS.x.status === 3 ? 'Deleted' : "" : ""
                                    }}</div> 
                    </div>
            </div>


        </div>
        <template #footer>
            <Button label="Duyệt" icon="pi pi-check" class="p-button-raised p-button-success p-button-text"
            v-if="this.isPm && this.OTS.x.status === 0 "
            @click="accept(true, this.OTS.x.id, this.OTS.x.leadCreate)"
            />
            <Button label="Hủy duyệt" icon="pi pi-times" class="p-button-raised p-button-danger p-button-text"
            v-if="this.isPm && this.OTS.x.status === 0"
            @click="accept(false, this.OTS.x.id, this.OTS.x.leadCreate)"
            />
            <Button label="Quay lại" icon="pi pi-sign-out" class="p-button-raised p-button-info p-button-text" @click="$emit('close')" />
	    </template>
        <Dialog
            header="Please enter reason!"
            :visible="displayDialog2"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
        >
            <Textarea v-model="reason" style="margin: auto; width: 100%; height: 100%"></Textarea>
            <p v-if="entered" class="p-error">Reason is required!</p>
            <template #footer>
                <Button label="No" icon="pi pi-times" @click="this.displayDialog2=false" class="p-button-text" />
                <Button label="Yes" icon="pi pi-check" @click="onSubmit(this.OTS.x.leadCreate)" autofocus />
            </template>
        </Dialog>
 </Dialog>
</template>

<script>
import { HTTP } from '@/http-common';
import jwtDecode from 'jwt-decode';

export default {
    props : ['show','OTS','isPm'],
    data(){
        return{
            display:true,
            Accept:true,
            Token : null,
            reason : "",
            displayDialog2: false,
            lead:null,
            id:null,
            entered: false,
        }
    },
    methods:{
        getFormattedDate(date) {
                var year = date.getFullYear();

                var month = (1 + date.getMonth()).toString();
                month = month.length > 1 ? month : '0' + month;

                var day = date.getDate().toString();
                day = day.length > 1 ? day : '0' + day;

                return day + '-' + month + '-' + year;
            },
            accept(accepted, id, lead) {
                let user = jwtDecode(localStorage.getItem('token'))
                this.PM = user.Id
                if (accepted) {
                    this.status = 1
                    HTTP.put('OTs/acceptOT', { id: id, status: this.status, pm: this.PM }).then(res=>{
                        this.$emit("alert");
                        this.$emit("close")
                    }).catch(err=>{
                        console.log(err);
                    })
                   
                    setTimeout(() => {
                        this.$emit("reloadAPI");
                    }, 500)
                } else {

                    this.displayDialog2 = true;
                    this.lead = lead
                    this.id = id
                }
            },
            onSubmit() {
                if (this.reason != null && this.reason != '') {
                    HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(
                        async (res) => {
                            if (res.status == 200) {
                                this.displayDialog2 = false;
                                this.$emit("alert")
                                await this.$emit("reloadAPI");
                                this.$emit("close")
                            }
                        },
                    )
                } else this.entered = true
            },
    }
}
</script>

<style scoped>

    .detail__content{
        display: flex;
        justify-content: space-between;
        padding: 20px;
        border-radius: 15px;
        box-shadow: rgba(0, 0, 0, 0.02) 0px 1px 3px 0px, rgba(27, 31, 35, 0.15) 0px 0px 0px 1px;
    }

    .detail__content-box{
        display: flex;
        flex-direction: column;
        box-shadow: rgba(6, 24, 44, 0.4) 0px 0px 0px 2px, rgba(6, 24, 44, 0.65) 0px 4px 6px -1px, rgba(255, 255, 255, 0.08) 0px 1px 0px inset;
        padding: 10px;
        border-radius: 10px;
    }

    .box-left{
        min-width: 300px;
    }
    .box-right{
        min-width: 300px;
        margin-left: 15px;
    }
    .box-center{
        min-width: 300px;
        margin-left: 15px;
    }


    .detail__content-box-items{
        display: flex;
        width: 100%;
        align-items: center;
    }
    .detail__content-box-items-text{
        font-size: 18px;
    }
    .top{
        margin-top: 10px;
    }
    .header{
        border: 1px solid black;
    }
</style>