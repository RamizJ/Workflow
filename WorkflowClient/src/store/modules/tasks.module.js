import tasksAPI from '~/api/tasks.api';

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
    async fetchTasks({ commit }, params) {
      const response = await tasksAPI.getPage(params);
      const tasks = response.data.filter(task => !task.parentGoalId);
      commit('setTasks', tasks);
      return tasks;
    },
    async searchTasks({ commit }, params) {
      const response = await tasksAPI.getPage(params);
      const tasks = response.data;
      return tasks;
    },
    async fetchTask({ commit }, id) {
      const response = await tasksAPI.get(id);
      const task = response.data;
      commit('setTask', task);
      return task;
    },
    async fetchChildTasks({ commit }, id) {
      const response = await tasksAPI.getChildTasks(id);
      const tasks = response.data;
      return tasks;
    },
    async addChildTasks({ state, dispatch, commit }, { parentId, tasks }) {
      let taskIds = [];
      for (let task of tasks) {
        await dispatch('createTask', task);
        taskIds.push(state.task.id);
      }
      const response = await tasksAPI.addChildTasks(parentId, taskIds);
      return response.data;
    },
    async createTask({ commit }, task) {
      const response = await tasksAPI.create(task);
      commit('setTask', response.data);
    },
    async updateTask({ commit }, task) {
      const response = await tasksAPI.update(task);
      commit('setTask', response.data);
    },
    async updateTasks({ commit }, tasks) {
      if (!tasks.length) return;
      const response = await tasksAPI.updateRange(tasks);
      commit('setTasks', response.data);
    },
    async deleteTask({ commit }, id) {
      const response = await tasksAPI.delete(id);
      const task = response.data;
      if (!task) throw Error;
    },
    async deleteTasks({ commit }, ids) {
      const response = await tasksAPI.deleteRange(ids);
      const tasks = response.data;
      if (!tasks) throw Error;
    },
    async restoreTask({ commit }, id) {
      const response = await tasksAPI.restore(id);
      const task = response.data;
      if (!task) throw Error;
    },
    async restoreTasks({ commit }, ids) {
      const response = await tasksAPI.restoreRange(ids);
      const tasks = response.data;
      if (!tasks) throw Error;
    },
    async getTasksCount({ commit }, projectId) {
      const response = await tasksAPI.getTasksCount(projectId);
      return response.data;
    },
    async getTasksCountByStatus({ commit }, { projectId, status }) {
      const response = await tasksAPI.getTasksCountByStatus(projectId, status);
      return response.data;
    },
    async fetchAttachments({ commit }, taskId) {
      const response = await tasksAPI.getAttachments(taskId);
      const attachments = response.data;
      commit('setTaskAttachments', attachments);
    },
    async addAttachments({ commit }, { taskId, files }) {
      const response = await tasksAPI.addAttachments(taskId, files);
    },
    async removeAttachments({ commit }, attachmentIds) {
      const response = await tasksAPI.removeAttachments(attachmentIds);
    },
    async downloadAttachment({ commit }, file) {
      const response = await tasksAPI.downloadAttachmentFile(file.id);
      const url = window.URL.createObjectURL(new Blob([response.data]));
      const link = document.createElement('a');
      link.href = url;
      link.setAttribute('download', file.name);
      document.body.appendChild(link);
      link.click();
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
