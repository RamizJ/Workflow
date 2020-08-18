import teamsAPI from '@/api/teams.api';

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
      const response = await teamsAPI.findAll(params);
      const teams = response.data.map(team => {
        team.userIds = [];
        team.projectIds = [];
        return team;
      });
      commit('setTeams', teams);
      return teams;
    },
    async fetchTeam({ commit }, id) {
      const response = await teamsAPI.findOneById(id);
      const team = response.data;
      team.userIds = [];
      team.projectIds = [];
      commit('setTeam', team);
    },
    async createTeam({ commit }, team) {
      let newTeam = {
        team: team,
        userIds: team.userIds,
        projectIds: team.projectIds
      };
      const response = await teamsAPI.createOne(newTeam);
      const createdTeam = response.data;
      commit('setTeam', createdTeam);
    },
    async updateTeam({ commit }, team) {
      const response = await teamsAPI.updateOne(team);
      const updatedTeam = response.data;
      commit('setTeam', updatedTeam);
    },
    async deleteTeam({ commit }, id) {
      const response = await teamsAPI.deleteOne(id);
      const team = response.data;
      if (!team) throw Error;
    },
    async deleteTeams({ commit }, ids) {
      const response = await teamsAPI.deleteMany(ids);
      const team = response.data;
      if (!team) throw Error;
    },
    async restoreTeam({ commit }, id) {
      const response = await teamsAPI.restoreOne(id);
      const team = response.data;
      if (!team) throw Error;
    },
    async restoreTeams({ commit }, ids) {
      const response = await teamsAPI.restoreMany(ids);
      const teams = response.data;
      if (!teams) throw Error;
    },
    async addUser({ commit }, { teamId, userId }) {
      await teamsAPI.addUser(teamId, userId);
    },
    async removeUser({ commit }, { teamId, userId }) {
      await teamsAPI.removeUser(teamId, userId);
    },
    async fetchTeamUsers({ commit }, params) {
      const response = await teamsAPI.findUsers(params);
      const teamUsers = response.data;
      commit('setTeamUsers', teamUsers);
      return teamUsers;
    },
    async fetchTeamProjects({ commit }, params) {
      const response = await teamsAPI.findProjects(params);
      const teamProjects = response.data;
      commit('setTeamProjects', teamProjects);
      return teamProjects;
    }
  },
  getters: {
    getTeams: state => state.teams,
    getTeam: state => state.team,
    getTeamUsers: state => state.teamUsers,
    getTeamProjects: state => state.teamProjects
  }
};
