import apiClient from './apiClient'
import type { AddRegionRequestDto, RegionDto, UpdateRegionRequestDto } from '../types'

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

const normalizeRegion = (item: RegionApiPayload): RegionDto => ({
  Id: item.Id ?? item.id ?? '',
  Code: item.Code ?? item.code ?? '',
  Name: item.Name ?? item.name ?? '',
  RegionImageUrl: item.RegionImageUrl ?? item.regionImageUrl,
})

export const regionService = {
  async getAll(): Promise<RegionDto[]> {
    const response = await apiClient.get<RegionApiPayload[]>('/Regions')
    return response.data.map(normalizeRegion)
  },

  async getById(id: string): Promise<RegionDto> {
    const response = await apiClient.get<RegionApiPayload>(`/Regions/${id}`)
    return normalizeRegion(response.data)
  },

  async create(payload: AddRegionRequestDto): Promise<RegionDto> {
    const response = await apiClient.post<RegionApiPayload>('/Regions', payload)
    return normalizeRegion(response.data)
  },

  async update(id: string, payload: UpdateRegionRequestDto): Promise<RegionDto> {
    const response = await apiClient.put<RegionApiPayload>(`/Regions/${id}`, payload)
    return normalizeRegion(response.data)
  },

  async delete(id: string): Promise<RegionDto> {
    const response = await apiClient.delete<RegionApiPayload>(`/Regions/${id}`)
    return normalizeRegion(response.data)
  },
}
