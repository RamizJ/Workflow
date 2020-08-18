import authAPI from '@/api/auth.api';

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
      if (token) localStorage.setItem('access_token', token);
      else localStorage.removeItem('access_token');
    }
  },
  actions: {
    async login({ commit }, credentials) {
      const response = await authAPI.login(
        credentials.login,
        credentials.password,
        credentials.rememberMe
      );
      const user = response.data.user;
      const token = response.data.token;
      commit('setUser', user);
      commit('setToken', token);
    },
    async logout({ commit }) {
      await authAPI.logout();
      commit('setToken', null);
      commit('setUser', null);
    },
    async fetchMe({ state, commit }) {
      if (!state.token) return;
      const response = await authAPI.getMe();
      if (!response || response.status === 401) {
        commit('setToken', null);
        commit('setUser', null);
      } else commit('setUser', response.data);
    },
    async updatePassword({ commit }, { currentPassword, newPassword }) {
      await authAPI.changePassword(currentPassword, newPassword);
    }
  },
  getters: {
    loggedIn: state => !!state.token && !!state.user,
    token: state => state.token,
    me: state => state.user
  }
};
