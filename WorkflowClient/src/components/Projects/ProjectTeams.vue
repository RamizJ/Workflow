<template lang="pug">
  div.project-teams
    team-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    team-list(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")
</template>

<script>
import TeamToolbar from '@/components/Teams/TeamToolbar';
import TeamList from '@/components/Teams/TeamTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  name: 'ProjectTeams',
  components: { TeamList, TeamToolbar },
  mixins: [pageMixin],
  created() {
    this.onSortChange('name');
    this.onOrderChange('Ascending');
  }
};
</script>

<style lang="scss" scoped>
.project-teams {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
