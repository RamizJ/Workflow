import { Component, Prop, Vue } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'

import projectsModule from '@/store/modules/projects.module'
import teamsModule from '@/store/modules/teams.module'
import usersModule from '@/store/modules/users.module'

@Component
export default class DialogMixin extends Vue {
  public visible = false
  public loading = false

  public get projects() {
    return projectsModule.projects.map(project => {
      return {
        value: project.name,
        id: project.id
      }
    })
  }
  public get teams() {
    return teamsModule.teams.map(team => {
      return {
        value: team.name,
        id: team.id
      }
    })
  }
  public get users() {
    return usersModule.users.map(user => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id
      }
    })
  }

  async searchProjects(query = '') {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  async searchTeams(query = '') {
    await teamsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  async searchUsers(query = '') {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  /*async sendForm() {
    this.loading = true
    const payload = { ...this.form }
    // if (this.isEdit) await this.updateItem(payload);
    // else await this.createItem(payload);
    this.loading = false
  }*/

  // public async submit(event: Event) {
  //   await (this.$refs.form as ElForm).validate(async valid => {
  //     if (valid) {
  //       // await this.sendForm()
  //       this.$emit('submit')
  //       this.exit()
  //     } else {
  //       this.$message({
  //         showClose: true,
  //         message: 'Форма заполнена некорректно',
  //         type: 'error'
  //       })
  //     }
  //   })
  // }

  exit() {
    ;(this.$refs.form as ElForm).resetFields()
    this.visible = false
    this.$emit('close')
  }
}

/*export default {
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
      fetchProjects: 'projects/findAll',
      fetchTeams: 'teams/findAll',
      fetchUsers: 'users/findAll'
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
      this.loading = true;
      const payload = { ...this.form };
      if (this.isEdit) await this.updateItem(payload);
      else await this.createItem(payload);
      this.loading = false;
    },
    async submit(event) {
      await this.$refs.form.validate(async valid => {
        if (valid) {
          await this.sendForm();
          this.$emit('submit');
          this.exit();
        } else {
          this.$message({
            showClose: true,
            message: 'Форма заполнена некорректно',
            type: 'error'
          });
        }
      });
    },
    exit() {
      this.$refs.form?.resetFields();
      this.visible = false;
      this.$emit('close');
    }
  }
};*/
