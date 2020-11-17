<template>
  <BaseWindow @closed="$emit('close')">
    <h1 slot="title">Область</h1>
    <el-form
      slot="body"
      ref="form"
      v-loading="loading"
      label-position="top"
      :model="form"
      :rules="rules"
      :disabled="form.id && form.isRemoved"
      @submit.native.prevent="submit"
    >
    </el-form>
  </BaseWindow>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BaseWindow from '@/core/components/base-window.vue'
@Component({
  components: { BaseWindow },
})
export default class GroupWindow extends Vue {}
</script>

<style scoped></style>
