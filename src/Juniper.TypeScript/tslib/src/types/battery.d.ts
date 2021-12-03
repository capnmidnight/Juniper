export interface ChargingChangedEvent extends Event {
    type: "chargingchange";
    charging: boolean;
}

export interface ChargingTimeChangedEvent extends Event {
    type: "chargingtimechange";
    chargingTime: number;
}

export interface DischargingTimeChangedEvent extends Event {
    type: "dischargingtimechange";
    dischargingTime: number;
}

export interface LevelChangedEvent extends Event {
    type: "levelchange";
    level: number;
}

export interface BatteryManagerEvents {
    chargingchange: ChargingChangedEvent;
    chargingtimechange: ChargingTimeChangedEvent;
    dischargingtimechange: DischargingTimeChangedEvent;
    levelchange: LevelChangedEvent;
}

export interface BatteryManager extends EventTarget {
    charging: boolean;
    chargingTime: number;
    dischargingTime: number;
    level: number;

    onchargingchange: (evt: ChargingChangedEvent) => void;
    onchargingtimechange: (evt: ChargingTimeChangedEvent) => void;
    ondischargingtimechange: (evt: DischargingTimeChangedEvent) => void;
    onlevelchange: (evt: LevelChangedEvent) => void;
}

export interface BatteryNavigator extends Navigator {
    getBattery(): Promise<BatteryManager>;
}