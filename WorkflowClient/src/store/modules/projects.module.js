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
      state.sidebarProjectsPage = params?.reload
        ? 0
        : state.sidebarProjectsPage;
      const page = state.sidebarProjectsPage;
      if (page === 0) commit('setSidebarProjects', []);
      commit('setSidebarProjectsPage', page + 1);
      console.log(state.sidebarProjectsPage);
      const response = await projectsAPI.getPage({
        pageNumber: page,
        pageSize: params?.pageSize || 10
      });
      const projects = response.data;
      commit('appendSidebarProjects', projects);
      return projects;
    },
    async fetchProjects({ commit }, params) {
      const response = await projectsAPI.getPage(params);
      const projects = response.data;
      commit('setProjects', projects);
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
      const projectTeams = response.data;
      commit('setProjectTeams', projectTeams);
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
      let newProject = {
        project: project,
        teamIds: project.teamIds
      };
      const response = await projectsAPI.update(newProject);
      const updatedProject = response.data;
      commit('setProject', updatedProject);
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
    }
  },
  getters: {
    getProjects: state => state.projects,
    getSidebarProjects: state => state.sidebarProjects,
    getProject: state => state.project,
    getProjectTeams: state => state.projectTeams
  }
};
