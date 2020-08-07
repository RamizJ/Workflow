<template lang="pug">
  div.team-projects
    project-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    project-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")

</template>

<script>
import ProjectToolbar from '@/components/Projects/ProjectToolbar';
import ProjectTable from '@/components/Projects/ProjectTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
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

<style lang="scss" scoped>
.team-projects {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
