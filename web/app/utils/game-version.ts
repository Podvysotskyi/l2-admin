export const gameVersionStorageKey = 'l2-admin.game-version'

export function selectedGameVersionKey() {
  if (!import.meta.client) return 'interlude'
  return window.localStorage.getItem(gameVersionStorageKey) ?? 'interlude'
}
