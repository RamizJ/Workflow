const path = require('path');
const webpack = require('webpack');

/**
 *  @typedef { import("@vue/cli-service").ProjectOptions } Options
 *  @type { Options }
 */
module.exports = {
  publicPath: process.env.VUE_APP_BASE_URL,
  configureWebpack: {
    mode:
      process.env.VUE_APP_MODE !== 'development' ? 'production' : 'development',
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src/')
      }
    },
    plugins: [
      new webpack.NormalModuleReplacementPlugin(
        /element-ui[\/\\]lib[\/\\]locale[\/\\]lang[\/\\]zh-CN/,
        'element-ui/lib/locale/lang/ru-RU'
      )
    ]
  },
  css: {
    loaderOptions: {
      sass: {
        prependData: `@import "@/styles/_variables.scss";`
      }
    }
  },
  devServer: {
    headers: { 'Cache-Control': 'no-store' }
  },
  chainWebpack: config => {
    if (process.env.NODE_ENV === 'development') {
      config.plugins.delete('preload');
    }
  }
};
