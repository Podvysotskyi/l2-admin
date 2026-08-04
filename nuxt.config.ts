export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  css: ['~/assets/css/main.css'],
  devtools: { enabled: true },
  modules: ['@nuxt/ui', '@pinia/nuxt'],
  runtimeConfig: {
    public: {
      apiBase: process.env.NUXT_PUBLIC_ADMIN_API_BASE ?? 'http://localhost:5201'
    }
  },
  typescript: { typeCheck: true }
})
