import { AxiosResponse } from 'axios'
import qs from 'qs'
import api from '@/core/api'
import Query from '@/core/types/query.type'
import Team from '@/modules/teams/models/team.type'
import User from '@/modules/users/models/user.type'
import Project from '@/modules/projects/models/project.type'

export default {
  get: (id: number): Promise<AxiosResponse<Team>> => {
    return api.request({
      url: `/api/Teams/Get/${id}`,
      method: 'GET',
    })
  },
  getPage: (query: Query): Promise<AxiosResponse<Team[]>> => {
    return api.request({
      url: `/api/Teams/GetPage`,
      method: 'POST',
      data: query,
    })
  },
  getUsersPage: (query: Query): Promise<AxiosResponse<User[]>> => {
    return api.request({
      url: `/api/Teams/GetUsersPage?teamId=${query.teamId}`,
      method: 'POST',
      data: query,
    })
  },
  getProjectsPage: (query: Query): Promise<AxiosResponse<Project[]>> => {
    return api.request({
      url: `/api/Teams/GetProjectsPage?teamId=${query.teamId}`,
      method: 'POST',
      data: query,
    })
  },
  getRange: (ids: number[]): Promise<AxiosResponse<Team[]>> => {
    return api.request({
      url: `/api/Teams/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },
  create: (team: Team): Promise<AxiosResponse<Team>> => {
    return api.request({
      url: `/api/Teams/Create`,
      method: 'POST',
      data: team,
    })
  },
  createByForm: (formTeam: {
    team: Team
    userIds: string[]
    projectIds: number[]
  }): Promise<AxiosResponse<Team>> => {
    return api.request({
      url: `/api/Teams/CreateByForm`,
      method: 'POST',
      data: formTeam,
    })
  },
  update: (team: Team): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/Update`,
      method: 'PUT',
      data: team,
    })
  },
  updateByForm: (formTeam: {
    team: Team
    userIds: string[]
    projectIds: number[]
  }): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/UpdateByForm`,
      method: 'PUT',
      data: formTeam,
    })
  },
  updateRange: (teams: Team[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/UpdateRange`,
      method: 'PUT',
      data: teams,
    })
  },
  updateByFormRange: (
    formTeams: {
      team: Team
      userIds: string[]
      projectIds: number[]
    }[]
  ): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/UpdateByFormRange`,
      method: 'PUT',
      data: formTeams,
    })
  },
  remove: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/Delete/${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  addUser: (teamId: number, userId: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/AddUser/${teamId}`,
      method: 'PATCH',
      data: JSON.stringify(userId),
    })
  },
  addUsers: (teamId: number, userIds: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/AddUsers/${teamId}`,
      method: 'PATCH',
      data: userIds,
    })
  },
  removeUser: (teamId: number, userId: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/RemoveUser/${teamId}`,
      method: 'PATCH',
      data: JSON.stringify(userId),
    })
  },
  removeUsers: (teamId: number, userIds: string): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/RemoveUsers/${teamId}`,
      method: 'PATCH',
      data: userIds,
    })
  },
  addProject: (teamId: number, projectId: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/AddProject/${teamId}`,
      method: 'PATCH',
      data: projectId,
    })
  },
  removeProject: (teamId: number, projectId: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Teams/RemoveProject/${teamId}/${projectId}`,
      method: 'PATCH',
    })
  },
}
