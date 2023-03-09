import { reactive } from 'vue'
import { HTTP } from '@/http-common'
import { ApiApplication } from "@/config/app.config";
import { LocalStorage } from "@/helper/local-storage.helper";

const permissionAccess = ({
    state: reactive({
        access: false,
        nameModuleSelected: null,
        listModule: [],
        selectedAccess: [],
    }),
    getters: {
        getListModule() {
            return permissionAccess.state.listModule;
        }
    },
    actions: {
        // comment
        async getListAccessModuleByGroup() {
            const idGroup = localStorage.getItem('IdGroup');

            const token = LocalStorage.jwtDecodeToken();
            //const listIdGroup = localStorage.getItem('ListGroup');
            console.log("listIdGroup: " + JSON.stringify(token.ListGroup));
        
            if (idGroup) {
                await this.getListModule();
                const dataAccess = await HTTP.get(ApiApplication.PERMISSION_GROUP_MENU.GET_BY_USER_GROUP +  idGroup);
                console.log("dataAccess:" + JSON.stringify(dataAccess.data._Data));

                if (dataAccess) {
                    permissionAccess.state.listModule.map(module => {
                        dataAccess.data.forEach(item => {
                            if (item.idModule === module.id) {
                                module['access'] = item.access;  
                            }
                        })
                    })
                }
            }
        },
        async getListModule() {
          await  HTTP.get(ApiApplication.MODULE.GET_ALL).then(res => {
                if (res.data) {
                    permissionAccess.state.listModule = res.data;
                }
            });
        },

    }
})

export default permissionAccess
