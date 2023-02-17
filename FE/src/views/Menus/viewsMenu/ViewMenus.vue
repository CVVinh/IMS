<template>
    <div class="col" @click="">
        <router-link style="text-decoration: none; width: min-content" :to="`/${controller}`">
            <div :class="`card text-white ${bground}`">
                <div class="card-body">
                    <h2 class="text-center">{{ title }}</h2>
                    <hr />
                    <div class="icon">
                        <i :class="icon"></i>
                    </div>
                </div>
            </div>
        </router-link>
    </div>
</template>
<script>
    export default {
        props: ['title', 'icon', 'controller', 'bground'],
    }
</script>
<style scoped>
    .card {
        width: 17rem;
        height: 17rem;
        border-radius: 20px;
    }
    .col {
        width: 17rem;
        height: 17rem;
    }
    .card-body {
        display: flex;
        flex-direction: column;
    }

    .icon {
        display: flex;
        flex: 1;
        justify-content: center;
        align-items: center;
    }

    .icon i {
        font-size: 10rem;
    }

    .height-h2 {
        height: 34px;
    }

    .background-color-cyan-600 {
        background-color: var(--cyan-600);
    }

    .background-color-yellow-800 {
        background-color: var(--yellow-800);
    }

    .background-color-green-600 {
        background-color: var(--green-600);
    }

    .background-color-blue-gray-500 {
        background-color: var(--bluegray-500);
    }
</style>
