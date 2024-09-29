export class Utils {
    /**
     * Checks if the current environment is localhost.
     * @returns {boolean} - Returns true if the environment is localhost, otherwise false.
     */
    static isLocalhost(): boolean {
        return ['localhost', '127.0.0.1', '::1'].includes(window.location.hostname);
    }
}
