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

  exit() {
    ;(this.$refs.form as ElForm).resetFields()
    this.visible = false
    this.$emit('close')
  }
}
