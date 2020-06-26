import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Projects/Get/${id}`),
  create: project => httpClient.post(`/api/Projects/CreateByForm`, project),
  update: project => httpClient.post(`/api/Projects/UpdateByForm`, project),
  delete: id => httpClient.delete(`/api/Projects/Delete/${id}`),
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
