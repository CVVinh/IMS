<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class='permission-user-menu col-12 row p-4'>
            <div class='col-sm-3 card p-2'>
                <Dropdown
                    class='mb-2'
                    v-model='selectedUserGroup'
                    :options='userGroup'
                    optionLabel='nameGroup'
                    optionValue='id'
                    @change='onChangeUserGroup()'
                    placeholder='Select a User Group' />
                <div class='card' style="height: calc(100vh - 200px)">
                    <template v-if='listUser.length <=0'>
                       <p class='p-2 text-black-50'> No user found.</p>
                    </template>
                    <OrderList v-model="listUser" style="height: calc(100vh - 270px)" dataKey="id" v-if='listUser.length > 0'>
                        <template #item="data">
                            <Button @click='selectedAUser(data.item.id)' class="p-button-text">
                                <div class='user-group'>
                                    <div class='group-list-detail'>
                                        <h5 class='mb-2 text-body'>{{ data.item.lastName }} {{ data.item.firstName }}</h5>
                                    </div>
                                </div>
                            </button>
                        </template>
                    </OrderList>
                    <div class='col-12 d-flex p-lg-3 justify-content-end position-absolute bottom-0'>
                        <button label='CANCEL' class="btn btn-secondary" @click="cancel">Cancel</button>
                        &nbsp;
                        <button label='SAVE' class="btn btn-primary" @click="confirmSave()" :disabled="!selectedUser">Save</button>
                    </div>
                </div>
            </div>
            <div class='col-sm-9 card p-2'>
                <div class='spinner-loading' v-if='loading'>
                    <ProgressSpinner style='width:50px; height:50px' strokeWidth='3' animationDuration='.6s' />
                </div>
                <div id='v-tabs' class='tab-panel tab-panel-module'>
                    <div v-for='module in listModule' @click='selectModule(module)'
                         :class='{ active : selectedTab === module.id }' class='tab-item col-12 d-flex'>
                        <div class='col-sm-4 tab-item-sub'>
                            <h4 class='tab-item__heading'>{{ module.nameModule }}</h4>
                        </div>
                        <div class='col-sm-8 tab-item-sub'>
                            <div class='col-12 d-flex justify-content-center'>
                                <div class='col-sm-2 field-checkbox d-flex align-items-center justify-content-center'>
                                    <input
                                        :disabled="!module.access"
                                        class='input-checkbox'
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked='module.all ? module.all : module.add'
                                        @change="isSelectedCheck(module, 'add', $event.target.checked)"
                                    />
                                    <label for='add' class='ms-3'>Add</label>
                                </div>
                                <div class='col-sm-2 field-checkbox d-flex align-items-center justify-content-center'>
                                    <input
                                        :disabled="!module.access"
                                        class='input-checkbox'
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked='module.all ? module.all : module.edit'
                                        @change="isSelectedCheck(module, 'edit', $event.target.checked)"
                                    />
                                    <label for='edit' class='ms-3'>Edit</label>
                                </div>
                                <div class='col-sm-2 field-checkbox d-flex align-items-center justify-content-center'>
                                    <input
                                        :disabled="!module.access"
                                        class='input-checkbox'
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked='module.all ? module.all : module.delete'
                                        @change="isSelectedCheck(module, 'delete', $event.target.checked)"
                                    />
                                    <label for='delete' class='ms-3'>Delete</label>
                                </div>
                                <div class='col-sm-2 field-checkbox d-flex align-items-center justify-content-center'>
                                    <input
                                        :disabled="!module.access"
                                        class='input-checkbox'
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked='module.all ? module.all : module.export'
                                        @change="isSelectedCheck(module, 'export', $event.target.checked)"
                                    />
                                    <label for='export' class='ms-3'>Export</label>
                                </div>
                                <div class='col-sm-4 field-checkbox d-flex align-items-center justify-content-center'>
                                    <input
                                        :disabled="!module.access"
                                        class='input-checkbox'
                                        type="checkbox"
                                        :id="module.id"
                                        :value="module.id"
                                        v-bind:checked='module.all'
                                        @change="checkAll(module, $event.target.checked)"
                                    />
                                    <label for='all' class='ms-3'>All</label>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <Dialog
            header="Access is denied!"
            :visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '30vw' }"
            :modal="true"
            :closable="false"
        >
            <p>You do not have permission to access this page!</p>
            You will be redirected to the homepage in <strong>{{ num }}</strong> seconds!
            <template #footer>
                <Button label="Yes" icon="pi pi-check" @click="submit" autofocus />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP } from '@/http-common';
