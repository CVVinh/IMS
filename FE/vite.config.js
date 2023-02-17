// import { fileURLToPath, URL } from 'url'

import { defineConfig, loadEnv } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueJsx from '@vitejs/plugin-vue-jsx'
import path from 'path'
import babel from 'vite-plugin-babel'

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [vue(), vueJsx(), babel()],

    resolve: {
        alias: {
            // '@': fileURLToPath(new URL('./src', import.meta.url)),
            '@': path.resolve(__dirname, './src')
        },
    },
    css: {
        devSourcemap: true,
    },
    build: {
        chunkSizeWarningLimit: 1600,
    },

})