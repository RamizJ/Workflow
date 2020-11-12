<template>
  <BaseContextMenu ref="baseContextMenu">
    <BaseContextMenuItem v-if="!$route.params.teamId && !$route.params.projectId" @click="edit">
      {{ active ? 'Изменить' : 'Информация' }}
    </BaseContextMenuItem>
    <BaseContextMenuDivider v-if="active && !$route.params.teamId && !$route.params.projectId" />
    <BaseContextMenuItem
      v-if="active && !$route.params.teamId && !$route.params.projectId"
      @click="createNew"
      >Новый пользователь</BaseContextMenuItem
    >
    <BaseContextMenuDivider v-if="active && !$route.params.teamId && !$route.params.projectId" />
    <BaseContextMenuItem v-if="$route.params.teamId" @click="removeFromTeam">
      Убрать из команды
    </BaseContextMenuItem>
    <BaseContextMenuItem
      v-if="active && !$route.params.teamId && !$route.params.projectId"
      @click="remove"
    >
      Удалить
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
export default class UserContextMenu extends Vue {
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

  private removeFromTeam(): void {
    this.$emit('remove-from-team')
  }

  private restore(): void {
    this.$emit('restore')
  }
}
</script>

<style scoped></style>
