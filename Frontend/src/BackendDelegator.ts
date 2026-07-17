import Enumerable from "linq";
import IEnumerable = Enumerable.IEnumerable;


class  BackendDelegator  {

    public static MessageStack : {request: Request, response: Response | null}[] = [];

    private static Request: (s: Request)=>void = (s)=>{
        (window.external as any).sendMessage(JSON.stringify(s));
    }

    private static IdGen() : number{
        const id = Math.floor(Math.random()*(10^5));
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
    public static GetConfigRequest: ()=>Promise<ConfigResponse> = ()=>{
        const id = BackendDelegator.IdGen();
        const req = {
            id: id, type: "getConfig"
        } as GetConfigRequest;
        BackendDelegator.Request(req);
        BackendDelegator.MessageStack.push({
            request: req, response: null
        });
        return new Promise<ConfigResponse>((resolve, reject)=>{
            let i = setInterval(()=>{
                const resp = Enumerable.from(BackendDelegator.MessageStack).firstOrDefault(i=>i.request.id ===id )?.response
                if (resp?.id){
                    switch (resp.type) {
                        case "getConfig":
                            resolve((resp as ConfigResponse));
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
    }
}
(window.external as any).receiveMessage((m: string )=>{
    const resp = JSON.parse(m) as Response;
    console.log("[Receiver] ", resp);
    const index = BackendDelegator.MessageStack.findIndex(i=>i.request.id === resp.id);
    if(index >= 0){
        BackendDelegator.MessageStack[index].response = resp;
    }
})
interface Request{
    id: number,
    type: "message" | "addition"| "error" | "getConfig" | "setConfig",
}
interface MessageRequest extends  Request{
    content: string
}
interface AdditionRequest extends Request{
    a: number;
    b: number;
}
interface GetConfigRequest extends Request{

}
interface ErrorRequest extends Request{
    error: string;
}
interface Response{
    id: number;
    type: "message" | "addition"| "error" | "getConfig" | "setConfig",
}
interface MessageResponse extends Response{
    content: string | undefined | null;
}
interface AdditionResponse extends Response{
    result :number;
}
interface ErrorResponse extends Response{
    error: string;
}
interface ConfigResponse extends Response{
    snapshot: {online: boolean};
}
interface ResponseRequest{
    id: number;
    req: Request;
    resp: Response;

}
export type {Request, MessageRequest, AdditionRequest, ErrorRequest, Response, MessageResponse, AdditionResponse, ErrorResponse, ResponseRequest};
export {BackendDelegator};