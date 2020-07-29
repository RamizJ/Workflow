<template lang="pug">
  div.team-users
    user-toolbar(
      @search="onSearch"
      @filters="onFiltersChange"
      @order="onOrderChange"
      @sort="onSortChange"
      @view="onViewChange")
    user-table(
      v-if="view === 'list'"
      ref="items"
      :search="search"
      :filters="filters"
      :order="order"
      :sort="sort")

</template>

<script>
import UserToolbar from '@/components/Users/UserToolbar';
import UserTable from '@/components/Users/UserTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    UserToolbar,
    UserTable
  },
  mixins: [pageMixin],
  created() {
    if (!this.$route.query.sort) this.onSortChange('lastName');
    if (!this.$route.query.order) this.onOrderChange('Ascending');
  }
};
</script>

<style lang="scss" scoped>
.team-users {
  height: 100%;
  display: flex;
  flex-direction: column;
}
</style>
