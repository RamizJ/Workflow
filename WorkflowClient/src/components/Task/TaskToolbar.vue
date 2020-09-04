<template>
  <toolbar>
    <toolbar-filters>
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
    </toolbar-filters>
    <toolbar-filters-extra>
      <el-row :gutter="20">
        <el-col :span="24">
          <div class="filter">
            <div class="label">Ответственный</div>
            <el-select
              class="remote"
              v-model="filters.performers"
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
        </el-col>
      </el-row>
      <el-row :gutter="20">
        <el-col :span="24">
          <el-checkbox v-model="filters.showOnlyDeleted" @change="onFiltersChange"
            >Только удалённые</el-checkbox
          >
        </el-col>
      </el-row>
    </toolbar-filters-extra>
    <toolbar-view
      :sort-fields="sortFields"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange"
      board="board"
      list="list"
    ></toolbar-view>
  </toolbar>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import Toolbar from '@/components/Toolbar/Toolbar.vue'
import ToolbarFilters from '@/components/Toolbar/ToolbarFilters.vue'
import ToolbarFiltersExtra from '@/components/Toolbar/ToolbarFiltersExtra.vue'
import ToolbarView from '@/components/Toolbar/ToolbarView.vue'
import ToolbarMixin from '@/mixins/toolbar.mixin.ts'

@Component({
  components: {
    Toolbar,
    ToolbarFilters,
    ToolbarFiltersExtra,
    ToolbarView,
  },
})
export default class TaskToolbar extends mixins(ToolbarMixin) {
  private sortFields = [
    { value: 'creationDate', label: 'По дате создания' },
    { value: 'title', label: 'По названию' },
    { value: 'performerFio', label: 'По ответственному' },
    { value: 'projectName', label: 'По проекту' },
    { value: 'state', label: 'По статусу' },
    { value: 'priority', label: 'По приоритету' },
  ]
}
</script>
