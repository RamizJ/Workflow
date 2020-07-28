<template lang="pug">
  toolbar
    toolbar-filters
      div.filter
        div.label Поиск
        el-input(v-model="q" size="medium" placeholder="Искать..." @change="onSearch")
          el-button(slot="prefix" type="text" size="mini" @click="onChange")
            feather(type="search" size="16")
      div.filter
        div.label Статус
        el-select(
          v-model="sort"
          size="medium"
          placeholder="Любой"
          @change="onSortChange")
          el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
      div.filter
        div.label Приоритет
        el-select(
          v-model="sort"
          size="medium"
          placeholder="Любой"
          @change="onSortChange")
          el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
      div.filter
        div.label Проект
        el-select(
          v-model="sort"
          size="medium"
          placeholder="Любой"
          @change="onSortChange")
          el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
    toolbar-filters-extra
      el-row(:gutter="20")
        el-col(:span="8")
          div.label Ответственный
          el-select(
            v-model="sort"
            size="medium"
            placeholder="Любой"
            @change="onSortChange")
            el-option(v-for="option in sortFields" :key="option.value" :value="option.value", :label="option.label")
        el-col(:span="16")
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
        el-col(:span="8")
          el-checkbox(v-model="filters.withAttachments") Только с вложениями
        el-col(v-if="!filters.showOnlyDeleted" :span="8")
          el-checkbox(v-model="filters.hideDeleted") Скрыть удалённые
        el-col(v-if="!filters.hideDeleted" :span="8")
          el-checkbox(v-model="filters.showOnlyDeleted") Только удалённые
    toolbar-view(:sort-fields="sortFields" @order="onOrderChange" @sort="onSortChange" @view="onViewChange")
</template>

<script>
import Toolbar from '@/components/Toolbar/Toolbar';
import ToolbarFilters from '@/components/Toolbar/ToolbarFilters';
import ToolbarFiltersExtra from '@/components/Toolbar/ToolbarFiltersExtra';
import ToolbarView from '@/components/Toolbar/ToolbarView';
import toolbarMixin from '@/mixins/toolbar.mixin';

export default {
  name: 'TaskToolbar',
  components: {
    Toolbar,
    ToolbarFilters,
    ToolbarFiltersExtra,
    ToolbarView
  },
  mixins: [toolbarMixin],
  data() {
    return {
      sortFields: [
        { value: 'title', label: 'По названию' },
        { value: 'projectName', label: 'По проекту' },
        { value: 'state', label: 'По статусу' },
        { value: 'priority', label: 'По приоритету' },
        { value: 'creationDate', label: 'По дате создания' }
      ]
    };
  }
};
</script>
