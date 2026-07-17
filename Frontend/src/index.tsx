/* @refresh reload */
import './index.css';
import { render } from 'solid-js/web';
import {HashRouter, Route, Router} from '@solidjs/router';
import 'solid-devtools';

import Home from './Home';
import Shell from "./Shell";

const external = (window as any).external;

const root = document.getElementById('root');

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    'Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?',
  );
}

render(() => <HashRouter>
<Route component={Shell} path={"/"}>
  <Route component={Home} path={"/"}/>
  <Route component={Home} path={"/home"}/>
  <Route component={()=><>Sets</>} path={"/sets"}/>
  <Route component={()=><>Settings</>} path={"/settings"}/>
</Route>
</HashRouter>, root!);
