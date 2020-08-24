<template lang="pug">
  toolbar
    toolbar-filters
      div.filter
        div.label Поиск
        el-input(v-model="search" size="medium" placeholder="Искать..." @change="onSearch")
          el-button(slot="prefix" type="text" size="mini" @click="onSearch")
            feather(type="search" size="16")
      div.filter(v-if="$route.query.view !== 'board'")
        div.label Статус
        el-select(
          v-model="filters.statuses"
          size="medium"
          placeholder="Любой"
          @change="onFiltersChange"
          multiple collapse-tags)
          el-option(v-for="option in statuses" :key="option.value" :value="option.value", :label="option.label")
      div.filter
        div.label Приоритет
        el-select(
          v-model="filters.priorities"
          size="medium"
          placeholder="Любой"
          @change="onFiltersChange"
          multiple collapse-tags)
          el-option(v-for="option in priorities" :key="option.value" :value="option.value", :label="option.label")
      div.filter(v-if="!$route.params.projectId")
        div.label Проект
        el-select.remote(
          v-model="filters.projects"
          size="medium"
          placeholder="Любой"
          :remote-method="searchProjects"
          @focus="onProjectsFocus"
          @change="onFiltersChange"
          multiple collapse-tags filterable remote default-first-option)
          el-option(v-for="item in projects" :key="item.id" :label="item.value" :value="item.id")
    toolbar-filters-extra
      el-row(:gutter="20")
        el-col(:span="24")
          div.filter
            div.label Ответственный
            el-select.remote(
              v-model="filters.performers"
              placeholder="Любой"
              :remote-method="searchUsers"
              @focus="onUsersFocus"
              @change="onFiltersChange"
              multiple collapse-tags filterable remote default-first-option)
              el-option(v-for="item in users" :key="item.id" :label="item.value" :value="item.id")
          //div.filter
            div.label Крайний срок
            el-date-picker(
              v-model="filters.deadlineRange"
              size="medium"
              type="daterange"
              format="dd.MM.yyyy"
              range-separator=""
              start-placeholder="От"
              end-placeholder="До")
      el-row(:gutter="20")
        el-col(:span="24")
          el-checkbox(v-model="filters.showOnlyDeleted" @change="onFiltersChange") Только удалённые
    toolbar-view(:sort-fields="sortFields" @order="onOrderChange" @sort="onSortChange" @view="onViewChange" board list)
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator';
import { mixins } from 'vue-class-component';

import Toolbar from '@/components/Toolbar/Toolbar.vue';
import ToolbarFilters from '@/components/Toolbar/ToolbarFilters.vue';
import ToolbarFiltersExtra from '@/components/Toolbar/ToolbarFiltersExtra.vue';
import ToolbarView from '@/components/Toolbar/ToolbarView.vue';
import ToolbarMixin from '@/mixins/toolbar.mixin.ts';

@Component({
  components: {
    Toolbar,
    ToolbarFilters,
    ToolbarFiltersExtra,
    ToolbarView
  }
})
export default class TaskToolbar extends mixins(ToolbarMixin) {
  private sortFields = [
    { value: 'creationDate', label: 'По дате создания' },
    { value: 'title', label: 'По названию' },
    { value: 'projectName', label: 'По проекту' },
    { value: 'state', label: 'По статусу' },
    { value: 'priority', label: 'По приоритету' }
  ];
}
</script>
