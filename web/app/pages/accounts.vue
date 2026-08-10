<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useDirectoryRouteSync } from '../composables/use-directory-route-sync'
import { useAccountDirectoryStore } from '../stores/account-directory'

const store = useAccountDirectoryStore()
const { items, total, query, page, pageSize, loading, error } =
  storeToRefs(store)

useDirectoryRouteSync('/accounts', { query, page, pageSize }, store.load)
</script>

<template>
  <div class="space-y-6">
    <PagesAccountsAccountDirectory
      v-model:query="query"
      v-model:page="page"
      v-model:page-size="pageSize"
      :items="items"
      :total="total"
      :loading="loading"
      :error="error"
      @refresh="store.load"
    />
    <PagesAccountsAccountSecurityNotice />
  </div>
</template>
