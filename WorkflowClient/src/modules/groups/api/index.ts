import { AxiosResponse } from 'axios'
import api from '@/core/api'
import Query from '@/core/types/query.type'
import Group from '@/modules/groups/models/group.model'

export default {
  get: (id: number): Promise<AxiosResponse<Group>> => {
    return api.request({
      url: `/api/Groups/Get/${id}`,
      method: 'GET',
    })
  },
  getAll: (): Promise<AxiosResponse<Array<Group>>> => {
    return api.request({
      url: `/api/Groups/GetAll`,
      method: 'GET',
    })
  },
  getPage: (query: Query, parentGroupId?: number): Promise<AxiosResponse<Array<Group>>> => {
    return api.request({
      url: `/api/Groups/GetPage${parentGroupId ? '?parentGroupId=' + parentGroupId : ''}`,
      method: 'POST',
      data: query,
    })
  },
  create: (group: Group): Promise<AxiosResponse<Group>> => {
    return api.request({
      url: `/api/Groups/Create`,
      method: 'POST',
      data: group,
    })
  },
  update: (group: Group): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/Update`,
      method: 'PUT',
      data: group,
    })
  },
  updateRange: (groups: Array<Group>): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/UpdateRange`,
      method: 'PUT',
      data: groups,
    })
  },
  remove: (id: number): Promise<AxiosResponse<Group>> => {
    return api.request({
      url: `/api/Groups/Delete/${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  addProjects: (groupId: number, projectIds: Array<number>): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/AddProjects/${groupId}`,
      method: 'POST',
      data: projectIds,
    })
  },
  deleteProjects: (groupId: number, projectIds: Array<number>): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Groups/DeleteProjects/${groupId}`,
      method: 'POST',
      data: projectIds,
    })
  },
}
