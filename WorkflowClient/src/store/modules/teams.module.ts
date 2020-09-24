import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'

import store from '@/store'
import teamsAPI from '@/api/teams.api'
import Team from '@/types/team.type'
import Query from '@/types/query.type'
import Project from '@/types/project.type'
import User from '@/types/user.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'teamsModule',
  store,
})
class TeamsModule extends VuexModule {
  _team: Team | null = null
  _teams: Team[] = []
  _teamUsers: User[] = []
  _teamProjects: Project[] = []

  public get team() {
    return this._team
  }
  public get teams() {
    return this._teams
  }
  public get teamUsers() {
    return this._teamUsers
  }
  public get teamProjects() {
    return this._teamProjects
  }

  @Mutation
  setTeam(team: Team) {
    this._team = team
  }

  @Mutation
  setTeams(teams: Team[]) {
    this._teams = teams
  }

  @Mutation
  setTeamUsers(users: User[]) {
    this._teamUsers = users
  }

  @Mutation
  setTeamProjects(projects: Project[]) {
    this._teamProjects = projects
  }

  @Action
  async findAll(query: Query): Promise<Team[]> {
    const response = await teamsAPI.findAll(query)
    const results = response.data as Team[]
    this.context.commit('setTeams', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Team[]> {
    const response = await teamsAPI.findAllByIds(ids)
    const results = response.data as Team[]
    this.context.commit('setTeams', results)
    return results
  }

  @Action
  async findOneById(id: number): Promise<Team> {
    const response = await teamsAPI.findOneById(id)
    const result = response.data as Team
    result.userIds = []
    result.projectIds = []
    this.context.commit('setTeam', result)
    return result
  }

  @Action
  async createOne(entity: Team): Promise<Team> {
    const request = {
      team: entity,
      userIds: entity.userIds || [],
      projectIds: entity.projectIds || [],
    }
    const response = await teamsAPI.createOne(request)
    const result = response.data as { team: Team; userIds: string[]; projectIds: number[] }
    result.team.userIds = result.userIds
    result.team.projectIds = result.projectIds
    this.context.commit('setTeam', result.team)
    return result.team
  }

  @Action
  async createMany(entities: Team[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('createOne', entity)
    }
  }

  @Action
  async updateOne(entity: Team): Promise<void> {
    const request = {
      team: entity,
      userIds: entity.userIds || [],
      projectIds: entity.projectIds || [],
    }
    await teamsAPI.updateOne(request)
  }

  @Action
  async updateMany(entities: Team[]): Promise<void> {
    await teamsAPI.updateMany(entities)
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await teamsAPI.deleteOne(id)
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await teamsAPI.deleteMany(ids)
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await teamsAPI.restoreOne(id)
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await teamsAPI.restoreMany(ids)
  }

  @Action
  async findUsers(query: Query): Promise<User[]> {
    const response = await teamsAPI.findUsers(query)
    const results = response.data as User[]
    this.context.commit('setTeamUsers', results)
    return results
  }

  @Action
  async addUser({
    teamId,
    userId,
    canEditUsers,
    canEditTasks,
    canCloseTasks,
  }: {
    teamId: number
    userId: string
    canEditUsers: boolean
    canEditTasks: boolean
    canCloseTasks: boolean
  }) {
    await teamsAPI.addUser(teamId, userId, canEditUsers, canEditTasks, canCloseTasks)
  }

  @Action
  async updateUser({
    teamId,
    userId,
    canEditUsers,
    canEditTasks,
    canCloseTasks,
  }: {
    teamId: number
    userId: string
    canEditUsers: boolean
    canEditTasks: boolean
    canCloseTasks: boolean
  }) {
    await teamsAPI.updateUser(teamId, userId, canEditUsers, canEditTasks, canCloseTasks)
  }

  @Action
  async addUsers({
    teamId,
    userIds,
    canEditUsers,
    canEditTasks,
    canCloseTasks,
  }: {
    teamId: number
    userIds: string[]
    canEditUsers: boolean
    canEditTasks: boolean
    canCloseTasks: boolean
  }) {
    for (const userId of userIds) {
      await teamsAPI.addUser(teamId, userId, canEditUsers, canEditTasks, canCloseTasks)
    }
  }

  @Action
  async removeUser({ teamId, userId }: { teamId: number; userId: string }) {
    await teamsAPI.removeUser(teamId, userId)
  }

  @Action
  async removeUsers({ teamId, userIds }: { teamId: number; userIds: string[] }) {
    for (const userId of userIds) {
      await teamsAPI.removeUser(teamId, userId)
    }
  }

  @Action
  async findProjects(query: Query): Promise<Project[]> {
    const response = await teamsAPI.findProjects(query)
    const results = response.data as Project[]
    this.context.commit('setTeamProjects', results)
    return results
  }

  @Action
  async addProject({ teamId, projectId }: { teamId: number; projectId: number }) {
    await teamsAPI.addProject(teamId, projectId)
  }

  @Action
  async addProjects({ teamId, projectIds }: { teamId: number; projectIds: number[] }) {
    for (const projectId of projectIds) {
      await teamsAPI.addProject(teamId, projectId)
    }
  }

  @Action
  async removeProject({ teamId, projectId }: { teamId: number; projectId: number }) {
    await teamsAPI.removeProject(teamId, projectId)
  }

  @Action
  async removeProjects({ teamId, projectIds }: { teamId: number; projectIds: number[] }) {
    for (const projectId of projectIds) {
      await teamsAPI.removeProject(teamId, projectId)
    }
  }
}

export default getModule(TeamsModule)
