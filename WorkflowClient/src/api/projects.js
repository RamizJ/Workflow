import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Projects/Get/${id}`),
  getAll: () => httpClient.get(`/api/Projects/GetAll`),
  create: scope => httpClient.post(`/api/Projects/Create`, scope),
  update: scope => httpClient.put(`/api/Projects/Update`, scope),
  delete: id => httpClient.delete(`/api/Projects/Delete`, id),
  getPage: query =>
    httpClient.get(
      `/api/Projects/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Projects/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
