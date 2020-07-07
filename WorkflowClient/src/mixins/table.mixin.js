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
        sortFields: [
          {
            fieldName: this.$route.query.sort || 'creationDate',
            sortType: this.$route.query.order || 'Descending'
          }
        ]
      },
      statuses: [
        { value: 'New', label: 'Новое' },
        { value: 'Perform', label: 'В работе' },
        { value: 'Delay', label: 'Отложено' },
        { value: 'Testing', label: 'Тестируется' },
        { value: 'Succeed', label: 'Выполнено' },
        { value: 'Rejected', label: 'Отклонено' }
      ],
      priorities: [
        { value: 'Low', label: 'Низкий' },
        { value: 'Normal', label: 'Обычный' },
        { value: 'High', label: 'Высокий' }
      ],
      dialogVisible: false,
      dialogData: null,
      selectedRow: null,
      addButtonVisible: true,
      editButtonVisible: false,
      completeButtonVisible: false,
      deleteButtonVisible: false,
      filtersActive: false
    };
  },
  computed: {
    ...mapGetters({
      items: '',
      projects: 'projects/getProjects',
      teams: 'teams/getTeams',
      users: 'users/getUsers'
    }),
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
    },
    projectList() {
      return this.projects.map(project => {
        return {
          value: project.name,
          id: project.id
        };
      });
    },
    teamList() {
      return this.teams.map(project => {
        return {
          value: project.name,
          id: project.id
        };
      });
    },
    userList() {
      return this.users.map(user => {
        return {
          value: `${user.lastName} ${user.firstName}`,
          id: user.id
        };
      });
    }
  },
  watch: {
    $route(to, from) {
      this.applyQuery();
    }
  },
  methods: {
    ...mapActions({
      fetchItems: '',
      deleteItem: '',
      deleteItems: '',
      completeItem: '',
      completeItems: '',

      fetchProjects: 'projects/fetchProjects',
      fetchTeams: 'teams/fetchTeams',
      fetchUsers: 'users/fetchUsers'
    }),
    async searchProjects(query) {
      await this.fetchProjects({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
    },
    async searchTeams(query) {
      await this.fetchTeams({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
    },
    async searchUsers(query) {
      await this.fetchUsers({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
    },
    refresh() {
      this.tableData = [];
      this.query.pageNumber = 0;
      this.loader.stateChanger.reset();
      this.dialogVisible = false;
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
    applyQuery() {
      if (this.$route.query.sort || this.$route.query.order) {
        this.query.sortFields[0] = {
          fieldName:
            this.$route.query.sort || this.query.sortFields[0].fieldName,
          sortType: this.$route.query.order || this.query.sortFields[0].sortType
        };
        this.refresh();
      }
    },
    applyFilters() {},
    applySort() {
      const query = { ...this.$route.query };
      query.sort = this.query.sortFields[0].fieldName;
      this.$router.push({ query });
    },
    switchOrder() {
      const query = { ...this.$route.query };
      query.order =
        this.query.sortFields[0].sortType === 'Ascending'
          ? 'Descending'
          : 'Ascending';
      this.query.sortFields[0].sortType = query.order;
      this.$router.push({ query });
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
      this.dialogData = row;
      this.dialogVisible = true;
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
      return this.priorities.find(priority => priority.value === cellValue)
        .label;
    },
    stateFormatter(row, column, cellValue, index) {
      return this.statuses.find(status => status.value === cellValue).label;
    }
  }
};
