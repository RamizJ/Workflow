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
    async fetchUsers({ rootState, commit }, params) {
      const response = await usersAPI.findAll(params);
      const users = response.data;
      commit('setUsers', users);
      return users;
    },
    async fetchUser({ commit }, id) {
      const response = await usersAPI.findOneById(id);
      const user = response.data;
      commit('setUser', user);
    },
    async createUser({ commit }, payload) {
      const response = await usersAPI.createOne(payload);
      const user = response.data;
      commit('setUser', user);
    },
    async updateUser({ commit }, payload) {
      const response = await usersAPI.updateOne(payload);
      const user = response.data;
      commit('setUser', user);
    },
    async deleteUser({ commit }, id) {
      const response = await usersAPI.deleteOne(id);
      const user = response.data;
      if (!user) throw Error;
    },
    async deleteUsers({ commit }, ids) {
      const response = await usersAPI.deleteMany(ids);
      const users = response.data;
      if (!users) throw Error;
    },
    async restoreUser({ commit }, id) {
      const response = await usersAPI.restoreOne(id);
      const user = response.data;
      if (!user) throw Error;
    },
    async restoreUsers({ commit }, ids) {
      const response = await usersAPI.restoreMany(ids);
      const users = response.data;
      if (!users) throw Error;
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
