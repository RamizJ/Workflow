import { AxiosResponse } from 'axios'
import api from '@/core/api'
import Query from '@/core/types/query.type'
import GoalMessage from '@/modules/goals/models/goal-message.model'

export default {
  getPage: (query: Query, goalId?: number): Promise<AxiosResponse<GoalMessage[]>> => {
    return api.request({
      url: `/api/GoalMessages/GetPage?goalId=${goalId}`,
      method: 'POST',
      data: query,
    })
  },
  getUnreadedPage: (query: Query, goalId?: number): Promise<AxiosResponse<GoalMessage[]>> => {
    return api.request({
      url: `/api/GoalMessages/GetUnreadedPage?goalId=${goalId}`,
      method: 'POST',
      data: query,
    })
  },
  updateMessage: (message: GoalMessage): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/GoalMessages/UpdateMessage`,
      method: 'PUT',
      data: message,
    })
  },
  addMessage: (message: GoalMessage): Promise<AxiosResponse<GoalMessage>> => {
    return api.request({
      url: `/api/GoalMessages/AddMessage`,
      method: 'POST',
      data: message,
    })
  },
  deleteMessage: (id: number): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/GoalMessages/DeleteMessage`,
      method: 'DELETE',
    })
  },
  markAsRead: (message: GoalMessage): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/GoalMessages/MarkAsRead`,
      method: 'POST',
      data: message,
    })
  },
}
