<template>
    <slot></slot>
</template>

<script setup>
    import LayoutFill from './LayoutFill.vue'

    const emits = defineEmits(['update:layout'])

    emits('update:layout', LayoutFill)
</script>

<script>
    export default {
        inheritAttrs: false,
    }
</script>
