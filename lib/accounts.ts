export interface AccountSummary {
  id: string
  username: string
  createdAt: string
  lastSuccessfulLoginAt?: string | null
  hasActiveLoginSession: boolean
}

export interface AccountDirectoryPage {
  items: AccountSummary[]
  total: number
  page: number
  pageSize: number
}

export function accountsUrl(
  apiBase: string,
  options: { query?: string; page?: number; pageSize?: number } = {}
): string {
  const url = new URL(`${apiBase.replace(/\/$/, '')}/api/accounts`)
  const query = options.query?.trim()
  if (query) url.searchParams.set('query', query)
  url.searchParams.set('page', String(options.page ?? 1))
  url.searchParams.set('pageSize', String(options.pageSize ?? 25))
  return url.toString()
}

export function formatAccountDate(value?: string | null): string {
  if (!value) return '—'
  return new Intl.DateTimeFormat(undefined, {
    dateStyle: 'medium',
    timeStyle: 'short'
  }).format(new Date(value))
}

export function pageFromQuery(value: unknown): number {
  const parsed = typeof value === 'string' ? Number.parseInt(value, 10) : 1
  return Number.isInteger(parsed) && parsed > 0 ? parsed : 1
}
