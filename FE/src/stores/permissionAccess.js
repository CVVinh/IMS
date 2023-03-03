import { reactive } from 'vue'
import { HTTP } from '@/http-common'
import { ApiApplication } from "@/config/app.config";

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
        async getListAccessModuleByGroup() {
            const idGroup = localStorage.getItem('IdGroup');
            if (idGroup) {
                await this.getListModule();
                const dataAccess = await HTTP.get(ApiApplication.PERMISSION_GROUP_MENU.GET_BY_USER_GROUP + "?idGroup=" + idGroup);
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
