import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Teams/Get/${id}`),
  getAll: () => httpClient.get(`/api/Teams/GetAll`),
  create: team => httpClient.post(`/api/Teams/Create`, team),
  update: team => httpClient.put(`/api/Teams/Update`, team),
  delete: id => httpClient.delete(`/api/Teams/Delete`, id),
  getPage: query =>
    httpClient.get(
      `/api/Teams/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Teams/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
