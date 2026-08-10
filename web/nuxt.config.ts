const adminApiBase = process.env.NUXT_ADMIN_API_BASE?.replace(/\/$/, '')

if (!adminApiBase) {
  throw new Error('NUXT_ADMIN_API_BASE is required')
}

export default defineNuxtConfig({
  compatibilityDate: '2025-07-15',
  devtools: { enabled: true },

  modules: ['@nuxt/ui', '@pinia/nuxt'],
  css: ['~/assets/css/main.css'],

  routeRules: {
    '/api/**': {
      proxy: `${adminApiBase}/api/**`
    }
  },

  typescript: { typeCheck: true }
})
