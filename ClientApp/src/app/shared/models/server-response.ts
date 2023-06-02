export interface ServerResponse {
    code: ResultCode;
    message?: string;
}

export interface ServerDataResponse<T> extends ServerResponse {
    data: T;
}

export enum ResultCode {
    Done = 1,
    Failed = 2,
}