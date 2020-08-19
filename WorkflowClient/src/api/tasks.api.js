import httpClient from './httpClient';
import qs from 'qs';

export default {
  findOneById: id => httpClient.get(`/api/Goals/Get/${id}`),
  findAll: params =>
    httpClient.post(
      `/api/Goals/GetPage${
        params.projectId ? '?projectId=' + params.projectId : ''
      }`,
      params
    ),
  findAllByIds: ids =>
    httpClient.get(`/api/Goals/GetRange?${qs.stringify(ids)}`),
  createOne: item => httpClient.post(`/api/Goals/Create`, item),
  updateOne: item => httpClient.put(`/api/Goals/Update`, item),
  updateMany: items => httpClient.put(`/api/Goals/UpdateRange`, items),
  deleteOne: id => httpClient.delete(`/api/Goals/Delete/${id}`),
  deleteMany: ids => httpClient.patch(`/api/Goals/DeleteRange`, ids),
  restoreOne: id => httpClient.patch(`/api/Goals/Restore/${id}`),
  restoreMany: ids => httpClient.patch(`/api/Goals/RestoreRange`, ids),
  findParent: id => httpClient.get(`/api/Goals/GetParentGoal/${id}`),
  findChild: id => httpClient.get(`/api/Goals/GetChildGoals/${id}`),
  addChild: (parentId, childIds) =>
    httpClient.patch(`/api/Goals/AddChildGoals/${parentId}`, childIds),
  findAttachments: taskId =>
    httpClient.get(`/api/Goals/GetAttachments/${taskId}`),
  uploadAttachments: (taskId, files) =>
    httpClient.patch(`/api/Goals/AddAttachments/${taskId}`, files, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    }),
  downloadAttachment: attachmentId =>
    httpClient.get(`/api/Goals/DownloadAttachmentFile/${attachmentId}`, {
      responseType: 'blob'
    }),
  removeAttachments: attachmentIds =>
    httpClient.patch(`/api/Goals/RemoveAttachments`, attachmentIds)
};
