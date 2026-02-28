import apiClient from './apiClient';
import type { AddDifficultyRequestDto, DifficultyDto, UpdateDifficultyRequestDto } from '../types';

type DifficultyApiPayload = {
    Id?: string;
    id?: string;
    Name?: string;
    name?: string;
};

const normalizeDifficulty = (item: DifficultyApiPayload): DifficultyDto => ({
    Id: item.Id ?? item.id ?? '',
    Name: item.Name ?? item.name ?? '',
});

export const difficultyService = {
    async getAll(): Promise<DifficultyDto[]> {
        const response = await apiClient.get<DifficultyApiPayload[]>('/Difficulties');
        return response.data.map(normalizeDifficulty);
    },

    async getById(id: string): Promise<DifficultyDto> {
        const response = await apiClient.get<DifficultyApiPayload>(`/Difficulties/${id}`);
        return normalizeDifficulty(response.data);
    },

    async create(payload: AddDifficultyRequestDto): Promise<DifficultyDto> {
        const response = await apiClient.post<DifficultyApiPayload>('/Difficulties', payload);
        return normalizeDifficulty(response.data);
    },

    async update(id: string, payload: UpdateDifficultyRequestDto): Promise<DifficultyDto> {
        const response = await apiClient.put<DifficultyApiPayload>(`/Difficulties/${id}`, payload);
        return normalizeDifficulty(response.data);
    },

    async delete(id: string): Promise<DifficultyDto> {
        const response = await apiClient.delete<DifficultyApiPayload>(`/Difficulties/${id}`);
        return normalizeDifficulty(response.data);
    },
};
