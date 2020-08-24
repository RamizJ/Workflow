<template lang="pug">
  toolbar
    toolbar-filters
      div.filter
        div.label Поиск
        el-input(v-model="q" size="medium" placeholder="Искать..." @change="onSearch")
          el-button(slot="prefix" type="text" size="mini" @click="onSearch")
            feather(type="search" size="16")
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
import toolbarMixin from '@/mixins/toolbar.mixin2';

export default {
  name: 'TeamToolbar',
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
        { value: 'lastName', label: 'По фамилии' },
        { value: 'firstName', label: 'По имени' },
        { value: 'middleName', label: 'По отчеству' },
        { value: 'userName', label: 'По логину' },
        { value: 'email', label: 'По почте' },
        { value: 'position', label: 'По должности' }
      ]
    };
  }
};
</script>
