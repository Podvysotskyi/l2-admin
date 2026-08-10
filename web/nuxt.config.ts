import { fileURLToPath } from 'node:url'

export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  css: ['~/assets/css/main.css'],
  devtools: { enabled: true },
  modules: ['@nuxt/ui', '@pinia/nuxt'],
  dir: {
    public:
      process.env.L2_PUBLIC_ASSETS_DIR ??
      fileURLToPath(new URL('./public', import.meta.url))
  },
  routeRules: {
    '/api/**': {
      proxy: `${process.env.NUXT_ADMIN_API_BASE ?? 'http://localhost:5201'}/api/**`
    }
  },
  typescript: { typeCheck: true }
})
