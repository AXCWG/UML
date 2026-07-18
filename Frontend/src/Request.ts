import {StateSnapshot} from "./BackendDelegator";

interface Request {
    id: number,
    type: "message" | "addition" | "error" | "getConfig" | "setConfig" | "play",
}

export type {Request};

interface MessageRequest extends Request {
    content: string
}

interface AdditionRequest extends Request {
    a: number;
    b: number;
}

export interface GetConfigRequest extends Request {

}

export interface SetConfigRequest extends Request {
    snapshot: StateSnapshot
}

interface ErrorRequest extends Request {
    error: string;
}
export interface PlayRequest extends Request {

}

export type {ErrorRequest};
export type {AdditionRequest};
export type {MessageRequest};