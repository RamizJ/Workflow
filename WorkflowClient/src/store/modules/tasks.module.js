import axios from 'axios';
import qs from 'qs';
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
      // try {
      //   const response = await tasksAPI.getPage(params);
      //   console.log(response);
      //   const tasks = response.data;
      //   commit('setTasks', tasks);
      // } catch (e) {
      //   console.log(e)
      // }


      let response;
      let query = {};
      if (params.search) query.q = params.search;
      else query.q = 'test';
      if (params.pageNumber) query.page = params.page;
      if (params.pageSize) query.per_page = params.limit;
      const queryString = qs.stringify(query, { addQueryPrefix: true });
      // const response = await axios.get(`/api/Goals/GetPage${queryString}`);
      response = await axios.get(
        `https://api.github.com/search/repositories${queryString}`
      );
      const tasks = response.data.items;
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
