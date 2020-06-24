import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Goals/Get/${id}`),
  getPage: query =>
    httpClient.get(
      `/api/Goals/GetPage${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  getRange: query =>
    httpClient.get(
      `/api/Goals/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  create: task => httpClient.post(`/api/Goals/Create`, task),
  update: task => httpClient.put(`/api/Goals/Update`, task),
  delete: taskId => httpClient.delete(`/api/Goals/Delete/${taskId}`),
  getAttachments: taskId =>
    httpClient.get(`/api/Goals/GetAttachments/${taskId}`),
  addAttachments: (taskId, files) =>
    httpClient.patch(`/api/Goals/AddAttachments/${taskId}`, files, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    }),
  removeAttachments: attachmentIds =>
    httpClient.patch(`/api/Goals/RemoveAttachments`, attachmentIds)
};
