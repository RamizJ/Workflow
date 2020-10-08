import { AxiosResponse } from 'axios'
import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Project from '@/types/project.type'
import { TeamRole } from '@/types/team-role.type'
import { UserRole } from '@/types/user-role.type'
import Team from '@/types/team.type'
import User from '@/types/user.type'
import { ProjectStatistics } from '@/types/project-statistics.type'

export default {
  findAll(query: Query): Promise<AxiosResponse<Project[]>> {
    return http.post(`/api/Projects/GetPage`, query)
  },
  findAllByIds(ids: number[]): Promise<AxiosResponse<Project[]>> {
    return http.get(`/api/Projects/GetRange?${qs.stringify(ids)}`)
  },
  findOneById(id: number): Promise<AxiosResponse<Project>> {
    return http.get(`/api/Projects/Get/${id}`)
  },
  createOne(request: { project: Project; teamIds: number[] }): Promise<AxiosResponse<Project>> {
    return http.post(`/api/Projects/CreateByForm`, request)
  },
  updateOne(request: { project: Project; teamIds: number[] }): Promise<AxiosResponse<void>> {
    return http.put(`/api/Projects/UpdateByForm`, request)
  },
  updateMany(entities: Project[]): Promise<AxiosResponse<void>> {
    return http.put(`/api/Goals/UpdateRange`, entities)
  },
  deleteOne(id: number): Promise<AxiosResponse<void>> {
    return http.delete(`/api/Projects/Delete/${id}`)
  },
  deleteMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Projects/DeleteRange`, ids)
  },
  restoreOne(id: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Projects/Restore/${id}`)
  },
  restoreMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Projects/RestoreRange`, ids)
  },
  findTeams(query: Query): Promise<AxiosResponse<Team>> {
    return http.post(`/api/Projects/GetTeamsPage?projectId=${query.projectId}`, query)
  },
  addTeam(projectId: number, teamId: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Projects/AddTeam/${projectId}/${teamId}`)
  },
  removeTeam(projectId: number, teamId: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Projects/RemoveTeam/${projectId}/${teamId}`)
  },
  getTeamRole(projectId: number, teamId: number): Promise<AxiosResponse<TeamRole>> {
    return http.get(`/api/Projects/GetTeamRole/${projectId}/${teamId}`)
  },
  updateTeamRole(teamRole: TeamRole): Promise<AxiosResponse<void>> {
    return http.post(`/api/Projects/UpdateTeamRole`, teamRole)
  },
  updateTeamRoles(teamRoles: TeamRole[]): Promise<AxiosResponse<void>> {
    return http.post(`/api/Projects/UpdateTeamRoles`, teamRoles)
  },
  findUsers(query: Query): Promise<AxiosResponse<User>> {
    return http.post(`/api/Projects/GetUsersPage?projectId=${query.projectId}`, query)
  },
  getUserRole(projectId: number, userId: string): Promise<AxiosResponse<UserRole>> {
    return http.get(`/api/Projects/GetUserRole/${projectId}/${userId}`)
  },
  updateUserRole(userRole: UserRole): Promise<AxiosResponse<void>> {
    return http.post(`/api/Projects/UpdateUserRole`, userRole)
  },
  updateUserRoles(userRoles: UserRole[]): Promise<AxiosResponse<void>> {
    return http.post(`/api/Projects/UpdateUserRoles`, userRoles)
  },
  getStatistics: (
    projectId: number,
    dateBegin: string,
    dateEnd: string
  ): Promise<AxiosResponse<ProjectStatistics>> =>
    http.post(`/api/Projects/GetProjectStatistic/${projectId}`, { dateBegin, dateEnd }),
  getTasksCount: (projectId: number): Promise<AxiosResponse<number>> =>
    http.get(`/api/Goals/GetTotalProjectGoalsCount/${projectId}`),
  getTasksCountByStatus(projectId: number, status: string): Promise<AxiosResponse<number>> {
    return http.get(`/api/Goals/GetProjectGoalsByStateCount/${projectId}?goalState=${status}`)
  },
}
