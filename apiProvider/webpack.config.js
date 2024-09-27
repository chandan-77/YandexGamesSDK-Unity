const path = require('path');

const sdkConfig = {
  entry: './src/index.ts', // Entry point for sdk.js
  output: {
    filename: '__sdk.js',
    path: path.resolve(__dirname, 'dist'),
    libraryTarget: 'window', // Attach exports to the window object
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [
      {
        test: /.ts$/,
        loader: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
  mode: 'production', // or 'development' if preferred
};

const libConfig = {
  entry: './src/lib.ts',
  output: {
    filename: '__sdk.jslib',
    path: path.resolve(__dirname, 'dist'),
    library: {
      name: 'LibraryManager.library',
      type: 'assign-properties',
    },
    iife: false, // Disable IIFE wrapping
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [
      {
        test: /.ts$/,
        use: {
          loader: 'ts-loader',
          options: {
            compilerOptions: {
            },
          },
        },
        exclude: /node_modules/,
      },
    ],
  },
  mode: 'production',
};

module.exports = [sdkConfig, libConfig];