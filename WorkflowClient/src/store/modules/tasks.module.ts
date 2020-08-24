import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators';

import store from '@/store';
import tasksAPI from '@/api/tasks.api';
import Task from '@/types/task.type';
import Query from '@/types/query.type';
import Attachment from '@/types/attachment.type';

@Module({
  dynamic: true,
  namespaced: true,
  name: 'tasksModule',
  store
})
class TasksModule extends VuexModule {
  _task: Task | null = null;
  _tasks: Task[] = [];

  public get task() {
    return this._task;
  }
  public get tasks() {
    return this._tasks;
  }

  @Mutation
  setTask(task: Task) {
    this._task = task;
  }

  @Mutation
  setTasks(tasks: Task[]) {
    this._tasks = tasks;
  }

  @Action({ rawError: true })
  async findAll(query: Query): Promise<Task[]> {
    const response = await tasksAPI.findAll(query);
    const results = response.data as Task[];
    this.context.commit('setTasks', results);
    return results;
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Task[]> {
    const response = await tasksAPI.findAllByIds(ids);
    const entities = response.data as Task[];
    this.context.commit('setTasks', entities);
    return entities;
  }

  @Action
  async findOneById(id: number): Promise<Task> {
    const response = await tasksAPI.findOneById(id);
    const result = response.data as Task;
    if (result.parentGoalId)
      result.parent = (await this.context.dispatch('findParent', id)) as Task[];
    if (result.isChildsExist)
      result.child = (await this.context.dispatch('findChild', id)) as Task[];
    if (result.isAttachmentsExist)
      result.attachments = (await this.context.dispatch('findAttachments', id)) as Attachment[];
    this.context.commit('setTask', result);
    return result;
  }

  @Action
  async createOne(entity: Task): Promise<Task> {
    const response = await tasksAPI.createOne(entity);
    const result = response.data as Task;
    this.context.commit('setTask', result);
    return result;
  }

  @Action
  async createMany(entities: Task[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('createOne', entity);
    }
  }

  @Action
  async updateOne(entity: Task): Promise<void> {
    await tasksAPI.updateOne(entity);
  }

  @Action
  async updateMany(entities: Task[]): Promise<void> {
    await tasksAPI.updateMany(entities);
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await tasksAPI.deleteOne(id);
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await tasksAPI.deleteMany(ids);
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await tasksAPI.restoreOne(id);
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await tasksAPI.restoreMany(ids);
  }

  @Action
  async findParent(id: number): Promise<Task> {
    const response = await tasksAPI.findParent(id);
    return response.data as Task;
  }

  @Action
  async findChild(id: number): Promise<Task[]> {
    const response = await tasksAPI.findChild(id);
    return response.data as Task[];
  }

  @Action
  async addChild({ id, entities }: { id: number; entities: Task[] }): Promise<void> {
    const childIds = [];
    for (const entity of entities) {
      const result = await this.context.dispatch('createOne', entity);
      childIds.push(result.id);
    }
    await tasksAPI.addChild(id, childIds);
  }

  @Action
  async findAttachments(id: number): Promise<Attachment[]> {
    const response = await tasksAPI.findAttachments(id);
    return response.data as Attachment[];
  }

  @Action
  async uploadAttachments({ id, files }: { id: number; files: FormData }): Promise<Attachment[]> {
    const response = await tasksAPI.uploadAttachments(id, files);
    return response.data as Attachment[];
  }

  @Action
  async downloadAttachment(attachment: Attachment): Promise<void> {
    const response = await tasksAPI.downloadAttachment(attachment.id);
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.setAttribute('download', attachment.fileName);
    document.body.appendChild(link);
    link.click();
  }

  @Action
  async removeAttachments(ids: number[]): Promise<void> {
    await tasksAPI.removeAttachments(ids);
  }
}

export default getModule(TasksModule);

// export default {
//   namespaced: true,
//   state: () => ({
//     tasks: [],
//     task: {},
//     taskAttachments: []
//   }),
//   mutations: {
//     setTasks(state, tasks) {
//       state.tasks = tasks;
//     },
//     setTask(state, task) {
//       state.task = task;
//     },
//     setTaskAttachments(state, attachments) {
//       state.taskAttachments = attachments;
//     }
//   },
//   actions: {
//     async findOneById({ commit }, id) {
//       const response = await tasksAPI.findOneById(id);
//       const task = response.data;
//       commit('setTask', task);
//       return task;
//     },
//     async findAll({ commit }, params) {
//       const response = await tasksAPI.findAll(params);
//       const tasks = response.data.filter(task => !task.parentGoalId);
//       commit('setTasks', tasks);
//       return tasks;
//     },
//     async findAllByIds({ commit }, ids) {
//       const response = await tasksAPI.findAllByIds(ids);
//       const tasks = response.data.filter(task => !task.parentGoalId);
//       commit('setTasks', tasks);
//       return tasks;
//     },
//     async createOne({ commit }, task) {
//       const response = await tasksAPI.createOne(task);
//       commit('setTask', response.data);
//       return response.data;
//     },
//     async updateOne({ commit }, task) {
//       const response = await tasksAPI.updateOne(task);
//       commit('setTask', response.data);
//       return response.data;
//     },
//     async updateMany({ commit }, tasks) {
//       if (!tasks.length) return;
//       const response = await tasksAPI.updateMany(tasks);
//       commit('setTasks', response.data);
//       return response.data;
//     },
//     async deleteOne({ commit }, id) {
//       await tasksAPI.deleteOne(id);
//     },
//     async deleteMany({ commit }, ids) {
//       await tasksAPI.deleteMany(ids);
//     },
//     async restoreOne({ commit }, id) {
//       await tasksAPI.restoreOne(id);
//     },
//     async restoreMany({ commit }, ids) {
//       await tasksAPI.restoreMany(ids);
//     },
//     async findParent({ commit }, id) {
//       const response = await tasksAPI.findParent(id);
//       return response.data;
//     },
//     async findChild({ commit }, id) {
//       const response = await tasksAPI.findChild(id);
//       return response.data;
//     },
//     async addChild({ state, dispatch, commit }, { parentId, tasks }) {
//       let taskIds = [];
//       for (let task of tasks) {
//         await dispatch('createOne', task);
//         taskIds.push(state.task.id);
//       }
//       const response = await tasksAPI.addChild(parentId, taskIds);
//       return response.data;
//     },
//     async findAttachments({ commit }, taskId) {
//       const response = await tasksAPI.findAttachments(taskId);
//       const attachments = response.data;
//       commit('setTaskAttachments', attachments);
//     },
//     async uploadAttachments({ commit }, { taskId, files }) {
//       await tasksAPI.uploadAttachments(taskId, files);
//     },
//     async downloadAttachment({ commit }, file) {
//       const response = await tasksAPI.downloadAttachment(file.id);
//       const url = window.URL.createObjectURL(new Blob([response.data]));
//       const link = document.createElement('a');
//       link.href = url;
//       link.setAttribute('download', file.name);
//       document.body.appendChild(link);
//       link.click();
//     },
//     async removeAttachments({ commit }, attachmentIds) {
//       await tasksAPI.removeAttachments(attachmentIds);
//     }
//   },
//   getters: {
//     getTasks(state) {
//       return state.tasks;
//     },
//     getTask(state) {
//       return state.task;
//     },
//     getTaskAttachments(state) {
//       return state.taskAttachments;
//     }
//   }
// };
