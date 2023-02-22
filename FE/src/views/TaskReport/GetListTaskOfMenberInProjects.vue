<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4 mt-3">
            <Card style="width: 100%; margin-bottom: 2em; background-color: #d3d3d382">
                <template #content>
                    <DataTable
                        :value="dataUsers"
                        :rows="5"
                        ref="dt"
                        :paginator="true"
                        :loading="loading"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                        :rowsPerPageOptions="[5, 10, 15, 30]"
                        currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                    >
                        <template #header>
                            <div class="row">
                                <div class="col-md-5">
                                    <label for="Project_drop" class="me-2">Dự án</label>
                                    <Dropdown
                                        id="Project_drop"
                                        style="width: 60%"
                                        v-model="fillterProject.selectedProject"
                                        :options="dataProjects"
                                        optionLabel="name"
                                        :filter="true"
                                        placeholder="Nhập dự án"
                                        :showClear="true"
                                    >
                                        <template #value="slotProps">
                                            <div class="country-item country-item-value" v-if="slotProps.value">
                                                <div>{{ slotProps.value.name }}</div>
                                            </div>
                                            <span v-else>
                                                {{ slotProps.placeholder }}
                                            </span>
                                        </template>
                                    </Dropdown>
                                    <Button class="p-button-outlined p-button-secondary ms-2" @click="clearSelect()">
                                        <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                                    ></Button>
                                </div>
                                <div class="col-md-7 d-flex justify-content-end">
                                    <div class="d-flex">
                                        <div class="me-2">
                                            <label for="Start_Date" class="me-2">Ngày bắt đầu</label>
                                            <InputText type="date" id="Start_Date" v-model="fillterProject.startDate" />
                                        </div>
                                        <div class="me-2">
                                            <label for="End_Date" class="me-2">Ngày kết thúc </label>
                                            <InputText type="date" id="End_Date" v-model="fillterProject.endDate" />
                                        </div>
                                        <div class="me-2">
                                            <Button
                                                class="p-button-outlined p-button-info"
                                                @click="handlerFillterProject"
                                                style="background-color: antiquewhite"
                                                icon="pi pi-filter-slash"
                                            />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-5">
                                    <label class="me-2" style="margin-top: 20px">Nhân viên</label>
                                    <div class="input-text" style="display: contents">
                                        <span
                                            class="p-input-icon-left"
                                            style="margin-left: 17px; margin-top: 10px; width: 60%"
                                        >
                                            <InputText
                                                class="p-inputtext-sm"
                                                style="padding-left: 1rem; width: 100%"
                                                v-model="searchtext"
                                                placeholder="Tìm kiếm"
                                            />
                                        </span>
                                    </div>
                                </div>
                            </div>
                        </template>
                        <Column field="name" header="Thành viên" exportHeader="Thành viên">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="time_estimate" header="Thời gian dự kiến">
                            <template #body="{ data }"> {{ Math.round(data.time_estimate * 100) / 100 }}h </template>
                        </Column>
                        <Column field="total_time_spent" header="Tổng thời gian thực hiện">
                            <template #body="{ data }"> {{ Math.round(data.total_time_spent * 100) / 100 }}h </template>
                        </Column>
                        <Column header="Chi tiết">
                            <template #body="{ data }">
                                <a class="btn btn-primary" @click="ViewDetail(data)">
                                    <span class="pi pi-eye p-button-icon"></span>
                                </a>
                            </template>
                        </Column>
                    </DataTable>
                </template>
            </Card>
        </div>
        <Dialog
            header="Chi tiết"
            v-model:visible="displayBasic"
            :breakpoints="{ '960px': '75vw', '640px': '90vw' }"
            :style="{ width: '72vw' }"
        >
            <table class="table">
                <thead>
                    <tr>
                        <th>Công việc</th>
                        <th>Dự án</th>
                        <th>Thời gian dự kiến (h)</th>
                        <th>Thời gian thực hiện (h)</th>
                        <th>Trạng thái</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in dataDetail" :key="index">
                        <td>{{ item.title }}</td>
                        <td>{{ item.projectName }}</td>
                        <td class="text-center">
                            {{ Math.round((item.time_stats.time_estimate / 3600) * 100) / 100 }}h
                        </td>
                        <td class="text-center">
                            {{ Math.round((item.time_stats.total_time_spent / 3600) * 100) / 100 }}h
                        </td>
                        <td v-if="item.labels.length > 0">
                            <h6 v-for="(lable, i) in item.labels" :key="i">
                                {{ lable }}
                            </h6>
                        </td>
                        <td v-else>Pending</td>
                    </tr>
                </tbody>
            </table>
            <template #footer>
                <Button
                    label="Hủy"
                    icon="pi pi-times"
                    @click="closeBasic"
                    class="p-button p-component p-button-secondary"
                />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>
