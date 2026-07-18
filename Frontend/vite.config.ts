import tailwindcss from '@tailwindcss/vite';
import { defineConfig } from 'vite';
import solidPlugin from 'vite-plugin-solid';
import devtools from 'solid-devtools/vite';
import {viteSingleFile} from "vite-plugin-singlefile";
import viteLegacyPlugin from "@vitejs/plugin-legacy";

export default defineConfig({
  plugins: [devtools(), solidPlugin(), tailwindcss(), viteSingleFile()],
  server: {
    port: 3000,
  },
  base: './',
  build: {
    target: 'esnext',
    outDir: '../Minecraft/wwwroot',
    emptyOutDir: true
  },
});
