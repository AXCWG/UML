import {createEffect, createSignal, For, on} from "solid-js";
import {BackendDelegator, state} from "./BackendDelegator";

const ProfilesPage = ()=>{
    createEffect(on(state, async ()=>{

    }, {defer: true}))
    return <>
        <div class={"grid grid-cols-[20rem_auto] h-full"}>
            <div class={"bg-base-300 gap-2 flex flex-col p-2 items-center overflow-hidden"}>
                <ul class={"list bg-base-100 rounded-md w-full min-h-0 overflow-y-auto hover-scrollbar"} >
                    <For each={state().profiles}>
                        {(profile, index) => {
                            return <li id={profile.uuid} class={"list-row"}>
                                <div >
                                    <div class={"text-md"}>{profile.name}</div>
                                    <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={profile.path}>{profile.path}</div>

                                </div>

                            </li>
                        }}
                    </For>

                </ul>
            </div>
            <div class={"bg-base-100 h-full"}>
                <For each={Object.entries(state().profiles.find(i => i.uuid === state().selectedProfileUuid)!.unreliableVersionList) }>
                    {([version, info], index) => {
                        return <div class={"flex flex-col gap-2 p-2"}>
                            <div class={"text-md"}>{info.name}</div>
                            <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={version}>{version}</div>
                            <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={info.jrePath ?? "null"}>JRE: {info.jrePath ?? "null"}</div>
                            <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={info.memMegabyte?.toString() ?? "null"}>Memory: {info.memMegabyte?.toString() ?? "null"}</div>
                        </div>
                    }}
                </For>
            </div>

        </div>
    </>
}
export default ProfilesPage;