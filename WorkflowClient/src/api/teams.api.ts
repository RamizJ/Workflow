import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Team from '@/types/team.type'

export default {
  findAll: (query: Query) => http.post(`/api/Teams/GetPage`, query),
  findAllByIds: (ids: number[]) => http.get(`/api/Teams/GetRange?${qs.stringify(ids)}`),
  findOneById: (id: number) => http.get(`/api/Teams/Get/${id}`),
  createOne: (request: { team: Team; userIds: string[]; projectIds: number[] }) =>
    http.post(`/api/Teams/CreateByForm`, request),
  updateOne: (request: { team: Team; userIds: string[]; projectIds: number[] }) =>
    http.put(`/api/Teams/UpdateByForm`, request),
  updateMany: (entities: Team[]) => http.put(`/api/Teams/UpdateByFormRange/${entities}`),
  deleteOne: (id: number) => http.delete(`/api/Teams/Delete/${id}`),
  deleteMany: (ids: number[]) => http.patch(`/api/Teams/DeleteRange`, ids),
  restoreOne: (id: number) => http.patch(`/api/Teams/Restore/${id}`),
  restoreMany: (ids: number[]) => http.patch(`/api/Teams/RestoreRange`, ids),
  findUsers: (query: Query) => http.post(`/api/Teams/GetUsersPage?teamId=${query.teamId}`, query),
  addUser: (
    teamId: number,
    userId: string,
    canEditUsers: boolean,
    canEditGoals: boolean,
    canCloseGoals: boolean
  ) =>
    http.patch(`/api/Teams/AddUser`, {
      teamId,
      userId,
      canEditUsers,
      canEditGoals,
      canCloseGoals,
    }),
  updateUser: (
    teamId: number,
    userId: string,
    canEditUsers: boolean,
    canEditGoals: boolean,
    canCloseGoals: boolean
  ) =>
    http.patch(`/api/Teams/AddUser`, {
      teamId,
      userId,
      canEditUsers,
      canEditGoals,
      canCloseGoals,
    }),
  addUsers: (teamId: number, userIds: string[]) =>
    http.patch(`/api/Teams/AddUser/${teamId}`, userIds),
  removeUser: (teamId: number, userId: string) =>
    http.patch(`/api/Teams/RemoveUser/${teamId}`, JSON.stringify(userId)),
  findProjects: (query: Query) =>
    http.post(`/api/Teams/GetProjectsPage?teamId=${query.teamId}`, query),
  addProject: (teamId: number, projectId: number) =>
    http.patch(`/api/Teams/AddProject/${teamId}`, projectId),
  removeProject: (teamId: number, projectId: number) =>
    http.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`),
}
