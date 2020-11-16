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
import Team from '@/modules/teams/models/team.type'
import Query from '@/core/types/query.type'
import Project from '@/modules/projects/models/project.type'
import User from '@/modules/users/models/user.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'teamsStore',
  store,
})
class TeamsStore extends VuexModule {
  _teamWindowOpened = false
  _team: Team | null = null
  _teams: Team[] = []
  _teamUsers: User[] = []
  _teamProjects: Project[] = []

  public get isTeamWindowOpened() {
    return this._teamWindowOpened
  }
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

  @MutationAction({ mutate: ['_teamWindowOpened'] })
  public async closeTeamWindow() {
    return {
      _teamWindowOpened: false,
    }
  }

  @MutationAction({ mutate: ['_teamWindowOpened', '_team'] })
  public async openTeamWindow(team?: Team) {
    return {
      _team: team || null,
      _teamWindowOpened: true,
    }
  }

  @Mutation
  setTeam(team: Team | null) {
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
    const response = await api.getPage(query)
    const results = response.data as Team[]
    this.context.commit('setTeams', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Team[]> {
    const response = await api.getRange(ids)
    const results = response.data as Team[]
    this.context.commit('setTeams', results)
    return results
  }

  @Action
  async findOneById(id: number): Promise<Team> {
    const response = await api.get(id)
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
    const response = await api.createByForm(request)
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
    await api.update(entity)
  }

  @Action
  async updateMany(entities: Team[]): Promise<void> {
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
  async findUsers(query: Query): Promise<User[]> {
    const response = await api.getUsersPage(query)
    const results = response.data as User[]
    this.context.commit('setTeamUsers', results)
    return results
  }

  @Action
  async addUser({ teamId, userId }: { teamId: number; userId: string }) {
    await api.addUser(teamId, userId)
  }

  @Action
  async addUsers({ teamId, userIds }: { teamId: number; userIds: string[] }) {
    for (const userId of userIds) {
      await api.addUser(teamId, userId)
    }
  }

  @Action
  async removeUser({ teamId, userId }: { teamId: number; userId: string }) {
    await api.removeUser(teamId, userId)
  }

  @Action
  async removeUsers({ teamId, userIds }: { teamId: number; userIds: string[] }) {
    for (const userId of userIds) {
      await api.removeUser(teamId, userId)
    }
  }

  @Action
  async findProjects(query: Query): Promise<Project[]> {
    const response = await api.getProjectsPage(query)
    const results = response.data as Project[]
    this.context.commit('setTeamProjects', results)
    return results
  }

  @Action
  async addProject({ teamId, projectId }: { teamId: number; projectId: number }) {
    await api.addProject(teamId, projectId)
  }

  @Action
  async addProjects({ teamId, projectIds }: { teamId: number; projectIds: number[] }) {
    for (const projectId of projectIds) {
      await api.addProject(teamId, projectId)
    }
  }

  @Action
  async removeProject({ teamId, projectId }: { teamId: number; projectId: number }) {
    await api.removeProject(teamId, projectId)
  }

  @Action
  async removeProjects({ teamId, projectIds }: { teamId: number; projectIds: number[] }) {
    for (const projectId of projectIds) {
      await api.removeProject(teamId, projectId)
    }
  }
}

export default getModule(TeamsStore)
