const path = require('path');
const GenerateJSLibPlugin = require('./webpack-plugins/GenerateJSLibPlugin');

const sdkConfig = {
  entry: './src/index.ts',
  output: {
    filename: '__sdk.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'window',
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [
      {
        test: /\.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  mode: 'production',
  plugins: [
    new GenerateJSLibPlugin({
      sourceFilePath: path.resolve(__dirname, 'src/index.ts'),
      outputFilePath: path.resolve(__dirname, 'dist/__sdk.jslib'),
    }),
  ],
};

module.exports = sdkConfig;

// const libConfig = {
//   entry: './src/lib.ts',
//   output: {
//     filename: '__sdk.jslib',
//     path: path.resolve(__dirname, 'dist'),
//     library: {
//       name: 'LibraryManager.library',
//       type: 'assign-properties',
//     },
//     iife: false, // Disable IIFE wrapping
//   },
//   resolve: {
//     extensions: ['.ts', '.js'],
//   },
//   module: {
//     rules: [
//       {
//         test: /.ts$/,
//         use: {
//           loader: 'ts-loader',
//           options: {
//             compilerOptions: {
//             },
//           },
//         },
//         exclude: /node_modules/,
//       },
//     ],
//   },
//   mode: 'production',
// };

// module.exports = [sdkConfig, libConfig];