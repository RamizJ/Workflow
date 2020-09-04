import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'
import { Message } from 'element-ui'

import store from '@/store'
import usersAPI from '@/api/users.api'
import User from '@/types/user.type'
import Query from '@/types/query.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'usersModule',
  store,
})
class UsersModule extends VuexModule {
  _user: User | null = null
  _users: User[] = []

  public get user() {
    return this._user
  }
  public get users() {
    return this._users
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
    const response = await usersAPI.findAll(query)
    const results = response.data as User[]
    this.context.commit('setUsers', results)
    return results
  }

  @Action
  async findAllByIds(ids: string[]): Promise<User[]> {
    const response = await usersAPI.findAllByIds(ids)
    const results = response.data as User[]
    this.context.commit('setUsers', results)
    return results
  }

  @Action
  async findOneById(id: string): Promise<User> {
    const response = await usersAPI.findOneById(id)
    const result = response.data as User
    this.context.commit('setUser', result)
    return result
  }

  @Action
  async createOne(entity: User): Promise<User> {
    const response = await usersAPI.createOne(entity)
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
    await usersAPI.updateOne(entity)
  }

  @Action
  async updateMany(entities: User[]): Promise<void> {
    await usersAPI.updateMany(entities)
  }

  @Action
  async deleteOne(id: string): Promise<void> {
    await usersAPI.deleteOne(id)
  }

  @Action
  async deleteMany(ids: string[]): Promise<void> {
    await usersAPI.deleteMany(ids)
  }

  @Action
  async restoreOne(id: string): Promise<void> {
    await usersAPI.restoreOne(id)
  }

  @Action
  async restoreMany(ids: string[]): Promise<void> {
    await usersAPI.restoreMany(ids)
  }

  @Action
  async resetPassword({ userId, newPassword }: { userId: string; newPassword: string }) {
    await usersAPI.resetPassword(userId, newPassword)
  }

  @Action
  async isLoginExist(login: string): Promise<boolean> {
    try {
      const response = await usersAPI.isUserNameExist(login)
      return response.data as boolean
    } catch (error) {
      Message.warning('Не удалось проверить уникальность логина')
      return false
    }
  }

  @Action
  async isEmailExist(email: string): Promise<boolean> {
    try {
      const response = await usersAPI.isEmailExist(email)
      return response.data as boolean
    } catch (error) {
      Message.warning('Не удалось проверить уникальность эл. почты')
      return false
    }
  }
}

export default getModule(UsersModule)
