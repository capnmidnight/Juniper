export interface BaseWorkerClientOptions {
    scriptPath: string;
    version?: string;
}
export interface FullWorkerClientOptions extends BaseWorkerClientOptions {
    workers?: number | {
        workers: Worker[];
        curTaskCounter: number;
    };
}
export interface NoWorkerClientOptions {
    disableWorkers: boolean;
}
export declare type WorkerClientOptions = FullWorkerClientOptions | NoWorkerClientOptions;
