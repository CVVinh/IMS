import { reactive } from 'vue'
import { HTTP } from '@/http-common'

const storeRole = ({
    state: reactive({
        key: null
    }),
    getters: {
        getKey() {
            return storeRole.state.key;
        }
    },
    actions: {
        async getKeyGroup() {
            const idGroup = localStorage.getItem('IdGroup')
            const res = await HTTP.get('Group/getListGroup')
            if (res.status === 200) {
                res.data._Data.forEach((element) => {
                    if (element.id === Number(idGroup)) {
                        storeRole.state.key = element.key
                    }
                })
            }
        }
    }
})

export default storeRole