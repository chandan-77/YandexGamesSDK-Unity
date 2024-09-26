module.exports = (env, argv) => {
  const isProduction = argv.mode === 'production';

  return {
    entry: './src/index.ts',
    output: {
      filename: 'api.jslib',
      path: path.resolve(__dirname, 'dist'),
      libraryTarget: 'var',
      library: 'ApiProvider'
    },
    resolve: {
      extensions: ['.ts', '.js'],
    },
    module: {
      rules: [
        {
          test: /\.ts$/,
          use: 'ts-loader',
          exclude: /node_modules/,
        },
      ],
    },
    mode: isProduction ? 'production' : 'development',
    optimization: {
      minimize: isProduction,
      minimizer: [
        new TerserPlugin({
          terserOptions: {
            compress: {
              drop_console: isProduction,  // Only remove console logs in production
            },
          },
        }),
      ],
    },
    devtool: isProduction ? false : 'source-map',  // Source maps only in development
  };
};
