import auth from '~/api/auth.api';

export default {
  namespaced: true,
  state: () => ({
    user: null,
    token: localStorage.getItem('access_token') || null
  }),
  mutations: {
    setUser(state, user) {
      state.user = user;
    },
    setToken(state, token) {
      state.token = token;
    }
  },
  actions: {
    async login({ commit }, credentials) {
      const response = await auth.login(
        credentials.login,
        credentials.password,
        credentials.rememberMe
      );
      const user = response.data.user;
      const token = response.data.token;
      localStorage.setItem('access_token', token);
      commit('setUser', user);
      commit('setToken', token);
    },
    async logout({ commit }) {
      await auth.logout();
      commit('setToken', null);
      commit('setUser', null);
      localStorage.removeItem('access_token');
    },
    async fetchMe({ state, commit }) {
      if (!state.token) return;
      const response = await auth.getMe();
      if (!response) return;
      const user = response.data;
      commit('setUser', user);
    },
    async updatePassword({ commit }, { currentPassword, newPassword }) {
      const response = await auth.changePassword(currentPassword, newPassword);
    },
    async hasRole({ commit }, role) {
      console.log(role);
    }
  },
  getters: {
    loggedIn: state => !!state.token && !!state.user,
    token: state => state.token,
    me: state => state.user
  }
};
