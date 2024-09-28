interface UnityInstance {
  SendMessage(gameObject: string, methodName: string, value: any): void;
}

declare var createUnityInstance: any;
declare var unityInstance: UnityInstance;
declare var YandexSDKExports: any;

interface ISDKExports {
  [methodName: string]: (...args: any[]) => any;
}

interface Window {
  yandexSDK: YandexSDK;
  SDKExports: ISDKExports;
  unityInstance: UnityInstance;
}

declare function SendMessage(gameObject: string, methodName: string, parameter?: any): void;
declare function UTF8ToString(ptr: number): string;

// Declare global variables
declare var mergeInto: (target: any, source: any) => void;
declare var LibraryManager: {
  library: any;
};
