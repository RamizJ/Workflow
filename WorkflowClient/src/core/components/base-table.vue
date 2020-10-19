<template>
  <div class="table-container">
    <el-table
      ref="table"
      height="100%"
      row-key="id"
      class="base-table"
      :data="tableData"
      :row-class-name="setIndex"
      tabindex="0"
      @keydown.space.native.prevent="$emit('space')"
      @select="onCheck"
      @row-click="onClick"
      @row-dblclick="onDoubleClick"
      @row-contextmenu="onRightClick"
      highlight-current-row="highlight-current-row"
      show-overflow-tooltip
      border
    >
      <slot />
      <infinite-loading
        slot="append"
        ref="loader"
        spinner="waveDots"
        @infinite="onLoad"
        force-use-infinite-wrapper=".el-table__body-wrapper"
      >
        <div slot="no-more"></div>
        <div slot="no-results"></div>
      </infinite-loading>
    </el-table>
    <slot name="footer" />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Ref, Vue, Watch } from 'vue-property-decorator'
import InfiniteLoading, { StateChanger } from 'vue-infinite-loading'
import { ElTableColumn } from 'element-ui/types/table-column'
import { ElTable } from 'element-ui/types/table'
import Entity from '@/core/types/entity.type'

@Component
export default class BaseTable extends Vue {
  @Prop() readonly infinite?: boolean
  @Prop() readonly data?: Entity[]
  @Ref('table') readonly table!: ElTable
  @Ref('loader') readonly loader!: InfiniteLoading

  private tableData: Entity[] = []
  private selectedRows: Entity[] = []
  private isShiftPressed = false
  private isCtrlPressed = false

  protected mounted(): void {
    document.onkeydown = this.onKeyDown
    document.onkeyup = this.onKeyUp
  }

  @Watch('data')
  private onDataChange(data: Entity[]) {
    this.tableData = data
    if (!data.length) this.loader.stateChanger.reset()
  }

  private onLoad($state: StateChanger): void {
    this.$emit('load', $state)
  }

  private setIndex({ row, rowIndex }: { row: Entity; rowIndex: number }): string | undefined {
    row.index = rowIndex
    if (this.selectedRows.some((entity) => entity.id === row.id)) return 'selected'
  }

  public onCheck(selection: Entity[], entity: Entity): void {
    this.selectedRows = selection
  }

  public onClick(row: Entity): void {
    this.table.clearSelection()
    if (this.isCtrlPressed) {
      this.selectedRows = [...this.selectedRows, row]
    } else if (this.isShiftPressed) {
      const previousIndex = row.index
      const currentIndex = this.selectedRows[this.selectedRows.length - 1].index
      if (previousIndex === undefined || currentIndex === undefined) return
      let from: number
      let to: number
      if (currentIndex < previousIndex) {
        from = currentIndex
        to = previousIndex
      } else {
        from = previousIndex
        to = currentIndex
      }
      this.data?.some((entity: Entity) => {
        if (entity.index === undefined) return
        if (entity.index >= from && entity.index <= to) {
          this.selectedRows = [...this.selectedRows, entity]
        }
      })
    } else {
      this.selectedRows = [row]
    }
    this.selectedRows.map((entity) => {
      this.table.toggleRowSelection(entity, true)
    })
    // this.selectedRows = [row]
    this.$emit('click', row)
  }

  public onDoubleClick(row: Entity): void {
    this.$emit('double-click', row)
  }

  public onRightClick(row: Entity, column: ElTableColumn, event: Event): void {
    if (!(this.selectedRows.length > 1)) {
      this.table?.clearSelection()
      this.table?.toggleRowSelection(row)
    }
    this.table?.setCurrentRow(row)
    this.selectedRows = [row]
    this.$emit('right-click', row)
    // this.contextMenu.open(event, { row, column })
    event.preventDefault()
  }

  private onKeyDown(): void {
    const key = (window.event as KeyboardEvent).keyCode
    if (key === 17 || key === 91) this.isCtrlPressed = true
    if (key === 16) this.isShiftPressed = true
  }

  private onKeyUp(): void {
    this.isShiftPressed = false
    this.isCtrlPressed = false
  }
}
</script>

<style scoped></style>
