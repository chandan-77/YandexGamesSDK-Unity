const fs = require('fs');
const path = require('path');
const ts = require('typescript');

class GenerateJSLibPlugin {
  constructor(options) {
    this.sourceFilePath = options.sourceFilePath || './src/index.ts';
    this.outputFilePath = options.outputFilePath || './dist/__sdk.jslib';
  }

  apply(compiler) {
    compiler.hooks.done.tap('GenerateJSLibPlugin', () => {
      // Extract methods from TypeScript file
      const methods = this.extractYandexSDKExports(this.sourceFilePath);

      // Generate jslib file content
      if (methods.length > 0) {
        this.generateJSLibFile(methods);
      } else {
        console.warn('No methods were extracted for YandexSDKExports. __sdk.jslib will be empty.');
      }
    });
  }

  extractYandexSDKExports(filePath) {
    console.log(`Extracting methods from: ${filePath}`);
    const source = fs.readFileSync(filePath, 'utf8');
    const sourceFile = ts.createSourceFile(filePath, source, ts.ScriptTarget.Latest, true);
    const methods = [];
    const imports = new Map();

    const visit = (node) => {
      // Collect imported modules and their names
      if (ts.isImportDeclaration(node) && node.importClause) {
        const moduleName = node.moduleSpecifier.getText().replace(/['"]/g, '');
        const bindings = node.importClause.namedBindings;
        if (bindings && ts.isNamedImports(bindings)) {
          bindings.elements.forEach((element) => {
            console.log(`Found import: ${element.name.getText()} from ${moduleName}`);
            imports.set(element.name.getText(), moduleName);
          });
        }
      }

      // Check if this is an assignment to `window.YandexSDKExports`
      if (
        ts.isExpressionStatement(node) &&
        ts.isBinaryExpression(node.expression) &&
        node.expression.operatorToken.kind === ts.SyntaxKind.EqualsToken
      ) {
        const left = node.expression.left;
        const right = node.expression.right;

        // Check if left-hand side is window.YandexSDKExports
        if (
          ts.isPropertyAccessExpression(left) &&
          left.expression.getText() === 'window' &&
          left.name.getText() === 'YandexSDKExports' &&
          ts.isObjectLiteralExpression(right)
        ) {
          console.log('Found window.YandexSDKExports assignment.');

          right.properties.forEach((property) => {
            if (ts.isPropertyAssignment(property) && ts.isIdentifier(property.name)) {
              const methodName = property.name.getText();
              if (/^[A-Z]/.test(methodName)) {
                console.log(`Found YandexSDKExports method: ${methodName}`);

                if (ts.isFunctionExpression(property.initializer) || ts.isArrowFunction(property.initializer)) {
                  const parameters = property.initializer.parameters.map((param) =>
                    param.name.getText()
                  );
                  methods.push({ name: methodName, parameters });
                  console.log(`Extracted inline method: ${methodName} with parameters: ${parameters}`);
                } else if (ts.isIdentifier(property.initializer)) {
                  // Handle imported functions
                  const importedModuleName = imports.get(property.initializer.getText());
                  if (importedModuleName) {
                    const importedFilePath = this.resolveImportPath(filePath, importedModuleName);
                    if (importedFilePath) {
                      console.log(`Attempting to extract parameters for imported method: ${methodName} from ${importedFilePath}`);
                      const importedParameters = this.getFunctionParameters(importedFilePath, property.initializer.getText());
                      methods.push({ name: methodName, parameters: importedParameters });
                      console.log(`Extracted imported method: ${methodName} with parameters: ${importedParameters}`);
                    } else {
                      console.warn(`Could not resolve path for import: ${importedModuleName}`);
                    }
                  } else {
                    console.warn(`No import found for method: ${methodName}`);
                  }
                } else if (ts.isPropertyAccessExpression(property.initializer)) {
                  // Handle property access expressions (new handling)
                  const objectName = property.initializer.expression.getText();
                  const functionName = property.initializer.name.getText();
                  const importedModuleName = imports.get(objectName);

                  if (importedModuleName) {
                    const importedFilePath = this.resolveImportPath(filePath, importedModuleName);
                    if (importedFilePath) {
                      console.log(`Attempting to extract parameters for method: ${functionName} from ${importedFilePath}`);
                      const parameters = this.getClassMethodParameters(importedFilePath, objectName, functionName);
                      methods.push({ name: methodName, parameters });
                      console.log(`Extracted method: ${methodName} with parameters: ${parameters}`);
                    } else {
                      console.warn(`Could not resolve path for import: ${importedModuleName}`);
                    }
                  } else {
                    console.warn(`No import found for object: ${objectName}`);
                  }
                }
              }
            }
          });
        }
      }
      ts.forEachChild(node, visit);
    };

    visit(sourceFile);
    return methods;
  }

  resolveImportPath(currentFilePath, moduleName) {
    // Try resolving the imported module path with different extensions and as a directory
    const possibleExtensions = ['.ts', '.js'];
    const possiblePaths = possibleExtensions.flatMap((ext) => [
      path.resolve(path.dirname(currentFilePath), moduleName + ext),
      path.resolve(path.dirname(currentFilePath), moduleName, 'index' + ext),
    ]);

    for (const possiblePath of possiblePaths) {
      if (fs.existsSync(possiblePath)) {
        return possiblePath;
      }
    }

    console.error(`Could not resolve import path for module: ${moduleName}`);
    return null;
  }

  getFunctionParameters(filePath, functionName) {
    if (!filePath || !fs.existsSync(filePath)) {
      console.error(`File not found: ${filePath}`);
      return [];
    }

    console.log(`Getting parameters for function: ${functionName} from file: ${filePath}`);
    const source = fs.readFileSync(filePath, 'utf8');
    const sourceFile = ts.createSourceFile(filePath, source, ts.ScriptTarget.Latest, true);
    let parameters = [];

    const findFunction = (node) => {
      if (ts.isFunctionDeclaration(node) && node.name && node.name.getText() === functionName) {
        parameters = node.parameters.map((param) => param.name.getText());
        console.log(`Found function declaration for ${functionName} with parameters: ${parameters}`);
      } else if (ts.isVariableStatement(node)) {
        node.declarationList.declarations.forEach((declaration) => {
          if (ts.isVariableDeclaration(declaration) && declaration.name.getText() === functionName) {
            if (declaration.initializer &&
              (ts.isFunctionExpression(declaration.initializer) || ts.isArrowFunction(declaration.initializer))
            ) {
              parameters = declaration.initializer.parameters.map((param) => param.name.getText());
              console.log(`Found variable function ${functionName} with parameters: ${parameters}`);
            }
          }
        });
      } else if (ts.isClassDeclaration(node)) {
        if (node.members) {
          node.members.forEach((member) => {
            if (ts.isMethodDeclaration(member) && member.name && member.name.getText() === functionName) {
              parameters = member.parameters.map((param) => param.name.getText());
              console.log(`Found class method ${functionName} with parameters: ${parameters}`);
            }
          });
        }
      } else if (ts.isExportAssignment(node) && node.expression) {
        if (ts.isIdentifier(node.expression) && node.expression.getText() === functionName) {
          parameters = this.getFunctionParameters(filePath, node.expression.getText());
        }
      }
      ts.forEachChild(node, findFunction);
    };

    findFunction(sourceFile);
    return parameters;
  }

  getClassMethodParameters(filePath, className, methodName) {
    if (!filePath || !fs.existsSync(filePath)) {
      console.error(`File not found: ${filePath}`);
      return [];
    }

    console.log(`Getting parameters for method: ${methodName} of class: ${className} from file: ${filePath}`);
    const source = fs.readFileSync(filePath, 'utf8');
    const sourceFile = ts.createSourceFile(filePath, source, ts.ScriptTarget.Latest, true);
    let parameters = [];

    const findClassMethod = (node) => {
      if (ts.isClassDeclaration(node) && node.name && node.name.getText() === className) {
        node.members.forEach((member) => {
          if (ts.isMethodDeclaration(member) && member.name.getText() === methodName) {
            parameters = member.parameters.map((param) => param.name.getText());
            console.log(`Found method ${methodName} in class ${className} with parameters: ${parameters}`);
          }
        });
      }
      ts.forEachChild(node, findClassMethod);
    };

    findClassMethod(sourceFile);
    return parameters;
  }

  generateJSLibFile(methods) {
    let jslibContent = 'mergeInto(LibraryManager.library, {\n';

    methods.forEach(({ name, parameters }) => {
        const paramList = parameters.length > 0 ? parameters.map((param) => `${param}Ptr`).join(', ') : '';
        const args = parameters.length > 0 ? parameters.map((param) => `UTF8ToString(${param}Ptr)`).join(', ') : '';

        jslibContent += `
        ${name}: function(${paramList}) {
          if (typeof window.YandexSDKExports.${name} === 'function') {
            ${parameters.length > 0 ? `var ${parameters.map((param) => `${param} = UTF8ToString(${param}Ptr)`).join(', ')};` : ''}
            window.YandexSDKExports.${name}(${parameters.join(', ')});
          } else {
            console.error('${name} is not defined on window.YandexSDKExports.');
          }
        },\n`;
    });

    jslibContent += '});';

    // Write the generated content to __sdk.jslib
    fs.writeFileSync(this.outputFilePath, jslibContent);
    console.log('__sdk.jslib has been generated successfully.');
}

}

module.exports = GenerateJSLibPlugin;
