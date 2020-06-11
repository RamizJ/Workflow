const path = require('path');

module.exports = {
  publicPath: process.env.VUE_APP_BASE_URL,
  configureWebpack: {
    mode: process.env.VUE_APP_MODE !== 'development' ? 'production' : 'development',
    resolve: {
      alias: {
        '~': path.resolve(__dirname, 'src/')
      }
    }
  },
  css: {
    loaderOptions: {
      sass: {
        prependData: `@import "~/styles/_variables.scss";`
      }
    }
  }
};
