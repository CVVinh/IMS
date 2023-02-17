<template>
    <MenuDashboard></MenuDashboard>
    <div :class="cx.wrapper">
        <div :class="cx.mainWrapper">
            <Sidebar>
                <slot>
                </slot>
            </Sidebar>
        </div>
        <div :class="cx.footerWrapper"></div>
    </div>
</template>

<script setup>
import Sidebar from './Sidebar.vue';
import MenuDashboard from './MenuDashboard.vue';
</script>

<style lang="scss" module="cx">
.wrapper {
    top: 80px;
    height: 100%;
    display: flex;
    flex-direction: column;
    background-color: #fff;
}

.headerWrapper {
    background-color: var(--color-white);
    box-shadow: 0 1px 1px 0 rgb(230 230 230);
    background-color: #e4e7eb !important;
}

.mainWrapper {
    flex: 1;
}
</style>
