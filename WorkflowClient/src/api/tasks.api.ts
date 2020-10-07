import { AxiosResponse } from 'axios'
import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Task from '@/types/task.type'
import Attachment from '@/types/attachment.type'

export default {
  findAll(query: Query): Promise<AxiosResponse<Task[]>> {
    return http.post(
      `/api/Goals/GetPage${query.projectId ? '?projectId=' + query.projectId : ''}`,
      query
    )
  },
  findAllByIds(ids: number[]): Promise<AxiosResponse<Task[]>> {
    return http.get(`/api/Goals/GetRange?${qs.stringify(ids)}`)
  },
  findOneById(id: number): Promise<AxiosResponse<Task>> {
    return http.get(`/api/Goals/Get/${id}`)
  },
  createOne(entity: Task): Promise<AxiosResponse<Task>> {
    return http.post(`/api/Goals/Create`, entity)
  },
  updateOne(entity: Task): Promise<AxiosResponse<void>> {
    return http.put(`/api/Goals/Update`, entity)
  },
  updateMany(entities: Task[]): Promise<AxiosResponse<void>> {
    return http.put(`/api/Goals/UpdateRange`, entities)
  },
  deleteOne(id: number): Promise<AxiosResponse<void>> {
    return http.delete<void>(`/api/Goals/Delete/${id}`)
  },
  deleteMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/DeleteRange`, ids)
  },
  restoreOne(id: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/Restore/${id}`)
  },
  restoreMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/RestoreRange`, ids)
  },
  findParent(id: number): Promise<AxiosResponse<Task>> {
    return http.get(`/api/Goals/GetParentGoal/${id}`)
  },
  findChild(id: number, query: Query | undefined): Promise<AxiosResponse<Task[]>> {
    return http.post(`/api/Goals/GetChildGoals/${id}`, query)
  },
  addChild(parentId: number, childIds: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/AddChildGoals/${parentId}`, childIds)
  },
  findAttachments(taskId: number): Promise<AxiosResponse<Attachment[]>> {
    return http.get(`/api/Goals/GetAttachments/${taskId}`)
  },
  uploadAttachments(taskId: number, files: FormData): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/AddAttachments/${taskId}`, files, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },
  downloadAttachment(attachmentId: number): Promise<AxiosResponse<Blob>> {
    return http.get(`/api/Goals/DownloadAttachmentFile/${attachmentId}`, {
      responseType: 'blob',
    })
  },
  removeAttachments(attachmentIds: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Goals/RemoveAttachments`, attachmentIds)
  },
}
