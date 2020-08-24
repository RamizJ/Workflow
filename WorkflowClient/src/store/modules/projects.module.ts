import { Action, getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators';

import projectsAPI from '@/api/projects.api';
import Project from '@/types/project.type';
import Team from '@/types/team.type';
import Query from '@/types/query.type';
import { Status } from '@/types/task.type';
import store from '@/store';

@Module({
  dynamic: true,
  namespaced: true,
  name: 'projectsModule',
  store
})
class ProjectsModule extends VuexModule {
  _projects: Project[] = [];
  _project: Project | null = null;
  _projectTeams: Team[] = [];

  public get project() {
    return this._project;
  }

  public get projects() {
    return this._projects;
  }

  public get projectTeams() {
    return this._projectTeams;
  }

  @Mutation
  setProject(project: Project) {
    this._project = project;
  }

  @Mutation
  setProjects(projects: Project[]) {
    this._projects = projects;
  }

  @Mutation
  setProjectTeams(teams: Team[]) {
    this._projectTeams = teams;
  }

  @Action
  async findAll(query: Query): Promise<Project[]> {
    const response = await projectsAPI.findAll(query);
    const results = response.data as Project[];
    this.context.commit('setProjects', results);
    return results;
  }

  @Action
  async findAllByIds(ids: number[]): Promise<Project[]> {
    const response = await projectsAPI.findAllByIds(ids);
    const entities = response.data as Project[];
    this.context.commit('setProjects', entities);
    return entities;
  }

  @Action
  async findOneById(id: number): Promise<Project> {
    const response = await projectsAPI.findOneById(id);
    const result = response.data as Project;
    this.context.commit('setProject', result);
    return result;
  }

  @Action
  async createOne(entity: Project): Promise<Project> {
    const response = await projectsAPI.createOne(entity);
    const result = response.data as Project;
    this.context.commit('setProject', result);
    return result;
  }

  @Action
  async createMany(entities: Project[]): Promise<void> {
    for (const entity of entities) {
      await this.context.dispatch('createOne', entity);
    }
  }

  @Action
  async updateOne(entity: Project): Promise<void> {
    await projectsAPI.updateOne(entity);
  }

  @Action
  async updateMany(entities: Project[]): Promise<void> {
    await projectsAPI.updateMany(entities);
  }

  @Action
  async deleteOne(id: number): Promise<void> {
    await projectsAPI.deleteOne(id);
  }

  @Action
  async deleteMany(ids: number[]): Promise<void> {
    await projectsAPI.deleteMany(ids);
  }

  @Action
  async restoreOne(id: number): Promise<void> {
    await projectsAPI.restoreOne(id);
  }

  @Action
  async restoreMany(ids: number[]): Promise<void> {
    await projectsAPI.restoreMany(ids);
  }

  @Action
  async findTeams(query: Query): Promise<Team[]> {
    const response = await projectsAPI.findTeams(query);
    const results = (response.data as Team[]).map(team => {
      team.userIds = [];
      team.projectIds = [];
      return team;
    });
    this.context.commit('setProjectTeams', results);
    return results;
  }

  @Action
  async addTeam({ projectId, teamId }: { projectId: number; teamId: number }) {
    await projectsAPI.addTeam(projectId, teamId);
  }

  @Action
  async addTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await projectsAPI.addTeam(projectId, teamId);
    }
  }

  @Action
  async removeTeam({ projectId, teamId }: { projectId: number; teamId: number }) {
    await projectsAPI.removeTeam(projectId, teamId);
  }

  @Action
  async removeTeams({ projectId, teamIds }: { projectId: number; teamIds: number[] }) {
    for (const teamId of teamIds) {
      await projectsAPI.removeTeam(projectId, teamId);
    }
  }

  @Action
  async getTasksCount(projectId: number): Promise<number> {
    const response = await projectsAPI.getTasksCount(projectId);
    return parseInt(response.data) as number;
  }

  @Action
  async getTasksCountByStatus({
    projectId,
    status
  }: {
    projectId: number;
    status: Status;
  }): Promise<number> {
    const response = await projectsAPI.getTasksCountByStatus(projectId, status);
    return parseInt(response.data) as number;
  }
}

export default getModule(ProjectsModule);

