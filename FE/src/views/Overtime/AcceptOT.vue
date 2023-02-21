<template>
    <layout-default-dynamic>
        <div class="mx-5 mt-5">
            <DataTable
                :value="data"
                :paginator="true"
                class="p-datatable-customers"
                :rows="10"
                dataKey="id"
                :rowHover="true"
                v-model:filters="filters"
                v-model:selection="selectedOT"
                filterDisplay="menu"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Hiển thị từ {first} đến {last} trong tổng {totalRecords} dữ liệu"
                :globalFilterFields="['date', 'description', 'leadCreate', 'dateCreate', 'user', 'idProject']"
                responsiveLayout="scroll"
            >
                <!-- Header -->

                <template #header>
                    <div class="flex align-items-center">
                        <div class="table-name">
                            <h3 class="">phê duyệt</h3>
                        </div>
                        <div>
                            <span class="p-input-icon-left">
                                <i class="pi pi-search" />
                                <InputText v-model="filters['global'].value" placeholder="Tìm kiếm" />
                            </span>
                        </div>
                        <div class="filter-pro">
                            <h5 style="display: inline">Dự án:&emsp;</h5>
                            <Dropdown
                                class="p-column-filter"
                                v-model="filters['idProject'].value"
                                :options="projects"
                                optionLabel="name"
                                optionValue="id"
                                placeholder="Chọn dự án"
                            />
                            <Button
                                type="button"
                                icon="pi pi-filter-slash"
                                class="p-button-outlined"
                                @click="clearFilter()"
                            />
                        </div>
                        <button type="submit" class="btn btn-primary btn-accept" v-on:click="cancel">Back</button>
                    </div>
                </template>
                <template #empty> Không tìm thấy. </template>

                <!-- Body -->
                <Column selectionMode="multiple" headerStyle="width: 3rem"></Column>
                <Column field="date" header="Ngày" sortable dataType="date" style="min-width: 10 rem">
                    <template #body="{ data }">
                        {{ formatDate(data.date) }}
                    </template>
                </Column>
                <Column field="user" header="Tên" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ username[data.user] }}
                    </template>
                </Column>
                <Column field="leadCreate" header="Leader đề xuất" sortable style="min-width: 11rem">
                    <template #body="{ data }">
                        {{ username[data.leadCreate] }}
                    </template>
                </Column>
                <Column
                    field="description"
                    header="Mô tả"
                    sortable
                    :filterMenuStyle="{ width: '14rem' }"
                    style="min-width: 6rem"
                >
                    <template #body="{ data }">
                        {{ data.description }}
                    </template>
                </Column>
                <Column field="realTime" header="Thời gian tăng ca" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.realTime }}
                    </template>
                </Column>
                <Column field="start" header="Thời gian bắt đầu" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.start }}
                    </template>
                </Column>
                <Column field="end" header=" Thời gian kết thúc" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.end }}
                    </template>
                </Column>
                <Column field="idProject" header="Dự án" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ pro_name[data.idProject] }}
                    </template>
                </Column>
                <Column field="" header="Thực thi" style="min-width: 15rem">
                    <template #body="{ data }">
                        <Button icon="pi pi-pencil" @click="_to(data.id)" />
                        &nbsp;
                        <Button icon="pi pi-check" class="p-button-primary" @click="accept(data.id, data.leadCreate)" />
                    </template>
                </Column>
            </DataTable>
        </div>
    </layout-default-dynamic>
</template>
<script>
    import jwtDecode from 'jwt-decode'
    import { HTTP } from '@/http-common.js'
    import { UserRoleHelper } from '@/helper/user-role.helper'
    import { FilterMatchMode, FilterOperator } from 'primevue/api'
    import router from '@/router'
    export default {
        data() {
            return {
                selectedOT: null,
                data: null,
                filters: {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    idProject: { value: null, matchMode: FilterMatchMode.EQUALS },
                },
                mail: {
                    to: '',
                    subject: '',
                    body: '',
                },
                projects: [],
                pro_name: [],
                user: [],
                username: [],
                axios: import.meta.env.VITE_VUE_API_URL,
            }
        },
        mounted() {
            if (UserRoleHelper.isPm()) this.getAll()
            else router.push('/ots')
            axios.get(this.axios + 'Project/getAllProject').then((res) => {
                if (res.status == 200) {
                    res.data.forEach((element) => {
                        if (element.isDeleted != true && element.isFinished != true) {
                            this.projects.push(element)
                            this.pro_name[element.id] = element.name
                        }
                    })
                }
            })
            axios.get(this.axios + 'Users/getAll').then((res) => {
                if (res.status == 200)
                    res.data.forEach((element) => {
                        if (element.isDeleted != true && element.isFinished != true) {
                            this.user.push(element)
                            this.username[element.id] = element.userCode
                        }
                    })
            })
        },
        methods: {
            cancel() {
                router.push('/ots')
            },
            getAll() {
                axios.get(this.axios + 'OTs/getAccept/' + this.$route.params.type).then((res) => {
                    if (res.status == 200) this.data = res.data
                })
            },
            _to(id) {
                this.$router.push('/ots/accept/' + this.$route.params.type + '/' + id)
            },
            formatDate(value) {
                return new Date(value).toLocaleDateString('en-CA', {
                    day: '2-digit',
                    month: '2-digit',
                    year: 'numeric',
                })
            },
            add() {
                router.replace('/ots/addOT')
            },
            accept(id, lead) {
                let status = 1
                let user = jwtDecode(localStorage.getItem('token'))
                let PM = user.Id
                let reason = ''
                axios.put(this.axios + 'OTs/acceptOT', { id: id, status: status, PM: PM, note: reason })
                axios.get(this.axios + 'Users/getUserById/' + lead).then((res) => {
                    if (res.status == 200) {
                        this.mail.to = res.data.email
                        this.mail.subject = 'Thực thi!'
                        this.mail.body = 'Đề xuất được phê duyệt!'
                        axios.post(this.axios + 'Users/SendEmail', this.mail)
                        this.showSuccess()
                        setTimeout(() => {
                            this.getAll()
                        }, 500)
                    }
                })
            },
            clearFilter() {
                this.filters = {
                    global: { value: null, matchMode: FilterMatchMode.CONTAINS },
                    idProject: { value: null, matchMode: FilterMatchMode.EQUALS },
                }
            },
            showSuccess() {
                this.$toast.add({ severity: 'success', summary: 'Thành công', detail: 'Thành công!', life: 3000 })
            },
        },
    }
</script>

<style lang="scss" scoped>
    .btn-accept {
        width: 220px;
        height: 50px;
    }

    .flex {
        display: flex;
        justify-content: space-between;
    }

    .table-name {
        display: flex;
        justify-content: flex-start;
    }

    .filter-pro {
        margin-right: 150px;
    }

    .p-button-outlined {
        margin-left: 15px;
    }

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
    }

    .btn-accept {
        width: 10%;
        height: 10%;
    }
</style>
