<template>
    <button class="btn btn-primary" @click.prevent="Logout">Logout</button>
</template>

<script>
import { HTTP } from '@/http-common'
import jwt_decode from 'jwt-decode'
export default {
    name: 'logout',
    methods: {
        Logout() {
            const CallApi = async () => {
                try {
                    const access_token = localStorage.getItem('token')
                    const response = await HTTP.post(
                        'Users/Logout',
                        JSON.stringify(localStorage.getItem('username')),
                    )
                    if (response.status == 200) {
                        localStorage.removeItem('username')
                        localStorage.removeItem('token')
                        this.$router.push('/login')
                    }
                } catch (error) {
                    switch (error.response.status) {
                        case 403:
                            break
                    }
                }
            }
            CallApi()
        },
    },
}
</script>

<style>
</style>
