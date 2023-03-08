<template>
    <LayoutDefaultDynamic>
        <ConfirmDialog></ConfirmDialog>
        <Toast position="top-right" />
        <div class="mx-4"></div>
        <div class="mx-3 mt-3">
            <DataTable
                :value="arr"
                ref="dt"
                :paginator="true"
                class="p-datatable-customers"
                :rows="5"
                dataKey="id"
                :rowHover="true"
                sortField="dateCreated"
                :sortOrder="-1"
                :loading="loading"
                responsiveLayout="scroll"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="pageIndex"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="['#', 'name', 'startDate', 'endDate', 'isDeleted', 'isFinished']"
            >
                <!-- Header -->
                <template #header>
                    <div class="flex justify-content-center">
                        <h5 class="" style="color: white; margin-top: 10px; float: left">Danh sách dự án</h5>

                        <div class="header-container">
                            <Button
                                style="float: right; color: white; height: 46px"
                                class="p-button-outlined p-button-secondary ms-2"
                                @click="clearSelect()"
                            >
                                <span class="pi pi-sync p-button-icon" style="font-size: 23px"></span
                            ></Button>
                            <div class="input-text">
                                <span class="p-input-icon-left">
                                    <i class="pi pi-search"> </i>
                                    <InputText class="p-inputtext-sm" v-model="searchtext" placeholder="Tìm kiếm" />
                                </span>
                            </div>
                            <InputText
                                style="height: 46px; float: right"
                                type="month"
                                v-model="sortMonth"
                                placeholder=" Sort by month"
                            />
                        </div>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>
                <template #loading>
                    <ProgressSpinner />
                </template>
                <Column field="name" header="Tên dự án" sortable style="min-width: 60rem">
                    <template #body="{ data }">
                        {{ data.name }}
                    </template>
                </Column>

                <Column
                    style="min-width: 8rem"
                    sortable
                    v-for="(col, index) of selectedColumns"
                    :field="col.field"
                    :header="col.header"
                    :key="col.field + '_' + index"
                ></Column>
                <Column header="&emsp;&emsp;&emsp;Chi tiết" style="text-align: center">
                    <template #body="{ data }">
                        <View @click="toDetailProject(data.projectCode)" />
                    </template>
                </Column>
            </DataTable>
        </div>
        <Dialog
            header="Không có quyền truy cập!"
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
    import { HTTP, GET_GITLAB_PROJECT, GET_GITLAB_PROJECT_BY_DATE } from '@/http-common'
    import View from '../../components/buttons/View.vue'

    export default {
        data() {
            return {
                searchtext: null,
                sortMonth: null,
                arr: [],
                loading: true,
                displayBasic: false,
                pageIndex: [5, 10, 15, 20],
                num: 5,
                isOpenDialog: false,
                defaultPageSize: 100,
            }
        },
        mounted() {
            this.getAllProject()
        },
        watch: {
            searchtext: {
                handler: function Change(newText) {
                    if (newText != '') {
                        this.search(newText)
                    } else {
                        this.arr = []
                        this.sortMonth = null
                        this.getAllProject()
                    }
                },
                deep: true,
            },
            sortMonth: {
                handler: function Change(newval) {
                    if (newval != null) {
                        // this.sort(newval)
                        this.getProjectByDate(newval)
                    }
                },
                deep: true,
            },
        },
        methods: {
            async clearSelect() {
                this.loading = true
                this.arr = []
                this.sortMonth = null
                await this.getAllProject()
            },
            currentDate(value) {
                const now = new Date(value)
                return now.toJSON()
            },
            formatDate(value) {
                if (value == null) return null
                else
                    return new Date(value).toLocaleDateString('en-CA', {
                        day: '2-digit',
                        month: '2-digit',
                        year: 'numeric',
                    })
            },
            async getAllProject() {
                this.arr = []
                HTTP.get(GET_GITLAB_PROJECT).then((res) => {
                    if (res.data) {
                        const result = res.data
                        let temp = []
                        result.forEach((element) => {
                            element.startDate = this.formatDate(element.startDate)
                            element.endDate = this.formatDate(element.endDate)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateUpdate = this.formatDate(element.dateUpdate)
                            element.userCreated = element.userCreated
                            element.userUpdate = element.userUpdate
                            element.userId = element.userId
                            element.leader = element.leader
                            temp.push(element)
                        })
                        this.arr = temp
                    }
                    this.loading = false
                })
            },
            async getProjectByDate(month) {
                this.arr = []
                let thang = this.currentDate(month)

                HTTP.get(GET_GITLAB_PROJECT_BY_DATE(thang)).then((res) => {
                    if (res.data) {
                        const result = res.data
                        let temp = []
                        result.forEach((element) => {
                            element.startDate = this.formatDate(element.startDate)
                            element.endDate = this.formatDate(element.endDate)
                            element.dateCreated = this.formatDate(element.dateCreated)
                            element.dateUpdate = this.formatDate(element.dateUpdate)
                            element.userCreated = element.userCreated
                            element.userUpdate = element.userUpdate
                            element.userId = element.userId
                            element.leader = element.leader
                            temp.push(element)
                        })
                        this.arr = temp
                    }
                    this.loading = false
                })
            },

            search(text) {
                var search = new RegExp(text, 'i')
                const data = this.arr.filter((el) => search.test(el.name))
                this.arr = data
            },
            toDetailProject(id) {
                this.$router.push({ name: 'ProjectTasks', params: { id: id } })
            },
        },

        components: {
            View,
        },
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

        .p-input-icon-left {
            float: right;
            margin-left: 1rem;
            display: inline-flex;
        }

        .p-inputtext-sm {
            font-size: 0.96rem;
        }

        .layout-left {
            float: right;
            display: inline;
        }

        .p-datatable-header {
            background-color: #607d8b;
            height: 75px;
        }

        .mazin {
            margin-left: 5px;
            margin-right: 5px;
        }

        .maz {
            margin-right: 5px;
        }

        .p-dropdown {
            background: #ffffff;
            border: 1px solid #ced4da;
            transition: background-color 0.2s, color 0.2s, border-color 0.2s, box-shadow 0.2s;
            border-radius: 6px;
            float: right;
            height: 46px;
        }
    }
</style>
