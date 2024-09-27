interface PlayerProfile {
  getUniqueID(): string;
  getName(): string;
  getPhoto(size: 'small' | 'medium' | 'large'): string;
}

interface Window {
  ysdk: {
    getPlayer: () => Promise<PlayerProfile>;
  };
}

declare var SendMessage: any;
declare var mergeInto: any;
declare var LibraryManager: any;
declare var SendMessage: any;