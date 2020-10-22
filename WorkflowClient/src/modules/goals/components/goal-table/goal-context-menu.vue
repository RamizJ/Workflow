<template>
  <BaseContextMenu ref="baseContextMenu">
    <BaseContextMenuItem @click="edit">
      {{ active ? 'Изменить' : 'Информация' }}
    </BaseContextMenuItem>
    <BaseContextMenuDivider v-if="active" />
    <BaseContextMenuItem v-if="active" @click="create">Новая задача</BaseContextMenuItem>
    <BaseContextMenuDivider v-if="active" />
    <BaseContextSubmenu v-if="active" title="Изменить статус">
      <BaseContextMenuItem @click="editStatus('New')"> Новое </BaseContextMenuItem>
      <BaseContextMenuItem @click="editStatus('Succeed')"> Выполнено </BaseContextMenuItem>
      <BaseContextMenuItem @click="editStatus('Delay')"> Отложено </BaseContextMenuItem>
      <BaseContextMenuItem @click="editStatus('Rejected')"> Отклонено </BaseContextMenuItem>
      <BaseContextMenuDivider />
      <BaseContextMenuItem @click="editStatus('Perform')"> Выполняется </BaseContextMenuItem>
      <BaseContextMenuItem @click="editStatus('Testing')"> Проверяется </BaseContextMenuItem>
    </BaseContextSubmenu>
    <BaseContextMenuItem v-if="active" @click="remove"> Удалить </BaseContextMenuItem>
    <BaseContextMenuItem v-if="!active" @click="restore"> Восстановить </BaseContextMenuItem>
  </BaseContextMenu>
</template>

<script lang="ts">
import { Component, Ref, Vue } from 'vue-property-decorator'
import BaseContextMenu from '@/core/components/base-context-menu/base-context-menu.vue'
import BaseContextMenuItem from '@/core/components/base-context-menu/base-context-menu-item.vue'
import BaseContextMenuDivider from '@/core/components/base-context-menu/base-context-menu-divider.vue'
import BaseContextSubmenu from '@/core/components/base-context-menu/base-context-submenu.vue'
import Task from '@/modules/goals/models/task.type'

@Component({
  components: { BaseContextSubmenu, BaseContextMenuDivider, BaseContextMenuItem, BaseContextMenu },
})
export default class GoalContextMenu extends Vue {
  @Ref() readonly baseContextMenu!: BaseContextMenu

  private active = true

  public open(event: Event, data: Task): void {
    this.active = !data.isRemoved
    this.baseContextMenu.open(event, data)
  }

  private edit(): void {
    this.$emit('edit')
  }

  private create(): void {
    this.$emit('create')
  }

  private editStatus(status: string): void {
    this.$emit('edit-status', status)
  }

  private remove(): void {
    this.$emit('remove')
  }

  private restore(): void {
    this.$emit('restore')
  }
}
</script>

<style scoped></style>
