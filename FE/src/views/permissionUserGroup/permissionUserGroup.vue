<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="spinner-loading" v-if="loading">
            <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="3" animationDuration=".6s" />
        </div>
        <div class="permission-user-menu col-12 row p-4">
            <div class="col-sm-3 card p-2">
                <div class="card">
                    <template v-if="userGroup.length <= 0">
                        <p class="p-2 text-black-50">Không tìm thấy.</p>
                    </template>
                    <OrderList
                        v-model="userGroup"
                        style="height: calc(100vh - 150px)"
                        dataKey="id"
                        v-if="userGroup.length > 0"
                    >
                        <template #item="data">
                            <Button @click="async () => await selectedGroup(data)" class="p-button-text">
                                <div class="user-group">
                                    <div class="group-list-detail">
                                        <h5 class="mb-2 text-body">{{ data.item.nameGroup }}</h5>
                                    </div>
                                </div>
                            </Button>
                        </template>
                    </OrderList>
                    <div
                        v-if="userGroup.length > 0"
                        class="col-12 d-flex p-lg-3 justify-content-end position-absolute bottom-0"
                    >
                        <button
                            label="SAVE"
                            class="btn btn-primary"
                            @click="confirmSave()"
                            :disabled="!this.selectedGroupId"
                        >
                            Lưu
                        </button>
                        &nbsp;
                        <button label="Hủy" class="btn btn-secondary me-2" @click="cancel">Quay về</button>
                    </div>
                </div>
            </div>
            <div class="col-sm-9 card p-2">
                <div id="v-tabs" class="tab-panel tab-panel-module">
                    <div
                        v-for="module in listModule"
                        @click="selectModule(module)"
                        :class="{ active: selectedTab === module.id }"
                        class="tab-item col-12 d-flex"
                    >
                        <div class="col-sm-3 tab-item-sub">
                            <h4 class="tab-item__heading">{{ module.nameModule }}</h4>
                        </div>
                        <div class="col-sm-9 tab-item-sub">
                            <div class="col-12 d-flex">
                                <div class="col-sm-2 field-checkbox d-flex align-items-center">
                                    <input
                                        class="input-checkbox"
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked="module.access"
                                        :disabled="!this.selectedGroupId"
                                        @change="isSelectedCheck(module, $event.target.checked)"
                                    />
                                    <label :for="module.id" class="ms-3">Quyền truy cập</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <Dialog
            header="Truy cập bị từ chối!"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>Bạn không có quyền truy cập !</p>
            <medium
                >Bạn sẽ được điều hướng vào trang chủ <strong>{{ num }}</strong> giây!</medium
            >
            <template #footer>
                <Button label="Lưu" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>

