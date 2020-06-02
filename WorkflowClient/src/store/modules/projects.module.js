import projectsAPI from '~/api/projects';

export default {
  namespaced: true,
  state: () => ({
    projects: [],
    project: {}
  }),
  mutations: {
    setProjects(state, projects) {
      state.projects = projects;
    },
    setProject(state, project) {
      state.project = project;
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
