import { AuthModule } from './modules/authModule';

declare var mergeInto: any;
declare var LibraryManager: any;

const loadYandexSDK = (): Promise<void> => {
  return new Promise((resolve, reject) => {
    if (typeof window.ysdk === 'undefined') {
      // Create a script element
      const script = document.createElement('script');
      script.src = 'https://yandex.ru/games/sdk/v2';  // Yandex SDK URL
      script.async = true;

      // On successful loading of the SDK
      script.onload = () => {
        window.YaGames.init()
          .then((ysdk) => {
            window.ysdk = ysdk;
            console.log('Yandex SDK initialized successfully');
            resolve();
          })
          .catch((error) => {
            console.error('Failed to initialize Yandex SDK:', error);
            reject(error);
          });
      };

      // On script loading error
      script.onerror = () => {
        reject(new Error('Failed to load Yandex SDK script'));
      };

      // Add the script tag to the document
      document.head.appendChild(script);
    } else {
      resolve();
    }
  });
};

// Load the Yandex SDK and merge the functions into the Unity library
loadYandexSDK()
  .then(() => {
    if (typeof mergeInto !== 'undefined' && typeof LibraryManager !== 'undefined') {
      mergeInto(LibraryManager.library, {
        AuthenticateUser: AuthModule.AuthenticateUser,  // Attach the AuthModule's method after YSDK is loaded
      });
      console.log('AuthenticateUser function added to the Unity library.');
    }
  })
  .catch((error) => {
    console.error('Error loading Yandex SDK:', error);
  });
