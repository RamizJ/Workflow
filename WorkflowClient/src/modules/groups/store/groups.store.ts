import {
  Action,
  getModule,
  Module,
  Mutation,
  MutationAction,
  VuexModule,
} from 'vuex-module-decorators'
import store from '@/core/store'
import api from '../api'
import Query from '@/core/types/query.type'
import Group from '@/modules/groups/models/group.model'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'groupsStore',
  store,
})
class GroupsStore extends VuexModule {
  _group: Group | null = null
  _groups: Array<Group> = []
  _groupWindowOpened = false

  public get group() {
    return this._group
  }
  public get groups() {
    return this._groups
  }
  public get isGroupWindowOpened(): boolean {
    return this._groupWindowOpened
  }

  @Mutation
  setGroup(group: Group) {
    this._group = group
  }

  @Mutation
  setGroups(groups: Array<Group>) {
    this._groups = groups
  }

  @MutationAction({ mutate: ['_groupWindowOpened'] })
  public async closeGroupWindow() {
    return {
      _groupWindowOpened: false,
    }
  }

  @MutationAction({ mutate: ['_groupWindowOpened', '_group'] })
  public async openGroupWindow(group?: Group) {
    return {
      _group: group || null,
      _groupWindowOpened: true,
    }
  }

  @Action({ rawError: true })
  async getPage(query: Query): Promise<Array<Group>> {
    const response = await api.getPage(query)
    const entities = response.data as Array<Group>
    return entities
  }

  @Action
  async get(id: number): Promise<Group> {
    const response = await api.get(id)
    const entity = response.data as Group
    return entity
  }

  @Action
  async create(entity: Group): Promise<Group> {
    const response = await api.create(entity)
    const createdEntity = response.data as Group
    return createdEntity
  }

  @Action
  async update(entity: Group): Promise<void> {
    await api.update(entity)
  }

  @Action
  async updateRange(entities: Array<Group>): Promise<void> {
    await api.updateRange(entities)
  }

  @Action
  async remove(id: number): Promise<void> {
    await api.remove(id)
  }

  @Action
  async removeRange(ids: Array<number>): Promise<void> {
    await api.removeRange(ids)
  }

  @Action
  async restore(id: number): Promise<void> {
    await api.restore(id)
  }

  @Action
  async restoreRange(ids: number[]): Promise<void> {
    await api.restoreRange(ids)
  }
}

export default getModule(GroupsStore)
