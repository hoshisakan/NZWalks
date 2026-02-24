export interface AddRegionRequestDto {
  Code: string
  Name: string
  RegionImageUrl?: string
}

export interface UpdateRegionRequestDto {
  Code: string
  Name: string
  RegionImageUrl?: string
}

export interface RegionDto {
  Id: string
  Code: string
  Name: string
  RegionImageUrl?: string
}

export interface AddDifficultyRequestDto {
  Name: string
}

export interface UpdateDifficultyRequestDto {
  Name: string
}

export interface DifficultyDto {
  Id: string
  Name: string
}

export interface AddWalkRequestDto {
  Name: string
  Description: string
  LengthInKm: number
  WalkImageUrl?: string
  DifficultyId: string
  RegionId: string
}

export interface UpdateWalkRequestDto {
  Name: string
  Description: string
  LengthInKm: number
  WalkImageUrl?: string
  DifficultyId: string
  RegionId: string
}

export interface WalkDto {
  Id: string
  Name: string
  Description: string
  LengthInKm: number
  WalkImageUrl?: string
  DifficultyId: string
  RegionId: string
  Difficulty: DifficultyDto
  Region: RegionDto
}

export interface LoginRequestDto {
  Email: string
  Password: string
}

export interface LoginResponseDto {
  JWTToken: string
}

export interface RegisterRequestDto {
  Username: string
  Email: string
  Password: string
  Roles: string[]
}

export interface ImageUploadRequestDto {
  File: File
  FileName: string
  FileDescription?: string
}
