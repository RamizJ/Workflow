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
    async findAll({ commit }, params) {
      const response = await teamsAPI.findAll(params);
      const teams = response.data.map(team => {
        team.userIds = [];
        team.projectIds = [];
        return team;
      });
      commit('setTeams', teams);
      return teams;
    },
    async findOneById({ commit }, id) {
      const response = await teamsAPI.findOneById(id);
      const team = response.data;
      team.userIds = [];
      team.projectIds = [];
      commit('setTeam', team);
      return team;
    },
    async createOne({ commit }, team) {
      let newTeam = {
        team: team,
        userIds: team.userIds,
        projectIds: team.projectIds
      };
      const response = await teamsAPI.createOne(newTeam);
      commit('setTeam', response.data);
      return response.data;
    },
    async updateOne({ commit }, team) {
      const response = await teamsAPI.updateOne(team);
      commit('setTeam', response.data);
      return response.data;
    },
    async deleteOne({ commit }, id) {
      await teamsAPI.deleteOne(id);
    },
    async deleteMany({ commit }, ids) {
      await teamsAPI.deleteMany(ids);
    },
    async restoreOne({ commit }, id) {
      await teamsAPI.restoreOne(id);
    },
    async restoreMany({ commit }, ids) {
      await teamsAPI.restoreMany(ids);
    },
    async addUser({ commit }, { teamId, userId }) {
      await teamsAPI.addUser(teamId, userId);
    },
    async removeUser({ commit }, { teamId, userId }) {
      await teamsAPI.removeUser(teamId, userId);
    },
    async findUsers({ commit }, params) {
      const response = await teamsAPI.findUsers(params);
      commit('setTeamUsers', response.data);
      return response.data;
    },
    async findProjects({ commit }, params) {
      const response = await teamsAPI.findProjects(params);
      commit('setTeamProjects', response.data);
      return response.data;
    }
  },
  getters: {
    getTeams: state => state.teams,
    getTeam: state => state.team,
    getTeamUsers: state => state.teamUsers,
    getTeamProjects: state => state.teamProjects
  }
};
