import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Project from '@/types/project.type'
import { TeamRole } from '@/types/team-role.type'
import { UserRole } from '@/types/user-role.type'

export default {
  findAll: (query: Query) => http.post(`/api/Projects/GetPage`, query),
  findAllByIds: (ids: number[]) => http.get(`/api/Projects/GetRange?${qs.stringify(ids)}`),
  findOneById: (id: number) => http.get(`/api/Projects/Get/${id}`),
  createOne: (request: { project: Project; teamIds: number[] }) =>
    http.post(`/api/Projects/CreateByForm`, request),
  updateOne: (request: { project: Project; teamIds: number[] }) =>
    http.put(`/api/Projects/UpdateByForm`, request),
  updateMany: (entities: Project[]) => http.put(`/api/Goals/UpdateRange`, entities),
  deleteOne: (id: number) => http.delete(`/api/Projects/Delete/${id}`),
  deleteMany: (ids: number[]) => http.patch(`/api/Projects/DeleteRange`, ids),
  restoreOne: (id: number) => http.patch(`/api/Projects/Restore/${id}`),
  restoreMany: (ids: number[]) => http.patch(`/api/Projects/RestoreRange`, ids),
  findTeams: (query: Query) =>
    http.post(`/api/Projects/GetTeamsPage?projectId=${query.projectId}`, query),
  addTeam: (projectId: number, teamId: number) =>
    http.patch(`/api/Projects/AddTeam/${projectId}/${teamId}`),
  removeTeam: (projectId: number, teamId: number) =>
    http.patch(`/api/Projects/RemoveTeam/${projectId}/${teamId}`),
  getTeamRole: (projectId, teamId) => http.get(`/api/Projects/GetTeamRole/${projectId}/${teamId}`),
  updateTeamRole: (teamRole: TeamRole) => http.post(`/api/Projects/UpdateTeamRole`, teamRole),
  updateTeamRoles: (teamRoles: TeamRole[]) => http.post(`/api/Projects/UpdateTeamRoles`, teamRoles),
  findUsers: (query: Query) =>
    http.post(`/api/Projects/GetUsersPage?projectId=${query.projectId}`, query),
  getUserRole: (projectId: number, userId: string) =>
    http.get(`/api/Projects/GetUserRole/${projectId}/${userId}`),
  getUserRoles: (projectId: number, teamId: number) =>
    http.get(`/api/Projects/GetUserRole/${projectId}/${teamId}`),
  updateUserRole: (userRole: UserRole) => http.post(`/api/Projects/UpdateUserRole`, userRole),
  updateUserRoles: (userRoles: UserRole[]) => http.post(`/api/Projects/UpdateUserRoles`, userRoles),
  getStatistics: (projectId: number, dateBegin: string, dateEnd: string) =>
    http.post(`/api/Projects/GetProjectStatistic/${projectId}`, { dateBegin, dateEnd }),
  getTasksCount: (projectId: number) =>
    http.get(`/api/Goals/GetTotalProjectGoalsCount/${projectId}`),
  getTasksCountByStatus: (projectId: number, status: string) =>
    http.get(`/api/Goals/GetProjectGoalsByStateCount/${projectId}?goalState=${status}`),
}
