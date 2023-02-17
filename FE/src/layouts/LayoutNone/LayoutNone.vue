<template>
    <div :class="cx.wrapper">


        <div :class="cx.mainWrapper">

            <div class="row" style="height:100%">

                <div class="col">

                </div>

            </div>
            <slot />
        </div>
    </div>
</template>

<script setup>
// import Header from './Header.vue'
// import Navbar from './Navbar.vue'
// import Sidebar from './Sidebar.vue'
</script>

<style lang="scss" module="cx">
.wrapper {
    display: flex;
    flex-direction: column;
}

.headerWrapper {
    min-height: var(--header-height);
    background-color: var(--color-white);
    box-shadow: 0 1px 1px 0 rgb(230 230 230);
    background-color: #e4e7eb !important;
}
</style>
