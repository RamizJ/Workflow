import {
  Action,
  getModule,
  Module,
  Mutation,
  MutationAction,
  VuexModule,
} from 'vuex-module-decorators'
import { Message } from 'element-ui'

import store from '@/core/store'
import api from '../api'
import User from '@/modules/users/models/user.type'
import Query from '@/core/types/query.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'usersStore',
  store,
})
class UsersStore extends VuexModule {
  _userWindowOpened = false
  _user: User | null = null
  _users: User[] = []

  public get isUserWindowOpened() {
    return this._userWindowOpened
  }
  public get user() {
    return this._user
  }
  public get users() {
    return this._users
  }

  @MutationAction({ mutate: ['_userWindowOpened'] })
  public async closeUserWindow() {
    return {
      _userWindowOpened: false,
    }
  }

  @MutationAction({ mutate: ['_userWindowOpened', '_user'] })
  public async openUserWindow(user?: User) {
    return {
      _user: user || null,
      _userWindowOpened: true,
    }
  }

  @Mutation
  setUser(user: User) {
    this._user = user
  }

  @Mutation
  setUsers(users: User[]) {
    this._users = users
  }

  @Action
  async findAll(query: Query): Promise<User[]> {
    const response = await api.getPage(query)
    const results = response.data as User[]
    this.context.commit('setUsers', results)
    return results
  }

  @Action
  async findAllByIds(ids: string[]): Promise<User[]> {
    const response = await api.getRange(ids)
    const results = response.data as User[]
    this.context.commit('setUsers', results)
    return results
  }

  @Action
  async findOneById(id: string): Promise<User> {
    const response = await api.get(id)
    const result = response.data as User
    this.context.commit('setUser', result)
    return result
  }

  @Action
  async createOne(entity: User): Promise<User> {
    const response = await api.create(entity)
    const result = response.data as User
    this.context.commit('setUser', result)
    return result
  }

  @Action
  async createMany(entities: User[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('createOne', entity)
    }
  }

  @Action
  async updateOne(entity: User): Promise<void> {
    await api.update(entity)
  }

  @Action
  async updateMany(entities: User[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('updateOne', entity)
    }
  }

  @Action
  async deleteOne(id: string): Promise<void> {
    await api.remove(id)
  }

  @Action
  async deleteMany(ids: string[]): Promise<void> {
    await api.removeRange(ids)
  }

  @Action
  async restoreOne(id: string): Promise<void> {
    await api.restore(id)
  }

  @Action
  async restoreMany(ids: string[]): Promise<void> {
    await api.restoreRange(ids)
  }

  @Action
  async resetPassword({ userId, newPassword }: { userId: string; newPassword: string }) {
    await api.resetPassword(userId, newPassword)
  }

  @Action
  async isLoginExist(login: string): Promise<boolean> {
    try {
      const response = await api.isUserNameExist(login)
      return response.data as boolean
    } catch (error) {
      Message.warning('Не удалось проверить уникальность логина')
      return false
    }
  }

  @Action
  async isEmailExist(email: string): Promise<boolean> {
    try {
      const response = await api.isEmailExist(email)
      return response.data as boolean
    } catch (error) {
      Message.warning('Не удалось проверить уникальность эл. почты')
      return false
    }
  }
}

export default getModule(UsersStore)
