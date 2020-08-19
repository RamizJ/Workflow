import projectsAPI from '@/api/projects.api';

export default {
  namespaced: true,
  state: () => ({
    projects: [],
    sidebarProjects: [],
    sidebarProjectsPage: 0,
    project: {},
    projectTeams: []
  }),
  mutations: {
    setProjects(state, projects) {
      state.projects = projects;
    },
    setSidebarProjects(state, projects) {
      state.sidebarProjects = projects;
    },
    setSidebarProjectsPage(state, page) {
      state.sidebarProjectsPage = page;
    },
    appendSidebarProjects(state, projects) {
      const isAlreadyInSidebar = state.sidebarProjects.find(sidebarProject => {
        return !!projects.find(project => project.id === sidebarProject.id);
      });
      if (isAlreadyInSidebar) return;
      if (!state.sidebarProjects.length) state.sidebarProjects = projects;
      else state.sidebarProjects = [...state.sidebarProjects, ...projects];
    },
    setProject(state, project) {
      state.project = project;
    },
    setProjectTeams(state, projectTeams) {
      state.projectTeams = projectTeams;
    }
  },
  actions: {
    async fetchSidebarProjects({ state, commit }, params) {
      let page = state.sidebarProjectsPage;

      if (
        !state.sidebarProjects.length ||
        params?.reload ||
        state.sidebarProjectsPage === 0
      ) {
        commit('setSidebarProjects', []);
        commit('setSidebarProjectsPage', 0);
        page = 0;
      }

      const response = await projectsAPI.findAll({
        pageNumber: page,
        pageSize: params?.pageSize || 20
      });
      const projects = response.data.map(project => {
        project.teamIds = [];
        return project;
      });

      const isAlreadyInSidebar = state.sidebarProjects.find(sidebarProject => {
        return !!projects.find(project => project.id === sidebarProject.id);
      });
      if (isAlreadyInSidebar) return [];

      commit('setSidebarProjectsPage', page + 1);
      commit('appendSidebarProjects', projects);
      return projects;
    },
    async findAll({ state, dispatch, commit }, params) {
      const response = await projectsAPI.findAll(params);
      const projects = response.data.map(project => {
        project.teamIds = [];
        return project;
      });
      commit('setProjects', projects);
      if (!projects.length) return projects;

      if (
        !params.filterFields?.length &&
        !params.filter &&
        params.pageNumber === 0
      ) {
        const sidebarProjects = [...projects].sort((a, b) => a.id - b.id);
        commit('setSidebarProjects', sidebarProjects);
        commit('setSidebarProjectsPage', 1);
      }

      return projects;
    },
    async findOneById({ commit, dispatch, state }, id) {
      const response = await projectsAPI.findOneById(id);
      const project = response.data;
      await dispatch('findTeams', {
        projectId: id,
        pageNumber: 0,
        pageSize: 10
      });
      project.teamIds = state.projectTeams.map(team => parseInt(team.id));
      commit('setProject', project);
    },

    async createOne({ commit }, project) {
      let newProject = {
        project: project,
        teamIds: project.teamIds
      };
      const response = await projectsAPI.createOne(newProject);
      const createdProject = response.data;
      commit('setProject', createdProject);
    },
    async updateOne({ commit }, project) {
      const teamIds = project.teamIds;
      delete project.teamIds;
      let newProject = {
        project,
        teamIds
      };
      const response = await projectsAPI.updateOne(newProject);
      return response.data;
    },
    async deleteOne({ commit }, id) {
      const response = await projectsAPI.deleteOne(id);
      const project = response.data;
      if (!project) throw Error;
    },
    async deleteMany({ commit }, ids) {
      const response = await projectsAPI.deleteMany(ids);
      const projects = response.data;
      if (!projects) throw Error;
    },
    async restoreOne({ commit }, id) {
      const response = await projectsAPI.restoreOne(id);
      const project = response.data;
      if (!project) throw Error;
    },
    async restoreMany({ commit }, ids) {
      const response = await projectsAPI.restoreMany(ids);
      const projects = response.data;
      if (!projects) throw Error;
    },
    async findTeams({ commit }, params) {
      const response = await projectsAPI.findTeams(params);
      const projectTeams = response.data.map(team => {
        team.userIds = [];
        team.projectIds = [];
        return team;
      });
      commit('setProjectTeams', projectTeams);
      return response.data;
    },
    async addTeams({ commit }, { projectId, teamIds }) {
      for (let teamId of teamIds) {
        await projectsAPI.addTeam(projectId, teamId);
      }
    },
    async addTeam({ commit }, { projectId, teamId }) {
      await projectsAPI.addTeam(teamId, projectId);
    },
    async removeTeam({ commit }, { projectId, teamId }) {
      await projectsAPI.removeTeam(teamId, projectId);
    },
    async removeTeams({ commit }, { projectId, teamIds }) {
      for (let teamId of teamIds) {
        await projectsAPI.removeTeam(teamId, projectId);
      }
    },
    async getTasksCount({ commit }, projectId) {
      const response = await projectsAPI.getTasksCount(projectId);
      return response.data;
    },
    async getTasksCountByStatus({ commit }, { projectId, status }) {
      const response = await projectsAPI.getTasksCountByStatus(
        projectId,
        status
      );
      return response.data;
    }
  },
  getters: {
    getProjects: state => state.projects,
    getSidebarProjects: state => state.sidebarProjects,
    getProject: state => state.project,
    getProjectTeams: state => state.projectTeams
  }
};
