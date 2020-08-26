import qs from 'qs'
import httpClient from './httpClient'
import Query from '@/types/query.type'
import Team from '@/types/team.type'

export default {
  findOneById: (id: number) => httpClient.get(`/api/Teams/Get/${id}`),
  findAll: (query: Query) => httpClient.post(`/api/Teams/GetPage`, query),
  findAllByIds: (ids: number[]) => httpClient.get(`/api/Teams/GetRange?${qs.stringify(ids)}`),
  createOne: (request: { team: Team; userIds: string[]; projectIds: number[] }) =>
    httpClient.post(`/api/Teams/CreateByForm`, request),
  updateOne: (request: { team: Team; userIds: string[]; projectIds: number[] }) =>
    httpClient.put(`/api/Teams/UpdateByForm`, request),
  updateMany: (entities: Team[]) => httpClient.put(`/api/Teams/UpdateByFormRange/${entities}`),
  deleteOne: (id: number) => httpClient.delete(`/api/Teams/Delete/${id}`),
  deleteMany: (ids: number[]) => httpClient.patch(`/api/Teams/DeleteRange`, ids),
  restoreOne: (id: number) => httpClient.patch(`/api/Teams/Restore/${id}`),
  restoreMany: (ids: number[]) => httpClient.patch(`/api/Teams/RestoreRange`, ids),
  findUsers: (query: Query) =>
    httpClient.post(`/api/Teams/GetUsersPage?teamId=${query.teamId}`, query),
  addUser: (teamId: number, userId: string) =>
    httpClient.patch(`/api/Teams/AddUser/${teamId}`, JSON.stringify(userId)),
  addUsers: (teamId: number, userIds: string[]) =>
    httpClient.patch(`/api/Teams/AddUser/${teamId}`, userIds),
  removeUser: (teamId: number, userId: string) =>
    httpClient.patch(`/api/Teams/RemoveUser/${teamId}/${userId}`),
  findProjects: (query: Query) =>
    httpClient.post(`/api/Teams/GetProjectsPage?teamId=${query.teamId}`, query),
  addProject: (teamId: number, projectId: number) =>
    httpClient.patch(`/api/Teams/AddProject/${teamId}`, projectId),
  removeProject: (teamId: number, projectId: number) =>
    httpClient.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`)
}
