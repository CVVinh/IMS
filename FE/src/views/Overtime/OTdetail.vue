<template>
    <layout-default-dynamic>
        <div class="container cont">
            <h2>Detail</h2>

            <div class="properties">
                <label class="item-label" for="n">Type:</label>
                <label class="item-data" for="n">{{ _type(data.type) }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Date OT:</label>
                <label class="item-data" for="n">{{ formatDate(data.date) }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">OT User:</label>
                <label class="item-data" for="n">{{ username[data.user] }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Proposed Lead:</label>
                <label class="item-data" for="n">{{ username[data.leadCreate] }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Proposed Date:</label>
                <label class="item-data" for="n">{{ formatDate(data.dateCreate) }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Time:</label>
                <label class="item-data" for="n">{{ data.realTime }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Start Time:</label>
                <label class="item-data" for="n">{{ data.start }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">End Time:</label>
                <label class="item-data" for="n">{{ data.end }}</label>
            </div>

            <div class="description">
                <label class="item-label" for="n">Description:</label>
                <label class="item-data">{{ data.description }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Updater:</label>
                <label class="item-data" for="n">{{ username[data.updateUser] }}</label>
            </div>
            <div class="properties">
                <label class="item-label" for="n">Update Day:</label>
                <label class="item-data" for="n">{{ formatDate(data.dateUpdate) }}</label>
            </div>

            <div class="properties">
                <label class="item-label" for="n">Project:</label>
                <label class="item-data" for="n">{{ project }}</label>
            </div>
        </div>
        <br />
        <div class="container-md">
            <button type="button" class="btn btn-primary btn-accept" @click="accept(true)">Accept</button>
            <button type="button" class="btn btn-primary btn-accept" @click="accept(false)">Deny</button>
            <button type="submit" class="btn btn-secondary btn-accept" v-on:click="cancel">Cancel</button>
            <Dialog header="Please enter reason!" :visible="displayBasic"
                :breakpoints="{ '960px': '75vw', '640px': '90vw' }" :style="{ width: '30vw' }" :modal="true">
                <Textarea v-model="reason" style="margin: auto; width: 100%; height: 100%"></Textarea>
                <medium v-if="entered" class="p-error"> Reason is requied! </medium>
                <template #footer>
                    <Button label="No" icon="pi pi-times" @click="closeBasic" class="p-button-text" />
                    <Button label="Yes" icon="pi pi-check" @click="submit" autofocus />
                </template>
            </Dialog>
        </div>
    </layout-default-dynamic>
</template>
<script>
import jwtDecode from 'jwt-decode'
import { HTTP } from '@/http-common.js'
import axios from 'axios'
import router from '@/router/index.js'
import { UserRoleHelper } from '@/helper/user-role.helper'
export default {
    
    data() {
        return {
            data: {},
            mail: {
                to: '',
                subject: '',
                body: '',
            },
            displayBasic: false,
            reason: '',
            status: 2,
            PM: '',
            entered: false,
            user: [],
            username: [],
            project: null,
            //axios: 'http://api.imsdemo.tk/api/',
            axios: import.meta.env.VITE_VUE_API_URL,
        }
    },
    mounted() {
        if (UserRoleHelper.isPm())
            axios
                .get(this.axios + 'OTs/getOTByID/' + this.$route.params.id, {
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${localStorage.getItem('token')}`,
                    },
                })
                .then((res) => {
                    if (res.status == 200) {
                        this.data = res.data
                        axios
                            .get(this.axios + 'Project/getById/' + this.data.idProject, {
                                headers: {
                                    'Content-Type': 'application/json',
                                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                                },
                            })
                            .then((res) => {
                                if (res.status == 200) this.project = res.data.name
                            })
                    }
                })
        else router.push('/ots')
        axios
            .get(this.axios + 'Users/getAll', {
                headers: {
                    'Content-Type': 'application/json',
                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                },
            })
            .then((res) => {
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
            router.go(-1)
        },
        formatDate(value) {
            return new Date(value).toLocaleDateString('en-US', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
            })
        },
        accept(accept) {
            let user = jwtDecode(localStorage.getItem('token'))
            this.PM = user.Id
            if (accept) {
                this.status = 1
                axios.put(
                    this.axios + 'OTs/acceptOT',
                    { id: parseInt(this.$route.params.id), status: this.status, PM: this.PM },
                    {
                        headers: {
                            'Content-Type': 'application/json',
                            Authorization: `Bearer ${localStorage.getItem('token')}`,
                        },
                    },
                )
                axios
                    .get(this.axios + 'Users/getUserById/' + this.data.leadCreate, {
                        headers: {
                            'Content-Type': 'application/json',
                            Authorization: `Bearer ${localStorage.getItem('token')}`,
                        },
                    })
                    .then((res) => {
                        if (res.status == 200) {
                            this.mail.to = res.data.email
                            this.mail.subject = 'Accepted!'
                            this.mail.body = 'Your proposed was accepted!'
                            axios.post(this.axios + 'Users/SendEmail', this.mail, {
                                headers: {
                                    'Content-Type': 'application/json',
                                    Authorization: `Bearer ${localStorage.getItem('token')}`,
                                },
                            })
                            this.showSuccess()
                            router.push('/ots/accept')
                        }
                    })
            } else {
                this.displayBasic = true
            }
        },
        closeBasic() {
            this.displayBasic = false
            this.entered = false
        },
        submit() {
            if (this.reason != null && this.reason != '') {
                axios
                    .put(this.axios + 'OTs/acceptOT', {
                        id: parseInt(this.$route.params.id),
                        status: this.status,
                        PM: this.PM,
                        note: this.reason,
                    })
                    .then((res) => {
                        if (res.status == 200) {
                            axios.get(this.axios + 'Users/getUserById/' + this.data.leadCreate).then((res) => {
                                if (res.status == 200) {
                                    this.mail.to = res.data.email
                                    this.mail.subject = 'Denied!'
                                    this.mail.body = 'Your proposed was denied!'
                                    axios.post(this.axios + 'Users/SendEmail', this.mail)
                                    this.closeBasic()
                                    this.showSuccess()
                                    router.push('/ots/accept')
                                }
                            })
                        }
                    })
            } else this.entered = true
        },

        _type(num) {
            if (num == 0) return 'OT'
            return 'Remote'
        },
        showSuccess() {
            this.$toast.add({ severity: 'success', summary: 'Success Message', detail: 'Successful!', life: 3000 })
        },
        showWarn() {
            this.$toast.add({ severity: 'warn', summary: 'Warn Message', detail: 'Error!', life: 3000 })
        },
    },
}
</script>
<style lang="scss">
.properties {
    border-top: 1px solid;
    border-bottom: 1px solid;
    width: 600px;
    display: inline-flex;
    font-size: 18px;
    height: auto;
    margin: 3px 0 3px 0;
}

.description {
    border-top: 1px solid;
    border-bottom: 1px solid;
    width: 1200px;
    display: inline-flex;
    font-size: 20px;
    height: auto;
    margin: 3px 0 3px 0;

    .item-label {
        flex: 0 1 17.5%;
    }
}

.item-data {
    -webkit-box-flex: 1;
    display: -webkit-box;
    display: -ms-flexbox;
    display: flex;
    -webkit-box-align: center;
    -ms-flex-align: center;
    align-items: center;
    -ms-flex-wrap: wrap;
    flex-wrap: wrap;
    padding: 10px 0 9px 10px;
    word-wrap: break-word;
    word-break: break-word;
    min-width: 0;
}

.item-label {
    color: rgb(19, 150, 150);
    -webkit-box-flex: 0;
    flex: 0 1 35%;
    display: -webkit-box;
    display: -ms-flexbox;
    display: flex;
    -webkit-box-align: center;
    -ms-flex-align: center;
    align-items: center;
    padding-right: 20px;
    padding: 2px 2px 2px 2px;
    min-width: 0;
    text-decoration: underline;
}

.btn-accept {
    width: 110px;
    height: 40px;
    margin: 2px;
}
</style>
