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
      const tasks = response.data;
      commit('setTasks', tasks);
    },
    async fetchTask({ commit }, id) {
      const response = await tasksAPI.get(id);
      const task = response.data;
      commit('setTask', task);
    },
    async createTask({ commit }, task) {
      const response = await tasksAPI.create(task);
      commit('setTask', response.data);
    },
    async updateTask({ commit }, task) {
      const response = await tasksAPI.update(task);
      commit('setTask', response.data);
    },
    async completeTask({ commit }, task) {
      let completedTask = task;
      completedTask.state = 'Succeed';
      const response = await tasksAPI.update(completedTask);
      commit('setTask', response.data);
    },
    async completeTasks({ commit }, tasks) {
      let completedTasks = tasks.map(task => {
        task.state = 'Succeed';
        return task;
      });
      const response = await tasksAPI.updateRange(completedTasks);
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
