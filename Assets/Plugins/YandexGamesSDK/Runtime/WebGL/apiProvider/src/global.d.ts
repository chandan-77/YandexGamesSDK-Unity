declare interface Window {
  YaGames: {
    init(): Promise<YandexSDK>;
  };
  ysdk: {
    getPlayer(): Promise<any>;
    adv: {
      showFullscreenAdv(callbacks: any): void;
    };
  };
  SendMessage(gameObject: string, methodName: string, message: string): void;
}
