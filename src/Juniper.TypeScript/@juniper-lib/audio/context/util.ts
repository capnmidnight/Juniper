export type AudioConnectionWithOutputChannel = [AudioNode | AudioParam, number];
export type AudioConnectionWithInputOutputChannels = [AudioNode, number, number];

export type AudioConnection = AudioNode |
    AudioParam |
    AudioConnectionWithOutputChannel |
    AudioConnectionWithInputOutputChannels;
