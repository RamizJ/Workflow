import { AxiosResponse } from 'axios'
import qs from 'qs'

import api from '@/core/api'
import Query from '@/core/types/query.type'
import Project from '@/modules/projects/models/project.type'
import Team from '@/modules/teams/models/team.type'
import { UserRole } from '@/modules/users/models/user-role.type'
import { TeamRole } from '@/modules/teams/models/team-role.type'
import { ProjectStatistics } from '@/modules/projects/models/project-statistics.type'

export default {
  get: (id: number): Promise<AxiosResponse<Project>> => {
    return api.request({
      url: `/api/Projects/Get/${id}`,
      method: 'GET',
    })
  },
  getPage: (query: Query): Promise<AxiosResponse<Project[]>> => {
    return api.request({
      url: `/api/Projects/GetPage`,
      method: 'POST',
      data: query,
    })
  },
  getTeamsPage: (query: Query): Promise<AxiosResponse<Team[]>> => {
    return api.request({
      url: `/api/Projects/GetTeamsPage?projectId=${query.projectId}`,
      method: 'POST',
      data: query,
    })
  },
  getRange: (ids: number[]): Promise<AxiosResponse<Project[]>> => {
    return api.request({
      url: `/api/Projects/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },
  create: (project: Project): Promise<AxiosResponse<Project>> => {
    return api.request({
      url: `/api/Projects/Create`,
      method: 'POST',
      data: project,
    })
  },
  createByForm: (formProject: {
    project: Project
    teamIds: number[]
  }): Promise<AxiosResponse<Project>> => {
    return api.request({
      url: `/api/Projects/CreateByForm`,
      method: 'POST',
      data: formProject,
    })
  },
  update: (project: Project): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/Update`,
      method: 'PUT',
      data: project,
    })
  },
  updateByForm: (formProject: {
    project: Project
    teamIds: number[]
  }): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateByForm`,
      method: 'PUT',
      data: formProject,
    })
  },
  updateRange: (projects: Project[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateRange`,
      method: 'PUT',
      data: projects,
    })
  },
  remove: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/Delete/${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  addTeam: (projectId: number, teamId: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/AddTeam/${projectId}/${teamId}`,
      method: 'PATCH',
    })
  },
  removeTeam: (projectId: number, teamId: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/RemoveTeam/${projectId}/${teamId}`,
      method: 'PATCH',
    })
  },
  getTeamRole: (projectId: number, teamId: number): Promise<AxiosResponse<TeamRole>> => {
    return api.request({
      url: `/api/Projects/GetTeamRole/${projectId}/${teamId}`,
      method: 'GET',
    })
  },
  updateTeamRole: (teamRole: TeamRole): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateTeamRole`,
      method: 'POST',
      data: teamRole,
    })
  },
  updateTeamRoles: (teamRoles: TeamRole[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateTeamRoles`,
      method: 'POST',
      data: teamRoles,
    })
  },
  getUserRole: (projectId: number, userId: string): Promise<AxiosResponse<UserRole>> => {
    return api.request({
      url: `/api/Projects/GetUserRole/${projectId}/${userId}`,
      method: 'GET',
    })
  },
  getUserRoles: (projectId: number, teamId: number): Promise<AxiosResponse<UserRole[]>> => {
    return api.request({
      url: `/api/Projects/GetUserRole/${projectId}/${teamId}`,
      method: 'GET',
    })
  },
  updateUserRole: (userRole: UserRole): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateUserRole`,
      method: 'POST',
      data: userRole,
    })
  },
  updateUserRoles: (userRoles: UserRole[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Projects/UpdateUserRoles`,
      method: 'POST',
      data: userRoles,
    })
  },
  getProjectStatistics: (
    projectId: number,
    dateBegin: string,
    dateEnd: string
  ): Promise<AxiosResponse<ProjectStatistics>> => {
    return api.request({
      url: `/api/Projects/GetProjectStatistic/${projectId}`,
      method: 'POST',
      data: { dateBegin, dateEnd },
    })
  },
}
