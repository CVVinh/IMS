<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <div class="mx-3 mt-3">
            <DataTable
                :value="data"
                :paginator="true"
                ref="dt"
                class="p-datatable-customers"
                sortField="dateCreate"
                :sortOrder="-1"
                :rows="10"
                :loading="loading"
                dataKey="x.id"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedOT"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[5, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="[
                    'x.id',
                    'x.date',
                    'x.start',
                    'x.end',  
                    'x.realTime',
                    'x.status',
                    'x.description',
                    'x.leadCreate',
                    'name',
                    'nameLead',
                    'nameUser',
                    'nameUserUpdate',
                    'x.dateCreate',
                    'x.updateUser',
                    'x.dateUpdate',
                    'x.note',
                    'x.user',
                    'x.idProject',
                ]"
                responsiveLayout="scroll"
            >
                <!-- Header -->
                <template #header>
                    <h5 style="color: white">OVERTIME LIST</h5>
                    <div class="flex align-items-center">
                        <div class="header-left">
                            <Button @click="exportToExcel()" label="Export" icon="pi pi-file-excel" class="size__Button"
                            v-if="showButton.ExportButton"
                            />
                            <Button label="Add OT" icon="pi pi-plus" @click="add()" class="size__Button left" 
                            v-if="showButton.addButton"
                            />
                            <Button label="Accept" icon="pi pi-check-square" @click="acceptMulti()" class="size__Button left"
                            v-if="showButton.confirmButton"
                            />
                            <div class="p-input-icon-left left" style="display: inline">
                                <i class="pi pi-search" />
                                <InputText v-model="filters['global'].value" placeholder="Keyword Search"  />
                            </div>
                        </div>
                        <div class="filter-pro">
                            <div class="filter-pro-input">
                                <div class="filter-pro-top"></div>
                                <Button
                                    type="button"
                                    style="background-color: antiquewhite"
                                    icon="pi pi-filter-slash"
                                    class="p-button-outlined right"
                                    @click="clearFilter()"
                                />
                                <Dropdown
                                    class="p-column-filter filter-pro-item right"
                                    v-model="selectedMonth"
                                    :options="selectMonth"
                                    optionLabel="label"
                                    placeholder="Select Month"
                                    :filter="true"
                                    filterPlaceholder="Find Month"
                                    @change="filterByMonthAndProject()"
                                />
                                <Dropdown
                                    class="p-column-filter filter-pro-item right"
                                    v-model="selectedProject"
                                    :options="selectProject"
                                    optionLabel="name"
                                    placeholder="Select Project"
                                    :filter="true"
                                    filterPlaceholder="Find Project"
                                    @change="filterByMonthAndProject()"
                                />
                                <MultiSelect
                                    class="filter-pro-item"
                                    :modelValue="selectedColumns"
                                    :options="columns"
                                    optionLabel="header"
                                    @update:modelValue="onToggle"
                                    placeholder="Select Columns"
                                />
                            </div>
                        </div>
                    </div>
                </template>
                <template #empty> No OT found. </template>

                <!-- Body -->
                <Column selectionMode="multiple" headerStyle="width: 2rem"></Column>
                <Column header="#">
                    <template #body="{ index }">
                        {{ index + 1 }}
                    </template>
                </Column>
                <Column field="x.date" header="OT Date" sortable dataType="date" style="min-width: 7.5rem">
                    <template #body="{ data }">
                        {{  getFormattedDate(new Date(data.x.date)) }}
                    </template>
                </Column>
                <Column field="x.user" header="OT User" sortable style="min-width: 4rem">
                    <template #body="{ data }">
                        {{ data.nameUser }}
                    </template>
                </Column>
                <Column field="x.leadCreate" header="Lead" sortable style="min-width: 6rem">
                    <template #body="{ data }">
                        {{ data.nameLead }}
                    </template>
                </Column>
               
                <Column field="x.realTime" header="OT Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.realTime }}
                    </template>
                </Column>
                <Column field="x.start" header="Start Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.start }}
                    </template>
                </Column>
                <Column field="x.end" header="End Time" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.x.end }}
                    </template>
                </Column>
                
                 <Column
                    field="x.description"
                    header="Description"
                    sortable
                    :filterMenuStyle="{ width: '14rem' }"
                    style="min-width: 10rem; width: 10rem"
                >
                    <template #body="{ data }">
                        {{ data.x.description }}
                    </template>
                </Column> 
                <!-- <Column field="updateUser" header="Approved by" sortable style="min-width: 10rem">
                    <template #body="{ data }">
                        {{ data.x.updateUser }}
                    </template>
                </Column> -->
                <Column
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="col.field + '_' + index"
                ></Column>
                <Column field="x.idProject" header="Project" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ data.name }}
                    </template>
                </Column>
                <Column field="x.dateCreate" header="Create Date" sortable dataType="date" style="min-width: 9.5rem">
                    <template #body="{ data }">
                        {{ getFormattedDate(new Date(data.x.dateCreate)) }}
                    </template>
                </Column>
                <Column field="" header="Action" style="max-width: 12rem">
                    <template #body="{ data }">
                        <div v-if="data.x.status == 0"> 
                            <!-- confirm  -->
                            <Edit icon="pi pi-check" @click="accept(true, data.x.id, data.x.leadCreate)" class="right" 
                            v-if="showButton.confirmButton"
                            /> 
                            <!--  VIEW  -->
                            <Button icon="pi pi-eye" @click="OpenDetailOT(data)" class="right top p-button-sm" 
                            v-if="showButton.viewButton"
                            />
                            <!-- Edit  -->
                            <Edit @click="EditData(data.x.id)" class="right top" 
                            v-if="showButton.editButton"
                            > </Edit>   
                            <!-- Refuse   -->
                            <Delete icon="pi pi-times" @click="accept(false, data.x.id, data.x.leadCreate)" :class=" showButton.refuseButton === true ? 'right top' : 'right'"
                            v-if="showButton.refuseButton"
                            /> 
                            <!--  Delete -->
                            <Delete @click="confirmDelete(data.x.id, token)" class="right top"
                            v-if="showButton.deleteButton"
                            ></Delete>
                        </div>
                    </template>
                </Column>
                <Column
                    field="x.status"
                    header="Status"
                    sortable
                    :filterMenuStyle="{ width: '14rem' }"
                    style="min-width: 6rem"
                >
                    <template #body="{ data }">
                        <span :class="'badge ' + color(data.x.status)">{{ statuses[data.x.status].text }}</span>
                    </template>
                </Column>
            </DataTable>
        </div>
        <Dialog
            header="Access is denied!"
            :visible="displayDialog1"
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
                <Button label="No" icon="pi pi-times" @click="closeBasic" class="p-button-text" />
                <Button label="Yes" icon="pi pi-check" @click="onSubmit(data.leadCreate)" autofocus />
            </template>
        </Dialog>
        <DetailOT :show="DetailOT" @open="OpenDetailOT" @close="CloseDetailOT" :OTS="ots"  :isPm="isPM" @reloadAPI="getAllOT"
            @alert="showSuccess" :displayDialog2="displayDialog2" :id="id" :lead="lead" @OpenFormRefuse="OpenFormRefuse"
        />
        
    </LayoutDefaultDynamic>
