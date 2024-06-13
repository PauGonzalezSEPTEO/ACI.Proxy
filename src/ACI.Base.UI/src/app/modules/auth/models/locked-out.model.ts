export class LockedOutModel {
    accessFailedCount: number;
    isLockedOut: boolean;
  
    setLockedOut(_lockedOut: unknown) {
      const lockedOut = _lockedOut as LockedOutModel;
      this.accessFailedCount = lockedOut.accessFailedCount;
      this.isLockedOut = lockedOut.isLockedOut;
    }
  }
  