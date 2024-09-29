const fs = require('fs');
const path = require('path');

const distDir = path.resolve(__dirname, 'dist');
const unityToolsDir = path.resolve(__dirname, '../Assets/Tools/sdk-dev-proxy');

// Ensure the target directory exists
if (!fs.existsSync(unityToolsDir)) {
  fs.mkdirSync(unityToolsDir, { recursive: true });
}

// Map of pkg target names to output executable names
const executables = [
  { target: 'node16-win-x64', filename: 'sdk-dev-proxy.exe', platform: 'win' },
  { target: 'node16-macos-x64', filename: 'sdk-dev-proxy-macos', platform: 'mac' },
  { target: 'node16-linux-x64', filename: 'sdk-dev-proxy-linux', platform: 'linux' },
];

// Copy each executable to the Unity project
executables.forEach(exec => {
  const sourcePath = path.join(distDir, `sdk-dev-proxy-${exec.target}`);
  const destPath = path.join(unityToolsDir, exec.filename);

  if (fs.existsSync(sourcePath)) {
    fs.copyFileSync(sourcePath, destPath);
    console.log(`Copied ${sourcePath} to ${destPath}`);

    // Set executable permissions on macOS and Linux
    if (exec.platform !== 'win') {
      fs.chmodSync(destPath, 0o755);
      console.log(`Set executable permissions on ${destPath}`);
    }
  } else {
    console.error(`Executable not found: ${sourcePath}`);
  }
});
