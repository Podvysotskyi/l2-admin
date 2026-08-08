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
