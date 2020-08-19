import usersAPI from '@/api/users.api';
import { Message } from 'element-ui';

export default {
  namespaced: true,
  state: () => ({
    users: [],
    user: {}
  }),
  mutations: {
    setUsers(state, users) {
      state.users = users;
    },
    setUser(state, user) {
      state.user = user;
    }
  },
  actions: {
    async findAll({ rootState, commit }, params) {
      const response = await usersAPI.findAll(params);
      const users = response.data;
      commit('setUsers', users);
      return users;
    },
    async findOneById({ commit }, id) {
      const response = await usersAPI.findOneById(id);
      commit('setUser', response.data);
      return response.data;
    },
    async createOne({ commit }, payload) {
      const response = await usersAPI.createOne(payload);
      commit('setUser', response.data);
      return response.data;
    },
    async updateOne({ commit }, payload) {
      const response = await usersAPI.updateOne(payload);
      commit('setUser', response.data);
      return response.data;
    },
    async deleteOne({ commit }, id) {
      await usersAPI.deleteOne(id);
    },
    async deleteMany({ commit }, ids) {
      await usersAPI.deleteMany(ids);
    },
    async restoreOne({ commit }, id) {
      await usersAPI.restoreOne(id);
    },
    async restoreMany({ commit }, ids) {
      await usersAPI.restoreMany(ids);
    },
    async isLoginExist({ commit }, login) {
      try {
        const response = await usersAPI.isUserNameExist(login);
        return response.data;
      } catch (error) {
        Message.warning('Не удалось проверить уникальность логина');
        return false;
      }
    },
    async isEmailExist({ commit }, email) {
      try {
        const response = await usersAPI.isEmailExist(email);
        return response.data;
      } catch (error) {
        Message.warning('Не удалось проверить уникальность почтового ящика');
        return false;
      }
    }
  },
  getters: {
    getUsers: state => state.users,
    getUser: state => state.user
  }
};
