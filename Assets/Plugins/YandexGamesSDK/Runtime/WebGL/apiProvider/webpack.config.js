const path = require('path');
const TerserPlugin = require('terser-webpack-plugin');

module.exports = {
  entry: './src/index.ts',  // Entry point for your TypeScript files
  output: {
    filename: 'sdk.jslib',  // The output file name, targeting Unity WebGL's format
    path: path.resolve(__dirname, '../'),  // Output directory
    libraryTarget: 'var',  // 'var' is necessary for Unity WebGL JSLib compatibility
    library: 'ApiProvider',  // Global variable for Unity WebGL to access
  },
  resolve: {
    extensions: ['.ts', '.js'],  // Resolve both TypeScript and JavaScript files
  },
  module: {
    rules: [
      {
        test: /\.ts$/,  // Target TypeScript files for compilation
        use: 'ts-loader',
        exclude: /node_modules/,  // Exclude node_modules for faster builds
      },
    ],
  },
  mode: 'production',  // Production mode optimizes the output
  optimization: {
    minimize: true,  // Enable code minimization for smaller file sizes
    minimizer: [
      new TerserPlugin({
        terserOptions: {
          compress: {
            drop_console: true,  // Remove console logs to reduce file size
            passes: 2,  // Multiple passes for better compression
          },
          output: {
            comments: false,  // Remove comments from the final output
          },
        },
        extractComments: false,  // Don't generate separate license files
      }),
    ],
  },
  devtool: false,  // Disable source maps for production builds for smaller output
};
