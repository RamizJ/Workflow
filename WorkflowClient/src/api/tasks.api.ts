import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Task from '@/types/task.type'

export default {
  findAll: (query: Query) =>
    http.post(`/api/Goals/GetPage${query.projectId ? '?projectId=' + query.projectId : ''}`, query),
  findAllByIds: (ids: number[]) => http.get(`/api/Goals/GetRange?${qs.stringify(ids)}`),
  findOneById: (id: number) => http.get(`/api/Goals/Get/${id}`),
  createOne: (entity: Task) => http.post(`/api/Goals/Create`, entity),
  updateOne: (entity: Task) => http.put(`/api/Goals/Update`, entity),
  updateMany: (entities: Task[]) => http.put(`/api/Goals/UpdateRange`, entities),
  deleteOne: (id: number) => http.delete(`/api/Goals/Delete/${id}`),
  deleteMany: (ids: number[]) => http.patch(`/api/Goals/DeleteRange`, ids),
  restoreOne: (id: number) => http.patch(`/api/Goals/Restore/${id}`),
  restoreMany: (ids: number[]) => http.patch(`/api/Goals/RestoreRange`, ids),
  findParent: (id: number) => http.get(`/api/Goals/GetParentGoal/${id}`),
  findChild: (id: number, query: Query | undefined) =>
    http.post(`/api/Goals/GetChildGoals/${id}`, query),
  addChild: (parentId: number, childIds: number[]) =>
    http.patch(`/api/Goals/AddChildGoals/${parentId}`, childIds),
  findAttachments: (taskId: number) => http.get(`/api/Goals/GetAttachments/${taskId}`),
  uploadAttachments: (taskId: number, files: FormData) =>
    http.patch(`/api/Goals/AddAttachments/${taskId}`, files, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    }),
  downloadAttachment: (attachmentId: number) =>
    http.get(`/api/Goals/DownloadAttachmentFile/${attachmentId}`, {
      responseType: 'blob'
    }),
  removeAttachments: (attachmentIds: number[]) =>
    http.patch(`/api/Goals/RemoveAttachments`, attachmentIds)
}
