<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>

        <div class="mx-3 mt-3">
            <DataTable
                ref="tableRule"
                class="p-datatable-customers"
                :value="listRule"
                :sort="1"
                :paginator="true"
                :loading="isLoading"
                responsiveLayout="scroll"
                filterDisplay="menu"
                :rowHover="true"
                :rows="10"
                dataKey="idRule"
                :rowsPerPageOptions="[5, 10, 25, 50]"
                v-model:filters="filters"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :exportFilename="'List_Rules_Effort Summary Report_' + new Date()"
                :globalFilterFields="[
                    'title',
                    'applyDay',
                    'expiredDay',
                    'content',
                    'userCreated',
                    'dateCreated',
                    'userUpdated',
                    'dateUpdated',
                ]"
            >
                <template #header>
                    <h5 style="color: white">Rule Info</h5>
                    <div class="header-container">
                        <div class="button-group">
                            <Add label="Add Rule" @click="openAdd" style="margin-right: 5px"
                            v-if="showButton.add"
                            />
                            <Export label="Export" @click="exportCSV($event)" />
                        </div>
                        <div class="input-text">
                            <!-- <MultiSelect
                            v-model="selectedColumns"
                            :options="columns"
                            optionLabel="header"
                            placeholder="Select Columns"
                            style="width: 20em; height: 100%; margin-right: 15px"
                        /> -->

                            <Button
                                type="button"
                                style="background-color: antiquewhite"
                                icon="pi pi-filter-slash"
                                class="p-button-outlined right me-2"
                                @click="handlerReload"
                            />

                            <span class="p-input-icon-left">
                                <i class="pi pi-search" />
                                <InputText
                                    class="p-inputtext-sm"
                                    v-model="keySearch"
                                    placeholder="Keyword Search"
                                    style="width: 20em; height: 100%; font-size: 16px"
                                />
                            </span>
                        </div>
                    </div>
                </template>

                <template #loading> </template>

                <template #empty>
                    <div v-if="this.isLoading" style="display: flex; justify-items: flex-end">
                        <ProgressSpinner style="width: 42px" />
                    </div>
                    <div v-else>No rule found.</div>
                </template>

                <Column field="#" header="#" dataType="date">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>

                <Column field="title" header="Title" sortable style="min-width: 15rem">
                    <template #body="{ data }">
                        {{ data.title }}
                    </template>
                </Column>

                <Column field="dateCreated" header="Date Created" sortable>
                    <template #body="{ data }">
                        {{ data.dateCreated }}
                    </template>
                </Column>

                <Column field="applyDay" header="Apply Day" sortable>
                    <template #body="{ data }">
                        {{ data.applyDay }}
                    </template>
                </Column>

                <Column field="expiredDay" header="Expired Day" sortable>
                    <template #body="{ data }">
                        {{ data.expiredDay }}
                    </template>
                </Column>

                <Column
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="index"
                    style="min-width: 14rem"
                ></Column>

                <Column field="" header="Actions" style="width: 10rem; text-align: left">
                    <template #body="{ data }">
                        <div class="actions-buttons">

                            <Button
                                icon="pi pi-eye"
                                class="p-button p-component p-button-success"
                                @click="openDetailt(data)"
                                v-if="showButton.view"
                            ></Button>
                            &nbsp;
                            <Button
                                icon="pi pi-pencil"
                                class="p-button p-component p-button-primary"
                                @click="openEdit(data)"
                                v-if="showButton.edit"
                            ></Button>
                            &nbsp;
                            <Button
                                icon="pi pi-trash"
                                class="p-button p-component p-button-danger"
                                @click="confirmDelete(data.id)"
                                v-if="showButton.delete"
                            ></Button>
                            &nbsp;
                            <Button
                                v-if="data.pathFile && showButton.download"
                                icon="pi pi-download"
                                class="p-button p-component p-button-warning"
                                @click="downloadFile(data.pathFile)"
                            ></Button>
                        </div>
                    </template>
                </Column>
            </DataTable>
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

        <AddRuleDiaLog :statusopen="openAddform" @closeAdd="closeAdd()" @reloadpage="GetAllRuleList" />
        <EditRuleDiaLog
            :statusopen="openEditform"
            @closeEdit="closeEdit()"
            :dataRuleById="dataRuleById"
            @reloadpage="GetAllRuleList"
        />

        <DetailtRuleDiaLog :statusopen="openDetailtform" @closeDetailt="closeDetailt()" :dataRuleById="dataRuleById" />
    </LayoutDefaultDynamic>
