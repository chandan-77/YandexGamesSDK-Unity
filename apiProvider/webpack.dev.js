const { merge } = require('webpack-merge');
const webpack = require('webpack'); // Import webpack to use plugins
const common = require('./webpack.common.js');

module.exports = merge(common, {
  mode: 'development',
  devtool: 'inline-source-map', // Detailed source maps for debugging
  optimization: {
    minimize: false, // Do not minimize code
  },
  watch: true, // Enable watch mode (optional)
  plugins: [
    new webpack.DefinePlugin({
      'process.env.NODE_ENV': JSON.stringify('development'),
    }),
  ],
});