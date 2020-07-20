export default {
  data() {
    return {
      search: this.$route.query.q || '',
      order: this.$route.query.order || '',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list',
      activeTab: 'active',
      tabs: [
        { value: 'active', label: 'Активные' },
        { value: 'deleted', label: 'Удаленные' }
      ]
    };
  },
  mounted() {
    const currentTab = this.$route.query.tab || this.activeTab;
    this.updateUrl('tab', currentTab || this.activeTab);
    this.activeTab = currentTab || this.activeTab;
  },
  methods: {
    onCreate() {
      this.$refs.items[0].onItemCreate();
    },
    onTabClick() {
      this.onSearch('');
      this.onOrderChange('');
      this.onSortChange('');
      this.onViewChange('');
      this.updateUrl('tab', this.activeTab);
    },
    onSearch(value) {
      this.search = value;
      this.updateUrl('q', value);
    },
    onOrderChange(value) {
      this.order = value;
      this.updateUrl('order', value);
    },
    onSortChange(value) {
      this.sort = value;
      this.updateUrl('sort', value);
    },
    onViewChange(value) {
      this.view = value;
      this.updateUrl('view', value);
    },
    updateUrl(queryLabel, queryValue) {
      const query = { ...this.$route.query };
      if (query[queryLabel] !== queryValue) {
        query[queryLabel] = queryValue || undefined;
        this.$router.push({ query });
      }
    }
  }
};
