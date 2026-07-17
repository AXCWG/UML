import Enumerable from "linq";
import IEnumerable = Enumerable.IEnumerable;

const Back = this;
class  BackendDelegator  {

    private static MessageStack : {request: Request, response: Response | null}[] = [];
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
                const resp = Enumerable.from(BackendDelegator.MessageStack).firstOrDefault(i=>i.request.id ===id )?.response
                if (resp?.id){
                    switch (resp.type) {
                        case "message":
                            resolve((resp as MessageResponse).content!);
                            break;
                        case "error":
                            reject();
                    }
                    BackendDelegator.MessageStack.
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
    };
}

interface Request{
    id: number,
    type: "message" | "addition"| "error",
}
interface MessageRequest extends  Request{
    content: string
}
interface AdditionRequest extends Request{
    a: number;
    b: number;
}
interface ErrorRequest extends Request{
    error: string;
}
interface Response{
    id: number;
    type: "message" | "addition"| "error",
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
interface ResponseRequest{
    id: number;
    req: Request;
    resp: Response;

}
export type {Request, MessageRequest, AdditionRequest, ErrorRequest, Response, MessageResponse, AdditionResponse};
export {BackendDelegator};