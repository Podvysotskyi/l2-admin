import type { AccountDirectoryRequest } from '../types/requests/account-directory-request'
import type { CharacterDirectoryRequest } from '../types/requests/character-directory-request'
import type { AccountDirectoryResponse } from '../types/responses/account-directory-response'
import type { AdminServiceInfo } from '../types/responses/admin-service-info'
import type { CharacterDirectoryResponse } from '../types/responses/character-directory-response'
import type { GameVersionSummary } from '../types/models/game-version'
import { selectedGameVersionKey } from '../utils/game-version'

export function getGameVersions(): Promise<GameVersionSummary[]> {
  return $fetch<GameVersionSummary[]>('/api/game-versions')
}

export function getAdminServiceInfo(): Promise<AdminServiceInfo> {
  return $fetch<AdminServiceInfo>('/api/system/info')
}

export function searchAccounts(
  request: AccountDirectoryRequest = {}
): Promise<AccountDirectoryResponse> {
  return $fetch<AccountDirectoryResponse>('/api/accounts', {
    query: directoryQuery(request)
  })
}

export function searchCharacters(
  request: CharacterDirectoryRequest = {}
): Promise<CharacterDirectoryResponse> {
  return $fetch<CharacterDirectoryResponse>('/api/characters', {
    query: {
      ...directoryQuery(request),
      gameVersion: request.gameVersion ?? selectedGameVersionKey()
    }
  })
}

function directoryQuery(request: {
  query?: string
  page?: number
  pageSize?: number
}) {
  const query = request.query?.trim()
  return {
    ...(query ? { query } : {}),
    page: request.page ?? 1,
    pageSize: request.pageSize ?? 25
  }
}
