import {StateSnapshot} from "./BackendDelegator";

interface Response {
    id: number;
    type: "message" | "addition" | "error" | "getConfig" | "setConfig",
}

interface MessageResponse extends Response {
    content: string | undefined | null;
}

interface AdditionResponse extends Response {
    result: number;
}

interface ErrorResponse extends Response {
    error: string;
}

export interface ConfigResponse extends Response {
    snapshot: StateSnapshot;
}

export type {ErrorResponse};
export type {AdditionResponse};
export type {MessageResponse};
export type {Response};