<template>
  <div class="toolbar">
    <div class="toolbar__wrapper">
      <BasePopover popover-class="filters-popover">
        <div class="toolbar__filters">
          <div class="toolbar__filters-wrapper">
            <slot name="filters" />
          </div>
        </div>
        <div slot="reference" class="filters">
          <el-button type="text" size="mini">
            <feather type="sliders" size="15"></feather>
          </el-button>
          <el-button class="filters__button" type="text">Фильтры</el-button>
        </div>
      </BasePopover>
      <div class="sort">
        <el-button type="text" size="mini" @click="onOrderChange">
          <feather :class="order" type="bar-chart" size="15"></feather>
        </el-button>
        <el-select
          class="sort-selection"
          v-model="sort"
          size="small"
          placeholder="По умолчанию"
          @change="onSortChange"
        >
          <el-option
            v-for="option in sortFields"
            :key="option.value"
            :value="option.value"
            :label="option.label"
          ></el-option>
        </el-select>
      </div>
      <div class="view">
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
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { SortType } from '@/core/types/query.type'
import { View } from '@/core/types/view.type'
import BasePopover from '@/core/components/base-popover/base-popover.vue'

@Component({
  components: { BasePopover },
})
export default class Toolbar extends Vue {
  @Prop() readonly sortFields: { value: string; label: string }[] | undefined
  @Prop() readonly board: boolean | undefined
  @Prop() readonly grid: boolean | undefined
  @Prop() readonly list: boolean | undefined

  private order = SortType.Descending
  private sort = ''
  private view = View.List
  private filtersCollapsed = true

  protected mounted(): void {
    this.order = (this.$route.query.order as SortType) || SortType.Descending
    this.sort =
      this.$route.query.sort?.toString() ||
      (this.sortFields ? this.sortFields[0].value?.toString() : '')
    if (this.$route.query.order) this.$emit('order', this.order)
    if (this.$route.query.sort) this.$emit('order', this.sort)
    this.view = (this.$route.query.view as View) || View.List
  }

  private onFiltersCollapse(): void {
    this.filtersCollapsed = !this.filtersCollapsed
    this.$emit('filters-collapse', this.filtersCollapsed)
  }

  private onOrderChange(): void {
    const value = this.order === SortType.Ascending ? SortType.Descending : SortType.Ascending
    this.order = value
    this.$emit('order', value)
  }

  private onSortChange(value: string): void {
    this.sort = value
    this.$emit('sort', value)
  }

  private onViewChange(value: string): void {
    this.view = value as View
    this.$emit('view', value)
  }
}
</script>

<style lang="scss">
.toolbar {
  padding: 4px 0;
}
.toolbar__filters {
  justify-content: space-between;
  align-items: flex-end;
  display: inline-flex;
  margin-top: 15px;
  margin-bottom: 15px;
}
.toolbar__filters-wrapper {
  display: grid;
  grid-gap: 24px;
  grid-template-columns: repeat(5, 200px);
  .label {
    color: var(--text);
    font-size: 13px;
    font-weight: 500;
    margin-bottom: 5px;
    opacity: 0.9;
  }
  .filter {
    display: flex;
    flex-direction: column;
    justify-content: center;
  }
}
.el-collapse-item__header {
  display: none !important;
}
.el-collapse-item__content {
  padding-bottom: 0;
}
.toolbar__wrapper {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  .sort,
  .view,
  .filters {
    display: flex;
    justify-content: flex-end;
    align-items: flex-start;
    .filters__button {
      font-size: 13px;
      line-height: 16px;
      margin-right: 20px;
      font-weight: 500;
    }
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
      width: 135px;
      margin-bottom: 1px;
      height: 18px;
      line-height: 18px;
      .el-input__inner {
        padding: 0;
        padding-left: 5px;
        font-size: 13px;
        font-weight: 500;
        line-height: 21px;
        height: 21px;
        box-shadow: none !important;
        border: none !important;
        &:hover,
        &:focus {
          box-shadow: none !important;
          border: none !important;
        }
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
      color: var(--text-muted);
      padding: 0;
      &.is-disabled {
        opacity: 0.3;
      }
      &.active {
        color: var(--text-placeholder);
      }
    }
    .el-button--mini {
      width: 21px;
      height: 21px;
    }
    .el-button + .el-button {
      margin-left: 5px;
    }
  }
  .view .el-button:first-child {
    margin-left: 20px;
  }
}
.toolbar-view {
  width: 100%;
  display: flex;
  justify-content: flex-end;
  align-items: center;
  margin-top: 20px;
}
.toolbar-view__display .el-button:first-child {
  margin-left: 20px;
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
    width: 128px;
    margin-bottom: 1px;
    .el-input__inner {
      padding: 0;
      padding-left: 5px;
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

.filters-popover {
  width: 1205px;
  left: 220px !important;
  padding: 5px 30px !important;
  border: none;
  border-radius: 0;
  border-bottom: 1px solid var(--sidebar-border);
  box-shadow: none;
}
</style>
