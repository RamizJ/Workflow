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
      const teams = response.data.map(team => {
        team.userIds = [];
        team.projectIds = [];
        return team;
      });
      commit('setTeams', teams);
      return teams;
    },
    async fetchTeam({ commit }, id) {
      const response = await teamsAPI.get(id);
      const team = response.data;
      team.userIds = [];
      team.projectIds = [];
      commit('setTeam', team);
    },
    async fetchTeamUsers({ commit }, params) {
      const response = await teamsAPI.getUsersPage(params);
      const teamUsers = response.data;
      commit('setTeamUsers', teamUsers);
      return teamUsers;
    },
    async fetchTeamProjects({ commit }, params) {
      const response = await teamsAPI.getProjectsPage(params);
      const teamProjects = response.data;
      commit('setTeamProjects', teamProjects);
      return teamProjects;
    },
    async createTeam({ commit }, team) {
      let newTeam = {
        team: team,
        userIds: team.userIds,
        projectIds: team.projectIds
      };
      const response = await teamsAPI.create(newTeam);
      const createdTeam = response.data;
      commit('setTeam', createdTeam);
    },
    async addUser({ commit }, { teamId, userId }) {
      await teamsAPI.addUser(teamId, userId);
    },
    async removeUser({ commit }, { teamId, userId }) {
      await teamsAPI.removeUser(teamId, userId);
    },
    async updateTeam({ commit }, team) {
      const response = await teamsAPI.update(team);
      const updatedTeam = response.data;
      commit('setTeam', updatedTeam);
    },
    async deleteTeam({ commit }, id) {
      const response = await teamsAPI.delete(id);
      const team = response.data;
      if (!team) throw Error;
    },
    async deleteTeams({ commit }, ids) {
      const response = await teamsAPI.deleteRange(ids);
      const team = response.data;
      if (!team) throw Error;
    },
    async restoreTeam({ commit }, id) {
      const response = await teamsAPI.restore(id);
      const team = response.data;
      if (!team) throw Error;
    },
    async restoreTeams({ commit }, ids) {
      const response = await teamsAPI.restoreRange(ids);
      const teams = response.data;
      if (!teams) throw Error;
    }
  },
  getters: {
    getTeams: state => state.teams,
    getTeam: state => state.team,
    getTeamUsers: state => state.teamUsers,
    getTeamProjects: state => state.teamProjects
  }
};
