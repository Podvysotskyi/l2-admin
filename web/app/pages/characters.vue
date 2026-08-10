<script setup lang="ts">
import { storeToRefs } from 'pinia'
import { useDirectoryRouteSync } from '../composables/use-directory-route-sync'
import { useCharacterDirectoryStore } from '../stores/character-directory'

const store = useCharacterDirectoryStore()
const { items, total, query, page, pageSize, loading, error } =
  storeToRefs(store)

useDirectoryRouteSync('/characters', { query, page, pageSize }, store.load)
</script>

<template>
  <div class="space-y-6">
    <PagesCharactersCharacterDirectory
      v-model:query="query"
      v-model:page="page"
      v-model:page-size="pageSize"
      :items="items"
      :total="total"
      :loading="loading"
      :error="error"
      @refresh="store.load"
    />
    <PagesCharactersCharacterDirectoryNotice />
  </div>
</template>
