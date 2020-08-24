import { mapActions, mapGetters } from 'vuex';
import moment from 'moment';

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
      tableData: [],
      query: {
        projectId: this.$route.params.projectId || null,
        teamId: this.$route.params.teamId || null,
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
      selectedRow: null,
      isShift: false
    };
  },
  created() {
    document.onkeydown = this.onKeyDown;
    document.onkeyup = this.onKeyUp;
  },
  watch: {
    search(value) {
      this.query.filter = value;
      this.refresh();
    },
    filters: {
      deep: true,
      handler(value) {
        this.query.filterFields = value;
        this.query.withRemoved = !!value.find(
          filter => filter.fieldName === 'isRemoved' && filter.values[0] === true
        );
        this.refresh();
      }
    },
    order(value) {
      this.query.sortFields[0].sortType = value;
      this.refresh();
    },
    sort(value) {
      this.query.sortFields[0].fieldName = value;
      this.refresh();
    }
  },
  computed: {
    projects() {
      return this.$store.getters['ProjectsModule/projects'];
    },
    teams() {
      return this.$store.getters['TeamsModule/teams'];
    },
    users() {
      return this.$store.getters['UsersModule/users'];
    },
    // ...mapGetters({
    // items: ''
    // projects: 'projects/getProjects',
    // teams: 'teams/getTeams',
    // users: 'users/getUsers'
    // }),
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
      if (Array.isArray(this.$refs.contextMenu)) return this.$refs.contextMenu[0];
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
  methods: {
    ...mapActions({
      fetchItems: '',
      deleteItem: '',
      deleteItems: '',
      restoreItem: '',
      restoreItems: '',
      updateItem: '',
      updateItems: '',

      fetchProjects: 'projects/findAll',
      fetchTeams: 'teams/findAll',
      fetchUsers: 'users/findAll'
    }),
    onKeyDown() {
      const key = window.event.keyCode;
      if (key === 16) {
        this.isShift = true;
      }
    },
    onKeyUp() {
      this.isShift = false;
    },
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
    },
    async load($state) {
      const firstLoad = !this.tableData.length;
      if (firstLoad) this.loading = true;
      const items = await this.fetch(this.query);
      if (items.length) $state.loaded();
      else $state.complete();
      this.tableData = firstLoad ? items : this.tableData.concat(items);
      if (firstLoad) this.loading = false;
    },
    async fetch(params) {
      // const tasksModule = GetModule();
      console.log('fetch data for table');
      console.log(this.actions);
      const test = await this.$store.dispatch('TasksModule/findAll', params);
      console.log(test);
      // const items = await this.$store.dispatch(this.actions.fetchItems, params);
      // this.query.pageNumber++;
      // return items;
    },
    onFilterChange(value) {
      console.log(value);
    },
    onOrderChange() {
      this.query.sortFields[0].sortType =
        this.query.sortFields[0].sortType === 'Ascending' ? 'Descending' : 'Ascending';
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
    onSetIndex({ row, rowIndex }) {
      row.index = rowIndex;
    },
    onItemSingleClick(row, column, event) {
      this.table.clearSelection();
      if (this.isShift) {
        const beginIndex = this.selectedRow.index < row.index ? this.selectedRow.index : row.index;
        const endIndex = this.selectedRow.index > row.index ? this.selectedRow.index : row.index;
        this.tableData.some(item => {
          if (item.index <= endIndex && item.index >= beginIndex)
            this.table.toggleRowSelection(item);
        });
      } else {
        this.table.toggleRowSelection(row);
      }
      this.selectedRow = row;
    },
    onItemDoubleClick(row, column, event) {
      if (!row.isRemoved) this.onItemEdit(event, row);
    },
    onItemRightClick(row, column, event) {
      if (!this.isMultipleSelected) {
        this.table.clearSelection();
        this.table.toggleRowSelection(row);
      }
      this.table.setCurrentRow(row);
      this.selectedRow = row;
      this.contextMenu.open(event, { row, column });
      event.preventDefault();
    },
    onItemCreate() {
      this.dialogData = null;
      this.dialogVisible = true;
    },
    onItemEdit(event, row) {
      this.dialogData = row;
      this.dialogVisible = true;
    },
    async onItemDelete(event, row) {
      if (this.isMultipleSelected)
        await this.$store.dispatch(
          this.actions.deleteItems,
          this.table.selection.map(item => item.id)
        );
      else await this.$store.dispatch(this.actions.deleteItem, row.id);
      this.refresh();
    },
    async onItemRestore(event, row) {
      if (this.isMultipleSelected)
        await this.$store.dispatch(
          this.actions.restoreItems,
          this.table.selection.map(item => item.id)
        );
      else await this.$store.dispatch(this.actions.restoreItem, row.id);
      this.refresh();
    },
    async onItemStatusChange(event, row, status) {
      if (this.isMultipleSelected) {
        const items = this.table.selection.map(item => {
          item.state = status;
          return item;
        });
        await this.$store.dispatch(this.actions.updateItems, items);
      } else {
        const item = row;
        item.state = status;
        await this.$store.dispatch(this.actions.updateItem, item);
      }
      await this.refresh();
    },
    dateFormatter(row, column, cellValue, index) {
      const dateUtc = moment.utc(cellValue);
      return dateUtc.format('DD.MM.YYYY HH:mm');
    },
    priorityFormatter(row, column, cellValue, index) {
      return this.priorities.find(priority => priority.value === cellValue).label;
    },
    stateFormatter(row, column, cellValue, index) {
      return this.statuses.find(status => status.value === cellValue).label;
    }
  }
};
