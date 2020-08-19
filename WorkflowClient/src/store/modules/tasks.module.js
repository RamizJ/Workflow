import tasksAPI from '@/api/tasks.api';

export default {
  namespaced: true,
  state: () => ({
    tasks: [],
    task: {},
    taskAttachments: []
  }),
  mutations: {
    setTasks(state, tasks) {
      state.tasks = tasks;
    },
    setTask(state, task) {
      state.task = task;
    },
    setTaskAttachments(state, attachments) {
      state.taskAttachments = attachments;
    }
  },
  actions: {
    async findOneById({ commit }, id) {
      const response = await tasksAPI.findOneById(id);
      const task = response.data;
      commit('setTask', task);
      return task;
    },
    async findAll({ commit }, params) {
      const response = await tasksAPI.findAll(params);
      const tasks = response.data.filter(task => !task.parentGoalId);
      commit('setTasks', tasks);
      return tasks;
    },
    async findAllByIds({ commit }, ids) {
      const response = await tasksAPI.findAllByIds(ids);
      const tasks = response.data.filter(task => !task.parentGoalId);
      commit('setTasks', tasks);
      return tasks;
    },
    async createOne({ commit }, task) {
      const response = await tasksAPI.createOne(task);
      commit('setTask', response.data);
      return response.data;
    },
    async updateOne({ commit }, task) {
      const response = await tasksAPI.updateOne(task);
      commit('setTask', response.data);
      return response.data;
    },
    async updateMany({ commit }, tasks) {
      if (!tasks.length) return;
      const response = await tasksAPI.updateMany(tasks);
      commit('setTasks', response.data);
      return response.data;
    },
    async deleteOne({ commit }, id) {
      await tasksAPI.deleteOne(id);
    },
    async deleteMany({ commit }, ids) {
      await tasksAPI.deleteMany(ids);
    },
    async restoreOne({ commit }, id) {
      await tasksAPI.restoreOne(id);
    },
    async restoreMany({ commit }, ids) {
      await tasksAPI.restoreMany(ids);
    },
    async findParent({ commit }, id) {
      const response = await tasksAPI.findParent(id);
      return response.data;
    },
    async findChild({ commit }, id) {
      const response = await tasksAPI.findChild(id);
      return response.data;
    },
    async addChild({ state, dispatch, commit }, { parentId, tasks }) {
      let taskIds = [];
      for (let task of tasks) {
        await dispatch('createOne', task);
        taskIds.push(state.task.id);
      }
      const response = await tasksAPI.addChild(parentId, taskIds);
      return response.data;
    },
    async findAttachments({ commit }, taskId) {
      const response = await tasksAPI.findAttachments(taskId);
      const attachments = response.data;
      commit('setTaskAttachments', attachments);
    },
    async uploadAttachments({ commit }, { taskId, files }) {
      await tasksAPI.uploadAttachments(taskId, files);
    },
    async downloadAttachment({ commit }, file) {
      const response = await tasksAPI.downloadAttachment(file.id);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', file.name);
      document.body.appendChild(link);
      link.click();
    },
    async removeAttachments({ commit }, attachmentIds) {
      await tasksAPI.removeAttachments(attachmentIds);
    }
  },
  getters: {
    getTasks(state) {
      return state.tasks;
    },
    getTask(state) {
      return state.task;
    },
    getTaskAttachments(state) {
      return state.taskAttachments;
    }
  }
};