<script>
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HTTP } from '@/http-common'
    import { ApiApplication, HttpStatus } from '@/config/app.config'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '@/router'

    export default {
        name: 'permissionUserGroup',
        data() {
            return {
                displayBasic: false,
                selectedGroupId: null,
                loading: true,
                token: null,
                selectedUserGroup: null,
                selectedTab: null,
                userGroup: [],
                listModule: [],
                selectedModule: [],
                selectedAccess: [],
                arrModuleAccess: [],
                arrModulePermissionAll: [],
                num: 5,
                arraySend : [],
                arraycompare : [],
            }
        },
        async mounted() {
            if (await UserRoleHelper.isAdmin()) {
                this.getUserGroup()
                this.getListModule()
                this.loading = false
            } else {
                this.loading = false
                this.countTime()
                this.displayBasic = true
            }
        },
        methods: {
            countTime() {
                if (this.num === 0) {
                    return this.submit()
                }
                this.num = this.num - 1
                this.timeout = setTimeout(() => this.countTime(), 1000)
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
            getUserGroup() {
                this.userGroup = []
                HTTP.get(ApiApplication.PERMISSION_USER_MENU.GET_ALL).then((res) => {
                    if (res.data) {
                        this.userGroup = res.data._Data
                    }
                })
            },
            
            async selectedGroup(data) {
                // this.loading = true
                // this.selectedGroupId = data.item.id
                // await this.getListModule()
                // const listModuleAccess = await HTTP.get(
                //     ApiApplication.PERMISSION_GROUP_MENU.GET_BY_USER_GROUP+ this.selectedGroupId,
                // )
                // if (listModuleAccess.status === HttpStatus.OK) {
                //     this.getUserMenuModule()
                //     this.listModule.map((module) => {
                //         listModuleAccess.data._Data.forEach((item) => {
                //             if (item.idModule === module.id && item.access) {
                //                 module['access'] = item.access
                //                 this.selectedAccess.push(module.id)
                //             }
                //         })
                //     })
                //     this.loading = false
                // } else {
                //     this.selectedAccess = []
                //     this.listModule = []
                //     this.loading = false
                // }
                this.arraycompare = []
                await this.getListModule()
                this.loading = true;
                this.selectedGroupId = data.item.id
                await HTTP.get("Permission_Groups/getPermissionGroup_By_IdGroup/"+this.selectedGroupId).then(res=>{
                       this.arraycompare = res.data._Data 
                       this.loading = false
                      }).catch(err=>console.log(err))

                      console.log(this.arraycompare);
                      
                        this.listModule.map(ele=>{
                            this.arraycompare.map(element=>{
                                    if(element.idModule === ele.id && element.access === true ){
                                        ele.access = true
                                    }
                                    if(element.idModule === ele.id && element.access === false){
                                        ele.access = false
                                    }
                             })
                            })
                
                    
            },
            // keep
            async getListModule() {
                this.listModule = []
                this.selectedAccess = []
                this.arraySend = []
                await HTTP.get(ApiApplication.MODULE.GET_ALL).then((res) => {
                    if (res.data) {
                        res.data._Data.map(ele=>{
                            const object = {
                                id : ele.id,
                                nameModule : ele.nameModule,
                                access : false, 
                            }
                            this.listModule.push(object)
                        })
                    }
                })
                
            },

            getPermissionModuleByIdGroup(){
                HTTP.get('Permission_Groups/getPermissionGroup_By_IdGroup/'+this.selectedGroupId).then(res=>{
                        this.arraycompare = res.data._Data
                    }).catch(err=>console.log(err))
            },


            getUserMenuModule() {
                this.arrModulePermissionAll = []
                if (this.selectedGroupId) {
                    HTTP.get(
                        ApiApplication.PERMISSION_USER_MENU.GET_USER_MENU  + this.selectedGroupId,
                    ).then((res) => {
                        if (res) {
                            this.listModule.map((module) => {
                                res.data._Data.forEach((item) => {
                                    if (item.idModule === module.id) {
                                        module['add'] = item.add
                                        module['edit'] = item.update
                                        module['delete'] = item.delete
                                        module['export'] = item.export
                                    }
                                })
                            })
                            this.listModule.filter((item) => {
                                if (item.id) {
                                    this.arrModulePermissionAll.push([
                                        0,
                                        item.id,
                                        item.add ?? 0,
                                        item.update ?? 0,
                                        item.delete ?? 0,
                                        item.export ?? 0,
                                    ])
                                }
                            })
                            this.loading = false
                        } else {
                            this.listModule = []
                        }
                    })
                }
            },
            cancel() {
                history.go(-1)
            },
            confirmSave() {
                if (this.arrModulePermissionAll.length > 0) {
                    this.$confirm.require({
                        message: `Quá trình xử lý có thể tốn vài phút. Bạn có muốn tiếp tục?`,
                        header: 'Xác nhận',
                        icon: 'pi pi-exclamation-triangle',
                        accept: () => {
                            this.save()
                        },
                    })
                } else {
                    this.$confirm.require({
                        message: 'Bạn có muốn tiếp tục?',
                        header: 'Xác nhận',
                        icon: 'pi pi-check',
                        accept: () => {
                            this.save()
                        },
                        reject: () => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Đã xảy ra lỗi',
                                life: 3000,
                            })
                        },
                        onHide: () => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Đã xảy ra lỗi',
                                life: 3000,
                            })
                        },
                    })
                }
            },
            save() {
                if (this.selectedGroupId) {
                    this.loading = true
                    const token = LocalStorage.jwtDecodeToken()
                    this.listModule.map(ele=>{
                        const item = {
                            "idModule": ele.id,
                            "access": ele.access
                        }
                        this.arraySend.push(item);
                })

                    HTTP.put(`Permission_Groups/updateMultiPermissionGroup/${this.selectedGroupId}`,this.arraySend)
                    .then((res) => {
                        if (res) {
                            this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Sửa thành công!',
                            life: 3000,
                            })
                            this.loading = false
                        } else {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Vui lòng kiểm tra lại dữ liệu!!!',
                                life: 3000,
                            })
                            this.getUserGroup()
                            this.getListModule()
                            this.selectedModule = null
                            this.selectedTab = null
                            this.selectedAccess = null
                            this.arrModuleAccess = []
                        }
                    }).catch(err=>{
                        console.log(err);
                    })
                    
                } else {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Lỗi',
                        detail: 'Cập nhật lỗi. Vui lòng liên hệ Admin.',
                        life: 3000,
                    })
                    this.getUserGroup()
                    this.getListModule()
                    this.selectedModule = null
                    this.selectedTab = null
                    this.selectedAccess = null
                    this.arrModuleAccess = []
                }
            },
            acceptPermissionUserMenu() {
                const token = LocalStorage.jwtDecodeToken()
                HTTP.post(
                    ApiApplication.PERMISSION_USER_MENU.ADD_BY_GROUP +
                        '?idGroup=' +
                        this.selectedGroupId +
                        '&idUserAdd=' +
                        token.Id,
                    this.arrModulePermissionAll,
                ).then((res) => {
                    if (res) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Thành công',
                            detail: 'Sửa thành công!',
                            life: 3000,
                        })
                        this.loading = false
                    } else {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Lỗi',
                            detail: 'Kiểm tra lại dữ liệu!',
                            life: 3000,
                        })
                        this.arrModulePermissionAll = []
                    }
                })
            },
            selectModule(module) {
                this.selectedModule = module
                this.selectedTab = module.id
                this.arrModuleAccess = []
                this.arrModuleAccess = this.selectedAccess
            },
            isSelectedCheck(module, isChecked) {
                    
                if (isChecked) {
                    module.access = true;
                } else {
                    module.access = false
                }

                const index = this.listModule.findIndex(({id})=> id === module.id)
                this.listModule[index].access = module.access
            },

        },
    }
