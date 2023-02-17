<template noInheritAttrs>
    <component :is="layout">
        <router-view v-model:layout="layout" />
    </component >
</template>

<script setup>
    import { shallowRef } from 'vue'
    let layout = shallowRef('div')
</script>

<style lang="scss">
    @import '@/styles/index.scss';
</style>
