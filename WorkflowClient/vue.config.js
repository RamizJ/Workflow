const path = require('path')
const webpack = require('webpack')
const BundleAnalyzerPlugin = require('webpack-bundle-analyzer').BundleAnalyzerPlugin
const MomentLocalesPlugin = require('moment-locales-webpack-plugin')

/**
 *  @typedef { import("@vue/cli-service").ProjectOptions } Options
 *  @type { Options }
 */
module.exports = {
  publicPath: process.env.VUE_APP_BASE_URL,
  productionSourceMap: false,
  configureWebpack: {
    mode: process.env.VUE_APP_MODE !== 'development' ? 'production' : 'development',
    devtool: 'source-map',
    resolve: {
      alias: {
        '@': path.resolve(__dirname, 'src/'),
      },
    },
    plugins: [
      // new BundleAnalyzerPlugin(),
      new MomentLocalesPlugin({ localesToKeep: ['ru'] }),
      new webpack.NormalModuleReplacementPlugin(
        /element-ui[\/\\]lib[\/\\]locale[\/\\]lang[\/\\]zh-CN/,
        'element-ui/lib/locale/lang/ru-RU'
      ),
    ],
  },
  css: {
    loaderOptions: {
      sass: {
        prependData: `@import "@/core/styles/constants.scss";`,
      },
    },
  },
  devServer: {
    headers: { 'Cache-Control': 'no-store' },
  },
  chainWebpack: (config) => {
    if (process.env.NODE_ENV === 'development') {
      config.plugins.delete('preload')
    }
  },
}
