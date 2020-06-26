import teamsAPI from '~/api/teams.api';

export default {
  namespaced: true,
  state: () => ({
    teams: [],
    team: {},
    teamUsers: [],
    teamProjects: []
  }),
  mutations: {
    setTeams(state, teams) {
      state.teams = teams;
    },
    setTeam(state, team) {
      state.team = team;
    },
    setTeamUsers(state, teamUsers) {
      state.teamUsers = teamUsers;
    },
    setTeamProjects(state, teamProjects) {
      state.teamProjects = teamProjects;
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
    async fetchTeamUsers({ commit }, params) {
      const response = await teamsAPI.getUsersPage(params);
      const teamUsers = response.data;
      commit('setTeamUsers', teamUsers);
    },
    async fetchTeamProjects({ commit }, params) {
      const response = await teamsAPI.getProjectsPage(params);
      const teamProjects = response.data;
      commit('setTeamProjects', teamProjects);
    },
    async createTeam({ commit }, payload) {
      const response = await teamsAPI.create(payload, payload.projectId);
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
    getTeam: state => state.team,
    getTeamUsers: state => state.teamUsers,
    getTeamProjects: state => state.teamProjects
  }
};
