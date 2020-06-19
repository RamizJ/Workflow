import { mapActions, mapGetters } from 'vuex';

export default {
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.id
    };
  },
  async mounted() {
    if (this.isEdit) {
      this.loading = true;
      await this.fetchItem(this.id);
      this.form = this.item;
      this.loading = false;
    }
    this.visible = true;
  },
  computed: {
    ...mapGetters({
      item: '',
      projects: 'projects/getProjects',
      teams: 'teams/getTeams',
      users: 'users/getUsers',
      me: 'auth/me'
    }),
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
      fetchItem: '',
      createItem: '',
      updateItem: '',
      deleteItem: '',
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
    submit() {
      this.form.ownerId = this.form.ownerId || this.me.id;
      const payload = { ...this.form };
      const form = this.$refs.form;
      form.validate(async valid => {
        if (valid) {
          try {
            if (this.isEdit) await this.updateItem(payload);
            else await this.createItem(payload);
            form.resetFields();
            this.$emit('submit');
            this.exit();
          } catch (e) {
            this.$message.error('Ошибка отправки запроса');
          }
        } else {
          this.$message.error('Укажите корректные данные');
        }
      });
    },
    exit() {
      this.$refs.form.resetFields();
      this.visible = false;
      this.$emit('close');
    }
  }
};
