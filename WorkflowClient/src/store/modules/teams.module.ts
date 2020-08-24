import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators';

import teamsAPI from '@/api/teams.api';
import Team from '@/types/team.type';
import Query from '@/types/query.type';
import store from '@/store';

@Module({
  dynamic: true,
  namespaced: true,
  name: 'teamsModule',
  store
})
class TeamsModule extends VuexModule {
  _teams: Team[] = [];
  _team: Team | null = null;

  public get team() {
    return this._team;
  }
  public get teams() {
    return this._teams;
  }

  @Mutation
  setTeam(team: Team) {
    this._team = team;
  }

  @Mutation
  setTeams(teams: Team[]) {
    this._teams = teams;
  }

  @Action
  async findAll(query: Query): Promise<Team[]> {
    const response = await teamsAPI.findAll(query);
    const results = response.data as Team[];
    this.context.commit('setTeams', results);
    return results;
  }

  @Action
  async findOneById(id: number): Promise<Team> {
    const response = await teamsAPI.findOneById(id);
    const result = response.data as Team;
    this.context.commit('setTeam', result);
    return result;
  }
}

export default getModule(TeamsModule);

// export default {
//   namespaced: true,
//   state: () => ({
//     teams: [],
//     team: {},
//     teamUsers: [],
//     teamProjects: []
//   }),
//   mutations: {
//     setTeams(state, teams) {
//       state.teams = teams;
//     },
//     setTeam(state, team) {
//       state.team = team;
//     },
//     setTeamUsers(state, teamUsers) {
//       state.teamUsers = teamUsers;
//     },
//     setTeamProjects(state, teamProjects) {
//       state.teamProjects = teamProjects;
//     }
//   },
//   actions: {
//     async findOneById({ commit }, id) {
//       const response = await teamsAPI.findOneById(id);
//       const team = response.data;
//       team.userIds = [];
//       team.projectIds = [];
//       commit('setTeam', team);
//       return team;
//     },
//     async findAll({ commit }, params) {
//       const response = await teamsAPI.findAll(params);
//       const teams = response.data.map(team => {
//         team.userIds = [];
//         team.projectIds = [];
//         return team;
//       });
//       commit('setTeams', teams);
//       return teams;
//     },
//     async findAllByIds({ commit }, ids) {
//       const response = await teamsAPI.findAllByIds(ids);
//       const teams = response.data.map(team => {
//         team.userIds = [];
//         team.projectIds = [];
//         return team;
//       });
//       commit('setTeams', teams);
//       return teams;
//     },
//     async createOne({ commit }, team) {
//       const response = await teamsAPI.createOne({
//         team,
//         userIds: team.userIds,
//         projectIds: team.projectIds
//       });
//       commit('setTeam', response.data);
//       return response.data;
//     },
//     async updateOne({ commit }, team) {
//       const response = await teamsAPI.updateOne({
//         team,
//         projectIds: team.projectIds,
//         userIds: team.userIds
//       });
//       commit('setTeam', response.data);
//       return response.data;
//     },
//     async updateMany({ commit }, teams) {
//       const response = await teamsAPI.updateMany(
//         teams.map(team => {
//           return {
//             team,
//             projectIds: team.projectIds,
//             userIds: team.userIds
//           };
//         })
//       );
//       commit('setTeams', response.data);
//       return response.data;
//     },
//     async deleteOne({ commit }, id) {
//       await teamsAPI.deleteOne(id);
//     },
//     async deleteMany({ commit }, ids) {
//       await teamsAPI.deleteMany(ids);
//     },
//     async restoreOne({ commit }, id) {
//       await teamsAPI.restoreOne(id);
//     },
//     async restoreMany({ commit }, ids) {
//       await teamsAPI.restoreMany(ids);
//     },
//     async findUsers({ commit }, params) {
//       const response = await teamsAPI.findUsers(params);
//       commit('setTeamUsers', response.data);
//       return response.data;
//     },
//     async addUser({ commit }, { teamId, userId }) {
//       await teamsAPI.addUser(teamId, userId);
//     },
//     async addUsers({ commit }, { teamId, userIds }) {
//       await teamsAPI.addUser(teamId, userIds);
//     },
//     async removeUser({ commit }, { teamId, userId }) {
//       await teamsAPI.removeUser(teamId, userId);
//     },
//     async findProjects({ commit }, params) {
//       const response = await teamsAPI.findProjects(params);
//       commit('setTeamProjects', response.data);
//       return response.data;
//     },
//     async addProject({ commit }, { teamId, projectId }) {
//       await teamsAPI.addProject(teamId, projectId);
//     },
//     async removeProject({ commit }, { teamId, projectId }) {
//       await teamsAPI.removeProject(teamId, projectId);
//     }
//   },
//   getters: {
//     getTeams: state => state.teams,
//     getTeam: state => state.team,
//     getTeamUsers: state => state.teamUsers,
//     getTeamProjects: state => state.teamProjects
//   }
// };
