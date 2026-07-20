import Enumerable from "linq";
import {createSignal} from "solid-js";
import {AdditionRequest, AddProfileRequest, PlayRequest} from "./Request";
import {GetConfigRequest, MessageRequest, Request, SetConfigRequest} from "./Request";
import {ErrorResponse} from "./Response";
import {AdditionResponse} from "./Response";
import {MessageResponse} from "./Response";
import {ConfigResponse, Response} from "./Response";


class  BackendDelegator  {
    public static State: StateSnapshot;
    public static MessageStack : {request: Request, response: Response | null}[] = [];

    private static Request: (s: Request)=>void = (s)=>{
        (window.external as any).sendMessage(JSON.stringify(s));
    }

    private static IdGen() : number{
        const id = Math.floor(Math.random()*(100000000));
        if(Enumerable.from(this.MessageStack).any(i=>i.request.id === id || i.response?.id === id)){
            return this.IdGen();
        }
        return id;
    }
    public static MessageRequest:  (content: string )=>Promise<string> = async (s)=> {
        const id = BackendDelegator.IdGen();
        const req = {
            id: id,
            type: "message",
            content: s
        } as MessageRequest;
        BackendDelegator.Request(req);
        BackendDelegator.MessageStack.push({
            request: req, response: null
        });
        return new Promise<string>((resolve, reject)=>{
            let i = setInterval(()=>{
                console.log("[Loop] ", req.id)
                const resp = Enumerable.from(BackendDelegator.MessageStack).firstOrDefault(i=>i.request.id ===id )?.response
                if (resp?.id){
                    switch (resp.type) {
                        case "message":
                            resolve((resp as MessageResponse).content!);
                            break;
                        case "error":
                            reject((resp as ErrorResponse).error);
                    }
                    // DS
                    const idx = BackendDelegator.MessageStack.findIndex(e => e.request.id === resp.id && e.response?.id === resp.id);
                    if (idx >= 0) BackendDelegator.MessageStack.splice(idx, 1);
                    // END:DS
                    clearInterval(i);
                }
            })
        });
    };
    public static AdditionRequest: (numA: number, numB: number) =>Promise<number>  =(numA, numB) =>{
        const id = BackendDelegator.IdGen();
        const req = {
            id: id, type: "addition", a: numA, b: numB
        } as AdditionRequest
        BackendDelegator.Request(req);
        BackendDelegator.MessageStack.push({
            request: req, response: null
        });
        return new Promise<number>((resolve, reject)=>{
            let i = setInterval(()=>{
                const resp = Enumerable.from(BackendDelegator.MessageStack).firstOrDefault(i=>i.request.id ===id )?.response
                if (resp?.id){
                    switch (resp.type) {
                        case "addition":
                            resolve((resp as AdditionResponse).result);
                            break;
                        case "error":
                            reject((resp as ErrorResponse).error);
                    }
                    // DS
                    const idx = BackendDelegator.MessageStack.findIndex(e => e.request.id === resp.id && e.response?.id === resp.id);
                    if (idx >= 0) BackendDelegator.MessageStack.splice(idx, 1);
                    // END:DS
                    clearInterval(i);
                }
            })
        });
    };

    public static GetConfigRequest() {
        const id = this.IdGen();
        const req = {
            id: id, type: "getConfig"
        } as GetConfigRequest;
        return this.RequestBase(req, (res: ConfigResponse) => res);
    }
    private static RequestBase<TReq extends Request, TResponse extends Response>(req: TReq) : Promise<void>
    private static RequestBase<TReq extends Request, TResponse extends Response, TRes>(req: TReq, predicate?: ((res: TResponse)=>TRes) ) : Promise<TRes>
    private static RequestBase<TReq extends Request, TResponse extends Response, TRes>(req: TReq, predicate?: ((res: TResponse)=>TRes) ) : Promise<TRes | void>{

        BackendDelegator.Request(req);
        BackendDelegator.MessageStack.push({
            request: req, response: null
        });
        return new Promise<TRes | void>((resolve, reject)=>{
            let i = setInterval(()=>{
                const resp = Enumerable.from(BackendDelegator.MessageStack).firstOrDefault(i=>i.request.id ===req.id )?.response
                if (resp?.id){
                    if(resp.type === "error"){
                        reject(resp as ErrorResponse);
                    }else{
                        if(predicate){
                            resolve(predicate(resp as TResponse));
                        }else{
                            resolve();
                        }
                    }
                    // DS
                    const idx = BackendDelegator.MessageStack.findIndex(e => e.request.id === resp.id && e.response?.id === resp.id);
                    if (idx >= 0) BackendDelegator.MessageStack.splice(idx, 1);
                    // END:DS
                    clearInterval(i);
                }
            })
        });
    }
    public static SetConfigRequest(snapshot: StateSnapshot) {
        const id = this.IdGen();
        const req = {
            id: id, type: "setConfig", snapshot: snapshot
        } as SetConfigRequest;
        return this.RequestBase(req);
    }
    public static PlayRequest():Promise<void>{
        const id = this.IdGen();
        const req = {
            id : id, type: "play"
        } as PlayRequest
        return this.RequestBase(req);
    }
    public static AddProfileRequest(name: string):Promise<void>{
        const id = this.IdGen();
        const req = {
            id : id, type: "addProfile", name: name
        } as AddProfileRequest
        return this.RequestBase(req);
    }
}
(window.external as any).receiveMessage((m: string )=>{
    const resp = JSON.parse(m) as Response;
    console.log("[Receiver] ", resp);
    const index = BackendDelegator.MessageStack.findIndex(i=> {
        return i.request.id === resp.id
    });
    if(index >= 0){
        BackendDelegator.MessageStack[index].response = resp;
    }
})
interface StateSnapshot{
    online: boolean;
    profiles: {
        path: string,
        name: string,
        uuid: string,
        unreliableVersionList: Record<string, {
            name: string,
            jrePath: string | null | undefined,
            memMegabyte: number | null | undefined
        }>
    }[];
    selectedProfileUuid: string,
    jrePath: string,
    memMegabyte: number,
}


export type {StateSnapshot};
export {BackendDelegator};
export const [state, setState] = createSignal<StateSnapshot>((await BackendDelegator.GetConfigRequest()).snapshot)