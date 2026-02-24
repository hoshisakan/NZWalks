import apiClient from './apiClient'
import type { AddWalkRequestDto, DifficultyDto, RegionDto, UpdateWalkRequestDto, WalkDto } from '../types'

type RegionApiPayload = {
  Id?: string
  id?: string
  Code?: string
  code?: string
  Name?: string
  name?: string
  RegionImageUrl?: string
  regionImageUrl?: string
}

type DifficultyApiPayload = {
  Id?: string
  id?: string
  Name?: string
  name?: string
}

type WalkApiPayload = {
  Id?: string
  id?: string
  Name?: string
  name?: string
  Description?: string
  description?: string
  LengthInKm?: number
  lengthInKm?: number
  WalkImageUrl?: string
  walkImageUrl?: string
  DifficultyId?: string
  difficultyId?: string
  RegionId?: string
  regionId?: string
  Difficulty?: DifficultyApiPayload
  difficulty?: DifficultyApiPayload
  Region?: RegionApiPayload
  region?: RegionApiPayload
}

const normalizeDifficulty = (item?: DifficultyApiPayload): DifficultyDto => ({
  Id: item?.Id ?? item?.id ?? '',
  Name: item?.Name ?? item?.name ?? '',
})

const normalizeRegion = (item?: RegionApiPayload): RegionDto => ({
  Id: item?.Id ?? item?.id ?? '',
  Code: item?.Code ?? item?.code ?? '',
  Name: item?.Name ?? item?.name ?? '',
  RegionImageUrl: item?.RegionImageUrl ?? item?.regionImageUrl,
})

const normalizeWalk = (item: WalkApiPayload): WalkDto => ({
  Id: item.Id ?? item.id ?? '',
  Name: item.Name ?? item.name ?? '',
  Description: item.Description ?? item.description ?? '',
  LengthInKm: item.LengthInKm ?? item.lengthInKm ?? 0,
  WalkImageUrl: item.WalkImageUrl ?? item.walkImageUrl,
  DifficultyId: item.DifficultyId ?? item.difficultyId ?? '',
  RegionId: item.RegionId ?? item.regionId ?? '',
  Difficulty: normalizeDifficulty(item.Difficulty ?? item.difficulty),
  Region: normalizeRegion(item.Region ?? item.region),
})

export const walkService = {
  async getAll(): Promise<WalkDto[]> {
    const response = await apiClient.get<WalkApiPayload[]>('/Walks')
    return response.data.map(normalizeWalk)
  },

  async getById(id: string): Promise<WalkDto> {
    const response = await apiClient.get<WalkApiPayload>(`/Walks/${id}`)
    return normalizeWalk(response.data)
  },

  async create(payload: AddWalkRequestDto): Promise<WalkDto> {
    const response = await apiClient.post<WalkApiPayload>('/Walks', payload)
    return normalizeWalk(response.data)
  },

  async update(id: string, payload: UpdateWalkRequestDto): Promise<WalkDto> {
    const response = await apiClient.put<WalkApiPayload>(`/Walks/${id}`, payload)
    return normalizeWalk(response.data)
  },

  async delete(id: string): Promise<WalkDto> {
    const response = await apiClient.delete<WalkApiPayload>(`/Walks/${id}`)
    return normalizeWalk(response.data)
  },
}
