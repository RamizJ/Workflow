import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'
import moment from 'moment'

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
    if (result.isChildsExist) {
      const child = (await this.context.dispatch('findChild', { id })) as Task[]
      result.childTasks = child.sort(this.compare).reverse()
    }
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
    const response = await tasksAPI.createOne(entity)
    const createdTask = response.data as Task
    createdTask.attachments = entity.attachments

    const hasChild = !!entity.childTasks?.length

    if (hasChild) {
      const child: Task[] = []
      for (const childTask of entity.childTasks!.reverse()) {
        childTask.parentGoalId = createdTask.id
        if (childTask.isRemoved) continue
        const createdChild = await this.context.dispatch('createOne', childTask)
        child.push(createdChild)
      }
      const childIds = child.map(task => task.id)
      await this.context.dispatch('addChild', { id: createdTask.id, childIds })
      await this.context.dispatch('updateMany', child)
    }

    this.context.commit('setTask', createdTask)
    return createdTask
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
    await tasksAPI.updateOne(entity)

    const hasChild = !!(entity.childTasks && entity.childTasks.length)

    if (hasChild) {
      const child: Task[] = []
      for (const childTask of entity.childTasks!.reverse()) {
        childTask.parentGoalId = entity.id
        let updatedTask = childTask
        if (childTask.isRemoved && childTask.id)
          await this.context.dispatch('deleteOne', childTask.id)
        if (!childTask.id && !childTask.isRemoved)
          updatedTask = await this.context.dispatch('createOne', childTask)
        if (childTask.id && !childTask.isRemoved) child.push(updatedTask)
      }
      const childIds = child.map(task => task.id)
      await this.context.dispatch('addChild', { id: entity.id, childIds })
      await this.context.dispatch('updateMany', child)
    }
  }

  private compare(taskA: Task, taskB: Task): number {
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
  async addChild({ id, childIds }: { id: number; childIds: number[] }): Promise<void> {
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
