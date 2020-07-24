export default {
  data() {
    return {
      q: this.$route.query.q || '',
      order: this.$route.query.order || 'Descending',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list',
      sortFields: [],
      filters: {
        deadlineRange: '',
        withAttachments: false,
        hideDeleted: true,
        showOnlyDeleted: false
      }
    };
  },
  methods: {
    onSearch(value) {
      this.$emit('search', value);
    },
    onOrderChange(value) {
      this.$emit('order', value);
    },
    onSortChange(value) {
      this.$emit('sort', value);
    },
    onViewChange(value) {
      this.view = value;
      this.$emit('view', value);
    }
  }
};
