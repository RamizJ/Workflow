import {
  Action,
  getModule,
  Module,
  Mutation,
  MutationAction,
  VuexModule,
} from 'vuex-module-decorators'
import moment from 'moment' // TODO: Replace with dateFns / Luxon.js
import store from '@/core/store'
import api from '../api'
import Goal, { Status } from '@/modules/goals/models/goal.type'
import Query, { FilterField, SortType } from '@/core/types/query.type'
import Attachment from '@/modules/goals/models/attachment.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'goalsStore',
  store,
})
class GoalsStore extends VuexModule {
  _goal: Goal | null = null
  _goals: Goal[] = []
  _goalWindowOpened = false

  public get goal() {
    return this._goal
  }
  public get tasks() {
    return this._goals
  }
  public get isGoalWindowOpened(): boolean {
    return this._goalWindowOpened
  }

  @Mutation
  setTask(task: Goal) {
    this._goal = task
  }

  @Mutation
  setTasks(tasks: Goal[]) {
    this._goals = tasks
  }

  @MutationAction({ mutate: ['_goalWindowOpened'] })
  public async closeGoalWindow() {
    return {
      _goalWindowOpened: false,
    }
  }

  @MutationAction({ mutate: ['_goalWindowOpened', '_goal'] })
  public async openGoalWindow(goal?: Goal) {
    return {
      _goal: goal || null,
      _goalWindowOpened: true,
    }
  }

  @Action({ rawError: true })
  async findAll(query: Query): Promise<Goal[]> {
    const response = await api.getPage(query)
    const results = (response.data as Goal[]).filter((task) => !task.parentGoalId)
    this.context.commit('setTasks', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Goal[]> {
    const response = await api.getRange(ids)
    const entities = response.data as Goal[]
    this.context.commit('setTasks', entities)
    return entities
  }

  @Action
  async findOneById(id: number): Promise<Goal> {
    const response = await api.get(id)
    const result = response.data as Goal
    if (result.parentGoalId)
      result.parent = (await this.context.dispatch('findParent', id)) as Goal[]
    if (result.hasChildren) {
      const child = (await this.context.dispatch('findChild', { id })) as Goal[]
      result.children = child.sort(this.compare).reverse()
    }
    if (result.isAttachmentsExist)
      result.attachments = (await this.context.dispatch('findAttachments', id)) as Attachment[]
    result.attachments = result.attachments?.map((attachment) => {
      attachment.name = attachment.fileName
      return attachment
    })
    this.context.commit('setTask', result)
    return result
  }

  @Action
  async createOne(entity: Goal): Promise<Goal> {
    const response = await api.create(entity)
    const createdTask = response.data as Goal
    createdTask.attachments = entity.attachments

    // if (entity.children?.length) {
    //   const child: Goal[] = []
    //   for (const childTask of entity.children.reverse()) {
    //     childTask.parentGoalId = createdTask.id
    //     if (childTask.isRemoved) continue
    //     const createdChild = await this.context.dispatch('createOne', childTask)
    //     child.push(createdChild)
    //   }
    //   const childIds = child.map((task) => task.id)
    //   await this.context.dispatch('addChild', { id: createdTask.id, childIds })
    //   await this.context.dispatch('updateMany', child)
    // }

    this.context.commit('setTask', createdTask)
    return createdTask
  }

  @Action
  async createMany(entities: Goal[]): Promise<Goal[]> {
    const results: Goal[] = []
    for (const entity of entities) {
      const createdEntity = await this.context.dispatch('createOne', entity)
      results.push(createdEntity)
    }
    return results
  }

  @Action
  async updateOne(entity: Goal): Promise<void> {
    await api.update(entity)

    if (entity.children?.length) {
      const child: Goal[] = []
      for (const childTask of entity.children.reverse()) {
        childTask.parentGoalId = entity.id
        let updatedTask = childTask
        if (childTask.isRemoved && childTask.id)
          await this.context.dispatch('deleteOne', childTask.id)
        if (!childTask.id && !childTask.isRemoved)
          updatedTask = await this.context.dispatch('createOne', childTask)
        if (childTask.id && !childTask.isRemoved) child.push(updatedTask)
      }
      const childIds = child.map((task) => task.id)
      await this.context.dispatch('addChild', { id: entity.id, childIds })
      await this.context.dispatch('updateMany', child)
    }
  }

  private compare(taskA: Goal, taskB: Goal): number {
    const dateA = moment.utc(taskA.creationDate)
    const dateB = moment.utc(taskB.creationDate)

    if (dateA < dateB) {
      return -1
    }
    if (dateA > dateB) {
      return 1
    }
    return 0
  }

  @Action
  async updateMany(entities: Goal[]): Promise<void> {
    await api.updateRange(entities)
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await api.remove(id)
  }

  @Action
  async deleteMany(ids: Array<number>): Promise<void> {
    await api.removeRange(ids)
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await api.restore(id)
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await api.restoreRange(ids)
  }

  @Action
  async findParent(id: number): Promise<Goal> {
    const response = await api.getParentGoal(id)
    return response.data as Goal
  }

  @Action
  async findChild({ id, query = new Query() }: { id: number; query?: Query }): Promise<Goal[]> {
    const response = await api.getChildGoals(id, query)
    return response.data as Goal[]
  }

  @Action
  async addChild({ id, childIds }: { id: number; childIds: number[] }): Promise<void> {
    await api.addChildGoals(id, childIds)
  }

  @Action
  async findAttachments(id: number): Promise<Attachment[]> {
    const response = await api.getAttachments(id)
    return response.data as Attachment[]
  }

  @Action
  async uploadAttachments({ id, files }: { id: number; files: FormData }): Promise<void> {
    await api.addAttachments(id, files)
  }

  @Action
  async downloadAttachment(attachment: Attachment): Promise<void> {
    if (!attachment.id || !attachment.fileName) return
    const response = await api.downloadAttachmentFile(attachment.id)
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', attachment.fileName)
    document.body.appendChild(link)
    link.click()
  }

  @Action
  async removeAttachments(ids: number[]): Promise<void> {
    await api.removeAttachments(ids)
  }

  @Action
  async getTasksCount(projectId: number): Promise<number> {
    const response = await api.getTotalProjectGoalsCount(projectId)
    return response.data
  }

  @Action
  async getTasksCountByStatus({
    projectId,
    status,
  }: {
    projectId: number
    status: Status
  }): Promise<number> {
    const response = await api.getProjectGoalsByStateCount(projectId, status)
    return response.data
  }
}

export default getModule(GoalsStore)