</script>

<style lang="scss" scoped>
    .p-progress-spinner {
        display: flex;
        position: fixed;
        top: 50%;
        bottom: 0;
        left: 0;
        right: 0;
    }

    .spinner-loading {
        display: flex;
        position: fixed;
        top: 0;
        bottom: 0;
        left: 0;
        right: 0;
        background: white;
        z-index: 9999999;
        opacity: 0.5;
    }

    .permission-user-menu {
        ::v-deep(.p-orderlist .p-orderlist-list) {
            padding: 0;
        }

        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item) {
            padding: 0;
        }

        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item.p-highlight) {
            color: white;
            background: #33adff;
        }

        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item:focus) {
            box-shadow: none;
        }

        ::v-deep(.p-orderlist-controls) {
            display: none;
        }

        ::v-deep(.p-orderlist-header) {
            display: none;
        }

        ::v-deep(.p-orderlist-list) {
            max-height: 100%;
            height: calc(100vh - 150px);
        }

        ::v-deep(.p-datatable-thead) {
            display: none;
        }

        ::v-deep(td.p-selection-column) {
            max-width: min-content;
        }

        body {
            font-family: 'Roboto', sans-serif;
            padding: 50px;
            background: #fcfcfc;
            height: 100vh;
        }

        label {
            cursor: pointer;
        }

        .p-button.p-button-text {
            width: 100%;
        }

        .input-checkbox {
            border: 2px solid #ced4da;
            background: #ffffff;
            width: 22px;
            height: 22px;
            color: #495057;
            border-radius: 6px;
            cursor: pointer;
            transition: background-color 0.2s, color 0.2s, border-color 0.2s, box-shadow 0.2s;
        }

        .tab-panel-module {
            height: calc(100vh - 146px);
            overflow-y: scroll;
        }

        .pen-heading {
            font-weight: bold;
            font-size: 4em;
            text-align: center;
            margin-bottom: 40px;
            color: #333;
        }

        .tab-item {
            background: white;
            border: 1px #d5dadf solid;
            border-left: 5px solid #d5dadf;
            box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
            border-radius: 3px;
            cursor: pointer;
            transition: all 0.2s ease;
            margin-bottom: 15px;
        }

        .tab-item-sub {
            border: 1px #d5dadf solid;
            padding: 30px;
        }

        .tab-item.active {
            box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
            border-left: 5px solid #33adff;
        }

        .tab-item:hover {
            box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
            background-color: var(--blue-100);
        }

        .tab-item__heading {
            font-weight: bold;
            line-height: 1.3em;
            letter-spacing: 0.02em;
            color: #314f8d;
            margin: 0;
        }

        ::-webkit-scrollbar {
            width: 20px;
        }

        ::-webkit-scrollbar-track {
            background-color: transparent;
        }

        ::-webkit-scrollbar-thumb {
            background-color: #d6dee1;
            border-radius: 20px;
            border: 6px solid transparent;
            background-clip: content-box;
        }

        ::-webkit-scrollbar-thumb:hover {
            background-color: #a8bbbf;
        }
    }
</style>
