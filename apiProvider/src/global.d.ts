interface PlayerProfile {
  getUniqueID(): string;
  getName(): string;
  getPhoto(size: 'small' | 'medium' | 'large'): string;
}

interface Window {
  sdk: SDK
}

interface UnityInstance {
  SendMessage(gameObject: string, methodName: string, value: any): void;
}

declare function UTF8ToString(data:any);

declare var SendMessage: any;
declare var mergeInto: any;
declare var LibraryManager: any;

declare var AuthenticateUser: any;
declare var SavePlayerData: any;
declare var LoadPlayerData: any;

declare var createUnityInstance: any;
declare var unityInstance: UnityInstance;
declare var UnityLoader: any;