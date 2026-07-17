import {Component, createSignal, onMount} from 'solid-js';
import {BackendDelegator} from "./BackendDelegator";

const App: Component = () => {
  const [value, setValue] = createSignal("?");
  onMount(()=>{
    BackendDelegator.Send({
      type: "message",
        content: "Hello from SolidJS!"
    })
  })
  return (
      <>
        <p class="text-4xl text-green-700 text-center py-20">Hello tailwind!</p>
        <p>1 + 1 = <p>{value()}</p></p>
        <button class={"btn btn-primary"} on:click={()=>{

        }}>Click Me. </button>
      </>

  );
};

export default App;
