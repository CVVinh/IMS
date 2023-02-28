<template>
  <Dialog v-model:visible="this.display" :closable="false">
    <template #header>
		<h3>Xóa thành viên ra khỏi dự án ?</h3>
	</template>

	Bạn có chắc chắn xóa thành viên này ra khỏi nhóm ?

	<template #footer>
		<Button label="No" icon="pi pi-times" class="p-button-text" @click="this.$emit('close')"/>
        <Button label="Yes" icon="pi pi-check" autofocus class="p-button-danger" @click="this.$emit('delete')"/>
	</template>
</Dialog>
</template>

<script>
export default {
    props:['display']
}
</script>

<style>

</style>