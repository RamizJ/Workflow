import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Goals/Get/${id}`),
  create: goal => httpClient.post(`/api/Goals/Create`, goal),
  update: goal => httpClient.put(`/api/Goals/Update`, goal),
  delete: id => httpClient.delete(`/api/Goals/Delete/${id}`),
  getPage: query =>
    httpClient.get(
      `/api/Goals/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Goals/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
