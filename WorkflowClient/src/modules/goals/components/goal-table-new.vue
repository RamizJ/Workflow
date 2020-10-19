<template>
  <BaseTable
    :data="data"
    @space="openGoalWindow"
    @double-click="openGoalWindow"
    @load="onLoad"
    infinite
  >
    <BaseTableColumn prop="title" label="Задача">
      <template v-slot:default="scope">
        <span class="cell-text">{{ scope.row.title }}</span>
        <span class="cell-icon" v-if="scope.row.description">
          <unicon name="file-alt" />
        </span>
        <span class="cell-icon" v-if="scope.row.hasChildren">
          <unicon name="list-ul" />
        </span>
        <span class="cell-icon" v-if="scope.row.isAttachmentsExist">
          <unicon name="paperclip" />
        </span>
      </template>
    </BaseTableColumn>
  </BaseTable>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import { StateChanger } from 'vue-infinite-loading'
import goalsStore from '@/modules/goals/store/goals.store'
import BaseTable from '../../../core/components/base-table.vue'
import BaseTableColumn from '@/core/components/base-table-column.vue'
import Task from '@/modules/goals/models/task.type'
import Query from '@/core/types/query.type'

@Component({
  components: { BaseTableColumn, BaseTable },
})
export default class GoalTableNew extends Vue {
  private query: Query = new Query()
  private data: Task[] = []

  private async onLoad($state: StateChanger): Promise<void> {
    const isFirstLoad = !this.data.length
    const data = await goalsStore.findAll(this.query)
    if (this.query.pageNumber !== undefined) this.query.pageNumber++
    if (data.length) $state.loaded()
    else $state.complete()
    this.data = isFirstLoad ? data : this.data.concat(data)
  }

  private openGoalWindow(row: Task): void {
    goalsStore.setTask(row)
    goalsStore.openGoalWindow()
  }
}
</script>

<style lang="scss"></style>
