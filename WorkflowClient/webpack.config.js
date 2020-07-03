module.exports = {
  devtool: 'source-map',
  mode:
    process.env.VUE_APP_MODE !== 'development' ? 'production' : 'development',
  resolve: {
    alias: {
      '~': require('path').resolve(__dirname, 'src')
    }
  }
};
