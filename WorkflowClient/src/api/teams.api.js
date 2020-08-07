import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Teams/Get/${id}`),
  create: team => httpClient.post(`/api/Teams/CreateByForm`, team),
  update: team =>
    httpClient.put(`/api/Teams/UpdateByForm`, {
      team,
      projectIds: team.projectIds,
      userIds: team.userIds
    }),
  delete: teamId => httpClient.delete(`/api/Teams/Delete/${teamId}`),
  deleteRange: teamIds => httpClient.patch(`/api/Teams/DeleteRange`, teamIds),
  addUser: (teamId, userId) =>
    httpClient.patch(`/api/Teams/AddUser/${teamId}`, JSON.stringify(userId)),
  removeUser: (teamId, userId) =>
    httpClient.patch(`/api/Teams/RemoveUser/${teamId}/${userId}`),
  addProject: teamId => httpClient.patch(`/api/Teams/AddProject/${teamId}`),
  removeProject: (teamId, projectId) =>
    httpClient.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`),
  getPage: query => httpClient.post(`/api/Teams/GetPage`, query),
  getUsersPage: query =>
    httpClient.post(
      `/api/Teams/GetUsersPage${query.teamId ? '?teamId=' + query.teamId : ''}`,
      query
    ),
  getProjectsPage: query =>
    httpClient.post(
      `/api/Teams/GetProjectsPage${
        query.teamId ? '?teamId=' + query.teamId : ''
      }`,
      query
    ),
  getRange: query =>
    httpClient.get(
      `/api/Teams/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  restore: teamId => httpClient.patch(`/api/Teams/Restore/${teamId}`),
  restoreRange: teamIds => httpClient.patch(`/api/Teams/RestoreRange`, teamIds)
};
