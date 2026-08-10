import type { CharacterSummary } from '../models/character-summary'

export interface CharacterDirectoryResponse {
  items: CharacterSummary[]
  total: number
  page: number
  pageSize: number
}
