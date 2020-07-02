import scopesAPI from '~/api/scopes.api';

export default {
  namespaced: true,
  state: () => ({
    scopes: [],
    scope: {}
  }),
  mutations: {
    setScopes(state, scopes) {
      state.scopes = scopes;
    },
    setScope(state, scope) {
      state.scope = scope;
    }
  },
  actions: {
    async fetchScopes({ commit }, params) {
      const response = await scopesAPI.getPage(params);
      const scopes = response.data;
      commit('setScopes', scopes);
    },
    async fetchScope({ commit }, id) {
      const response = await scopesAPI.get(id);
      const scope = response.data;
      commit('setScope', scope);
    },
    async createScope({ commit }, payload) {
      const response = await scopesAPI.create(payload);
      const scope = response.data;
      commit('setScope', scope);
    },
    async updateScope({ commit }, payload) {
      const response = await scopesAPI.update(payload);
      const scope = response.data;
      commit('setScope', scope);
    },
    async deleteScope({ commit }, id) {
      const response = await scopesAPI.delete(id);
      const scope = response.data;
      if (!scope) throw Error;
    }
  },
  getters: {
    getScopes: state => state.scopes,
    getScope: state => state.scope
  }
};
