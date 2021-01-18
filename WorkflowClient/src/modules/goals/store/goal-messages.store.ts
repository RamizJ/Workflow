import { Action, getModule, Module, MutationAction, VuexModule } from 'vuex-module-decorators'
import store from '@/core/store'
import goalMessagesApi from '../api/goal-messages.api'
import GoalMessage from '@/modules/goals/models/goal-message.model'
import Query from '@/core/types/query.type'
import { Message } from 'element-ui'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'goalMessagesStore',
  store,
})
class GoalMessagesStore extends VuexModule {
  _messages: GoalMessage[] = []
  _unreadMessages: GoalMessage[] = []
  _unreadMessagesCount = 0

  public get messages(): GoalMessage[] {
    return this._messages
  }

  public get unreadMessages(): GoalMessage[] {
    return this._unreadMessages
  }

  public get unreadMessagesCount(): number {
    return this._unreadMessagesCount
  }

  @MutationAction({ mutate: ['_messages'] })
  public async getMessages(goalId?: number) {
    try {
      const response = await goalMessagesApi.getPage(new Query(), goalId)
      return {
        _messages: response.data.map((data) => new GoalMessage(data)),
      }
    } catch (e) {
      Message.error('Ошибка получения списка сообщений')
      return {
        _messages: [],
      }
    }
  }

  @MutationAction({ mutate: ['_unreadMessages'] })
  public async getUnreadMessages(goalId?: number) {
    try {
      const response = await goalMessagesApi.getUnreadPage(new Query(), goalId)
      return {
        _unreadMessages: response.data.map((data) => new GoalMessage(data)),
      }
    } catch (e) {
      Message.error('Ошибка получения списка непрочитанных сообщений')
      return {
        _unreadMessages: [],
      }
    }
  }

  @MutationAction({ mutate: ['_unreadMessagesCount'] })
  public async getUnreadMessagesCount(goalId?: number) {
    try {
      const response = await goalMessagesApi.getUnreadCount(goalId)
      return {
        _unreadMessagesCount: response.data,
      }
    } catch (e) {
      Message.error('Ошибка получения списка непрочитанных сообщений')
      return {
        _unreadMessagesCount: 0,
      }
    }
  }

  @Action
  public async updateMessage(message: GoalMessage): Promise<void> {
    try {
      await goalMessagesApi.updateMessage(message)
    } catch (e) {
      Message.error('Ошибка обновления сообщения')
    }
  }

  @Action
  public async sendMessage(message: GoalMessage): Promise<GoalMessage | undefined> {
    try {
      const response = await goalMessagesApi.addMessage(message)
      const createdMessage = new GoalMessage(response.data)
      this._messages.push(createdMessage)
      return createdMessage
    } catch (e) {
      Message.error('Ошибка отправки сообщения')
    }
  }

  @Action
  public async deleteMessage(id: number): Promise<void> {
    try {
      await goalMessagesApi.deleteMessage(id)
    } catch (e) {
      Message.error('Ошибка удаления сообщения')
    }
  }

  @Action
  public async markAsRead(ids: number[]): Promise<void> {
    try {
      await goalMessagesApi.markAsRead(ids)
    } catch (e) {
      Message.error('Ошибка обновления уведомлений')
    }
  }
}

export default getModule(GoalMessagesStore)
