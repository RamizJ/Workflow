import projectsAPI from '~/api/projects.api';

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

      const response = await projectsAPI.getPage({
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
    async fetchProjects({ state, dispatch, commit }, params) {
      const response = await projectsAPI.getPage(params);
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
    async fetchProject({ commit, dispatch, state }, id) {
      const response = await projectsAPI.get(id);
      const project = response.data;
      await dispatch('fetchProjectTeams', {
        projectId: id,
        pageNumber: 0,
        pageSize: 10
      });
      project.teamIds = state.projectTeams.map(team => parseInt(team.id));
      commit('setProject', project);
    },
    async fetchProjectTeams({ commit }, params) {
      const response = await projectsAPI.getTeamsPage(params);
      const projectTeams = response.data.map(team => {
        team.userIds = [];
        team.projectIds = [];
        return team;
      });
      commit('setProjectTeams', projectTeams);
      return response.data;
    },
    async createProject({ commit }, project) {
      let newProject = {
        project: project,
        teamIds: project.teamIds
      };
      const response = await projectsAPI.create(newProject);
      const createdProject = response.data;
      commit('setProject', createdProject);
    },
    async updateProject({ commit }, project) {
      const teamIds = project.teamIds;
      delete project.teamIds;
      let newProject = {
        project,
        teamIds
      };
      const response = await projectsAPI.update(newProject);
      return response.data;
    },
    async updateProjectTeams({ commit }, { projectId, teamIds }) {
      for (let teamId in teamIds) {
        await projectsAPI.addTeam(projectId, teamId);
      }
      // const response = await projectsAPI.addTeam(projectId, teamIds);
      // const projectTeams = response.data;
    },
    async deleteProject({ commit }, id) {
      const response = await projectsAPI.delete(id);
      const project = response.data;
      if (!project) throw Error;
    },
    async deleteProjects({ commit }, ids) {
      const response = await projectsAPI.deleteRange(ids);
      const projects = response.data;
      if (!projects) throw Error;
    },
    async restoreProject({ commit }, id) {
      const response = await projectsAPI.restore(id);
      const project = response.data;
      if (!project) throw Error;
    },
    async restoreProjects({ commit }, ids) {
      const response = await projectsAPI.restoreRange(ids);
      const projects = response.data;
      if (!projects) throw Error;
    }
  },
  getters: {
    getProjects: state => state.projects,
    getSidebarProjects: state => state.sidebarProjects,
    getProject: state => state.project,
    getProjectTeams: state => state.projectTeams
  }
};
