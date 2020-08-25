export default {
  data() {
    return {
      search: this.$route.query.q || '',
      filters: [],
      order: this.$route.query.order || '',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list'
    };
  },
  methods: {
    onCreate() {
      this.$refs.items.onItemCreate();
    },
    onSearch(value) {
      this.search = value;
      this.updateUrl('q', value);
    },
    onFiltersChange(value) {
      this.filters = value;
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
        this.$router.replace({ query });
      }
    }
  }
};
