<template>
  <vue-context ref="vueContext">
    <template slot-scope="child">
      <slot :data="child.data" />
    </template>
  </vue-context>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import { ContextMenu } from '@/core/types/context-menu.type'

@Component
export default class BaseContextMenu extends Vue {
  @Ref() readonly vueContext!: ContextMenu

  public open(event: Event, data: any): void {
    this.vueContext.open(event, data)
  }
}
</script>

<style scoped></style>
