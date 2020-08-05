<template lang="pug">
  toolbar
    toolbar-filters
      div.filter
        div.label Поиск
        el-input(v-model="q" size="medium" placeholder="Искать..." @change="onSearch")
          el-button(slot="prefix" type="text" size="mini" @click="onSearch")
            feather(type="search" size="16")
      //div.filter
        div.label Руководитель
        el-select.remote(
          v-model="filters.owners"
          size="medium"
          placeholder="Любой"
          //:remote-method="searchUsers"
          @focus="onUsersFocus"
          @change="onFiltersChange"
          multiple collapse-tags filterable remote clearable default-first-option)
          el-option(v-for="item in userList" :key="item.id" :label="item.value" :value="item.id")
    toolbar-filters-extra
      el-row(:gutter="20")
        el-col(:span="24")
          el-checkbox(v-model="filters.showOnlyDeleted" @change="onFiltersChange") Только удалённые
    toolbar-view(:sort-fields="sortFields" @order="onOrderChange" @sort="onSortChange" @view="onViewChange" list)
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
        { value: 'creationDate', label: 'По дате создания' },
        { value: 'name', label: 'По названию' },
        { value: 'state', label: 'По руководителю' }
      ]
    };
  }
};
</script>
