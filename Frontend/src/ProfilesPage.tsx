import {createEffect, createSignal, For, on} from "solid-js";
import {BackendDelegator, setState, state} from "./BackendDelegator";
import Enumerable from "linq";

const ProfilesPage = ()=>{
    let ignoreEffect = false;
    createEffect(on(state, async ()=>{
        if(!ignoreEffect){
            console.log(state())
            await BackendDelegator.SetConfigRequest(state())
        }
    }, {defer: true}))
    const addProfile = async () => {
        ignoreEffect = true;
        const name = prompt("Profile name:");
        if (!name) return;
        await BackendDelegator.AddProfileRequest(name);
        const config = await BackendDelegator.GetConfigRequest();
        setState(config.snapshot);
        ignoreEffect = false;
    };
    return <>
        <div class={"grid grid-cols-[20rem_auto] h-full gap-2 bg-base-300"}>
            <div class={"gap-2 flex flex-col py-2 pl-2 pr-0 items-center overflow-hidden"}>
                <button class={"btn btn-primary btn-sm w-full"} onclick={addProfile}>Add Profile</button>
                <ul class={"list bg-base-100 rounded-md w-full min-h-0 overflow-y-auto hover-scrollbar flex-1"} >
                    <For each={state().profiles}>
                        {(profile, index) => {
                            return <li id={profile.uuid} class={"list-row hover:bg-base-200 cursor-pointer transition-colors"} onclick={()=>{
                                setState({...state(), selectedProfileUuid: profile.uuid})
                            }}>
                                <div class={"list-col-grow flex flex-col min-w-0"}>
                                    <div class={"text-md truncate"}>{profile.name}</div>
                                    <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={profile.path}>{profile.path}</div>
                                </div>
                                <div class={"flex flex-row gap-0.5 flex-shrink-0"}>
                                    <button class={"btn btn-ghost btn-xs btn-square"} title="Modify" onclick={(e) => {
                                        e.stopPropagation();

                                    }}>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="w-3 h-3"><path d="M12.238 3.762a2.302 2.302 0 0 0-3.256 0l-6.468 6.468a.9.9 0 0 0-.24.422l-1.05 3.85a.45.45 0 0 0 .55.55l3.85-1.05a.9.9 0 0 0 .422-.24l6.468-6.468a2.302 2.302 0 0 0 0-3.256l-.275-.276Zm-1.987 1.269a1.102 1.102 0 0 1 1.558 0l.275.275a1.102 1.102 0 0 1 0 1.558l-.657.657-1.832-1.832.656-.658Zm-9.86 10.315 1.832-1.832-1.832-1.832-1.055 3.664Z" clip-rule="evenodd" fill-rule="evenodd" /></svg>
                                    </button>
                                    <button class={"btn btn-ghost btn-xs btn-square text-error"} title="Delete" onclick={() => {
                                        const newArray = Enumerable.from(state().profiles).where(i=>i.uuid !== profile.uuid).toArray()

                                        setState({...state(), profiles:newArray, selectedProfileUuid: newArray.length === 0 ? null! : newArray[0].uuid })

                                    }}>
                                        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 16 16" fill="currentColor" class="w-3 h-3"><path fill-rule="evenodd" d="M5 3.25V4H2.75a.75.75 0 0 0 0 1.5h.3l.815 8.15A1.5 1.5 0 0 0 5.357 15h5.285a1.5 1.5 0 0 0 1.493-1.35l.815-8.15h.3a.75.75 0 0 0 0-1.5H11v-.75A2.25 2.25 0 0 0 8.75 1h-1.5A2.25 2.25 0 0 0 5 3.25Zm2.25-1.75a.75.75 0 0 0-.75.75V4h3v-.75a.75.75 0 0 0-.75-.75h-1.5ZM6.05 6a.75.75 0 0 1 .787.713l.275 5.5a.75.75 0 0 1-1.498.075l-.275-5.5A.75.75 0 0 1 6.05 6Zm3.9 0a.75.75 0 0 1 .712.787l-.275 5.5a.75.75 0 0 1-1.498-.075l.275-5.5a.75.75 0 0 1 .786-.712Z" clip-rule="evenodd" /></svg>
                                    </button>
                                </div>
                            </li>
                        }}
                    </For>

                </ul>
            </div>
            <div class={"h-full overflow-hidden py-2 pr-2 pl-0"}>
                <div class={"bg-base-100 rounded-md min-h-0 overflow-y-auto h-full hover-scrollbar"}>
                    <For each={Object.entries(state().profiles.find(i => i.uuid === state().selectedProfileUuid)?.unreliableVersionList ?? {}) }>
                        {([version, info], index) => {
                            return <div class={"flex flex-col gap-2 p-2 hover:bg-base-200 cursor-pointer transition-colors"}>
                                <div class={"text-md"}>{info.name}</div>
                            </div>
                        }}
                    </For>
                </div>
            </div>

        </div>
    </>
}
export default ProfilesPage;