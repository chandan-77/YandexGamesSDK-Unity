const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const GenerateJSLibPlugin = require('./webpack-plugins/GenerateJSLibPlugin');
const { DefinePlugin } = require('webpack');

const sdkVersion = require('./package.json').version || '1.0.0';

module.exports = {
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
  plugins: [
    new HtmlWebpackPlugin({
      template: './public/index.html',
      filename: 'index.html',
      inject: 'body',
      minify: false,
      templateParameters: {
        sdkVersion: sdkVersion,
      }
    }),
    new DefinePlugin({
      YANDEX_SDK_VERSION: JSON.stringify(sdkVersion),
    }),
    new GenerateJSLibPlugin({
      sourceFilePath: path.resolve(__dirname, 'src/index.ts'),
      outputFilePath: path.resolve(__dirname, 'dist/__sdk.jslib'),  
    }),
  ],
  mode: 'production',  
};
