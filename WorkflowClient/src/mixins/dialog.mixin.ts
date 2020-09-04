import { Component, Vue } from 'vue-property-decorator'
import { ElForm } from 'element-ui/types/form'

import projectsModule from '@/store/modules/projects.module'
import teamsModule from '@/store/modules/teams.module'
import usersModule from '@/store/modules/users.module'

@Component
export default class DialogMixin extends Vue {
  public visible = false
  public loading = false

  public get projects(): { value: string | undefined; id: number | undefined }[] {
    return projectsModule.projects.map((project) => {
      return {
        value: project.name,
        id: project.id,
      }
    })
  }
  public get teams(): { value: string | undefined; id: number | undefined }[] {
    return teamsModule.teams.map((team) => {
      return {
        value: team.name,
        id: team.id,
      }
    })
  }
  public get users(): { value: string | undefined; id: string | undefined }[] {
    return usersModule.users.map((user) => {
      return {
        value: this.shortenFullName(`${user.lastName} ${user.firstName} ${user.middleName}`),
        id: user.id,
      }
    })
  }

  public async searchProjects(query = ''): Promise<void> {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  public async searchTeams(query = ''): Promise<void> {
    await teamsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  public async searchUsers(query = ''): Promise<void> {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  public shortenFullName(value: string): string {
    if (!value) return value
    const fioArray = value.split(' ')
    const lastName = fioArray[0]
    const firstNameInitial = fioArray[1][0] ? `${fioArray[1][0]}.` : ''
    const middleNameInitial = fioArray[2][0] ? `${fioArray[2][0]}.` : ''
    return `${lastName} ${firstNameInitial} ${middleNameInitial}`
  }

  public exit(): void {
    ;(this.$refs.form as ElForm).resetFields()
    this.visible = false
    this.$emit('close')
  }
}
