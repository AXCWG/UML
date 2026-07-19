import {A, RouteSectionProps, useLocation, useNavigate} from "@solidjs/router";
import {createEffect, createSignal} from "solid-js";

const Shell = (props: RouteSectionProps)=>{
    const navigate = useNavigate();
    const location = useLocation();
    createEffect(()=>{
        setCurrent(location.pathname as any)
    })
    const [current, setCurrent] = createSignal<"/" | "/home" | "/sets" | "/settings">(location.pathname as any);
    return <div class={"flex flex-col min-h-0 flex-1"}>
            <div>
                <div class={"text-center py-2 border-t-1 border-base-200"}>Minecraft</div>
                <div role={"tablist"} class={"tabs tabs-lift justify-center bg-base-200"}>
                    <a id={"/home"} role={"tab"} class={"tab "} classList={{["tab-active"]: current() === "/home" || current() === "/"}} onclick={()=>{
                        navigate("/home");
                    }}>Home</a>
                    <a id={"/sets"} role={"tab"} class={"tab"} classList={{["tab-active"]: current() === "/sets"}} onclick={()=>{
                        navigate("/sets");
                    }}>Profiles</a>
                    <a id={"/settings"} role={"tab"} class={"tab"} classList={{["tab-active"]: current() === "/settings"}} onclick={()=>{
                        navigate("/settings");
                    }}>Settings</a>
                </div>
            </div>
            <div class={"flex-1 min-h-0 overflow-hidden"}>
                {props.children}
            </div>
        </div>
}

export default Shell;