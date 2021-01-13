<template>
  <BaseContextMenu ref="baseContextMenu">
    <BaseContextMenuItem
      v-if="active && !$route.params.teamId && !$route.params.groupId"
      @click="openAction"
    >
      Открыть
    </BaseContextMenuItem>
    <BaseContextMenuItem
      v-if="!$route.params.teamId && !$route.params.groupId && !active"
      @click="edit"
    >
      {{ 'Информация' }}
    </BaseContextMenuItem>
    <BaseContextMenuDivider v-if="active && !$route.params.teamId && !$route.params.groupId" />
    <BaseContextMenuItem
      v-if="active && !$route.params.teamId && !$route.params.groupId"
      @click="createNew"
      >Новый проект</BaseContextMenuItem
    >
    <BaseContextMenuDivider v-if="active && !$route.params.teamId && !$route.params.groupId" />
    <BaseContextMenuItem
      v-if="active && !$route.params.teamId && !$route.params.groupId"
      @click="remove"
    >
      Удалить
    </BaseContextMenuItem>
    <BaseContextMenuItem v-if="active && $route.params.groupId" @click="removeFromGroup">
      Убрать из области
    </BaseContextMenuItem>
    <BaseContextMenuItem v-if="!active" @click="restore">Восстановить</BaseContextMenuItem>
  </BaseContextMenu>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import BaseContextMenuItem from '@/core/components/base-context-menu/base-context-menu-item.vue'
import BaseContextMenuDivider from '@/core/components/base-context-menu/base-context-menu-divider.vue'
import BaseContextSubmenu from '@/core/components/base-context-menu/base-context-submenu.vue'
import Entity from '@/core/types/entity.type'

@Component({
  components: { BaseContextSubmenu, BaseContextMenuDivider, BaseContextMenuItem, BaseContextMenu },
})
export default class ProjectContextMenu extends Vue {
  @Ref() readonly baseContextMenu!: BaseContextMenu

  private active = true

  public open(event: Event, data: Entity): void {
    this.active = !data.isRemoved
    this.baseContextMenu.open(event, data)
  }

  private openAction(): void {
    this.$emit('open')
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

  private async removeFromGroup(): Promise<void> {
    const allowDelete = await this.baseContextMenu.confirmDelete()
    if (!allowDelete) return
    else this.$emit('remove-from-group')
  }

  private restore(): void {
    this.$emit('restore')
  }
}
</script>

<style scoped></style>
