import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'

import store from '@/store'
import projectsAPI from '@/api/projects.api'
import Project from '@/types/project.type'
import Team from '@/types/team.type'
import Query from '@/types/query.type'
import { Status } from '@/types/task.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'projectsModule',
  store,
})
class ProjectsModule extends VuexModule {
  _project: Project | null = null
  _projects: Project[] = []
  _projectTeams: Team[] = []

  public get project() {
    return this._project
  }

  public get projects() {
    return this._projects
  }

  public get projectTeams() {
    return this._projectTeams
  }

  @Mutation
  setProject(project: Project) {
    this._project = project
  }

  @Mutation
  setProjects(projects: Project[]) {
    this._projects = projects
  }

  @Mutation
  setProjectTeams(teams: Team[]) {
    this._projectTeams = teams
  }

  @Action
  async findAll(query: Query): Promise<Project[]> {
    const response = await projectsAPI.findAll(query)
    const results = response.data as Project[]
    this.context.commit('setProjects', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Project[]> {
    const response = await projectsAPI.findAllByIds(ids)
    const entities = response.data as Project[]
    this.context.commit('setProjects', entities)
    return entities
  }

  @Action
  async findOneById(id: number): Promise<Project> {
    const response = await projectsAPI.findOneById(id)
    const result = response.data as Project
    const projectTeams: Team[] = await this.context.dispatch('findTeams', {
      projectId: id,
      pageNumber: 0,
      pageSize: 20,
    })
    result.teams = projectTeams
    result.teamIds = projectTeams.map((team) => (team.id ? team.id : -1))
    this.context.commit('setProject', result)
    this.context.commit('setProjectTeams', projectTeams)
    return result
  }

  @Action
  async createOne(entity: Project): Promise<Project> {
    const request = {
      project: entity,
      teamIds: entity.teamIds || [],
    }
    const response = await projectsAPI.createOne(request)
    const result = response.data as Project
    result.teamIds = entity.teamIds
    this.context.commit('setProject', result)
    return result
  }

  @Action
  async createMany(entities: Project[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('createOne', entity)
    }
  }

  @Action
  async updateOne(entity: Project): Promise<void> {
    const request = {
      project: entity,
      teamIds: entity.teamIds || [],
    }
    await projectsAPI.updateOne(request)
  }

  @Action
  async updateMany(entities: Project[]): Promise<void> {
    await projectsAPI.updateMany(entities)
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await projectsAPI.deleteOne(id)
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await projectsAPI.deleteMany(ids)
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await projectsAPI.restoreOne(id)
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await projectsAPI.restoreMany(ids)
  }

  @Action
  async findTeams(query: Query): Promise<Team[]> {
    const response = await projectsAPI.findTeams(query)
    const results = (response.data as Team[]).map((team) => {
      team.userIds = []
      team.projectIds = []
      return team
    })
    this.context.commit('setProjectTeams', results)
    return results
  }

  @Action
  async addTeam({ projectId, teamId }: { projectId: number; teamId: number }) {
    await projectsAPI.addTeam(projectId, teamId)
  }

  @Action
  async addTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await projectsAPI.addTeam(projectId, teamId)
    }
  }

  @Action
  async removeTeam({ projectId, teamId }: { projectId: number; teamId: number }) {
    await projectsAPI.removeTeam(projectId, teamId)
  }

  @Action
  async removeTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await projectsAPI.removeTeam(projectId, teamId)
    }
  }

  @Action
  async getTasksCount(projectId: number): Promise<number> {
    const response = await projectsAPI.getTasksCount(projectId)
    return parseInt(response.data) as number
  }

  @Action
  async getTasksCountByStatus({
    projectId,
    status,
  }: {
    projectId: number
    status: Status
  }): Promise<number> {
    const response = await projectsAPI.getTasksCountByStatus(projectId, status)
    return parseInt(response.data) as number
  }
}

export default getModule(ProjectsModule)
