/* @refresh reload */
import './index.css';
import { render } from 'solid-js/web';
import 'solid-devtools';

import App from './App';

const external = (window as any).external;

// window.onerror = (msg, url, line, col, err) => {
//     external?.sendMessage?.(JSON.stringify({
//         type: 'error',
//         message: msg,
//         stack: err?.stack ?? null,
//         url: url ?? null,
//         line: line ?? null,
//         col: col ?? null
//     }));
// };

// window.onunhandledrejection = (e) => {
//     external?.sendMessage?.(JSON.stringify({
//         type: 'unhandled-rejection',
//         reason: String(e.reason)
//     }));
// };

const root = document.getElementById('root');

if (import.meta.env.DEV && !(root instanceof HTMLElement)) {
  throw new Error(
    'Root element not found. Did you forget to add it to your index.html? Or maybe the id attribute got misspelled?',
  );
}

render(() => <App />, root!);