</template>

<script>
    import Export from '../../components/buttons/Export.vue'
    import Edit from '../../components/buttons/Edit.vue'
    import Add from '../../components/buttons/Add.vue'
    import View from '../../components/buttons/View.vue'
    import Delete from '../../components/buttons/Delete.vue'
    import { HTTP, HTTP_LOCAL } from '@/http-common'
    import { FilterMatchMode } from 'primevue/api'
    import AddRuleDiaLog from './AddRuleDiaLog.vue'
    import EditRuleDiaLog from './EditRuleDiaLog.vue'
    import DetailtRuleDiaLog from './DetailtRuleDiaLog.vue'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HttpStatus } from '@/config/app.config'
    import { ref } from 'vue'
    import { DateHelper } from '@/helper/date.helper'
    import { saveAs } from 'file-saver'

    export default {
        name: 'RuleInfo',

        data() {
            return {
                dataRuleById: null,
                listRule: [],
                selectedColumns: null,
                openAddform: false,
                openEditform: false,
                openDetailtform: false,
                isLoading: true,
                displayBasic: false,
                keySearch: null,
                num: 5,
                timeOut: null,
                token: null,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                },
                columns: [
                    { field: 'userCreated', header: 'User Created' },
                    { field: 'userUpdated', header: 'User Updated' },
                    { field: 'dateUpdated', header: 'Date Updated' },
                ],
                showButton : {
                    view : false,
                    edit : false,
                    delete : false,
                    download : false,
                    addl:false,
                },
            }
        },
        watch: {
            keySearch: {
                handler: async function change(newVal) {
                    this.listRule = null

                    if (newVal == null || newVal == '') {
                        await this.GetAllRuleList()
                    } else {
                        this.isLoading = true
                        await this.getRuleByName()
                    }
                },
                deep: true,
            },
        },
        methods: {
            async handlerReload() {
                this.keySearch = null
                this.listRule = null
                this.isLoading = true
                await this.GetAllRuleList()
            },

            formatData() {
                this.listRule.forEach((element) => {
                    element.applyDay = DateHelper.formatDate(element.applyDay)
                    element.expiredDay = DateHelper.formatDate(element.expiredDay)
                    element.dateCreated = DateHelper.formatDate(element.dateCreated)
                    element.dateUpdated = DateHelper.formatDate(element.dateUpdated)
                })
            },

            async getRuleByName() {
                await HTTP.get('Rules/searchRulesByTitle/' + this.keySearch).then((res) => {
                    this.listRule = res.data._Data
                    this.formatData()
                    this.isLoading = false
                })
            },

            async GetAllRuleList() {
                await HTTP.get('Rules/getAllRules').then((res) => {
                    this.listRule = res.data._Data
                    this.formatData()
                    this.isLoading = false
                })
            },

            confirmDelete(id) {
                this.$confirm.require({
                    message: 'Do you want to delete this rule? ',
                    header: 'Delete Confirmation',
                    icon: 'pi pi-exclamation-triangle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteRule(id)
                    },
                    reject: () => {
                        return
                    },
                })
            },

            deleteRule(ruleID) {
                let API_URL = 'Rules/deleteRules/' + ruleID
                HTTP.put(API_URL, {
                    idUser: this.decode.Id,
                })
                    .then((res) => {
                        if (res.status == HttpStatus.OK) {
                            this.listRule = []
                            this.GetAllRuleList()
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Success Message',
                                detail: 'Delete rule success !!!',
                                life: 2000,
                            })
                        }
                    })
                    .catch((error) => {
                        this.$toast.add({ severity: 'warn', summary: 'Warning Message', detail: error, life: 2000 })
                    })
            },

            countTime() {
                if (this.num == 0) {
                    this.navigationToHome()
                    return
                }
                this.num = this.num - 1
                this.timeOut = setTimeout(() => this.countTime(), 1000)
            },

            navigationToHome() {
                clearTimeout(this.timeOut)
                this.$router.push({ name: 'home' })
            },

            exportCSV() {
                this.$refs.tableRule.exportCSV()
            },

            openEdit(dataRuleById) {
                this.dataRuleById = dataRuleById
                this.openEditform = true
            },

            closeEdit() {
                this.openEditform = false
            },

            openAdd() {
                this.openAddform = true
            },

            closeAdd() {
                this.openAddform = false
            },

            openDetailt(dataRuleById) {
                this.dataRuleById = dataRuleById
                this.openDetailtform = true
            },

            closeDetailt() {
                this.openDetailtform = false
            },

            downloadFile(url) {
                var nameFile = url.substring(url.lastIndexOf('/') + 1)
                HTTP.get(url, { responseType: 'blob' })
                    .then((response) => {
                        saveAs(response.data, nameFile)
                    })
                    .catch((error) => {
                        if (error.code == 'ERR_BAD_REQUEST') {
                            this.$toast.add({
                                severity: 'error',
                                summary: 'Error',
                                detail: 'File không tồn tại!',
                                life: 3000,
                            })
                        }
                    })
            },
        },
        async mounted() {
          
            try{
                this.token = LocalStorage.jwtDecodeToken()
                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if (UserRoleHelper.isAccess) {
                    if(Number(this.token.IdGroup) === 2 ){
                        this.showButton.view = true;
                        this.showButton.edit = true;
                        this.showButton.add = true
                        this.showButton.delete = true;
                        this.showButton.download = true;
                    }
                    if(Number(this.token.IdGroup) !== 2){
                        this.showButton.download = true;
                    }
                      this.GetAllRuleList()
                }else{
                    this.countTime()
                    this.displayBasic = true
                }
            }catch(err){
                this.countTime()
                 this.displayBasic = true
            }          
        },

        components: { Export, Add, Edit, View, Delete, AddRuleDiaLog, EditRuleDiaLog, DetailtRuleDiaLog },
    }
</script>

<style lang="scss" scoped>
    ::v-deep(.p-paginator) {
        .p-paginator-current {
            margin-left: auto;
        }
    }

    ::v-deep(.p-progressbar) {
        height: 0.5rem;
        background-color: #d8dadc;

        .p-progressbar-value {
            background-color: #607d8b;
        }
    }

    ::v-deep(.p-datepicker) {
        min-width: 25rem;

        td {
            font-weight: 400;
        }
    }

    ::v-deep(.p-datatable.p-datatable-customers) {
        .p-datatable-header {
            padding: 1rem;
            text-align: left;
            font-size: 1.5rem;
        }

        .p-paginator {
            padding: 1rem;
        }

        .p-datatable-thead > tr > th {
            text-align: left;
        }

        .p-datatable-tbody > tr > td {
            cursor: auto;
        }

        .p-dropdown-label:not(.p-placeholder) {
            text-transform: uppercase;
        }

        .p-datatable-header {
            background-color: #607d8b;
        }
    }
    .header-container {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 1rem;
        height: 40px;
    }
    .input-text {
        display: flex;
        height: 45px;
    }
    .actions-buttons {
        display: flex;

        .btn-margin {
            margin-right: 5px;
        }
    }
</style>
