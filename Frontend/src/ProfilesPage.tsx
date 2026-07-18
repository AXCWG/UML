import {For} from "solid-js";
import {BackendDelegator, state} from "./BackendDelegator";

const ProfilesPage = ()=>{
    return <>
        <div class={"grid grid-cols-[20rem_auto] h-full"}>
            <div class={" bg-base-300 gap-2 flex flex-col p-2  items-center"}>
                <ul class={"list bg-base-100 rounded-md w-full overflow-clip"}>
                    <For each={state().profiles}>
                        {(profile, index) => {
                            return <li class={"list-row"}>
                                <div >
                                    <div class={"text-md"}>{profile.name}</div>
                                    <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title={profile.path}>{profile.path}</div>

                                </div>
                            </li>
                        }}
                    </For>
                    <li class={"list-row "}>
                        <div >
                            <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                            <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                        </div>
                    </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li> <li class={"list-row "}>
                    <div >
                        <div class={"text-md"}>aaaaaaaaaaaaaa</div>
                        <div class={"text-[.6rem] text-base-content/50 line-clamp-1"} title="bbbbbbbbbbbbbb">bbbbbbbbbbbbbbbbbbb</div>

                    </div>
                </li>
                </ul>
            </div>
            <div class={"bg-base-100 h-full"}>b</div>

        </div>
    </>
}
export default ProfilesPage;