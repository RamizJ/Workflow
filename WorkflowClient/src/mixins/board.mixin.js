import { mapActions, mapGetters } from 'vuex';

export default {
  props: {
    search: String,
    order: String,
    sort: String,
    filters: Array
  },
  data() {
    return {
      loading: true,
      data: [],
      query: {
        projectId: this.$route.params.projectId || null,
        filter: '',
        pageNumber: 0,
        pageSize: 20,
        filterFields: [],
        sortFields: [
          {
            fieldName: this.$route.query.sort || 'creationDate',
            sortType: this.$route.query.order || 'Descending'
          }
        ]
      },
      statuses: [
        { value: 'New', label: 'Новое' },
        { value: 'Perform', label: 'Выполняется' },
        { value: 'Testing', label: 'Проверяется' },
        { value: 'Delay', label: 'Отложено' },
        { value: 'Succeed', label: 'Выполнено' },
        { value: 'Rejected', label: 'Отклонено' }
      ],
      priorities: [
        { value: 'Low', label: 'Низкий' },
        { value: 'Normal', label: 'Средний' },
        { value: 'High', label: 'Высокий' }
      ],
      dialogVisible: false,
      dialogData: null,
      selectedRow: null
    };
  },
  watch: {
    search(value) {
      this.query.filter = value;
      this.refresh();
    },
    order(value) {
      this.query.sortFields[0].sortType = value;
      this.refresh();
    },
    sort(value) {
      this.query.sortFields[0].fieldName = value;
      this.refresh();
    },
    filters: {
      deep: true,
      handler(value) {
        this.query.filterFields = value;
        this.refresh();
      }
    }
  },
  computed: {
    ...mapGetters({
      items: '',
      projects: 'projects/getProjects',
      teams: 'teams/getTeams',
      users: 'users/getUsers'
    }),
    isEditVisible() {
      return !this.selectedRow?.isRemoved;
    },
    isStatusVisible() {
      return !this.selectedRow?.isRemoved;
    },
    isDeleteVisible() {
      return !this.selectedRow?.isRemoved;
    },
    isRestoreVisible() {
      return !!this.selectedRow?.isRemoved;
    },
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
  mounted() {
    if (this.$route.query.tab === 'deleted') {
      this.query.filterFields.push({
        fieldName: 'isRemoved',
        values: [true]
      });
      this.query.withRemoved = true;
      this.refresh();
    }
  },
  methods: {
    ...mapActions({
      fetchItems: '',
      deleteItem: '',
      deleteItems: '',
      restoreItem: '',
      restoreItems: '',
      updateItem: '',
      updateItems: '',

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
      this.data = [];
      this.query.pageNumber = 0;
      this.loader.stateChanger.reset();
      this.dialogVisible = false;
    },
    async load($state) {
      const firstLoad = !this.data.length;
      if (firstLoad) this.loading = true;
      const items = await this.fetch(this.query);
      if (items.length) $state.loaded();
      else $state.complete();
      this.data = firstLoad ? items : this.data.concat(items);
      if (firstLoad) this.loading = false;
      this.updateLists();
    },
    async fetch(params) {
      const items = await this.$store.dispatch(this.actions.fetchItems, params);
      this.query.pageNumber++;
      return items;
    },
    onFilterChange(value) {
      console.log(value);
    },
    onOrderChange() {
      this.query.sortFields[0].sortType =
        this.query.sortFields[0].sortType === 'Ascending'
          ? 'Descending'
          : 'Ascending';
      this.refresh();
    },
    onSortChange(value) {
      this.query.sortFields[0].fieldName = value;
      this.refresh();
    },
    onViewChange(value) {
      this.view = value;
    },
    onItemSelect(selection, row) {
      this.selectedRow = row;
    },
    onItemSingleClick(row, column, event) {
      this.table.clearSelection();
      this.table.toggleRowSelection(row);
      this.selectedRow = row;
    },
    onItemDoubleClick(event, item) {
      this.onItemEdit(item);
    },
    onItemRightClick(event, item) {
      this.contextMenu.open(event, { item });
      event.preventDefault();
    },
    onItemCreate() {
      this.dialogData = null;
      this.dialogVisible = true;
    },
    onItemEdit(item) {
      this.dialogData = item;
      this.dialogVisible = true;
    },
    async onItemDelete(item) {
      await this.$store.dispatch(this.actions.deleteItem, item.id);
      await this.refresh();
    },
    async onItemRestore(item) {
      await this.$store.dispatch(this.actions.restoreItem, item.id);
      await this.refresh();
    },
    async onItemStatusChange(item, status) {
      item.state = status;
      await this.$store.dispatch(this.actions.updateItem, item);
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
