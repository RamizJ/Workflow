import {
  Action,
  getModule,
  Module,
  Mutation,
  MutationAction,
  VuexModule,
} from 'vuex-module-decorators'

import store from '@/core/store'
import api from '../api'
import goalsAPI from '@/modules/goals/api'
import Project from '@/modules/projects/models/project.type'
import Team from '@/modules/teams/models/team.type'
import Query from '@/core/types/query.type'
import User from '@/modules/users/models/user.type'
import { Status } from '@/modules/goals/models/goal.type'
import { TeamRole } from '@/modules/teams/models/team-role.type'
import { UserRole } from '@/modules/users/models/user-role.type'
import { ProjectStatistics } from '@/modules/projects/models/project-statistics.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'projectsStore',
  store,
})
class ProjectsStore extends VuexModule {
  _project: Project | null = null
  _projects: Project[] = []
  _projectTeams: Team[] = []
  _projectUsers: User[] = []
  _projectWindowOpened = false

  public get isProjectWindowOpened(): boolean {
    return this._projectWindowOpened
  }
  public get project() {
    return this._project
  }
  public get projects() {
    return this._projects
  }
  public get projectTeams() {
    return this._projectTeams
  }
  public get projectUsers() {
    return this._projectUsers
  }

  @MutationAction({ mutate: ['_projectWindowOpened'] })
  public async closeProjectWindow() {
    return {
      _projectWindowOpened: false,
    }
  }

  @MutationAction({ mutate: ['_projectWindowOpened'] })
  public async openProjectWindow() {
    return {
      _projectWindowOpened: true,
    }
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

  @Mutation
  setProjectUsers(users: User[]) {
    this._projectUsers = users
  }

  @Action
  async findAll(query: Query): Promise<Project[]> {
    const response = await api.getPage(query)
    const results = response.data as Project[]
    this.context.commit('setProjects', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Project[]> {
    const response = await api.getRange(ids)
    const entities = response.data as Project[]
    this.context.commit('setProjects', entities)
    return entities
  }

  @Action
  async findOneById(id: number): Promise<Project> {
    const response = await api.get(id)
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
    const response = await api.createByForm(request)
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
    await api.update(request)
  }

  @Action
  async updateMany(entities: Project[]): Promise<void> {
    await api.updateRange(entities)
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await api.remove(id)
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await api.removeRange(ids)
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await api.restore(id)
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await api.restoreRange(ids)
  }

  @Action
  async findTeams(query: Query): Promise<Team[]> {
    const response = await api.getTeamsPage(query)
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
    await api.addTeam(projectId, teamId)
  }

  @Action
  async addTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await api.addTeam(projectId, teamId)
    }
  }

  @Action
  async getTeamRole({
    projectId,
    teamId,
  }: {
    projectId: number
    teamId: number
  }): Promise<TeamRole> {
    const response = await api.getTeamRole(projectId, teamId)
    const results = response.data as TeamRole
    return results
  }

  @Action
  async updateTeamRole(teamRole: TeamRole) {
    await api.updateTeamRole(teamRole)
  }

  @Action
  async updateTeamRoles(teamRoles: TeamRole[]) {
    await api.updateTeamRoles(teamRoles)
  }

  @Action
  async removeTeam({ projectId, teamId }: { projectId: number; teamId: number }) {
    await api.removeTeam(projectId, teamId)
  }

  @Action
  async removeTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await api.removeTeam(projectId, teamId)
    }
  }

  @Action
  async getUserRole({
    projectId,
    userId,
  }: {
    projectId: number
    userId: string
  }): Promise<UserRole> {
    const response = await api.getUserRole(projectId, userId)
    const results = response.data as UserRole
    return results
  }

  @Action
  async updateUserRole(userRole: UserRole) {
    await api.updateUserRole(userRole)
  }

  @Action
  async updateUserRoles(userRoles: UserRole[]) {
    await api.updateUserRoles(userRoles)
  }

  @Action
  async getStatistics({
    projectId,
    dateBegin,
    dateEnd,
  }: {
    projectId: number
    dateBegin: string
    dateEnd: string
  }): Promise<ProjectStatistics> {
    const response = await api.getProjectStatistics(projectId, dateBegin, dateEnd)
    return response.data
  }

  // TODO: Move API methods below to projects API

  @Action
  async getTasksCount(projectId: number): Promise<number> {
    const response = await goalsAPI.getTotalProjectGoalsCount(projectId)
    return response.data
  }

  @Action
  async getTasksCountByStatus({
    projectId,
    status,
  }: {
    projectId: number
    status: Status
  }): Promise<number> {
    const response = await goalsAPI.getProjectGoalsByStateCount(projectId, status)
    return response.data
  }
}

export default getModule(ProjectsStore)