import { LocalStorage } from '@/helper/local-storage.helper';
import { ApiApplication, HttpStatus } from "@/config/app.config";
import { UserRoleHelper } from '@/helper/user-role.helper'
import router from "@/router";

export default {
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
            this.getUserGroup();
            await this.getListModule();
            this.loading = false;
        } else {
            this.loading = false;
            this.countTime();
            this.displayBasic = true;
        }
    },
    methods: {
        countTime() {
            if (this.num === 0) {
                return this.submit();
            }
            this.num = this.num - 1;
            this.timeout = setTimeout(() => this.countTime(), 1000);
        },
        submit() {
            clearTimeout(this.timeout);
            router.push('/');
        },
        checkAll(module, isChecked){
            this.listModule.forEach(item => {
                if (module.id === item.id) {
                    item.all = isChecked;
                    item.add = isChecked;
                    item.edit = isChecked;
                    item.delete = isChecked;
                    item.export = isChecked;
                    if (isChecked) {
                        this.arrModuleAll.push([0, module.id, 1, 1, 1, 1]);
                    } else {
                        item.add = false;
                        item.edit = false;
                        item.delete = false;
                        item.export = false;
                        this.arrModuleAll.push([0, module.id, 0, 0, 0, 0]);
                    }
                }
            })
        },
        async getUserMenuModule() {
            await this.getListModule()
            const listModuleAccess = await HTTP.get(ApiApplication.PERMISSION_GROUP_MENU.GET_BY_USER_GROUP + "?idGroup=" + this.selectedUserGroup)
            if (listModuleAccess.status === HttpStatus.OK) {
                this.listModule.map(module => {
                    listModuleAccess.data.forEach(item => {
                        if (item.idModule === module.id) {
                            module['access'] = item.access;
                        }
                    })
                })
            }
            const listModulePermission = await HTTP.get(ApiApplication.PERMISSION_USER_MENU.GET_USER_MENU + "?idUser=" + this.selectedUser)
            if (listModulePermission.status === HttpStatus.OK) {
                this.listModule.map(module => {
                    listModulePermission.data.forEach(item => {
                        if (item.idModule === module.id) {
                            module['add'] = item.add;
                            module['edit'] = item.update;
                            module['delete'] = item.delete;
                            module['export'] = item.export;
                            if ((item.add && item.update && item.delete && item.export) === 1) {
                                module['all'] = true;
                            }
                            this.arrModuleAll.push([0, module.id, item.add, item.update, item.delete, item.export]);
                        }
                    });
                });
                this.loading = false;
            } else {
                this.listModule = [];
            }
        },
        getUserGroup() {
            this.userGroup = [];
            HTTP.get(ApiApplication.PERMISSION_USER_MENU.GET_ALL).then(res => {
                if (res.data) {
                    this.userGroup = res.data;
                }
            });
        },
        async onChangeUserGroup() {
            this.loading = true;
            this.arrModuleAll = [];
            this.selectedUser = null;
            await this.getListModule();
            if (this.selectedUserGroup) {
                HTTP.get(ApiApplication.PERMISSION_USER_MENU.GET_USER_BY_GROUP + `/${this.selectedUserGroup}`).then(res => {
                    if (res.data) {
                        this.listUser = res.data;
                        this.loading = false;
                    }
                });
            }
        },
        selectedAUser(data) {
            this.loading = true;
            this.selectedUser = data;
            this.arrModuleAll = [];
            if (data) {
                this.getUserMenuModule();
            }
        },
        selectedGroup(data) {
            this.selectedId = data.id;
        },
        async getListModule() {
            this.listModule = [];
            try {
                const result = await HTTP.get(ApiApplication.MODULE.GET_ALL)
                const data = result.data;
                if (data) {
                    const array = [];
                    await data.forEach(item => {
                        if (item.isDeleted === 0) {
                            array.push(item);
                        }
                    });
                    this.listModule = array;
                }
            } catch (error) {
            }
           
        },
        confirmSave() {
            if (this.selectedUser && this.arrModuleAll.length > 0) {
                this.$confirm.require({
                    message: 'Are you sure you want to proceed?',
                    header: 'Confirmation',
                    icon: 'pi pi-check',
                    accept: () => {
                        this.save();
                    },
                    onHide: () => {
                        this.$toast.add({ severity: 'error', summary: 'Hide', detail: 'You have hidden', life: 3000 });
                        this.selectedAUser(this.selectedUser);
                        this.arrModuleAll = [];
                    },
                })
            } else {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Rejected',
                    detail: 'Invalid data. Please check again!!!',
                    life: 3000,
                });
            }
        },
        save() {
            if (this.selectedUser && this.arrModuleAll.length > 0) {
                this.loading = true;
                const token = LocalStorage.jwtDecodeToken()
                HTTP.post(ApiApplication.PERMISSION_USER_MENU.ADD_BY_USER
                    + "?idUser=" + this.selectedUser + "&idUserAdd="
                    + token.Id, this.arrModuleAll
                ).then(res => {
                    if (res) {
                        this.$toast.add({
                            severity: 'success',
                            summary: 'Success Message',
                            detail: 'Update successful',
                            life: 3000,
                        })
                        this.loading = false;
                    } else {
                        this.$toast.add({
                            severity: 'error',
                            summary: 'Rejected',
                            detail: 'Invalid data. Please check again!!!',
                            life: 3000,
                        });
                        this.selectedAUser(this.selectedUser);
                        this.arrModuleAll = [];
                    }
                });
            } else {
                this.$toast.add({
                    severity: 'error',
                    summary: 'Rejected',
                    detail: 'Update failed. Please contact the administrator.',
                    life: 3000,
                });
                this.selectedAUser(this.selectedUser);
                this.arrModuleAll = [];
            }
        },
        selectModule(module) {
            this.selectedModule = module;
            this.selectedTab = module.id;
        },
        isSelectedCheck(module, nameLabel, isChecked) {
            if (isChecked) {
                this.selectedModule[nameLabel] = isChecked;
                this.arrModuleAll.push([
                    0,
                    module.id,
                    this.selectedModule.add ? 1 : 0,
                    this.selectedModule.edit ? 1 : 0,
                    this.selectedModule.delete ? 1 : 0,
                    this.selectedModule.export ? 1 : 0,
                ]);
            } else {
                this.selectedModule[nameLabel] = isChecked;
                this.arrModuleAll.map(item => {
                    if (item[1] === this.selectedModule.id) {
                        switch (this.selectedModule[nameLabel]) {
                            case this.selectedModule['add']:
                                item[2] = 0;
                                break;
                            case this.selectedModule['edit']:
                                item[3] = 0;
                                break;
                            case this.selectedModule['delete']:
                                item[4] = 0;
                                break;
                            case this.selectedModule['export']:
                                item[5] = 0;
                                break;
                            default:
                        }
                    }
                });
            }
            if (!module.add || !module.edit || !module.delete || !module.export) {
                module.all = 0;
                this.selectedModule.add = module.add;
                this.selectedModule.edit = module.edit;
                this.selectedModule.delete = module.delete;
                this.selectedModule.export = module.export;
            } else {
                module.all = 1;
                this.selectedModule.add = module.add;
                this.selectedModule.edit = module.edit;
                this.selectedModule.delete = module.delete;
                this.selectedModule.export = module.export;
            }
        },
        cancel() {
            history.go(-1);
        },
    },
}
</script>

<style lang='scss' scoped>

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
        i, span {
            vertical-align: middle;
        }

        span {
            margin: 0 .5rem;
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
        border: 1px #D5DADF solid;
        border-left: 5px solid #D5DADF;
        box-shadow: 0 2px 3px rgba(213, 218, 223, 0.35);
        border-radius: 3px;
        cursor: pointer;
        transition: all .2s ease;
        margin-bottom: 15px;
    }

    .tab-item-sub {
        border-right: 1px #D5DADF solid;
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
        transition: all .3s ease;
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
        color: rgba(0, 0, 0, .75);
        font-style: italic;
    }

    .tab-content__testimonial-author {
        margin-bottom: 5px;
        font-size: 1em;
        color: rgba(0, 0, 0, .75);
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
