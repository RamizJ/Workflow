import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Users/Get/${id}`),
  getAll: () => httpClient.get(`/api/Users/GetAll`),
  create: user => httpClient.post(`/api/Users/Create`, user),
  update: user => httpClient.put(`/api/Users/Update`, user),
  delete: id => httpClient.delete(`/api/Users/Delete`, id),
  getPage: query =>
    httpClient.get(
      `/api/Users/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Users/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
