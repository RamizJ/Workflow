const path = require('path');

module.exports = {
  configureWebpack: {
    resolve: {
      alias: {
        // '~': path.resolve('src')
        '~': path.resolve(__dirname, 'src/')
      }
    }
  },
  css: {
    loaderOptions: {
      sass: {
        prependData: `
          @import "~/styles/_variables.scss";
        `
      }
    }
  }
};
