import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Teams/Get/${id}`),
  create: team => httpClient.post(`/api/Teams/Create`, team),
  update: team => httpClient.put(`/api/Teams/Update`, team),
  delete: id => httpClient.delete(`/api/Teams/Delete/${id}`),
  addUser: teamId => httpClient.patch(`/api/Teams/AddUser/${teamId}`),
  removeUser: (teamId, userId) =>
    httpClient.patch(`/api/Teams/RemoveUser/${teamId}/${userId}`),
  addProject: teamId => httpClient.patch(`/api/Teams/AddProject/${teamId}`),
  removeProject: (teamId, projectId) =>
    httpClient.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`),
  getPage: query =>
    httpClient.get(
      `/api/Teams/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getUsersPage: query =>
    httpClient.get(
      `/api/Teams/GetUsers${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getProjectsPage: query =>
    httpClient.get(
      `/api/Teams/GetProjects${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Teams/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
