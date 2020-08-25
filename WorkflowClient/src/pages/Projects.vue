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
import ProjectToolbar from '@/components/Project/ProjectToolbar';
import ProjectTable from '@/components/Project/ProjectTable';
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
    if (!this.$route.query.sort) this.onSortChange('creationDate');
    if (!this.$route.query.order) this.onOrderChange('Descending');
  }
};
</script>