<script>
    import {
        HTTP_API_GITLAB,
        GET_ALL_PROJECT,
        GET_ALL_USERS,
        GET_ALL_ISSUE,
        KEY_PROJECT,
        PROJECT_MEMBER,
    } from '@/http-common'
    import dayjs from 'dayjs'
    export default {
        async created() {
            await this.handlerGetInfoProjects()
            await this.handlerGetInfoUser()
            await this.handleAction()
        },
        data() {
            return {
                defaultPageSize: 100,
                dataUsers: [],
                loading: true,
                dataProjects: [],
                dataIssue: [],
                fillterProject: {
                    selectedProject: null,
                    startDate: null,
                    endDate: null,
                },
                dataDetail: [],
                displayBasic: false,
                dataFillMath: [],
                searchtext: null,
                timeSheet: { label: 'Report', icon: 'me-1 bx bx-spreadsheet' },
                itemsTimeSheet: [{ label: 'Task report', to: '/report/task' }],
            }
        },
        watch: {
            searchtext: {
                handler: function Change(newText) {
                    if (newText != '') {
                        this.searchByName(newText)
                    } else {
                        if (
                            this.fillterProject.selectedProject != null ||
                            (this.fillterProject.startDate != null && this.fillterProject.endDate != null)
                        ) {
                            this.loading = true
                            this.dataUsers = []
                            this.dataIssue = []
                            this.handleAction()
                            this.getAllMenberInProject(this.fillterProject.selectedProject.id)
                            this.searchByName(newText)
                        } else {
                            this.loading = true
                            this.dataUsers = []
                            this.dataIssue = []
                            this.handlerGetInfoUser()
                            this.handleAction()
                        }
                    }
                },
                deep: true,
            },
        },
        methods: {
            async clearSelect() {
                this.loading = true
                this.fillterProject.selectedProject = null
                this.fillterProject.startDate = null
                this.fillterProject.endDate = null
                this.dataUsers = []
                this.dataIssue = []
                await this.handlerGetInfoUser()
                await this.handleAction()
            },
            checkDate(startDate, endDate) {
                if (startDate > endDate) {
                    this.$toast.add({
                        severity: 'error',
                        summary: 'Lỗi',
                        detail: 'Ngày kết thúc phải lớn hơn ngày bắt đầu!',
                        life: 3000,
                    })
                    return true
                }
                return false
            },
            getAllMenberInProject(id) {
                return HTTP_API_GITLAB.get(PROJECT_MEMBER(id)).then((res) => {
                    res.data
                        .filter((item) => item.id != 1)
                        .forEach((el) => {
                            this.dataUsers.push({
                                id: el.id,
                                name: el.name,
                                time_estimate: null,
                                total_time_spent: null,
                                username: el.username,
                                created_at: el.created_at,
                                commit_email: el.commit_email,
                                email: el.email,
                                state: el.state,
                            })
                        })
                })
            },
            getProject(id) {
                return HTTP_API_GITLAB.get(KEY_PROJECT(id)).then((res) => res.data.name)
            },
            getAllUsers(page) {
                return HTTP_API_GITLAB.get(GET_ALL_USERS(this.defaultPageSize, page)).then((res) => res.data)
            },
            getAllProects(page) {
                return HTTP_API_GITLAB.get(GET_ALL_PROJECT(this.defaultPageSize, page)).then((res) => res.data)
            },
            getAllIssue(id, page) {
                return HTTP_API_GITLAB.get(GET_ALL_ISSUE(id, this.defaultPageSize, page)).then((res) => res.data)
            },
            mathSumTimeSpent() {
                this.dataUsers.forEach((els) => {
                    this.dataFillMath.forEach((el) => {
                        if (els.id === el.assignee.id) {
                            els.total_time_spent += el.time_stats.total_time_spent
                            els.time_estimate += el.time_stats.time_estimate
                        }
                    })
                    els.total_time_spent = els.total_time_spent / 3600
                    els.time_estimate = els.time_estimate / 3600
                })
            },
            async handlerFillterProject() {
                console.log(this.fillterProject)
                if (
                    this.fillterProject.selectedProject !== null &&
                    this.fillterProject.startDate !== null &&
                    this.fillterProject.endDate !== null
                ) {
                    this.loading = true
                    await this.handlerFillterIssueByDate(
                        this.fillterProject.selectedProject.id,
                        this.fillterProject.startDate,
                        this.fillterProject.endDate,
                    )
                }
                if (this.fillterProject.startDate !== null && this.fillterProject.endDate !== null) {
                    await this.handlerFillterIssueAllProjectByDate()
                }

                if (
                    this.fillterProject.selectedProject !== null &&
                    this.fillterProject.startDate === null &&
                    this.fillterProject.endDate === null
                ) {
                    await this.handlerFillterIssueByProject()
                }
            },
            async handlerFillterIssueByProject() {
                this.loading = true
                this.dataUsers = []
                this.dataIssue = []
                this.getAllMenberInProject(this.fillterProject.selectedProject.id)
                await this.handlerGetAllIssue(this.fillterProject.selectedProject.id)
                this.mathSumTimeSpent()
                this.loading = false
            },
            async handlerFillterIssueAllProjectByDate() {
                this.loading = true
                const start = new Date(this.fillterProject.startDate)
                const end = new Date(this.fillterProject.endDate)
                const start_date = dayjs(start).format('YYYY-MM-DD')
                const end_date = dayjs(end).format('YYYY-MM-DD')
                if (this.checkDate(start, end)) {
                    this.loading = false
                } else {
                    const resultFill = this.dataIssue.filter(function (el) {
                        const date = new Date(el.updated_at)
                        const updated_at = dayjs(date).format('YYYY-MM-DD')
                        return updated_at >= start_date && updated_at <= end_date
                    })
                    this.dataFillMath = resultFill
                    this.mathSumTimeSpent()
                    this.loading = false
                }
            },
            async handlerFillterIssueByDate(id, startDate, endDate) {
                const start = new Date(startDate)
                const end = new Date(endDate)
                const start_date = dayjs(start).format('YYYY-MM-DD')
                const end_date = dayjs(end).format('YYYY-MM-DD')
                if (this.checkDate(start, end)) {
                    this.loading = false
                } else {
                    this.dataUsers = []
                    this.dataIssue = []
                    this.getAllMenberInProject(id)

                    await this.handlerGetAllIssue(id)

                    const resultFill = this.dataIssue.filter(function (el) {
                        const date = new Date(el.updated_at)
                        const updated_at = dayjs(date).format('YYYY-MM-DD')
                        return updated_at >= start_date && updated_at <= end_date
                    })
                    this.dataFillMath = resultFill
                    this.mathSumTimeSpent()
                }
            },
            async handleAction() {
                this.dataProjects.forEach((project) => {
                    this.handlerGetAllIssue(project.id)
                })
            },
            ViewDetail(item) {
                this.displayBasic = true
                this.dataDetail = []
                var IssueDetails = this.dataFillMath.filter(function (el) {
                    return el.assignee.id === item.id
                })
                this.dataDetail = IssueDetails
                console.log(this.dataDetail)
            },
            async handlerGetAllIssue(id) {
                let resultCount = 100
                let results = []
                let page = 1
                while (resultCount === 100) {
                    let newResults = await this.getAllIssue(id, page)
                    let projectName = await this.getProject(id)

                    page++
                    resultCount = newResults.length
                    newResults
                        .filter((item) => item.assignee !== null)
                        .forEach((el) => {
                            this.dataIssue.push({
                                projectName: projectName,
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
                            })
                        })
                    results = results.concat(newResults)
                }

                this.dataFillMath = this.dataIssue
                this.mathSumTimeSpent()
                this.loading = false
            },

            async handlerGetInfoUser() {
                let resultCount = 100
                let results = []
                let page = 1

                while (resultCount === 100) {
                    let newResults = await this.getAllUsers(page)

                    page++
                    resultCount = newResults.length
                    newResults
                        .filter((item) => item.id != 1)
                        .forEach((el) => {
                            this.dataUsers.push({
                                id: el.id,
                                name: el.name,
                                time_estimate: null,
                                total_time_spent: null,
                                username: el.username,
                                created_at: el.created_at,
                                commit_email: el.commit_email,
                                email: el.email,
                                state: el.state,
                            })
                        })
                    results = results.concat(newResults)
                }
            },
            async handlerGetInfoProjects() {
                let resultCountPr = 100
                let resultPr = []
                let page = 1
                while (resultCountPr === 100) {
                    let newResultsPr = await this.getAllProects(page)

                    page++
                    this.dataProjects.push(...newResultsPr)
                    resultCountPr = newResultsPr.length
                    resultPr = resultPr.concat(newResultsPr)
                }
            },
            closeBasic() {
                this.displayBasic = false
            },
            searchByName(text) {
                var search = new RegExp(text, 'i')
                const data = this.dataUsers.filter((el) => search.test(el.name))
                this.dataUsers = data
                this.mathSumTimeSpent()
            },
        },
    }
</script>
<style scoped>
    .custom-button {
        display: flex;
        align-items: center;
        justify-content: flex-end;
    }
</style>
