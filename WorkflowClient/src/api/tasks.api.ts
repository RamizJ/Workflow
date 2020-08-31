import qs from 'qs'
import httpClient from './httpClient'
import Query from '@/types/query.type'
import Task from '@/types/task.type'

export default {
  findOneById: (id: number) => httpClient.get(`/api/Goals/Get/${id}`),
  findAll: (query: Query) =>
    httpClient.post(
      `/api/Goals/GetPage${query.projectId ? '?projectId=' + query.projectId : ''}`,
      query
    ),
  findAllByIds: (ids: number[]) => httpClient.get(`/api/Goals/GetRange?${qs.stringify(ids)}`),
  createOne: (entity: Task) => httpClient.post(`/api/Goals/Create`, entity),
  updateOne: (entity: Task) => httpClient.put(`/api/Goals/Update`, entity),
  updateMany: (entities: Task[]) => httpClient.put(`/api/Goals/UpdateRange`, entities),
  deleteOne: (id: number) => httpClient.delete(`/api/Goals/Delete/${id}`),
  deleteMany: (ids: number[]) => httpClient.patch(`/api/Goals/DeleteRange`, ids),
  restoreOne: (id: number) => httpClient.patch(`/api/Goals/Restore/${id}`),
  restoreMany: (ids: number[]) => httpClient.patch(`/api/Goals/RestoreRange`, ids),
  findParent: (id: number) => httpClient.get(`/api/Goals/GetParentGoal/${id}`),
  findChild: (id: number, query: Query | undefined) =>
    httpClient.post(`/api/Goals/GetChildGoals/${id}`, query),
  addChild: (parentId: number, childIds: number[]) =>
    httpClient.patch(`/api/Goals/AddChildGoals/${parentId}`, childIds),
  findAttachments: (taskId: number) => httpClient.get(`/api/Goals/GetAttachments/${taskId}`),
  uploadAttachments: (taskId: number, files: FormData) =>
    httpClient.patch(`/api/Goals/AddAttachments/${taskId}`, files, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    }),
  downloadAttachment: (attachmentId: number) =>
    httpClient.get(`/api/Goals/DownloadAttachmentFile/${attachmentId}`, {
      responseType: 'blob'
    }),
  removeAttachments: (attachmentIds: number[]) =>
    httpClient.patch(`/api/Goals/RemoveAttachments`, attachmentIds)
}
