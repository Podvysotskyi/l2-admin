export interface AccountSummary {
  id: string
  username: string
  email: string
  createdAt: string
  lastSuccessfulLoginAt: string | null
  hasActiveLoginSession: boolean
}
