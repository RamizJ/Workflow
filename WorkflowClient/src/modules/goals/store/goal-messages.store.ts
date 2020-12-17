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

  public get messages(): GoalMessage[] {
    return this._messages
  }

  public get unreadMessages(): GoalMessage[] {
    return this._messages
  }

  @MutationAction({ mutate: ['_messages'] })
  public async getMessages(goalId?: number) {
    try {
      const response = await goalMessagesApi.getPage(new Query(), goalId)
      return {
        _messages: response.data,
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
      const response = await goalMessagesApi.getUnreadedPage(new Query(), goalId)
      return {
        _unreadMessages: response.data,
      }
    } catch (e) {
      Message.error('Ошибка получения списка непрочитанных сообщений')
      return {
        _unreadMessages: [],
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
      this._messages.push(response.data)
      return response.data
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
}

export default getModule(GoalMessagesStore)
