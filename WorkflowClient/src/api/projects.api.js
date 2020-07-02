import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: projectId => httpClient.get(`/api/Projects/Get/${projectId}`),
  create: project => httpClient.post(`/api/Projects/CreateByForm`, project),
  update: project => httpClient.post(`/api/Projects/UpdateByForm`, project),
  delete: projectId => httpClient.delete(`/api/Projects/Delete/${projectId}`),
  deleteRange: projectIds =>
    httpClient.patch(`/api/Projects/DeleteRange`, projectIds),
  addTeam: (teamId, projectId) =>
    httpClient.patch(`/api/Projects/AddTeam/${teamId}?projectId=${projectId}`),
  removeTeam: (teamId, projectId) =>
    httpClient.patch(`/api/Projects/RemoveTeam/${teamId}/${projectId}`),
  getPage: query =>
    httpClient.get(
      `/api/Projects/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getTeamsPage: query =>
    httpClient.get(
      `/api/Projects/GetTeamsPage${qs.stringify(query, {
        addQueryPrefix: true
      })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Projects/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
