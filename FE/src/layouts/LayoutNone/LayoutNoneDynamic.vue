<template>
    <slot></slot>
</template>

<script setup>
import LayoutNone from '@/layouts/LayoutNone/LayoutNone.vue';

const emits = defineEmits(['update:layout'])

emits('update:layout', LayoutNone)
</script>

<script>
export default {
    inheritAttrs: false,
}
</script>
