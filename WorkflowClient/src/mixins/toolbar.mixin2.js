import { mapActions, mapGetters } from 'vuex';

export default {
  data() {
    return {
      q: this.$route.query.q || '',
      order: this.$route.query.order || 'Descending',
      sort: this.$route.query.sort || '',
      view: this.$route.query.view || 'list',
      sortFields: [],
      filters: {
        statuses: [],
        priorities: [],
        projects: [],
        performers: [],
        owners: [],
        deadlineRange: '',
        showOnlyDeleted: false,
        withAttachments: false
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
      ]
    };
  },
  computed: {
    projects() {
      return this.$store.getters['ProjectsModule/projects'];
    },
    users() {
      return this.$store.getters['UsersModule/users'];
    },
    // ...mapGetters({
    //   projects: 'projects/getProjects',
    //   users: 'users/getUsers'
    // }),
    filterFields() {
      const filterFields = [];
      if (this.filters.statuses.length)
        filterFields.push({
          fieldName: 'state',
          values: this.filters.statuses
        });
      if (this.filters.priorities.length)
        filterFields.push({
          fieldName: 'priority',
          values: this.filters.priorities
        });
      if (this.filters.projects.length)
        filterFields.push({
          fieldName: 'projectId',
          values: this.filters.projects
        });
      if (this.filters.performers.length)
        filterFields.push({
          fieldName: 'performerId',
          values: this.filters.performers
        });
      if (this.filters.owners.length)
        filterFields.push({
          fieldName: 'ownerId',
          values: this.filters.owners
        });
      if (this.filters.showOnlyDeleted)
        filterFields.push({
          fieldName: 'isRemoved',
          values: [true]
        });
      return filterFields;
    },
    projectList() {
      return this.projects.map(project => {
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
    document.querySelectorAll('.el-select.remote').forEach(element => {
      const arrow = element.children[1].children[1];
      arrow.addEventListener('click', function() {
        this.click();
        element.click();
      });
    });
  },
  methods: {
    ...mapActions({
      fetchProjects: 'projects/fetchProjects',
      fetchUsers: 'users/fetchUsers'
    }),
    onSearch(value) {
      this.$emit('search', value);
    },
    onFiltersChange() {
      this.$emit('filters', this.filterFields);
    },
    onOrderChange(value) {
      this.order = value;
      this.$emit('order', value);
    },
    onSortChange(value) {
      this.sort = value;
      this.$emit('sort', value);
    },
    onViewChange(value) {
      this.view = value;
      this.$emit('view', value);
    },
    async onProjectsFocus() {
      await this.searchProjects();
    },
    async searchProjects(query) {
      await this.fetchProjects({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
    },
    async onUsersFocus() {
      await this.searchUsers();
    },
    async searchUsers(query) {
      await this.fetchUsers({
        filter: query,
        pageNumber: 0,
        pageSize: 10
      });
    }
  }
};
