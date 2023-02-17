<template>
    <slot></slot>
</template>

<script setup>
    import LayoutDefault from '@/layouts/LayoutDefault/LayoutDefault.vue'

    const emits = defineEmits(['update:layout'])

    emits('update:layout', LayoutDefault)
</script>

<script>
    export default {
        inheritAttrs: false,
    }
</script>
