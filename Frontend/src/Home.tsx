import {Component, createResource, createSignal, onMount, Show, Suspense} from 'solid-js';
import {BackendDelegator} from "./BackendDelegator";

const Home: Component = () => {
  interface State{
    online: boolean;
  }
  const [state, setState] = createSignal<State>({
    online: false,
  })
  BackendDelegator.GetConfigRequest();
  return <div class={"flex flex-row h-full"}>
    <div class={"grow-[.3] bg-base-300 h-full flex flex-col justify-center items-center"}>
      <div role="tablist" class="tabs tabs-box tabs-sm">
        <a role="tab" on:click={()=>{
            setState({online: true})
        }} class="tab" classList={{["tab-active"]: state().online}}>Online</a>
        <a role="tab" on:click={()=>{
          setState({online: false})
        }} class="tab" classList={{["tab-active"]: !state().online}}>Offline</a>
      </div>
    </div>
    <div class={"grow bg-base-100 h-full"}>b</div>
  </div>;
};

export default Home;
