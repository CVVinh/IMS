import { LocalStorage } from "@/helper/local-storage.helper"

const token = LocalStorage.jwtDecodeToken()


const checkAccessModule = (path) => {
   

   console.log(token);
    var access = false
    token.role.map(ele=>{
        if(ele.includes('permission_group: True module: '+path) === true){
            access = true
        }
    })
    if(access === true) {
        console.log('welcome to ' + path);
    }else{
        console.log('cut');
    }
    return access
}

const checkShowButton = (path,showButton) => {
    var arrayShowButton = [];
    
    token.role.map(ele=>{
        if(ele.includes('modules: '+path) === true){
            arrayShowButton.push(ele)
        }
    })
    arrayShowButton.map(ele=>{
        if(ele.includes('add: 1')){
            showButton.add = true
        }
        if(ele.includes('update: 1')){
            showButton.update = true
        }
        if(ele.includes('delete: 1')){
            showButton.delete = true
        }
        if(ele.includes('deleteMulti: 1')){
            showButton.deleteMulti = true
        }
        if(ele.includes('confirm: 1')){
            showButton.confirm = true
        }
        if(ele.includes('confirmMulti: 1')){
            showButton.confirmMulti = true
        }
        if(ele.includes('refuse: 1')){
            showButton.refuse = true
        }
        if(ele.includes('addMember: 1')){
            showButton.addMember = true
        }
        if(ele.includes('export: 1')){
            showButton.export = true
        }
        
    })
}

export default {checkAccessModule,checkShowButton}