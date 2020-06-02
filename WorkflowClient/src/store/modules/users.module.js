import usersAPI from '~/api/users';

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
    async getUsers({ commit }, params) {
      const response = await usersAPI.getPage(params);
      const users = response.data;
      commit('setUsers', users);
    },
    async getUser({ commit }, id) {
      const response = await usersAPI.get(id);
      const user = response.data;
      commit('setUser', user);
    },
    async createUser({ commit }, payload) {
      const response = await usersAPI.create(payload);
      const user = response.data;
      commit('setUser', user);
    },
    async updateUser({ commit }, payload) {
      const response = await usersAPI.update(payload);
      const user = response.data;
      commit('setUser', user);
    },
    async deleteUser({ commit }, id) {
      const response = await usersAPI.delete(id);
      const user = response.data;
      if (!user) throw Error;
    }
  },
  getters: {
    users: state => state.users,
    user: state => state.user
  }
};
