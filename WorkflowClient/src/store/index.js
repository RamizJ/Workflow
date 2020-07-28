import Vue from 'vue';
import Vuex from 'vuex';
import modules from './modules';

Vue.use(Vuex);

export default new Vuex.Store({
  getters: {
    appVersion: () => {
      return require('../../package.json').version;
    }
  },
  modules
});
