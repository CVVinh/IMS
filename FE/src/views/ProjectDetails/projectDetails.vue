<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4">
            <Breadcrumb :home="timeSheet" :model="itemsTimeSheet" />
        </div>
        <div class="mx-4 mt-3">
            <div v-if="filteredArr.length > 0">
                <Card style="width: 100%; margin-bottom: 2em; background-color: #d3d3d382">
                    <template #content>
                        <DataTable
                            :value="filteredArr"
                            :loading="loading"
                            :rows="5"
                            :paginator="true"
                            showGridlines="true"
                            responsiveLayout="scroll"
                            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                            :rowsPerPageOptions="[5, 10, 15, 30]"
                            currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                        >
                            <template #header>
                                <div class="d-flex justify-content-between">
                                    <div class="d-flex">
                                        <Dropdown
                                            v-model="idProject"
                                            :options="arr"
                                            optionLabel="name"
                                            :filter="true"
                                            placeholder="Select a Project"
                                            :showClear="true"
                                        >
                                        </Dropdown>
                                        <Button
                                            class="p-button-outlined p-button-secondary ms-2"
                                            @click="reload(idProject)"
                                        >
                                            <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                                        ></Button>
                                    </div>
                                    <div class="d-flex align-items-center">
                                        <span class="me-1">Start Date</span>
                                        <div class="me-3">
                                            <InputText type="date" v-model="startDate" />
                                        </div>
                                        <span class="me-1">End Date</span>
                                        <div class="me-2">
                                            <InputText type="date" v-model="endDate" />
                                        </div>
                                        <div>
                                            <Button
                                                class="p-button-outlined p-button-info"
                                                @click="filterItems"
                                                icon="pi pi-filter-fill"
                                            />
                                        </div>
                                    </div>
                                </div>
                            </template>
                            <template #empty> No project found. </template>
                            <template #loading>
                                <ProgressSpinner />
                            </template>
                            <div>
                                <Column field="taskName" header="Task">
                                    <template #body="{ data }">
                                        {{ data.title }}
                                    </template>
                                </Column>
                                <Column field="status" header="Status">
                                    <template #body="{ data }"> {{ data.state }} </template>
                                </Column>
                                <Column field="estimate" header="Time Estimate">
                                    <template #body="{ data }"> {{ data.time_stats.time_estimate / 3600 }}h </template>
                                </Column>
                                <Column field="spent" header="Time Spent">
                                    <template #body="{ data }">
                                        {{ data.time_stats.total_time_spent / 3600 }}h
                                    </template>
                                </Column>
                                <Column field="dueDate" header="Due Date">
                                    <template #body="{ data }"> {{ data.due_date }}</template>
                                </Column>
                            </div>
                        </DataTable>
                    </template>
                </Card>
            </div>
            <div v-else>
                <Card style="width: 100%; margin-bottom: 2em; background-color: #d3d3d382">
                    <template #content>
                        <DataTable
                            :value="dataIssue"
                            :loading="loading"
                            :rows="5"
                            :paginator="true"
                            showGridlines="true"
                            responsiveLayout="scroll"
                            paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                            :rowsPerPageOptions="[5, 10, 15, 30]"
                            currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                        >
                            <template #header>
                                <div class="d-flex justify-content-between">
                                    <div class="d-flex">
                                        <Dropdown
                                            v-model="idProject"
                                            :options="arr"
                                            optionLabel="name"
                                            :filter="true"
                                            placeholder="Select a Project"
                                            :showClear="true"
                                        >
                                        </Dropdown>
                                        <Button
                                            class="p-button-outlined p-button-secondary ms-2"
                                            @click="reload(idProject)"
                                        >
                                            <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                                        ></Button>
                                    </div>
                                    <div class="d-flex align-items-center">
                                        <span class="me-1">Start Date</span>
                                        <div class="me-3">
                                            <InputText type="date" v-model="startDate" />
                                        </div>
                                        <span class="me-1">End Date</span>
                                        <div class="me-2">
                                            <InputText type="date" v-model="endDate" />
                                        </div>
                                        <div>
                                            <Button
                                                class="p-button-outlined p-button-info"
                                                @click="filterItems"
                                                icon="pi pi-filter-fill"
                                            />
                                        </div>
                                    </div>
                                </div>
                            </template>
                            <template #empty> No project found. </template>
                            <template #loading>
                                <ProgressSpinner />
                            </template>
                            <div>
                                <Column field="taskName" header="Task">
                                    <template #body="{ data }">
                                        {{ data.title }}
                                    </template>
                                </Column>
                                <Column field="status" header="Status">
                                    <template #body="{ data }"> {{ data.state }} </template>
                                </Column>
                                <Column field="estimate" header="Time Estimate">
                                    <template #body="{ data }"> {{ data.time_stats.time_estimate / 3600 }}h </template>
                                </Column>
                                <Column field="spent" header="Time Spent">
                                    <template #body="{ data }">
                                        {{ data.time_stats.total_time_spent / 3600 }}h
                                    </template>
                                </Column>
                                <Column field="dueDate" header="Due Date">
                                    <template #body="{ data }"> {{ data.due_date }}</template>
                                </Column>
                            </div>
                        </DataTable>
                    </template>
                </Card>
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
        <DialogAddEdit
            :isOpenDialog="isOpenDialog"
            :projectSelected="{ ...projectSelected }"
            @closeDialog="closeDialog"
            @getAllProject="getAllProject"
            :user="user"
            :leader="leader"
        />
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP_API_GITLAB, PROJECT_MEMBER, GET_ALL_ISSUE, GET_ALL_PROJECT } from '@/http-common'
    export default {
        created() {
            //this.getdataIssue(),
            this.getAllProject()
        },
        data() {
            return {
                usersData: [],
                dataIssue: [],
                arr: [],
                loading: true,
                defaultPageSize: 100,
                idProject: null,
                startDate: null,
                endDate: null,
                dataIssueNotNull: [],
                dataIssueNull: [],
                dataIssueDetails: [],
                displayBasic: false,
                titleProject: null,
                filteredArr: [],
                timeSheet: { label: 'Report', icon: 'me-1 bx bx-spreadsheet' },
                itemsTimeSheet: [{ label: 'Project Detail', to: '/projectDetails' }],
            }
        },
        watch: {
            idProject: {
                handler: async function Change(newval, olval) {
                    this.dataIssue = []
                    //await this.checkvalue(newval)
                    if (newval == null) {
                        this.loading = false
                    } else {
                        await this.getdataIssue(newval)
                    }
                },
            },
            deep: true,
        },
        methods: {
            checkDate(startDate, endDate) {
                if (startDate > endDate) {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'error',
                        detail: 'End date need to be bigger than start date !',
                        life: 3000,
                    })
                    return true
                }
                return false
            },
            async filterItems() {
                if (this.checkDate(this.startDate, this.endDate)) {
                    this.loading = false
                } else {
                    var filterArr = await this.dataIssue.filter(function (el) {
                        return el.updated_at !== null && el.assignee !== null && el.time_stats.total_time_spent > 0
                    })
                    var filterDate = await filterArr.filter((el) => {
                        //return el.due_date >= this.startDate && el.due_date <= this.endDate
                        return (
                            this.dateToYMD(el.updated_at) >= this.startDate &&
                            this.dateToYMD(el.updated_at) <= this.endDate
                        )
                    })
                    this.filteredArr = filterDate
                    if (!this.filteredArr.length) {
                        this.dataIssue = []
                    }
                }
            },
            async getAllProject() {
                let resultCount = 100
                let results = []
                let page = 1
                while (resultCount === 100) {
                    let newResults = await this.getAllPageProject(page)
                    page++
                    resultCount = newResults.length
                    this.arr.push(...newResults)
                    results = results.concat(newResults)
                }
                this.loading = false
            },
            getAllPageProject(page) {
                return HTTP_API_GITLAB.get(GET_ALL_PROJECT(this.defaultPageSize, page)).then((res) => res.data)
            },
            getAllPageIssue(id, page) {
                return HTTP_API_GITLAB.get(GET_ALL_ISSUE(id, this.defaultPageSize, page)).then((res) => res.data)
            },
            async getdataIssue(id) {
                this.loading = true
                this.filteredArr = []
                let resultCount = 100
                let results = []
                let page = 1

                HTTP_API_GITLAB.get(PROJECT_MEMBER(id.id)).then((res) => {
                    res.data
                        .filter((item) => item.id != 1)
                        .forEach((el) => {
                            this.usersData.push({
                                id: el.id,
                                name: el.name,
                                username: el.username,
                                total_time_spent: 0,
                                time_estimate: 0,
                            })
                        })
                })
                while (resultCount === 100) {
                    let newResults = await this.getAllPageIssue(id.id, page)
                    page++
                    resultCount = newResults.length
                    newResults.forEach((el) => {
                        this.dataIssue.push({
                            assignee: el.assignee,
                            created_at: el.created_at,
                            description: el.description,
                            due_date: el.due_date,
                            id: el.id,
                            iid: el.iid,
                            labels: el.labels,
                            references: el.references,
                            time_stats: el.time_stats,
                            title: el.title,
                            updated_at: el.updated_at,
                            state: el.state,
                            due_date: el.due_date,
                        })
                    })
                    results = results.concat(newResults)
                }

                const listIssueNotNull = this.dataIssue.filter((item) => item.assignee !== null)
                const listIssueNull = this.dataIssue.filter((item) => item.assignee === null)

                this.usersData.forEach((els) => {
                    if (els.id != 1) {
                        listIssueNotNull.forEach((el) => {
                            if (els.id === el.assignee.id) {
                                els.total_time_spent += el.time_stats.total_time_spent
                                els.time_estimate += el.time_stats.time_estimate
                            }
                        })
                    }
                    els.total_time_spent = els.total_time_spent / 3600
                    els.time_estimate = els.time_estimate / 3600
                })
                this.dataIssueNotNull = listIssueNotNull
                this.dataIssueNull = listIssueNull
                this.loading = false
            },
            dateToYMD(end_date) {
                var ed = new Date(end_date)
                var d = ed.getDate()
                var m = ed.getMonth() + 1
                var y = ed.getFullYear()
                return '' + y + '-' + (m <= 9 ? '0' + m : m) + '-' + (d <= 9 ? '0' + d : d)
            },
            reload(item) {
                this.filteredArr = []
                this.dataIssue = []
                this.getdataIssue(item)
            },
        },
    }
</script>
