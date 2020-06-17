import tasksAPI from '~/api/tasks';

export default {
  namespaced: true,
  state: () => ({
    tasks: [],
    task: {}
  }),
  mutations: {
    setTasks(state, tasks) {
      state.tasks = tasks;
    },
    setTask(state, task) {
      state.task = task;
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
    async deleteTask({ commit }, id) {
      const response = await tasksAPI.delete(id);
      const project = response.data;
      if (!project) throw Error;
    }
  },
  getters: {
    getTasks(state) {
      return state.tasks;
    },
    getTask(state) {
      return state.task;
    }
  }
};
