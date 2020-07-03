import { mapActions, mapGetters } from 'vuex';

export default {
  data() {
    return {
      loading: true,
      tableData: [],
      query: {
        filter: '',
        pageNumber: 0,
        pageSize: 20,
        sortFields: [{ fieldName: 'creationDate', sortType: 'Ascending' }]
      },
      sort: {
        field: 'creationDate',
        type: 'Ascending',
        fields: []
      },
      dialogOpened: false,
      selectedItemId: null,
      selectedRow: null,
      addButtonVisible: true,
      editButtonVisible: false,
      completeButtonVisible: false,
      deleteButtonVisible: false,
      filtersVisible: false
    };
  },
  computed: {
    ...mapGetters({ items: '' }),
    table() {
      if (Array.isArray(this.$refs.table)) return this.$refs.table[0];
      else return this.$refs.table;
    },
    loader() {
      if (Array.isArray(this.$refs.loader)) return this.$refs.loader[0];
      else return this.$refs.loader;
    },
    contextMenu() {
      if (Array.isArray(this.$refs.contextMenu))
        return this.$refs.contextMenu[0];
      else return this.$refs.contextMenu;
    },
    isMultipleSelected() {
      return this.table.selection?.length > 1;
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
      this.loader.stateChanger.reset();
      this.dialogOpened = false;
      this.editButtonVisible = false;
      this.completeButtonVisible = false;
      this.deleteButtonVisible = false;
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
    applySort() {
      this.query.sortFields[0] = {
        fieldName: this.sort.field,
        sortType: this.sort.type
      };
      this.refresh();
    },
    switchSortType() {
      this.sort.type =
        this.sort.type === 'Ascending' ? 'Descending' : 'Ascending';
      this.query.sortFields[0].sortType = this.sort.type;
      this.refresh();
    },
    onItemSelect(selection, row) {
      this.selectedRow = row;
      this.editButtonVisible = true;
      this.completeButtonVisible = true;
      this.deleteButtonVisible = true;
      if (!selection.length) {
        this.editButtonVisible = false;
        this.completeButtonVisible = false;
        this.deleteButtonVisible = false;
      }
    },
    onItemSingleClick(row, column, event) {
      this.table.clearSelection();
      this.table.toggleRowSelection(row);
      this.selectedRow = row;
      this.editButtonVisible = true;
      this.completeButtonVisible = true;
      this.deleteButtonVisible = true;
    },
    onItemDoubleClick(row, column, event) {
      this.onItemEdit(event, row);
    },
    onItemRightClick(row, column, event) {
      this.table.setCurrentRow(row);
      this.contextMenu.open(event, { row, column });
      event.preventDefault();
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
      await this.deleteItems(this.table.selection.map(item => item.id));
      await this.refresh();
    },
    async onItemComplete(event, row) {
      await this.completeItem(row);
      await this.refresh();
    },
    async onItemMultipleComplete(event, row) {
      await this.completeItems(this.table.selection);
      await this.refresh();
    },
    dateFormatter(row, column, cellValue, index) {
      const dateRaw = new Date(cellValue);
      const dateRu = dateRaw.toLocaleString();
      return dateRu;
    },
    priorityFormatter(row, column, cellValue, index) {
      switch (cellValue) {
        case 'Low':
          return 'Низкий';
        case 'Normal':
          return 'Обычный';
        case 'High':
          return 'Высокий';
        default:
          return 'Отсутствует';
      }
    },
    stateFormatter(row, column, cellValue, index) {
      switch (cellValue) {
        case 'New':
          return 'Новое';
        case 'Perform':
          return 'В работе';
        case 'Delay':
          return 'Отложено';
        case 'Testing':
          return 'Тестируется';
        case 'Succeed':
          return 'Выполнено';
        case 'Rejected':
          return 'Отклонено';
        default:
          return 'Отсутствует';
      }
    }
  }
};
