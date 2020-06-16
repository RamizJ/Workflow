import teamsAPI from '~/api/teams';

export default {
  namespaced: true,
  state: () => ({
    teams: [],
    team: {}
  }),
  mutations: {
    setTeams(state, teams) {
      state.teams = teams;
    },
    setTeam(state, team) {
      state.team = team;
    }
  },
  actions: {
    async fetchTeams({ commit }, params) {
      const response = await teamsAPI.getPage(params);
      const teams = response.data;
      commit('setTeams', teams);
    },
    async fetchTeam({ commit }, id) {
      const response = await teamsAPI.get(id);
      const team = response.data;
      commit('setTeam', team);
    },
    async createTeam({ commit }, { payload, projectId }) {
      const response = await teamsAPI.create(payload, projectId);
      const team = response.data;
      commit('setTeam', team);
    },
    async updateTeam({ commit }, payload) {
      const response = await teamsAPI.update(payload);
      const team = response.data;
      commit('setTeam', team);
    },
    async deleteTeam({ commit }, id) {
      const response = await teamsAPI.delete(id);
      const team = response.data;
      if (!team) throw Error;
    }
  },
  getters: {
    getTeams: state => state.teams,
    getTeam: state => state.team
  }
};
