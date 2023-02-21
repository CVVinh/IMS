<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4"></div>
        <div class="mx-4 mt-3">
            <Card style="width: 100%; margin-bottom: 2em; background-color: #d3d3d382">
                <template #content>
                    <DataTable
                        :value="usersData"
                        ref="dt"
                        :loading="loading"
                        :rows="5"
                        :paginator="true"
                        :exportFilename="titleProject + 'Báo cáo tổng hợp năng suất'"
                        showGridlines="true"
                        responsiveLayout="scroll"
                        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                        :rowsPerPageOptions="[5, 10, 15, 30]"
                        currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                    >
                        <template #header>
                            <div style="text-align: left">
                                <Button icon="pi pi-external-link" label="Xuất Excel" @click="exportCSV($event)" />
                            </div>
                        </template>
                        <Column field="name" header="Tên" exportHeader="Tên">
                            <template #body="{ data }">
                                {{ data.name }}
                            </template>
                        </Column>
                        <Column field="total_time_spent" header="Tổng thời gian" exportHeader="Tổng thời gian(TS)">
                            <template #body="{ data }"> {{ data.total_time_spent }}h </template>
                        </Column>
                        <Column header="Chi tiết">
                            <template #body="{ data }">
                                <a class="btn btn-primary" @click="ViewDetails(data)">
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
            :style="{ width: '80vw' }"
        >
            <table class="table">
                <thead>
                    <tr>
                        <th>Công việc</th>
                        <th>Thời gian dự kiến</th>
                        <th>Trạng thái</th>
                        <th>Thời gian thực hiện (h)</th>
                        <th>Ngày hết hạn</th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="(item, index) in dataIssueDetails" :key="index">
                        <td>{{ item.title }}</td>
                        <td class="text-center">{{ item.time_stats.time_estimate / 3600 }}h</td>
                        <td v-if="item.labels.length > 0">
                            <h6 v-for="(lable, i) in item.labels" :key="i">
                                {{ lable }}
                            </h6>
                        </td>
                        <td v-else>Pending</td>
                        <td class="text-center">{{ item.time_stats.total_time_spent / 3600 }}h</td>
                        <td v-if="item.due_date != null">{{ item.due_date }}</td>
                        <td v-else>Pending</td>
                    </tr>
                </tbody>
            </table>
            <template #footer>
                <Button
                    label="Thoát"
                    icon="pi pi-times"
                    @click="closeBasic"
                    class="p-button p-component p-button-secondary"
                />
            </template>
        </Dialog>
    </LayoutDefaultDynamic>
</template>
<script>
    import { HTTP_API_GITLAB, KEY_PROJECT, PROJECT_MEMBER, GET_ALL_ISSUE } from '@/http-common'
    export default {
        created() {
            this.getdataIssue()
            this.getProjectData()
        },
        data() {
            return {
                usersData: [],
                dataIssue: [],
                loading: true,
                defaultPageSize: 100,
                idProject: this.$route.params.id,
                dataIssueNotNull: [],
                dataIssueNull: [],
                dataIssueDetails: [],
                displayBasic: false,
                titleProject: null,
            }
        },
        methods: {
            getAllPageIssue(page) {
                return HTTP_API_GITLAB.get(GET_ALL_ISSUE(this.idProject, this.defaultPageSize, page)).then(
                    (res) => res.data,
                )
            },
            getProjectData() {
                HTTP_API_GITLAB.get(KEY_PROJECT(this.idProject)).then((res) => {
                    this.titleProject = res.data.name_with_namespace
                    this.itemsTimeSheet.push({ label: res.data.name })
                })
            },
            async getdataIssue() {
                let resultCount = 100
                let results = []
                let page = 1

                HTTP_API_GITLAB.get(PROJECT_MEMBER(this.idProject)).then((res) => {
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
                    let newResults = await this.getAllPageIssue(page)
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
            ViewDetails(item) {
                this.displayBasic = true
                this.dataIssueDetails = []
                var IssueDetails = this.dataIssueNotNull.filter(function (el) {
                    return el.assignee.id === item.id
                })
                this.dataIssueDetails = IssueDetails
            },
            closeBasic() {
                this.displayBasic = false
            },
            exportCSV() {
                this.$refs.dt.exportCSV()
            },
        },
    }
</script>