</template>

<script>
    import Edit from '@/components/buttons/Edit.vue'
    import Export from '@/components/buttons/Export.vue'
    import router from '@/router/index'
    import axios from 'axios'
    import { LocalStorage } from '@/helper/local-storage.helper'
    import { HTTP } from '@/http-common.js'
    import { FilterMatchMode } from 'primevue/api'
    import Delete from '@/components/buttons/Delete.vue'
    import jwtDecode from 'jwt-decode'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { HttpStatus } from '@/config/app.config'
    import DetailOT from './DetailOT.vue'
import { cloneVNode } from '@vue/runtime-core'
import storeRole from '@/stores/role'
    export default {
        name: 'ots',
        data() {
            return {
                selectedOT: null,
                data: null,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    status: { value: null, matchMode: FilterMatchMode.EQUALS },
                },
                selectedColumns: null,
                columns: null,
                project: [],
                proName: [],
                proPM: [],
                user: [],
                username: [],
                cantAdd: false,
                token: null,
                rolePM: true,
                displayDialog1: false,
                displayDialog2: false,
                num: 5,
                timeout: null,
                canAccept: true,
                entered: false,
                reason: '',
                id: '',
                lead: '',
                statuses: [
                    { num: 0, text: 'Waiting' },
                    { num: 1, text: 'Accepted' },
                    { num: 2, text: 'Denied' },
                    { num: 3, text: 'Deleted' },
                ],
                userInfo: [],
                selectMonth: [],
                selectedMonth: null,
                selectProject: [],
                selectedProject: null,
                //axios: 'http://api.imsdemo.tk/api/',
                axios: import.meta.env.VITE_VUE_API_URL,
                DetailOT : false,
                ots : 0,
                isPM : false,
                showButton : {
                    confirmButton : false,
                    viewButton : false,
                    editButton : false,
                    refuseButton : false,
                    deleteButton : false,
                    addButton : false,
                    allAcceptButton : false,
                    ExportButton : false,
                },
                loading : true,
                
            }

        },
        async mounted() {
            try {
                this.token = LocalStorage.jwtDecodeToken()
                await UserRoleHelper.isAccessModule(this.$route.path.replace('/', ''))
                if (await UserRoleHelper.isAccess) {
                    
                    HTTP.get("Project/getAllProject").then(res => {                    
                            if (res.status === HttpStatus.OK){
                                res.data.forEach((element) => {          
                                    this.proName[element.id] = element.name
                                    if (!element.isDeleted && !element.isFinished) {
                                        this.project.push(element)
                                        this.proPM[element.name] = element.userId
                                    }
                                })
                            }           
                        }).catch(err=>{
                            console.log(err);
                        })
                    setTimeout(() => {
                        this.getAllOT()
                    }, 2000)
                } else {
                    this.countTime()
                    this.displayDialog1 = true
                }
                this.columns = [
                    { field: 'dateUpdate', header: 'Approve Date' },
                    { field: 'note', header: 'Note' },
                ]
                this.getMonthFrom()
                this.getProject()
            } catch (error) {
                this.countTime()
                this.displayDialog1 = true
            }      
            
        },
        methods: {
            // GET OTS BY ROLE PM
            getOTsByPM(idPM){
                HTTP.get(`OTs/getOTsByidPM/${idPM}`)
                .then(res=>{
                    this.data = res.data;
                })
                .catch(err=>console.log(err))
            },
            // GET OTS BY ROLE LEAD
            getOTsByLead(idLEAD){
                HTTP.get(`OTs/GetAllOTsByLead/${idLEAD}`)
                .then(res=>{
                    this.data = res.data;
                })
                .catch(err=>console.log(err))
            },
            // GET OTS BY ROLE STAFF
            getOTsByStaff(idSTAFF){
                HTTP.get(`OTs/GetAllOTsByStaff/${idSTAFF}`)
                .then(res=>{
                    this.data = res.data;
                })
                .catch(err=>console.log(err))
            },
            // GET OTS BY ROLE SAMPLE
            getOTsBySample(){
                HTTP.get("OTs/GetAllOTs")
                .then(res=>{
                    this.data = res.data;
                }).catch(err=>console.log(err))
            },

            getFormattedDate(date) {
                var year = date.getFullYear();

                var month = (1 + date.getMonth()).toString();
                month = month.length > 1 ? month : '0' + month;

                var day = date.getDate().toString();
                day = day.length > 1 ? day : '0' + day;

                return day + '-' + month + '-' + year;
            },
            acceptMulti() {
                let bool = true
                if (this.selectedOT == null) {
                    this.showWarn('Please select an OT to Accept!')
                    return
                }
                this.selectedOT.forEach((element) => {
                    if (element.status != 0) {
                        bool = false
                    }
                })
                if (bool) {
                    this.selectedOT.forEach((element) => {
                        this.accept(true, element.x.id, element.x.leadCreate)
                    })
                } else this.showWarn("Can not accept OTs that don't have Waiting status!")
                this.selectedOT = []
            },
            countTime() {
                if (this.num == 0) {
                    this.submit()
                    return
                }
                this.num = this.num - 1
                this.timeout = setTimeout(() => this.countTime(), 1000)
            },
            submit() {
                clearTimeout(this.timeout)
                router.push('/')
            },
            onToggle(value) {
                this.selectedColumns = this.columns.filter((col) => value.includes(col))
            },
            formatDate(value) {
                return new Date(value).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
            },
            async getAllOT() {
                let Role = 0;
                if(this.token){
                    Role=  this.token.IdGroup;
                }

                if(storeRole.getters.getKey() === null){
                    if(Role == 1) {
                        storeRole.state.key === 'admin'
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.getOTsByPM(this.token.Id)
                    }
                    if(Role == 2) {
                        storeRole.state.key === 'sample'
                        this.showButton.ExportButton = true;
                        this.getOTsBySample()
                    }
                    if(Role == 3) {
                        storeRole.state.key === 'lead'
                        this.showButton.ExportButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true;  
                        this.getOTsByLead(this.token.Id)
                    }
                    if(Role == 4) {
                        storeRole.state.key === 'staff'
                        this.showButton.viewButton = true;  
                        this.getOTsByStaff(this.token.Id)
                    }
                    if(Role == 5) {
                        storeRole.state.key === 'pm'
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.isPM = true;
                        this.getOTsByPM(this.token.Id)
                    }
                }else{
                    if(storeRole.getters.getKey() == 'pm') {    
                        this.getOTsByPM(this.token.Id)
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.isPM = true;
                        //console.log("pm");
                    }
                    if(storeRole.getters.getKey() == 'lead') {
                        this.getOTsByLead(this.token.Id)
                        this.showButton.ExportButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true; 
                        //console.log("lead");
                    }
                    if(storeRole.getters.getKey() =='staff') {
                        this.getOTsByStaff(this.token.Id)
                        this.showButton.viewButton = true;  
                        //console.log("staff");
                    }
                    if(storeRole.getters.getKey() =='sample') {
                        //console.log("sample");
                        this.showButton.ExportButton = true;

                        this.getOTsBySample()
                    }  
                    if(storeRole.getters.getKey() =='admin') {
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.getOTsBySample()
                    } 
                    
                }
                this.loading = false
                this.CheckButtonFlowGroup(Role);

               
            },
            getUsername(userId) {
                if (userId && userId !== 0) {
                    for (var user of this.userInfo) {
                        if (user.id == userId) {
                            const arrName = user.lastName.split(' ')
                            var fullName = user.firstName + ' '
                            arrName.forEach((x) => {
                                fullName += x.substring(0, 1) + '.'
                            })
                            return fullName
                        }
                    }
                }
            },
            async getUserInfo() {
                HTTP.get('Users/getInfo').then(res=>{this.userInfo = res.data}).catch(err=>console.log(err))
            },
            async canEdit(status, token, lead) {
                let user = this.getUsername(token.Id)
                if (status == 1 || status == 3) return true
                if (lead != user || !(await UserRoleHelper.isLeader())) return true
                return false
            },
            CheckButtonFlowGroup(value){
                if(value == 1) {       
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.getOTsByPM(this.token.Id)
                    }
                    if(value == 2) {
                      
                        this.showButton.ExportButton = true;
                        this.getOTsBySample()
                    }
                    if(value == 3) {
                    
                        this.showButton.ExportButton = true;
                        this.showButton.editButton = true;
                        this.showButton.addButton = true;
                        this.showButton.deleteButton = true;
                        this.showButton.viewButton = true;  
                        this.getOTsByLead(this.token.Id)
                    }
                    if(value == 4) {
                      
                        this.showButton.viewButton = true;  
                        this.getOTsByStaff(this.token.Id)
                    }
                    if(value == 5) {
                        
                        this.showButton.ExportButton = true;
                        this.showButton.confirmButton = true;
                        this.showButton.allAcceptButton = true;
                        this.showButton.viewButton = true;  
                        this.showButton.refuseButton = true;
                        this.getOTsByPM(this.token.Id)
                    }
            },
            deleteData(id, token) {
                HTTP.put('OTs/deleteOT?' + 'idOT=' + id + '&PM=' + token)
                .then ((res) => {
                    if (res.status == 200) {
                        this.showSuccess("Xóa thẻ OTs thành công.");
                    }
                })
                .catch((err) => {
                    this.showWarn("Bạn không có quyền thực hiện thao tác xóa OT.")
                })
            },
            EditData: (id) => {
                router.push({ name: 'editOT', params: { id: id } })
            },
            color(status) {
                if (status == 0) return 'bg-primary'
                if (status == 1) return 'bg-success'
                if (status == 2) return 'bg-secondary'
                return 'bg-danger'
            },
            add() {
                router.push('/ots/addOT')
            },
            confirmDelete(id, token) {
                this.$confirm.require({
                    message: 'Do you want to delete this OT?',
                    header: 'Delete Confirmation',
                    icon: 'pi pi-question-circle',
                    acceptClass: 'p-button-danger',
                    accept: () => {
                        this.deleteData(id, token.Id)
                        setTimeout(() => {
                            this.getAllOT()
                        }, 500)
                    },
                    reject: () => {
                        return
                    },
                })
            },
            showSuccess(err) {
                this.$toast.add({ severity: 'success', summary: 'Success Message', detail: err, life: 3000 })
            },
            exportToExcel() {
                var month = 0;
                var year = 0;
                var idProject = 0;
                if (this.selectedMonth != null) {
                    month = this.selectedMonth.month;
                    year = this.selectedMonth.year;
                }
                if (this.selectedProject != null)
                    idProject = this.selectedProject.code;
                HTTP.get('OTs/exportExcel/month=' +
                                month +
                                '&year=' +
                                year +
                                '&idProject=' +
                                idProject)
                    .then((res) => {
                        if (res.status == 200) {
                            this.$toast.add({
                                severity: 'success',
                                summary: 'Successfully',
                                detail: 'File excel đã được lưu thành công.',
                                life: 3000,
                            })
                            window.location = res.data
                        }
                    })
                    .catch((err) => {
                        this.showWarn('Bạn không có quyền thực hiện thao tác xuất file excel!'); 
                    })
            },
            accept(accepted, id, lead) {
                let user = jwtDecode(localStorage.getItem('token'))
                this.PM = user.Id
                if (accepted) {
                    this.status = 1
                    HTTP.put('OTs/acceptOT', { id: id, status: this.status, pm: this.PM }).then(res=>{
                        this.showSuccess()
                    }).catch(err=>{
                        console.log(err);
                    })
                   
                    setTimeout(() => {
                        this.getAllOT()
                    }, 500)
                } else {
                    this.displayDialog2 = true
                    this.id = id
                    this.lead = lead
                }
            },
            closeBasic() {
                this.displayDialog2 = false
                this.entered = false
            },
            onSubmit() {
                if (this.reason != null && this.reason != '') {
                    HTTP.put('OTs/acceptOT', { id: this.id, status: 2, PM: this.PM, note: this.reason }).then(
                        async (res) => {
                            if (res.status == 200) {
                                this.closeBasic()
                                this.showSuccess()
                                await this.getAllOT()
                            }
                        },
                    )
                } else this.entered = true
            },
            clearFilter() {
                this.filters = {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    status: { value: null, matchMode: FilterMatchMode.EQUALS },
                }
                this.getAllOT()
                this.selectedMonth = null
                this.selectedProject = null
            },
            showWarn(message) {
                this.$toast.add({ severity: 'warn', summary: 'Warn Message', detail: message, life: 5000 })
            },
            getMonthFrom() {
                const now = new Date()
                var code = 1
                for (var y = 2020; y <= now.getFullYear(); y++) {
                    for (var m = 1; m <= 12; m++) {
                        if (y == now.getFullYear() && m > now.getMonth() + 1) {
                            break
                        }
                        this.selectMonth.push({ id: code, month: m, year: y, label: m + '/' + y })
                        code++
                    }
                }
            },
            FilterbyMonth() {
                this.selectedProject = null
                HTTP.get('OTs/getOTByMonth/month=' +
                            this.selectedMonth.month +
                            '&year=' +
                            this.selectedMonth.year)    .then((res) => {
                        if (res.data) {
                            this.data = res.data
                            this.data.forEach((element) => {
                                element.date = this.formatDate(element.date)
                                element.dateCreate = this.formatDate(element.dateCreate)
                                element.dateUpdate = this.formatDate(element.dateUpdate)
                                setTimeout(() => {
                                    element.user = this.getUsername(element.user)
                                    element.leadCreate = this.getUsername(element.leadCreate)
                                    element.updateUser = this.getUsername(element.updateUser)
                                }, 100)
                                element.idProject = this.proName.at(element.idProject)
                            })
                        }
                    })
            },
            FilterbyProject() {
                this.selectedMonth = null
                HTTP.get('OTs/getOTByProject/' + this.selectedProject.code).then((res) => {
                        if (res.data) {
                            this.data = res.data
                            this.data.forEach((element) => {
                                element.date = this.formatDate(element.x.date)
                                element.x.dateCreate = this.formatDate(element.x.dateCreate)
                                element.x.dateUpdate = this.formatDate(element.x.dateUpdate)
                                setTimeout(() => {
                                    element.x.user = this.getUsername(element.x.user)
                                    element.x.leadCreate = this.getUsername(element.x.leadCreate)
                                    element.x.updateUser = this.getUsername(element.x.updateUser)
                                }, 100)
                                element.x.idProject = this.proName.at(element.x.idProject)
                            })
                        }
                    })
            },
            filterByMonthAndProject() {
                if (this.selectedMonth == null) this.FilterbyProject()
                else if (this.selectedProject == null) this.FilterbyMonth()
                else {
                    HTTP.get('OTs/GetByMonthAndProject/month=' +
                                this.selectedMonth.month +
                                '&year=' +
                                this.selectedMonth.year +
                                '&idProject=' +
                                this.selectedProject.code).then((res) => {
                            if (res.data) {
                                this.data = res.data
                                this.data.forEach((element) => {
                                    element.x.date = this.formatDate(element.x.date)
                                    element.x.dateCreate = this.formatDate(element.x.dateCreate)
                                    element.x.dateUpdate = this.formatDate(element.x.dateUpdate)
                                    setTimeout(() => {
                                        element.x.user = this.getUsername(element.x.user)
                                        element.x.leadCreate = this.getUsername(element.x.leadCreate)
                                        element.x.updateUser = this.getUsername(element.x.updateUser)
                                    }, 100)
                                    element.x.idProject = this.proName.at(element.x.idProject)
                                })
                            }
                        })           
                }
            },
            getProject() {
                HTTP.get("Project/getAllProject").then(res=>{
                if (res.data) {
                            res.data.forEach((element) => {
                                this.selectProject.push({ code: element.id, name: element.name })
                            })
                        }
            }).catch(err=>{
                console.log(err)
            })
            },
            OpenDetailOT(id) {
                this.DetailOT = true;
                this.ots = id
            },
            CloseDetailOT () {
                this.DetailOT = false;
            },
            OpenFormRefuse(){
                this.displayDialog2 = true;
            }


            
        },
        components: { Edit, Delete, Export, DetailOT },
    }
