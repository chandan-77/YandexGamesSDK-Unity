---
icon: globe-pointer
---

# Setting Up Local Development Server

The local development server enables testing WebGL builds and SDK features in a simulated Yandex environment directly from Unity. Follow these steps to configure and troubleshoot your local server setup.

**1. Access the Local Server Settings**

* Open the **Yandex Games SDK Dashboard** within Unity.

**2. Specify Build Path and Server Port**

* **WebGL Build Path**: Set the path to your WebGL build output.
* **Server Port**: Enter your preferred port for the server (e.g., `8080`).

**3. Start the Local Server**

* Click **"Start Server"** to launch the local development environment.
* This server simulates Yandex Games interactions, allowing you to test SDK features without needing a live connection.

**4. Certificate Setup for Secure Connection**

The SDK generates certificates to secure the local server connection. These certificates are saved in a specific directory on your system:

* **Certificate Path**: `~/.yandex_sdk/yandex_sdk_cert.pem`
* **Key Path**: `~/.yandex_sdk/yandex_sdk_key.pem`

These files are stored in a hidden `.yandex_sdk` folder within the user’s home directory. The SDK creates the directory automatically if it does not already exist.

**5. Resolving Connection and Certificate Errors**

When accessing the local development server, you may encounter errors like **“Can't establish a connection”** or **“Cannot establish a secure connection.”** These errors are due to the browser not trusting the local server’s self-signed certificate. Here’s how to fix it:

**Option A: Bypass the Certificate Warning in Browser (Temporary)**

1. **Open the Error URL**: Click on the URL displayed in the error message (e.g., `https://localhost:8080/`) to open it in a new browser tab.
2. **Continue Anyway**: When you see the warning page (e.g., “Your connection is not private”), click **“Advanced”** and then **“Proceed to localhost (unsafe)”**. This allows your browser to **temporarily** accept the connection.

**Option B: Add the Certificate to Your OS Trust Store (Recommended)**

Add the certificate to your operating system’s trust store for a permanent solution. Use the following commands based on your OS:

*   **macOS**

    * Open Terminal and run this command to add the certificate to the macOS trust store:

    ```
    security add-trusted-cert -p basic -p ssl -k ~/Library/Keychains/login.keychain-db ~/.yandex_sdk/yandex_sdk_cert.pem
    ```


*   **Windows**

    * Open Command Prompt as Administrator and run this command to add the certificate to the Windows trust store:

    ```
    certutil -addstore Root %HOMEPATH%\.yandex_sdk\yandex_sdk_cert.pem
    ```

> **Note**: Adding your certificate to the OS trust store allows the browser to recognize the server as secure. This step ensures you won’t encounter connection warnings each time you test.

**6. Restart the Browser and Test**

After adding the certificate to your OS trust store, restart your browser and access the local server again. This should establish a secure connection without any security warnings.
