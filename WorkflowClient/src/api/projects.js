import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Projects/Get/${id}`),
  create: project => httpClient.post(`/api/Projects/Create`, project),
  update: project => httpClient.put(`/api/Projects/Update`, project),
  delete: id => httpClient.delete(`/api/Projects/Delete/${id}`),
  addTeam: teamId => httpClient.patch(`/api/Projects/AddTeam/${teamId}`),
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
