import { Component, Vue } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'

import projectsModule from '@/store/modules/projects.module'
import teamsModule from '@/store/modules/teams.module'
import usersModule from '@/store/modules/users.module'

@Component
export default class DialogMixin extends Vue {
  public visible = false
  public loading = false

  public get projects(): { value: string; id: number | undefined }[] {
    return projectsModule.projects.map(project => {
      return {
        value: project.name,
        id: project.id
      }
    })
  }
  public get teams(): { value: string; id: number | undefined }[] {
    return teamsModule.teams.map(team => {
      return {
        value: team.name,
        id: team.id
      }
    })
  }
  public get users(): { value: string; id: string | undefined }[] {
    return usersModule.users.map(user => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id
      }
    })
  }

  public async searchProjects(query = ''): Promise<void> {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  public async searchTeams(query = ''): Promise<void> {
    await teamsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  public async searchUsers(query = ''): Promise<void> {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    })
  }

  public exit(): void {
    ;(this.$refs.form as ElForm).resetFields()
    this.visible = false
    this.$emit('close')
  }
}
