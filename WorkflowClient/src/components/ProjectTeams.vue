<template lang="pug">
  div.project-teams
    base-toolbar(
      :sort-fields="sortFields"
      @search="onSearch"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    team-list(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :order="order"
      :sort="sort")
</template>

<script>
import BaseToolbar from '@/components/BaseToolbar';
import TeamList from '@/components/TeamTable';
import pageMixin from '~/mixins/page.mixin';

export default {
  name: 'ProjectTeams',
  components: { TeamList, BaseToolbar },
  mixins: [pageMixin],
  data() {
    return {
      sortFields: [{ value: 'name', label: 'По названию' }]
    };
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
