const { merge } = require('webpack-merge');
const TerserPlugin = require('terser-webpack-plugin'); // Import TerserPlugin
const common = require('./webpack.common.js');

module.exports = merge(common, {
  mode: 'production',
  optimization: {
    minimize: true,
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          keep_fnames: true, // Preserve all function names
          keep_classnames: true, // Preserve all class names
          mangle: {
            reserved: ['mergeInto', '_mergeIntoLibrary', 'LibraryManager'], // Do not mangle these identifiers
            properties: false, // Do not mangle property (method) names
          },
          output: {
            comments: false,
          },
        },
        extractComments: false,
      }),
    ],
  },
});