<template>
    <li class="nav-link">
        <router-link :to="`/${controller}`">
            <i :class="`${icon} icon`"></i>
            <span class="text nav-text">{{ title }} </span>
        </router-link>
    </li>
</template>
<script>
    export default {
        props: ['title', 'icon', 'controller'],
    }
</script>
<style></style>
