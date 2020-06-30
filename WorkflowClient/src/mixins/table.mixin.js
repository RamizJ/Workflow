import { mapActions, mapGetters } from 'vuex';

export default {
  data() {
    return {
      loading: true,
      tableData: [],
      query: {
        pageNumber: 0,
        pageSize: 30
      },
      dialogOpened: false,
      selectedItemId: null
    };
  },
  computed: {
    ...mapGetters({ items: '' }),
    isMultipleSelected() {
      return this.$refs.table.selection?.length > 1;
    }
  },
  methods: {
    ...mapActions({
      fetchItems: '',
      deleteItem: '',
      deleteItems: '',
      completeItem: '',
      completeItems: ''
    }),
    refresh() {
      this.tableData = [];
      this.query.pageNumber = 0;
      const loader =
        this.$refs.loader.stateChanger || this.$refs.loader[0].stateChanger;
      loader.reset();
      this.dialogOpened = false;
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      await this.fetch(this.query);
      if (this.items.length) $state.loaded();
      else $state.complete();
      this.tableData = firstLoad
        ? this.items
        : this.tableData.concat(this.items);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      try {
        await this.fetchItems(params);
        this.query.pageNumber++;
      } catch (e) {
        this.$message.error('Не удалось получить данные');
        console.log(e);
      }
    },
    dateFormatter(row, column, cellValue, index) {
      const dateRaw = new Date(cellValue);
      const dateRu = dateRaw.toLocaleString();
      return dateRu;
    },
    onItemRightClick(row, column, event) {
      this.$refs.table.setCurrentRow(row);
      this.$refs.contextMenu.open(event, { row, column });
      event.preventDefault();
    },
    onItemDoubleClick(row, column, event) {
      this.onItemEdit(event, row);
    },
    onItemEdit(event, row) {
      this.selectedItemId = row.id;
      this.dialogOpened = true;
    },
    async onItemDelete(event, row) {
      await this.deleteItem(row.id);
      await this.refresh();
    },
    async onItemMultipleDelete(event, row) {
      await this.deleteItems(this.$refs.table.selection.map(item => item.id));
      await this.refresh();
    },
    async onItemComplete(event, row) {
      await this.completeItem(row);
      await this.refresh();
    },
    async onItemMultipleComplete(event, row) {
      await this.completeItems(this.$refs.table.selection);
      await this.refresh();
    }
  }
};
