interface PlayerProfile {
  getUniqueID(): string;
  getName(): string;
  getPhoto(size: 'small' | 'medium' | 'large'): string;
}

interface Window {
  ysdk: {
    getPlayer: () => Promise<PlayerProfile>;
  };
  isLocalHost
}

interface UnityInstance {
  SendMessage(gameObject: string, methodName: string, value: string): void;
}

declare var SendMessage: any;
declare var mergeInto: any;
declare var LibraryManager: any;
declare var AuthenticateUser: any;
declare var YaGames: any;
declare var createUnityInstance: any;
declare var unityInstance: UnityInstance;
declare var UnityLoader: any;