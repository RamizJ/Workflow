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
    ...mapGetters({ items: '' })
  },
  methods: {
    ...mapActions({ fetchItems: '', deleteItem: '' }),
    async refresh() {
      this.tableData = [];
      this.query.pageNumber = 0;
      this.$refs.loader.stateChanger.reset();
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
    onItemDelete(event, row) {
      this.selectedItemId = row.id;
      this.$confirm(
        'Вы действительно хотите удалить элемент?',
        'Предупреждение',
        {
          confirmButtonText: 'Да',
          cancelButtonText: 'Закрыть',
          type: 'warning'
        }
      ).then(async () => {
        try {
          await this.deleteItem(this.selectedItemId);
          await this.refresh();
        } catch (e) {
          this.$message.error('Не удалось удалить элемент');
          console.error(e);
        }
      });
    }
  }
};
