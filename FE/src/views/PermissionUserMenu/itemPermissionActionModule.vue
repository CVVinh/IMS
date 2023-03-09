<template>

            <!-- Item -->
                    
            <div class="field-checkbox d-flex align-items-center justify-content-center" v-for="x in listaction" style="margin-left: 10px;">
                <input
                    :disabled="this.check"
                    class="input-checkbox"
                    type="checkbox"
                    :id="x.actionModule.id"
                    :value="x.actionModule.id"
                    v-bind:checked="checkCheckedbox(x)"
                    @change="isSelectedCheck(x.actionModule.id ,$event.target.checked)"
                />
                    <label for="add" class="ms-3" :style="{color:check === true ? 'gray' : null}">{{x.actionModule.name}}</label>
            </div>


            <!-- 
                1 = add
                2 = update  
                3 = delete
                4 = deleteMulti
                5 = confirm
                6 = confirmMulti
                7 = refuse
                8 = addMember
                9 = export excel
             -->
    
        <!-- <input
                    :disabled="x.module.access"
                    class="input-checkbox"
                    type="checkbox"
                    :id="module.id"
                    :value="module.id"
                    v-bind:checked="module.all ? module.all : module.add"
                    @change="isSelectedCheck(module, 'add', $event.target.checked)"
                    />
                <label for="add" class="ms-3">Thêm</label> -->
      
</template>

<script>
import { HTTP } from '@/http-common';

export default {
    data() {
        return {
            listaction : null,
            check : true,
        }
    },
    props: ['module','canCheck'],
    async mounted(){
       await this.getActionByIdModule()
       await this.ischeckked()
    },

    methods:{

        isSelectedCheck(idSelected,Checked){    
            if(Checked){
                if(idSelected === 1){
                    this.module.add = 1
                }
                if(idSelected === 2){
                    this.module.update = 1
                }
                if(idSelected === 3){
                    this.module.delete = 1
                }
                if(idSelected === 4){
                    this.module.deleteMulti = 1
                }
                if(idSelected === 5){
                    this.module.confirm = 1
                }
                if(idSelected === 6){
                    this.module.confirmMulti = 1
                }
                if(idSelected === 7){
                    this.module.refuse = 1
                }
                if(idSelected === 8){
                    this.module.addMember = 1
                }
                if(idSelected === 9){
                    this.module.export = 1
                }

            }else{
                if(idSelected === 1){
                    this.module.add = 0
                }
                if(idSelected === 2){
                    this.module.update = 0
                }
                if(idSelected === 3){
                    this.module.delete = 0
                }
                if(idSelected === 4){
                    this.module.deleteMulti = 0
                }
                if(idSelected === 5){
                    this.module.confirm = 0
                }
                if(idSelected === 6){
                    this.module.confirmMulti = 0
                }
                if(idSelected === 7){
                    this.module.refuse = 0
                }
                if(idSelected === 8){
                    this.module.addMember = 0
                }
                if(idSelected === 9){
                    this.module.export = 0
                }
            }
        },
        getActionByIdModule(){
            if(this.module){
                HTTP.get("PermissionActionModule/getPermissionActionModuleWithModuleId/" + this.module.id)
                .then(res=>{
                    this.listaction = res.data._Data
                    // .sort((a,b)=>{
                    //     if(a.actionModule.id < b.actionModule.id){
                    //         return -1
                    //     }else{
                    //         return 1
                    //     }
                    // })
                })
                .catch(err=>console.log(err))
            }
        },
        checkCheckedbox(idaction){
           if(idaction){
                if(idaction.actionModule.id === 1){
                   if(this.module.add === 1){
                    return true
                   }
                } 
                if(idaction.actionModule.id === 2){
                    if(this.module.update === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 3){
                    if(this.module.delete === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 4){
                    if(this.module.deleteMulti === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 5){
                    if(this.module.confirm === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 6){
                    if(this.module.confirmMulti === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 7){
                    if(this.module.refuse === 1){
                      return true
                   }
                }
                if(idaction.actionModule.id === 8){
                    if(this.module.addMember === 1){
                    return true
                   }
                }
                if(idaction.actionModule.id === 9){
                    if(this.module.export === 1){
                    return true
                   }
                }
           }
        },
        ischeckked(){
            console.log(this.canCheck);
            if(this.canCheck){
                this.check = false
            }
        }
    }
}
</script>

<style>

</style>