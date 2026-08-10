export interface SystemInfo {
  service: string
  buildVersion: string
  environment: string
}

export interface AccountRecord {
  id: string
  username: string
  email: string
  createdAt: string
  lastSuccessfulLoginAt?: string | null
  hasActiveLoginSession: boolean
}

export interface AccountPage {
  items: AccountRecord[]
  total: number
  page: number
  pageSize: number
}

export type CharacterStatus = 'active' | 'pending_deletion' | 'deletion_expired'

export interface CharacterRecord {
  id: string
  name: string
  accountId: string
  username?: string | null
  raceId: number
  raceName?: string | null
  sexId: number
  sexName?: string | null
  baseClassId: number
  baseClassName?: string | null
  activeClassId: number
  activeClassName?: string | null
  level: number
  experience: number
  createdAt: string
  deleteAfter?: string | null
  status: CharacterStatus
}

export interface CharacterPage {
  items: CharacterRecord[]
  total: number
  page: number
  pageSize: number
}

export function systemInfoUrl(apiBase: string): string {
  return `${normalizeBase(apiBase)}/api/system/info`
}

export function accountDirectoryUrl(
  apiBase: string,
  options: { query?: string; page?: number; pageSize?: number } = {}
): string {
  const url = new URL(`${normalizeBase(apiBase)}/api/accounts`)
  const query = options.query?.trim()
  if (query) url.searchParams.set('query', query)
  url.searchParams.set('page', String(options.page ?? 1))
  url.searchParams.set('pageSize', String(options.pageSize ?? 25))
  return url.toString()
}

export function characterDirectoryUrl(
  apiBase: string,
  options: { query?: string; page?: number; pageSize?: number } = {}
): string {
  const url = new URL(`${normalizeBase(apiBase)}/api/characters`)
  const query = options.query?.trim()
  if (query) url.searchParams.set('query', query)
  url.searchParams.set('page', String(options.page ?? 1))
  url.searchParams.set('pageSize', String(options.pageSize ?? 25))
  return url.toString()
}

export function directoryRouteState(query: Record<string, unknown>) {
  return {
    query: typeof query.query === 'string' ? query.query : '',
    page: positiveInteger(query.page, 1),
    pageSize: positiveInteger(query.pageSize, 25)
  }
}

export function resolvedName(
  name: string | null | undefined,
  id: string | number
) {
  return name ?? `Unknown (${id})`
}

export function characterStatusLabel(status: CharacterStatus): string {
  switch (status) {
    case 'active':
      return 'Active'
    case 'pending_deletion':
      return 'Pending deletion'
    case 'deletion_expired':
      return 'Deletion expired'
  }
}

export function characterStatusColor(
  status: CharacterStatus
): 'success' | 'warning' | 'error' {
  return status === 'active'
    ? 'success'
    : status === 'pending_deletion'
      ? 'warning'
      : 'error'
}

export function positiveInteger(value: unknown, fallback: number): number {
  if (typeof value !== 'string') return fallback
  const parsed = Number.parseInt(value, 10)
  return Number.isInteger(parsed) && parsed > 0 ? parsed : fallback
}

export function formatTimestamp(value?: string | null): string {
  if (!value) return 'Never'
  const date = new Date(value)
  if (Number.isNaN(date.valueOf())) return 'Invalid date'
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(date)
}

export function paginationRange(
  total: number,
  page: number,
  pageSize: number
): { first: number; last: number } {
  if (total <= 0) return { first: 0, last: 0 }
  const first = (Math.max(1, page) - 1) * pageSize + 1
  return {
    first: Math.min(first, total),
    last: Math.min(first + pageSize - 1, total)
  }
}

function normalizeBase(apiBase: string): string {
  return apiBase.replace(/\/$/, '')
}
