import { mapActions, mapGetters } from 'vuex';

export default {
  props: {
    data: Object
  },
  data() {
    return {
      visible: false,
      loading: false,
      isEdit: !!this.data
    };
  },
  async created() {
    this.visible = true;
    if (this.isEdit) this.form = { ...this.data };
  },
  computed: {
    ...mapGetters({
      projects: 'projects/getProjects',
      teams: 'teams/getTeams',
      users: 'users/getUsers',
      me: 'auth/me'
    }),
    isFormValid() {
      let formValid = false;
      this.$refs.form.validate(valid => {
        if (valid) formValid = true;
        else {
          formValid = false;
          this.$message({
            showClose: true,
            message: 'Форма заполнена некорректно',
            type: 'error'
          });
        }
      });
      return formValid;
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
      return this.teams.map(team => {
        return {
          value: team.name,
          id: team.id
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
    async sendForm() {
      const payload = { ...this.form };
      this.loading = true;
      try {
        if (this.isEdit) await this.updateItem(payload);
        else await this.createItem(payload);
      } catch (error) {
        this.$message.error(`Ошибка отправки запроса`);
      }
      this.loading = false;
    },
    async submit(event) {
      if (this.isFormValid) {
        await this.sendForm();
        this.$emit('submit');
        this.exit();
      }
    },
    exit() {
      this.$refs.form?.resetFields();
      this.visible = false;
      this.$emit('close');
    }
  }
};
