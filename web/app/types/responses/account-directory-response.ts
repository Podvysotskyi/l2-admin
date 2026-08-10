import type { AccountSummary } from '../models/account-summary'

export interface AccountDirectoryResponse {
  items: AccountSummary[]
  total: number
  page: number
  pageSize: number
}