// export default {
//   namespaced: true,
//   state: () => ({
//     projects: [],
//     sidebarProjects: [],
//     sidebarProjectsPage: 0,
//     project: {},
//     projectTeams: []
//   }),
//   mutations: {
//     setProjects(state, projects) {
//       state.projects = projects;
//     },
//     setSidebarProjects(state, projects) {
//       state.sidebarProjects = projects;
//     },
//     setSidebarProjectsPage(state, page) {
//       state.sidebarProjectsPage = page;
//     },
//     appendSidebarProjects(state, projects) {
//       const isAlreadyInSidebar = state.sidebarProjects.find(sidebarProject => {
//         return !!projects.find(project => project.id === sidebarProject.id);
//       });
//       if (isAlreadyInSidebar) return;
//       if (!state.sidebarProjects.length) state.sidebarProjects = projects;
//       else state.sidebarProjects = [...state.sidebarProjects, ...projects];
//     },
//     setProject(state, project) {
//       state.project = project;
//     },
//     setProjectTeams(state, projectTeams) {
//       state.projectTeams = projectTeams;
//     }
//   },
//   actions: {
//     async findOneById({ commit, dispatch, state }, id) {
//       const response = await projectsAPI.findOneById(id);
//       const project = response.data;
//       await dispatch('findTeams', {
//         projectId: id,
//         pageNumber: 0,
//         pageSize: 10
//       });
//       project.teamIds = state.projectTeams.map(team => parseInt(team.id));
//       commit('setProject', project);
//     },
//     async findAll({ state, dispatch, commit }, params) {
//       const response = await projectsAPI.findAll(params);
//       const projects = response.data.map(project => {
//         project.teamIds = [];
//         return project;
//       });
//       commit('setProjects', projects);
//       if (!projects.length) return projects;
//
//       if (
//         !params.filterFields?.length &&
//         !params.filter &&
//         params.pageNumber === 0
//       ) {
//         const sidebarProjects = [...projects].sort((a, b) => a.id - b.id);
//         commit('setSidebarProjects', sidebarProjects);
//         commit('setSidebarProjectsPage', 1);
//       }
//
//       return projects;
//     },
//     async findAllByIds({ state, dispatch, commit }, ids) {
//       const response = await projectsAPI.findAllByIds(ids);
//       const projects = response.data.map(project => {
//         project.teamIds = [];
//         return project;
//       });
//       commit('setProjects', projects);
//       if (!projects.length) return projects;
//       return projects;
//     },
//     async createOne({ commit }, project) {
//       let newProject = {
//         project: project,
//         teamIds: project.teamIds
//       };
//       const response = await projectsAPI.createOne(newProject);
//       const createdProject = response.data;
//       commit('setProject', createdProject);
//     },
//     async updateOne({ commit }, project) {
//       const teamIds = project.teamIds;
//       delete project.teamIds;
//       const convertedProject = {
//         project,
//         teamIds
//       };
//       const response = await projectsAPI.updateOne(convertedProject);
//       return response.data;
//     },
//     async updateMany({ commit }, projects) {
//       const convertedProjects = projects.map(project => {
//         const teamIds = project.teamIds;
//         delete project.teamIds;
//         return {
//           project,
//           teamIds
//         };
//       });
//       const response = await projectsAPI.updateOne(convertedProjects);
//       return response.data;
//     },
//     async deleteOne({ commit }, id) {
//       await projectsAPI.deleteOne(id);
//     },
//     async deleteMany({ commit }, ids) {
//       await projectsAPI.deleteMany(ids);
//     },
//     async restoreOne({ commit }, id) {
//       await projectsAPI.restoreOne(id);
//     },
//     async restoreMany({ commit }, ids) {
//       await projectsAPI.restoreMany(ids);
//     },
//     async findTeams({ commit }, params) {
//       const response = await projectsAPI.findTeams(params);
//       const projectTeams = response.data.map(team => {
//         team.userIds = [];
//         team.projectIds = [];
//         return team;
//       });
//       commit('setProjectTeams', projectTeams);
//       return response.data;
//     },
//     async addTeam({ commit }, { projectId, teamId }) {
//       await projectsAPI.addTeam(projectId, teamId);
//     },
//     async addTeams({ commit }, { projectId, teamIds }) {
//       for (let teamId of teamIds) {
//         await projectsAPI.addTeam(projectId, teamId);
//       }
//     },
//     async removeTeam({ commit }, { projectId, teamId }) {
//       await projectsAPI.removeTeam(projectId, teamId);
//     },
//     async removeTeams({ commit }, { projectId, teamIds }) {
//       for (let teamId of teamIds) {
//         await projectsAPI.removeTeam(projectId, teamId);
//       }
//     },
//     async getTasksCount({ commit }, projectId) {
//       const response = await projectsAPI.getTasksCount(projectId);
//       return response.data;
//     },
//     async getTasksCountByStatus({ commit }, { projectId, status }) {
//       const response = await projectsAPI.getTasksCountByStatus(
//         projectId,
//         status
//       );
//       return response.data;
//     },
//     async fetchSidebarProjects({ state, commit }, params) {
//       let page = state.sidebarProjectsPage;
//
//       if (
//         !state.sidebarProjects.length ||
//         params?.reload ||
//         state.sidebarProjectsPage === 0
//       ) {
//         commit('setSidebarProjects', []);
//         commit('setSidebarProjectsPage', 0);
//         page = 0;
//       }
//
//       const response = await projectsAPI.findAll({
//         pageNumber: page,
//         pageSize: params?.pageSize || 20
//       });
//       const projects = response.data.map(project => {
//         project.teamIds = [];
//         return project;
//       });
//
//       const isAlreadyInSidebar = state.sidebarProjects.find(sidebarProject => {
//         return !!projects.find(project => project.id === sidebarProject.id);
//       });
//       if (isAlreadyInSidebar) return [];
//
//       commit('setSidebarProjectsPage', page + 1);
//       commit('appendSidebarProjects', projects);
//       return projects;
//     }
//   },
//   getters: {
//     getProjects: state => state.projects,
//     getSidebarProjects: state => state.sidebarProjects,
//     getProject: state => state.project,
//     getProjectTeams: state => state.projectTeams
//   }
// };
