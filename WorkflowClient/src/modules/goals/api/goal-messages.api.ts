import { AxiosResponse } from 'axios'
import api from '@/core/api'
import Query from '@/core/types/query.type'
import GoalMessage, { GoalMessageData } from '@/modules/goals/models/goal-message.model'

export default {
  getPage: (query: Query, goalId?: number): Promise<AxiosResponse<GoalMessageData[]>> => {
    const url = goalId ? `/api/GoalMessages/GetPage?goalId=${goalId}` : '/api/GoalMessages/GetPage'

    return api.request({
      url: `/api/GoalMessages/GetPage?goalId=${goalId}`,
      method: 'POST',
      data: query,
    })
  },
  getUnreadPage: (query: Query, goalId?: number): Promise<AxiosResponse<GoalMessageData[]>> => {
    const url = goalId
      ? `/api/GoalMessages/GetUnreadPage?goalId=${goalId}`
      : '/api/GoalMessages/GetUnreadPage'

    return api.request({
      url: url,
      method: 'POST',
      data: query,
    })
  },
  getUnreadCount: (goalId?: number): Promise<AxiosResponse<number>> => {
    const url = goalId
      ? `/api/GoalMessages/GetUnreadCount?goalId=${goalId}`
      : '/api/GoalMessages/GetUnreadCount'
    return api.request({ url, method: 'POST' })
  },
  updateMessage: (message: GoalMessage): Promise<AxiosResponse<void>> => {
    return api.request({
      url: `/api/GoalMessages/UpdateMessage`,
      method: 'PUT',
      data: message,
    })
  },
  addMessage: (message: GoalMessage): Promise<AxiosResponse<GoalMessageData>> => {
    return api.request({
      url: `/api/GoalMessages/Create`,
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
