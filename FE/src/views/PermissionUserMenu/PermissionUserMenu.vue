<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <template #header>
            <div class="flex justify-content-center">
                <h5 class="" style="color: white">Danh sách dự án</h5> 
                <div class="inline">
                    <Export label="Xuất Excel" class="me-2" @click="exportToExcel()" />
                    <Button @click="finishMulti()" label="Lưu" class="p-button-sm me-2" icon="pi pi-check" />
                    <Add @click="openDialogAdd()" label="Thêm" v-if="canAddProject()" />
                    <div class="p-input-icon-left layout-left">
                        <i class="pi pi-search" />
                        <InputText class="p-inputtext-sm" v-model="filters['global'].value" placeholder="Tìm kiếm" />
                    </div>
                    <div class="layout-left">
                        <MultiSelect
                            :modelValue="selectedColumns"
                            :options="columns"
                            optionLabel="header"
                            @update:modelValue="onToggle"
                            placeholder="Chọn"
                            style="width: 20em"
                        />
                    </div>
                </div>
            </div>
        </template>
        <div class="permission-user-menu col-12 row p-4">
            <div class="col-sm-2 card p-2">
                <Dropdown
                    class="mb-2"
                    v-model="selectedUserGroup"
                    :options="userGroup"
                    optionLabel="nameGroup"
                    optionValue="id"
                    @change="onChangeUserGroup()"
                    placeholder="Chọn nhóm người dùng"
                />
                <div class="card" style="height: calc(100vh - 200px)">
                    <template v-if="listUser.length <= 0">
                        <p class="p-2 text-black-50">Không tìm thấy.</p>
                    </template>
                    <OrderList
                        v-model="listUser"
                        style="height: calc(100vh - 270px)"
                        dataKey="id"
                        v-if="listUser.length > 0"
                    >
                 
                        <template #item="data">
                            <Button @click="selectedAUser(data.item.idUser)" class="p-button-text">
                                <div class="user-group">
                                    <div class="group-list-detail">
                                        <h5 class="mb-2 text-body">
                                            <!-- {{ data.item.lastName }} {{ data.item.firstName }} -->
                                            {{ data.item.idUser }}
                                        </h5>
                                    </div>
                                </div>
                            </Button>
                        </template>
                    </OrderList>
                    <div class="col-12 d-flex p-lg-3 justify-content-end position-absolute bottom-0">
                        <button
                            label="Lưu"
                            icon="pi pi-check"
                            class="btn btn-primary"
                            @click="confirmSave()"
                            :disabled="!selectedUser"
                        >
                            Lưu
                        </button>
                        &nbsp;
                        <button label="Hủy" class="btn btn-secondary" @click="cancel">Quay về</button>
                    </div>
                </div>
            </div>
            <div class="col-sm-9 card p-2">
                <div class="spinner-loading" v-if="loading">
                    <ProgressSpinner style="width: 50px; height: 50px" strokeWidth="3" animationDuration=".6s" />
                </div>
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

                       <div class="col-sm-10 tab-item-sub">
                       
                             <div class="col-12 d-flex justify-content-left">
                                <itemPermissionActionModule
                                    :module = module
                                    :canCheck = this.selectedUser
                                />
                                
                            </div> 
                        </div>
                    </div>
                </div>
            </div>
         
        </div>
        <Dialog
            header="Không có quyền truy cập !"
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
    import { HTTP } from '@/http-common'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { ApiApplication, HttpStatus } from '@/config/app.config'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import router from '@/router'
    import itemPermissionActionModule from './itemPermissionActionModule.vue'
    export default {
    components: { itemPermissionActionModule },
        name: 'PermissionUserMenu',
        data() {
            return {
                displayBasic: false,
                num: 5,
                loading: true,
                selectedId: null,
                token: null,
                selectedUserGroup: null,
                selectedTab: null,
                userGroup: [],
                listUser: [],
                listModule: [],
                selectedUser: null,
                selectedModule: [],
                arrModuleAll: [],
            }
        },
        async mounted() {
            
           

            if (await UserRoleHelper.isAdmin()) {
                console.log('run');
                this.getUserGroup()
                await this.getListModule()
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
            getListActionModule(){
                HTTP.get("")
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
            checkAll(module, isChecked) {
                this.listModule.forEach((item) => {
                    if (module.id === item.id) {
                        item.all = isChecked
                        item.add = isChecked
                        item.edit = isChecked
                        item.delete = isChecked
                        item.export = isChecked
                        if (isChecked) {
                            this.arrModuleAll.push([0, module.id, 1, 1, 1, 1])
                        } else {
                            item.add = false
                            item.edit = false
                            item.delete = false
                            item.export = false
                            this.arrModuleAll.push([0, module.id, 0, 0, 0, 0])
                        }
                    }
                })
            },
            async getUserMenuModule() {
                await this.getListModule()
                const listModuleAccess = await HTTP.get('Permission_Groups/getPermissionGroup_By_IdModule/' + this.selectedUserGroup)
                
                if (listModuleAccess.status === HttpStatus.OK) {
                    this.listModule.map((module) => {
                        listModuleAccess.data._Data.forEach((item) => {
                            if (item.idModule === module.id) {
                                module['access'] = item.access
                            }
                        })
                    })
                }
                const listModulePermission = await HTTP.get(
                    'Permission_Use_Menus/getPermissionUserMenuWithUserId/' + this.selectedUser,
                )
                if (listModulePermission.status === HttpStatus.OK) {
                    this.listModule.map((module) => {
                        listModulePermission.data._Data.forEach((item) => {
                            if (item.idModule === module.id) {
                                module['add'] = item.add
                                module['edit'] = item.update
                                module['delete'] = item.delete
                                module['export'] = item.export
                                if ((item.add && item.update && item.delete && item.export) === 1) {
                                    module['all'] = true
                                }
                                this.arrModuleAll.push([0, module.id, item.add, item.update, item.delete, item.export])
                            }
                        })
                    })
                    this.loading = false
                } else {
                    this.listModule = []
                }
            },
            getUserGroup() {
                this.userGroup = []
                HTTP.get(ApiApplication.PERMISSION_USER_MENU.GET_ALL).then((res) => {
                    if (res.data) {
                        this.userGroup = res.data._Data
                    }
                })
            },
            async onChangeUserGroup() {
                this.loading = true
                this.arrModuleAll = []
                this.selectedUser = null
                await this.getListModule()
                if (this.selectedUserGroup) {
                    HTTP.get('User_Group/getUserGroupWithGroupId' + `/${this.selectedUserGroup}`).then(
                        (res) => {
                            if (res) {
                                this.listUser = res.data._Data
                                this.loading = false
                            }
                        },
                    )
                }
            },
           async selectedAUser(data) {
                this.loading = true
                this.selectedUser = data
            await    this.getListModule();


            await HTTP.get("/Permission_Use_Menus/getPermissionUserMenuWithUserId/"+data).then(res=>{
                    console.log(res.data._Data);
                    if(res.data._Data.length > 0){
                        this.listModule.map(ele=>{
                            res.data._Data.map(element=>{
                            if(ele.id === element.idModule){
                                 ele.add = element.add 
                                 ele.addMember = element.addMember
                                 ele.confirm = element.confirm
                                 ele.confirmMulti = element.confirmMulti
                                 ele.delete = element.delete
                                 ele.deleteMulti = element.deleteMulti
                                 ele.export = element.export
                                 ele.refuse = element.refuse
                                 ele.update = element.update
                              }
                            })
                         })
                    }
                }).catch(err=>console.log(err));
                console.log(this.listModule);
                this.loading = false
            },
            selectedGroup(data) {
                this.selectedId = data.id
            },
            async getListModule() {
                this.listModule = []
                try {
                    const result = await HTTP.get(ApiApplication.MODULE.GET_ALL)
                    const data = result.data._Data
                    if (data) {
                        const array = []
                        await data.forEach((item) => {
                            if (item.isDeleted === 0) {
                                array.push(item)
                            }
                        })
                        this.listModule = array
                    }
                } catch (error) {}
            },
            confirmSave() {
                
                if (this.selectedUser && this.listModule.length > 0) {
                    this.$confirm.require({
                        message: 'Bạn có muốn tiếp tục?',
                        header: 'Xác nhận',
                        icon: 'pi pi-check',
                        accept: () => {
                            this.save()
                        },
                        onHide: () => {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Lỗi',
                                detail: 'Đã xảy ra lỗi',
                                life: 3000,
                            })
                            this.selectedAUser(this.selectedUser)
                            this.arrModuleAll = []
                        },
                    })
                } else {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Lỗi',
                        detail: 'Vui lòng kiểm tra lại dữ liệu!!!',
                        life: 3000,
                    })
                }
            },
            save() {
                this.listModule.map(ele=>{
                    const newPermission = {
                        "add": ele.add,
                        "update": ele.update,
                        "delete": ele.delete,
                        "deleteMulti": ele.deleteMulti,
                        "confirm": ele.confirm,
                        "confirmMulti": ele.confirmMulti,
                        "refuse": ele.refuse,
                        "addMember": ele.addMember,
                        "export": ele.export,
                        "userModified": ele.userModified
                    }

                    HTTP.put(`Permission_Use_Menus/updatePermissionUserMenu/${this.selectedUser}/${ele.id}/${this.selectedUserGroup}`,newPermission)
                    .then(res=>console.log(res.data))
                    .catch(err=>console.log(err))
                })

                // if (this.selectedUser && this.arrModuleAll.length > 0) {
                //     this.loading = true
                //     const token = LocalStorage.jwtDecodeToken()
                //     HTTP.post(
                //         ApiApplication.PERMISSION_USER_MENU.ADD_BY_USER +
                //             '?idUser=' +
                //             this.selectedUser +
                //             '&idUserAdd=' +
                //             token.Id,
                //         this.arrModuleAll,
                //     ).then((res) => {
                //         if (res) {
                //             this.$toast.add({
                //                 severity: 'success',
                //                 summary: 'Thành công',
                //                 detail: 'Sửa thành công!',
                //                 life: 3000,
                //             })
                //             this.loading = false
                //         } else {
                //             this.$toast.add({
                //                 severity: 'error',
                //                 summary: 'Lỗi',
                //                 detail: 'Kiểm tra lại dữ liệu!',
                //                 life: 3000,
                //             })
                //             this.selectedAUser(this.selectedUser)
                //             this.arrModuleAll = []
                //         }
                //     })
                // } else {
                //     this.$toast.add({
                //         severity: 'error',
                //         summary: 'Lỗi',
                //         detail: 'Cập nhật lỗi. Vui lòng liên hệ Admin.',
                //         life: 3000,
                //     })
                //     this.selectedAUser(this.selectedUser)
                //     this.arrModuleAll = []
                // }
            },
            selectModule(module) {
                this.selectedModule = module
                this.selectedTab = module.id
            },
            isSelectedCheck(module, nameLabel, isChecked) {
                if (isChecked) {
                    this.selectedModule[nameLabel] = isChecked
                    this.arrModuleAll.push([
                        0,
                        module.id,
                        this.selectedModule.add ? 1 : 0,
                        this.selectedModule.edit ? 1 : 0,
                        this.selectedModule.delete ? 1 : 0,
                        this.selectedModule.export ? 1 : 0,
                    ])
                } else {
                    this.selectedModule[nameLabel] = isChecked
                    this.arrModuleAll.map((item) => {
                        if (item[1] === this.selectedModule.id) {
                            switch (this.selectedModule[nameLabel]) {
                                case this.selectedModule['add']:
                                    item[2] = 0
                                    break
                                case this.selectedModule['edit']:
                                    item[3] = 0
                                    break
                                case this.selectedModule['delete']:
                                    item[4] = 0
                                    break
                                case this.selectedModule['export']:
                                    item[5] = 0
                                    break
                                default:
                            }
                        }
                    })
                }
                if (!module.add || !module.edit || !module.delete || !module.export) {
                    module.all = 0
                    this.selectedModule.add = module.add
                    this.selectedModule.edit = module.edit
                    this.selectedModule.delete = module.delete
                    this.selectedModule.export = module.export
                } else {
                    module.all = 1
                    this.selectedModule.add = module.add
                    this.selectedModule.edit = module.edit
                    this.selectedModule.delete = module.delete
                    this.selectedModule.export = module.export
                }
            },
            cancel() {
                history.go(-1)
            },
        },
        components:{itemPermissionActionModule}
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
        .p-button.p-button-text {
            width: -webkit-fill-available;
        }
        ::v-deep(.p-orderlist .p-orderlist-list) {
            padding: 0;
        }
        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item.p-highlight) {
            color: white;
            background: #33adff;
        }
        ::v-deep(.p-orderlist .p-orderlist-list .p-orderlist-item) {
            padding: 0;
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
            height: calc(100vh - 200px);
        }
        ::v-deep(.p-datatable-thead) {
            display: none;
        }

        ::v-deep(.p-datatable-thead) {
            display: none;
        }

        ::v-deep(td.p-selection-column) {
            max-width: min-content;
        }

        .tabview-custom {
            i,
            span {
                vertical-align: middle;
            }

            span {
                margin: 0 0.5rem;
            }
        }

        .p-tabview p {
            line-height: 1.5;
            margin: 0;
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
            border-right: 1px #d5dadf solid;
            padding: 20px;
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

        .tab-item__subheading {
            font-size: 18px;
            color: #333;
            margin: 0;
        }

        .tab-content__header {
            color: #314f8d;
            font-weight: bold;
            margin: 0 0 30px;
            font-size: 36px;
            line-height: 1.3em;
            letter-spacing: 0.02em;
        }

        .tab-content__text {
            margin: 0 0 30px;
            font-size: 1.25em;
        }

        .tab-content__btn {
            display: inline-block;
            margin-bottom: 30px;
            padding: 16px 50px;
            cursor: pointer;
            text-decoration: none;
            font-size: 14px;
            text-transform: uppercase;
            font-weight: 900;
            position: relative;
            transition: all 0.3s ease;
            text-align: center;
            line-height: 1;
            border-radius: 3px;
            background-color: transparent;
            box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
            color: #33adff;
            fill: #33adff;
            border: 2px solid #33adff;
        }

        .tab-content__btn:hover {
            color: #33adff;
            text-decoration: none;
            box-shadow: 0 3px 3px 2px rgba(213, 218, 223, 0.35);
        }

        .tab-content__testimonial {
            margin-bottom: 15px;
            font-size: 1em;
            color: rgba(0, 0, 0, 0.75);
            font-style: italic;
        }

        .tab-content__testimonial-author {
            margin-bottom: 5px;
            font-size: 1em;
            color: rgba(0, 0, 0, 0.75);
            font-weight: bold;
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
