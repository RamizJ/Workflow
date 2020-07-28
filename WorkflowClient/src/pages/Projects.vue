<template lang="pug">
  page
    base-header
      template(slot="title") Проекты
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    project-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    project-table(
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")

</template>

<script>
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import ProjectToolbar from '@/components/Projects/ProjectToolbar';
import ProjectTable from '@/components/Projects/ProjectTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    ProjectToolbar,
    ProjectTable
  },
  mixins: [pageMixin],
  created() {
    this.onSortChange('creationDate');
    this.onOrderChange('Descending');
  }
};
</script>
