import {Component, createEffect, createResource, createSignal, on, onMount, Show, Suspense} from 'solid-js';
import {BackendDelegator, setState, state, StateSnapshot} from "./BackendDelegator";

const Home: Component = () => {

  createEffect(on(state, async ()=>{
    console.log("shot. ")
    await BackendDelegator.SetConfigRequest(state());
  }, {defer: true}))
  return (
      <div class={"grid grid-cols-[20rem_auto] h-full gap-2 bg-base-300"}>
        <div class={"h-full overflow-hidden py-2 pl-2 pr-0"}>
          <div class={"bg-base-100 rounded-md h-full gap-2 flex flex-col justify-center items-center"}>
            <div role="tablist" class="tabs tabs-box tabs-sm">
              <a role="tab" on:click={() => {
                setState({...state(), online: true})
              }} class="tab" classList={{["tab-active"]: state().online}}>Online</a>
              <a role="tab" on:click={() => {
                setState({...state(), online: false})
              }} class="tab" classList={{["tab-active"]: !state().online}}>Offline</a>
            </div>
            <button class={"btn btn-primary"} onclick={() => {
            }}>Play
            </button>
          </div>
        </div>
        <div class={"h-full overflow-hidden py-2 pr-2 pl-0"}>
          <div class={"bg-base-100 rounded-md min-h-0 overflow-y-auto h-full hover-scrollbar"}>
          </div>
        </div>
      </div>
  );
};

export default Home;