</script>

<style lang="scss" scoped>
    ::v-deep(.p-paginator) {
        .p-paginator-current {
            margin-left: auto;
        }
    }

    ::v-deep(.p-input-icon-left) {
        margin-top: -2px;
        i:first-of-type {
            left: 1rem;
            color: #6c757d;
        }
    }

    ::v-deep(.p-inputtext) {
        margin: 0 4px 0 3px;
        padding-top: 0.613rem;
        padding-right: 0.75rem;
        padding-bottom: 0.613rem;
        padding-left: 2.5rem;
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

    .project_Search_box {
        margin-top: 10px;
        display: flex;
        justify-content: end;
    }

    ::v-deep(.p-datatable.p-datatable-customers) {
        .p-datatable-header {
            padding: 1rem;
            text-align: left;
            font-size: 1.5rem;
            overflow-x: auto;
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

        .p-dropdown-label.p-placeholder {
            padding-top: 6px;
        }

        .p-dropdown-label.p-inputtext {
            padding-left: 0px;
        }

        .p-dropdown-label:not(.p-placeholder) {
            text-transform: uppercase;
            padding-top: 6px;
        }

        .p-multiselect {
            height: 40px;

            .p-multiselect-label {
                height: 100%;
                padding: 0.41rem 0.41rem;
            }
        }

        .p-datatable-header {
            background-color: #607d8b;
            height: auto;
        }
    }

    .header-div {
        height: auto;
    }

    .filter-pro-input {
        display: flex;
        max-width: 900px;
    }

    ::v-deep(.p-dropdown) {
        height: 40px;
        position: static;
    }
    ::v-deep(.p-inputtext.p-component){
        height: 38px;
    }
    .p-button-outlined {
        height: 40px;
    }

    .flex {
        display: flex;
        justify-content: space-between;
    }

    .filter-pro {
        display: flex;
        height: auto;
        float: right;
    }

    .filter-pro-item {
        width: 190px;
    }

    .header-left {
        height: 100%;
        display: flex;
        flex-direction: row;
        justify-content: end;
        align-items: flex-end;
        margin-right: 10px;
        width: max-content;
    }

    .right{
        margin-right: 10px;
    }
    .left {
        margin-left: 10px;
    }

    .size__Button{
        padding: 0.4rem;
        font-size: 14px;
    }
    .top{
        margin-top:6.5px;
    }
</style>
