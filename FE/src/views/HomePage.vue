<template>
    <div class="wrapper">
        <div class="header-wrapper">
            <Header />
        </div>
        <div class="main-wrapper">
            <slot></slot>
        </div>
    </div>
</template>

<script setup>
import Header from './demo.vue'
</script>

<style lang="scss" scoped>
.wrapper {
    height: 100%;
    display: flex;
    flex-direction: column;
}

.header-wrapper {
    height: 50px;
    box-shadow: 0 1px 1px 0 rgb(230 230 230);
    background-color: var(--primary-color-text);
    display: flex;
    align-items: center;
    justify-content: center;
}

.main-wrapper {
    width: 90%;
    align-self: center;
    flex: 1;
}
</style>
