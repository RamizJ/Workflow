import httpClient from './httpClient';
import qs from 'qs';

export default {
  findOneById: id => httpClient.get(`/api/Teams/Get/${id}`),
  findAll: params => httpClient.post(`/api/Teams/GetPage`, params),
  findAllByIds: params =>
    httpClient.get(`/api/Teams/GetRange?${qs.stringify(params)}`),
  createOne: item => httpClient.post(`/api/Teams/CreateByForm`, item),
  updateOne: item =>
    httpClient.put(`/api/Teams/UpdateByForm`, {
      item,
      projectIds: item.projectIds,
      userIds: item.userIds
    }),
  updateMany: items => httpClient.put(`/api/Teams/UpdateByFormRange/${items}`),
  deleteOne: id => httpClient.delete(`/api/Teams/Delete/${id}`),
  deleteMany: ids => httpClient.patch(`/api/Teams/DeleteRange`, ids),
  restoreOne: id => httpClient.patch(`/api/Teams/Restore/${id}`),
  restoreMany: ids => httpClient.patch(`/api/Teams/RestoreRange`, ids),
  findUsers: params =>
    httpClient.post(`/api/Teams/GetUsersPage?teamId=${params.teamId}`, params),
  addUser: (teamId, userId) =>
    httpClient.patch(`/api/Teams/AddUser/${teamId}`, JSON.stringify(userId)),
  removeUser: (teamId, userId) =>
    httpClient.patch(`/api/Teams/RemoveUser/${teamId}/${userId}`),
  findProjects: params =>
    httpClient.post(
      `/api/Teams/GetProjectsPage?teamId=${params.teamId}`,
      params
    ),
  addProject: (teamId, projectId) =>
    httpClient.patch(`/api/Teams/AddProject/${teamId}`, projectId),
  removeProject: (teamId, projectId) =>
    httpClient.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`)
};
