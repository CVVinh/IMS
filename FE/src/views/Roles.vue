<template>
    <LayoutDefaultDynamic>
        <div class="mx-3 mt-3">
            <DataTable :value="data" :paginator="true" class="p-datatable-customers" :rows="10" dataKey="id"
                :rowHover="true"
                paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink CurrentPageReport RowsPerPageDropdown"
                :rowsPerPageOptions="[2, 10, 25, 50]"
                currentPageReportTemplate="Showing {first} to {last} of {totalRecords} entries"
                :globalFilterFields="['userCode', 'userCreated', 'dateCreated', 'userModified', 'dateModified', 'firstName', 'lastName', 'phoneNumber', 'dOB', 'gender', 'address', 'university', 'yearGraduated', 'email', 'skype', 'dateStartWork', 'IdGroup']"
                responsiveLayout="scroll">
                <!-- Header -->
                <!-- Header -->
                <template #header>
                    <div class="flex justify-content-center align-items-center">
                        <h5 class="" style="color: #fff">Roles List</h5>
                        &emsp;
                    </div>
                </template>
                <template #empty>
                    No users found.
                </template>
                <!-- Body -->
                <Column field="id" header="id" sortable style="min-width: 5rem">
                    <template #body="{ data }">
                        {{ data.IdGroup }}
                    </template>
                </Column>
                <Column field="nameRole" header="Roles name" sortable style="min-width: 8rem">
                    <template #body="{ data }">
                        {{ data.nameRole }}
                    </template>
                </Column>
                <Column field="description" header="Description" sortable style="min-width: 14rem">
                    <template #body="{ data }">
                        {{ data.description }}
                    </template>
                </Column>
            </DataTable>
        </div>
    </LayoutDefaultDynamic>
</template>

<script>
import { HTTP } from '@/http-common'
import LayoutDefaultDynamic from '../layouts/LayoutDefault/LayoutDefaultDynamic.vue'
export default {
    data() {
        return {
            data: null,
        };
    },
    mounted() {
        HTTP.get("Roles").then((res) => {
            if (res.data) {
                this.data = res.data;
            }
            /* const key = localStorage.getItem('key')
            if(!key){
                this.$router.push('/')
            } */
        });
    },
    methods: {
        checkStatus: (status) => {
            if (status == true)
                return 1;
            return 0;
        }
    },
    components: { LayoutDefaultDynamic }
}
</script>
<style lang="scss" scoped>
::v-deep(.p-paginator) {
    .p-paginator-current {
        margin-left: auto;
    }
}

::v-deep(.p-progressbar) {
    height: .5rem;
    background-color: #D8DADC;

    .p-progressbar-value {
        background-color: #607D8B;
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

    .p-datatable-thead>tr>th {
        text-align: left;
    }

    .p-datatable-tbody>tr>td {
        cursor: auto;
    }

    .p-dropdown-label:not(.p-placeholder) {
        text-transform: uppercase;
    }

    .p-input-icon-left {
        float: right;
        margin-left: 1rem;
        display: inline;
    }

    .p-inputtext-sm {
        font-size: 0.96rem;
    }

    .layout-left {
        float: right;
        display: inline;
    }

    .p-multiselect .p-multiselect-label {
        padding: 0.41rem 0.41rem;
    }

    .p-datatable-header {
        background-color: #607D8B;
    }

    .mazin {
        margin-left: 5px;
        margin-right: 5px;
    }

    .maz {
        margin-right: 5px;
    }
}
</style>