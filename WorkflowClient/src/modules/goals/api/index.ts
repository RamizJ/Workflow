import { AxiosResponse } from 'axios'
import qs from 'qs'
import api from '@/core/api'
import Attachment from '@/modules/goals/models/attachment.type'
import Query from '@/core/types/query.type'
import Goal from '@/modules/goals/models/goal.type'

export default {
  get: (id: number): Promise<AxiosResponse<Goal>> => {
    return api.request({
      url: `/api/Goals/Get/${id}`,
      method: 'GET',
    })
  },
  getPage: (query: Query): Promise<AxiosResponse<Goal[]>> => {
    return api.request({
      url: `/api/Goals/GetPage${query.projectId ? '?projectId=' + query.projectId : ''}`,
      method: 'POST',
      data: query,
    })
  },
  getTotalProjectGoalsCount: (projectId: number): Promise<AxiosResponse<number>> => {
    return api.request({
      url: `/api/Goals/GetTotalProjectGoalsCount/${projectId}`,
      method: 'GET',
    })
  },
  getProjectGoalsByStateCount: (
    projectId: number,
    goalState: string
  ): Promise<AxiosResponse<number>> => {
    return api.request({
      url: `/api/Goals/GetTotalProjectGoalsCount/${projectId}?goalState=${goalState}`,
      method: 'GET',
    })
  },
  getRange: (ids: number[]): Promise<AxiosResponse<Goal[]>> => {
    return api.request({
      url: `/api/Goals/GetRange?ids=${qs.stringify(ids)}`,
      method: 'GET',
    })
  },
  create: (goal: Goal): Promise<AxiosResponse<Goal>> => {
    return api.request({
      url: `/api/Goals/Create`,
      method: 'POST',
      data: goal,
    })
  },
  createByForm: (
    goal: Goal,
    observerIds: number[],
    childGoals: Goal[]
  ): Promise<AxiosResponse<Goal>> => {
    return api.request({
      url: `/api/Goals/CreateByForm`,
      method: 'POST',
      data: { goal, observerIds, childGoals },
    })
  },
  update: (goal: Goal): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/Update`,
      method: 'PUT',
      data: goal,
    })
  },
  updateRange: (goals: Goal[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/UpdateRange`,
      method: 'PUT',
      data: goals,
    })
  },
  updateByForm: (goalForm: {
    goal: Goal
    observerIds: number[]
    childGoals: Goal[]
  }): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/UpdateByForm`,
      method: 'PUT',
      data: goalForm,
    })
  },
  updateByFormRange: (
    goalForms: {
      goal: Goal
      observerIds: number[]
      childGoals: Goal[]
    }[]
  ): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/UpdateByForm`,
      method: 'PUT',
      data: goalForms,
    })
  },
  remove: (id: number): Promise<AxiosResponse<Goal>> => {
    return api.request({
      url: `/api/Goals/Delete/${id}`,
      method: 'DELETE',
    })
  },
  removeRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/DeleteRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  restore: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/Restore/${id}`,
      method: 'PATCH',
    })
  },
  restoreRange: (ids: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/RestoreRange`,
      method: 'PATCH',
      data: ids,
    })
  },
  getAttachments: (goalId: number): Promise<AxiosResponse<Attachment[]>> => {
    return api.request({
      url: `/api/Goals/GetAttachments/${goalId}`,
      method: 'GET',
    })
  },
  addAttachments: (goalId: number, files: FormData): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/AddAttachments/${goalId}`,
      method: 'PATCH',
      data: files,
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
  },
  removeAttachments: (attachmentIds: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/RemoveAttachments`,
      method: 'PATCH',
      data: attachmentIds,
    })
  },
  downloadAttachmentFile: (attachmentId: number): Promise<AxiosResponse<Blob>> => {
    return api.request({
      url: `/api/Goals/DownloadAttachmentFile/${attachmentId}`,
      method: 'GET',
      responseType: 'blob',
    })
  },
  getParentGoal: (goalId: number): Promise<AxiosResponse<Goal>> => {
    return api.request({
      url: `/api/Goals/GetParentGoal/${goalId}`,
      method: 'GET',
    })
  },
  getChildGoals: (parentGoalId: number, query: Query): Promise<AxiosResponse<Goal[]>> => {
    return api.request({
      url: `/api/Goals/GetChildGoals/${parentGoalId}`,
      method: 'POST',
      data: query,
    })
  },
  addChildGoals: (parentGoalId: number, childGoalIds: number[]): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/Goals/AddChildGoals/${parentGoalId}`,
      method: 'PATCH',
      data: childGoalIds,
    })
  },
}
