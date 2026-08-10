import type { CharacterStatus } from './character-status'

export interface CharacterSummary {
  id: string
  name: string
  accountId: string
  username: string | null
  raceId: number
  raceName: string | null
  sexId: number
  sexName: string | null
  baseClassId: number
  baseClassName: string | null
  activeClassId: number
  activeClassName: string | null
  level: number
  experience: number
  createdAt: string
  deleteAfter: string | null
  status: CharacterStatus
}
