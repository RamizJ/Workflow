import projectsAPI from '~/api/projects.api';

export default {
  namespaced: true,
  state: () => ({
    projects: [],
    project: {},
    projectTeams: []
  }),
  mutations: {
    setProjects(state, projects) {
      state.projects = projects;
    },
    setProject(state, project) {
      state.project = project;
    },
    setProjectTeams(state, projectTeams) {
      state.projectTeams = projectTeams;
    }
  },
  actions: {
    async fetchProjects({ commit }, params) {
      const response = await projectsAPI.getPage(params);
      const projects = response.data;
      commit('setProjects', projects);
    },
    async fetchProject({ commit }, id) {
      const response = await projectsAPI.get(id);
      const project = response.data;
      commit('setProject', project);
    },
    async fetchProjectTeams({ commit }, params) {
      const response = await projectsAPI.getTeamsPage(params);
      const projectTeams = response.data;
      commit('setProjectTeams', projectTeams);
    },
    async createProject({ commit }, payload) {
      const response = await projectsAPI.create(payload);
      const project = response.data;
      commit('setProject', project);
    },
    async updateProject({ commit }, payload) {
      const response = await projectsAPI.update(payload);
      const project = response.data;
      commit('setProject', project);
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
    }
  },
  getters: {
    getProjects: state => state.projects,
    getProject: state => state.project
  }
};
