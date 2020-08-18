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
    async fetchTask({ commit }, id) {
      const response = await tasksAPI.findOneById(id);
      const task = response.data;
      commit('setTask', task);
      return task;
    },
    async fetchTasks({ commit }, params) {
      const response = await tasksAPI.findAll(params);
      const tasks = response.data.filter(task => !task.parentGoalId);
      commit('setTasks', tasks);
      return tasks;
    },
    // async searchTasks({ commit }, params) {
    //   const response = await tasksAPI.getPage(params);
    //   const tasks = response.data;
    //   return tasks;
    // },

    async fetchChildTasks({ commit }, id) {
      const response = await tasksAPI.findChildTasks(id);
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
      const response = await tasksAPI.createOne(task);
      commit('setTask', response.data);
    },
    async updateTask({ commit }, task) {
      const response = await tasksAPI.updateOne(task);
      commit('setTask', response.data);
    },
    async updateTasks({ commit }, tasks) {
      if (!tasks.length) return;
      const response = await tasksAPI.updateMany(tasks);
      commit('setTasks', response.data);
    },
    async deleteTask({ commit }, id) {
      const response = await tasksAPI.deleteOne(id);
      const task = response.data;
      if (!task) throw Error;
    },
    async deleteTasks({ commit }, ids) {
      const response = await tasksAPI.deleteMany(ids);
      const tasks = response.data;
      if (!tasks) throw Error;
    },
    async restoreTask({ commit }, id) {
      const response = await tasksAPI.restoreOne(id);
      const task = response.data;
      if (!task) throw Error;
    },
    async restoreTasks({ commit }, ids) {
      const response = await tasksAPI.restoreMany(ids);
      const tasks = response.data;
      if (!tasks) throw Error;
    },
    async fetchAttachments({ commit }, taskId) {
      const response = await tasksAPI.findAttachments(taskId);
      const attachments = response.data;
      commit('setTaskAttachments', attachments);
    },
    async addAttachments({ commit }, { taskId, files }) {
      await tasksAPI.uploadAttachments(taskId, files);
    },
    async removeAttachments({ commit }, attachmentIds) {
      await tasksAPI.removeAttachments(attachmentIds);
    },
    async downloadAttachment({ commit }, file) {
      const response = await tasksAPI.downloadAttachment(file.id);
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
