import { BaseListener } from "./BaseListener";


export abstract class BaseWebAudioListener extends BaseListener {
    protected get listener(): AudioListener {
        return this.context.listener;
    }
}
