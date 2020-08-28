import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'

import store from '@/store'
import tasksAPI from '@/api/tasks.api'
import Task from '@/types/task.type'
import Query from '@/types/query.type'
import Attachment from '@/types/attachment.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'tasksModule',
  store
})
class TasksModule extends VuexModule {
  _task: Task | null = null
  _tasks: Task[] = []

  public get task() {
    return this._task
  }
  public get tasks() {
    return this._tasks
  }

  @Mutation
  setTask(task: Task) {
    this._task = task
  }

  @Mutation
  setTasks(tasks: Task[]) {
    this._tasks = tasks
  }

  @Action({ rawError: true })
  async findAll(query: Query): Promise<Task[]> {
    const response = await tasksAPI.findAll(query)
    const results = (response.data as Task[]).filter(task => !task.parentGoalId)
    this.context.commit('setTasks', results)
    return results
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Task[]> {
    const response = await tasksAPI.findAllByIds(ids)
    const entities = response.data as Task[]
    this.context.commit('setTasks', entities)
    return entities
  }

  @Action
  async findOneById(id: number): Promise<Task> {
    const response = await tasksAPI.findOneById(id)
    const result = response.data as Task
    if (result.parentGoalId)
      result.parent = (await this.context.dispatch('findParent', id)) as Task[]
    if (result.isChildsExist)
      result.childTasks = (await this.context.dispatch('findChild', { id })) as Task[]
    if (result.isAttachmentsExist)
      result.attachments = (await this.context.dispatch('findAttachments', id)) as Attachment[]
    result.attachments = result.attachments?.map(attachment => {
      attachment.name = attachment.fileName
      return attachment
    })
    this.context.commit('setTask', result)
    return result
  }

  @Action
  async createOne(entity: Task): Promise<Task> {
    const request = {
      goal: entity,
      childGoals: entity.childTasks || []
    }

    if (entity.childTasks?.length) {
      const updatedTasks: Task[] = []
      for (const childTask of entity.childTasks) {
        let updatedTask = childTask
        if (!childTask.id) updatedTask = await this.context.dispatch('createOne', childTask)
        updatedTask.parentGoalId = entity.id
        if (childTask.isRemoved && childTask.id)
          await this.context.dispatch('deleteOne', childTask.id)
        else updatedTasks.push(updatedTask)
      }
      await this.context.dispatch('updateMany', updatedTasks)
      entity.childTasks = updatedTasks
    }

    const response = await tasksAPI.createOne(request)
    const result = response.data as Task
    result.attachments = entity.attachments
    this.context.commit('setTask', result)
    return result
  }

  @Action
  async createMany(entities: Task[]): Promise<Task[]> {
    const results: Task[] = []
    for (const entity of entities) {
      const createdEntity = await this.context.dispatch('createOne', entity)
      results.push(createdEntity)
    }
    return results
  }

  @Action
  async updateOne(entity: Task): Promise<void> {
    const request = {
      goal: entity,
      childGoals: entity.childTasks || []
    }

    if (entity.childTasks?.length) {
      const updatedTasks: Task[] = []
      for (const childTask of entity.childTasks) {
        let updatedTask = childTask
        if (!childTask.id) updatedTask = await this.context.dispatch('createOne', childTask)
        if (childTask.isRemoved && childTask.id)
          await this.context.dispatch('deleteOne', childTask.id)
        else updatedTasks.push(updatedTask)
      }
      await this.context.dispatch('updateMany', updatedTasks)
      entity.childTasks = updatedTasks
    }
    await tasksAPI.updateOne(request)
  }

  @Action
  async updateMany(entities: Task[]): Promise<void> {
    await tasksAPI.updateMany(entities)
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await tasksAPI.deleteOne(id)
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await tasksAPI.deleteMany(ids)
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await tasksAPI.restoreOne(id)
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await tasksAPI.restoreMany(ids)
  }

  @Action
  async findParent(id: number): Promise<Task> {
    const response = await tasksAPI.findParent(id)
    return response.data as Task
  }

  @Action
  async findChild({ id, query }: { id: number; query: Query | undefined }): Promise<Task[]> {
    if (!query)
      query = {
        pageNumber: 0,
        pageSize: 20
      }
    const response = await tasksAPI.findChild(id, query)
    return response.data as Task[]
  }

  @Action
  async addChild({ id, entities }: { id: number; entities: Task[] }): Promise<void> {
    const childIds: number[] = []
    for (const entity of entities) {
      const result = await this.context.dispatch('createOne', entity)
      childIds.push(result.id)
    }
    await tasksAPI.addChild(id, childIds)
  }

  @Action
  async findAttachments(id: number): Promise<Attachment[]> {
    const response = await tasksAPI.findAttachments(id)
    return response.data as Attachment[]
  }

  @Action
  async uploadAttachments({ id, files }: { id: number; files: FormData }): Promise<Attachment[]> {
    const response = await tasksAPI.uploadAttachments(id, files)
    return response.data as Attachment[]
  }

  @Action
  async downloadAttachment(attachment: Attachment): Promise<void> {
    const response = await tasksAPI.downloadAttachment(attachment.id)
    const url = window.URL.createObjectURL(new Blob([response.data]))
    const link = document.createElement('a')
    link.href = url
    link.setAttribute('download', attachment.fileName)
    document.body.appendChild(link)
    link.click()
  }

  @Action
  async removeAttachments(ids: number[]): Promise<void> {
    await tasksAPI.removeAttachments(ids)
  }
}

export default getModule(TasksModule)
