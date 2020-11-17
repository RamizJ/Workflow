<template>
  <BaseContextMenu ref="baseContextMenu">
    <BaseContextMenuItem v-if="active" @click="edit">Изменить</BaseContextMenuItem>
    <BaseContextMenuItem v-if="active" @click="remove">Удалить</BaseContextMenuItem>
    <BaseContextMenuItem v-if="!active" @click="restore">Восстановить</BaseContextMenuItem>
  </BaseContextMenu>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import BaseContextMenuItem from '@/core/components/base-context-menu/base-context-menu-item.vue'
import Entity from '@/core/types/entity.type'

@Component({
  components: { BaseContextMenuItem, BaseContextMenu },
})
export default class GroupContextMenu extends Vue {
  @Ref() readonly baseContextMenu!: BaseContextMenu

  private active = true

  public open(event: Event, data: Entity): void {
    this.active = !data.isRemoved
    this.baseContextMenu.open(event, data)
  }

  private edit(): void {
    this.$emit('edit')
  }

  private createNew(): void {
    this.$emit('create')
  }

  private async remove(): Promise<void> {
    const allowDelete = await this.baseContextMenu.confirmDelete()
    if (!allowDelete) return
    else this.$emit('remove')
  }

  private restore(): void {
    this.$emit('restore')
  }
}
</script>

<style scoped></style>
