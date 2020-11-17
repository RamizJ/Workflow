<template>
  <toolbar
    :sort-fields="sortFields"
    @order="onOrderChange"
    @sort="onSortChange"
    @view="onViewChange"
    @filters-collapse="onFiltersCollapse"
    :board="true"
    :list="true"
  >
    <template slot="filters">
      <div class="filter">
        <div class="label">Поиск</div>
        <el-input v-model="search" size="medium" placeholder="Искать..." @change="onSearch">
          <el-button slot="prefix" type="text" size="mini" @click="onSearch(search || '')">
            <feather type="search" size="16"></feather>
          </el-button>
        </el-input>
      </div>
      <div class="filter" v-if="$route.query.view !== 'board'">
        <div class="label">Статус</div>
        <el-select
          v-model="filters.statuses"
          size="medium"
          placeholder="Любой"
          @change="onFiltersChange"
          multiple="multiple"
          collapse-tags="collapse-tags"
        >
          <el-option
            v-for="option in statuses"
            :key="option.value"
            :value="option.value"
            :label="option.label"
          ></el-option>
        </el-select>
      </div>
      <div class="filter">
        <div class="label">Приоритет</div>
        <el-select
          v-model="filters.priorities"
          size="medium"
          placeholder="Любой"
          @change="onFiltersChange"
          multiple="multiple"
          collapse-tags="collapse-tags"
        >
          <el-option
            v-for="option in priorities"
            :key="option.value"
            :value="option.value"
            :label="option.label"
          ></el-option>
        </el-select>
      </div>
      <div class="filter" v-if="!$route.params.projectId">
        <div class="label">Проект</div>
        <el-select
          class="remote"
          v-model="filters.projects"
          size="medium"
          placeholder="Любой"
          :remote-method="searchProjects"
          @focus="onProjectsFocus"
          @change="onFiltersChange"
          multiple="multiple"
          collapse-tags="collapse-tags"
          filterable="filterable"
          remote="remote"
          default-first-option="default-first-option"
        >
          <el-option
            v-for="item in projects"
            :key="item.id"
            :label="item.value"
            :value="item.id"
          ></el-option>
        </el-select>
      </div>
      <div class="filter">
        <div class="label">Исполнитель</div>
        <el-select
          class="remote"
          v-model="filters.performers"
          size="medium"
          placeholder="Любой"
          :remote-method="searchUsers"
          @focus="onUsersFocus"
          @change="onFiltersChange"
          multiple="multiple"
          collapse-tags="collapse-tags"
          filterable="filterable"
          remote="remote"
          default-first-option="default-first-option"
        >
          <el-option
            v-for="item in users"
            :key="item.id"
            :label="item.value"
            :value="item.id"
          ></el-option>
        </el-select>
      </div>
      <div class="filter">
        <div class="label">Архив</div>
        <el-checkbox
          v-model="filters.showOnlyDeleted"
          size="medium"
          @change="onFiltersChange"
          border
          >Только удалённые</el-checkbox
        >
      </div>
    </template>
  </toolbar>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import Toolbar from '@/core/components/base-toolbar.vue'
import ToolbarMixin from '@/core/mixins/toolbar.mixin.ts'
import tableStore from '@/core/store/table.store'
import { SortType } from '@/core/types/query.type'
import BasePopover from '@/core/components/base-popover/base-popover.vue'

@Component({ components: { BasePopover, Toolbar } })
export default class TaskToolbar extends Mixins(ToolbarMixin) {
  public sort = this.$route.query.sort || 'creationDate'
  private sortFields = [
    { value: 'creationDate', label: 'По дате создания' },
    { value: 'title', label: 'По названию' },
    { value: 'performerFio', label: 'По исполнителю' },
    { value: 'projectName', label: 'По проекту' },
    { value: 'state', label: 'По статусу' },
    { value: 'priority', label: 'По приоритету' },
  ]

  protected mounted(): void {
    tableStore.setSort('creationDate')
    tableStore.setOrder(SortType.Descending)
  }
}
</script>

<style lang="scss"></style>
