import authentication from '~/api/authentication';

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
      const response = await authentication.login(
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
      await authentication.logout();
      commit('setToken', null);
      commit('setUser', null);
      localStorage.removeItem('access_token');
    },
    async fetchMe({ commit }) {
      // const response = await authentication.getMe();
      // console.log(response);
      const response = {
        data: {
          user: {
            id: "b4396eb2-0405-43d5-ab28-2432b9336aff",
            firstName: "Administrator",
            middleName: "",
            lastName: "",
            email: "admin@admin.ru",
            roles: ["ADMINISTRATOR_ROLE"]
          }
        }
      };
      const user = response.data.user;
      commit('setUser', user);
    },
    async updatePassword({ commit }, { currentPassword, newPassword }) {
      const response = await authentication.changePassword(currentPassword, newPassword);
    },
    async hasRole({ commit }, role) {
      console.log(role);
    }
  },
  getters: {
    loggedIn: state => !!state.token,
    token: state => state.token,
    me: state => state.user
  }
};
