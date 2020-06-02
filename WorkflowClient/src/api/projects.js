import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Scopes/Get/${id}`),
  getAll: () => httpClient.get(`/api/Scopes/GetAll`),
  create: scope => httpClient.post(`/api/Scopes/Create`, scope),
  update: scope => httpClient.put(`/api/Scopes/Update`, scope),
  delete: id => httpClient.delete(`/api/Scopes/Delete`, id),
  getPage: query =>
    httpClient.get(
      `/api/Scopes/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Scopes/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
