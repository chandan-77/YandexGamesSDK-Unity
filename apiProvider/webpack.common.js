const path = require('path');

module.exports = {
  entry: './src/index.ts',
  output: {
    filename: 'sdk.jslib',
    path: path.resolve(__dirname, 'dist'),
    library: {
      name: 'l',
      type: 'assign-properties',
    },
  },
  resolve: {
    extensions: ['.ts', '.js'],
  },
  module: {
    rules: [
      {
        test: /.ts$/,
        enforce: 'pre', // Run before other loaders
        exclude: /node_modules/,
        use: [
          
        ],
      },
      {
        test: /.ts$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
    ],
  },
};