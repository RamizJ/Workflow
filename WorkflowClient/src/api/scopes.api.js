import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Groups/Get/${id}`),
  getAll: () => httpClient.get(`/api/Groups/GetAll`),
  create: group => httpClient.post(`/api/Groups/Create`, group),
  update: group => httpClient.put(`/api/Groups/Update`, group),
  delete: id => httpClient.delete(`/api/Groups/Delete`, id),
  getPage: query =>
    httpClient.get(
      `/api/Groups/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Groups/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
