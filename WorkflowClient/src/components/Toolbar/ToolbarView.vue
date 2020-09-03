<template>
  <div class="toolbar-view">
    <div class="toolbar-view__sort">
      <el-button type="text" size="mini" @click="onOrderChange">
        <feather :class="order" type="bar-chart" size="15"></feather>
        <!--feather(:class="order" type="code" size="13")-->
      </el-button>
      <el-select v-model="sort" size="small" placeholder="По умолчанию" @change="onSortChange">
        <el-option
          v-for="option in sortFields"
          :key="option.value"
          :value="option.value"
          :label="option.label"
        ></el-option>
      </el-select>
    </div>
    <div class="toolbar-view__display">
      <el-button
        v-if="board"
        type="text"
        size="mini"
        :class="view === 'board' ? 'active' : ''"
        @click="onViewChange('board')"
      >
        <feather type="columns" size="18"></feather>
      </el-button>
      <el-button
        v-if="grid"
        type="text"
        size="mini"
        :class="view === 'grid' ? 'active' : ''"
        @click="onViewChange('grid')"
      >
        <feather type="grid" size="18"></feather>
      </el-button>
      <el-button
        v-if="list"
        type="text"
        size="mini"
        :class="view === 'list' ? 'active' : ''"
        @click="onViewChange('list')"
      >
        <feather type="menu" size="18"></feather>
      </el-button>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { SortType } from '@/types/query.type'
import { View } from '@/types/view.type'

@Component
export default class ToolbarView extends Vue {
  @Prop() readonly sortFields: [] | undefined
  @Prop() readonly board: boolean | undefined
  @Prop() readonly grid: boolean | undefined
  @Prop() readonly list: boolean | undefined

  private order = (this.$route.query.order as SortType) || SortType.Descending
  private sort = this.$route.query.sort || ''
  private view = (this.$route.query.view as View) || View.List

  private onOrderChange(): void {
    const value = this.order === SortType.Ascending ? SortType.Descending : SortType.Ascending
    this.order = value
    this.$emit('order', value)
  }

  private onSortChange(value: string): void {
    this.$emit('sort', value)
  }

  private onViewChange(value: string): void {
    this.view = value as View
    this.$emit('view', value)
  }
}
</script>

<style lang="scss">
.toolbar-view {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  margin-top: 20px;
}
.toolbar-view__sort,
.toolbar-view__display {
  display: flex;
  justify-content: flex-end;
  align-items: flex-start;
  .Ascending {
    transform: rotate(90deg);
    margin-top: 1px;
    margin-right: -5px;
  }
  .Descending {
    transform: rotate(270deg) scaleY(-1);
    margin-top: 1px;
    margin-right: -5px;
  }
  .el-select {
    width: 160px;
    margin-bottom: 3px;
    .el-input__inner {
      padding: 0 5px;
      font-size: 14px;
      line-height: 21px;
      height: 21px;
      font-weight: 500;
    }
    .el-input__suffix {
      display: none;
    }
  }
  .el-input {
    i {
      color: var(--text-muted) !important;
    }
  }
  .el-input__inner {
    color: var(--text-muted) !important;
    font-weight: 500;
    background-color: transparent !important;
    height: 21px;
    line-height: 21px;
    font-size: 13.5px;
    &::placeholder {
      color: var(--text-muted) !important;
      opacity: 1;
    }
  }
  .el-button {
    width: 21px;
    height: 21px;
    padding: 0;
    color: var(--text-muted);
    &.is-disabled {
      opacity: 0.3;
    }
    &.active {
      color: var(--text-placeholder);
    }
  }
  .el-button + .el-button {
    margin-left: 5px;
  }
}
</style>
