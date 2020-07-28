<template lang="pug">
  toolbar
    toolbar-filters
      div.filter
        div.label Поиск
        el-input(v-model="q" size="medium" placeholder="Искать..." @change="onSearch")
          el-button(slot="prefix" type="text" size="mini" @click="onChange")
            feather(type="search" size="16")
    toolbar-filters-extra
      el-row(:gutter="20")
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
  name: 'ProjectToolbar',
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
        { value: 'name', label: 'По названию' },
        { value: 'state', label: 'По руководителю' },
        { value: 'creationDate', label: 'По дате создания' }
      ]
    };
  }
};
</script>
