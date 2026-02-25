import axios from 'axios'
import {
  AUTH_STATE_CHANGE_EVENT,
  JWT_TOKEN_STORAGE_KEY,
  LAST_TOKEN_REFRESH_AT_STORAGE_KEY,
  REFRESH_TOKEN_STORAGE_KEY,
} from '../constants/auth'
import type { LoginResponseDto, TokenRequestDto } from '../types'

const authHttpClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'https://localhost/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

const REFRESH_BEFORE_EXPIRY_MS = 60_000
let refreshTimerId: number | null = null
let refreshPromise: Promise<string | null> | null = null

const getTokenPayload = (jwtToken: string): Record<string, unknown> | null => {
  try {
    const [, payload] = jwtToken.split('.')
    if (!payload) {
      return null
    }
    return JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/'))) as Record<string, unknown>
  } catch {
    return null
  }
}

const notifyAuthStateChanged = () => {
  window.dispatchEvent(new Event(AUTH_STATE_CHANGE_EVENT))
}

const clearRefreshTimer = () => {
  if (refreshTimerId) {
    window.clearTimeout(refreshTimerId)
    refreshTimerId = null
  }
}

export const getLastTokenRefreshAt = (): number | null => {
  const value = localStorage.getItem(LAST_TOKEN_REFRESH_AT_STORAGE_KEY)
  if (!value) {
    return null
  }

  const parsed = Number(value)
  return Number.isNaN(parsed) ? null : parsed
}

export const getAccessToken = (): string => localStorage.getItem(JWT_TOKEN_STORAGE_KEY) ?? ''

export const getRefreshToken = (): string => localStorage.getItem(REFRESH_TOKEN_STORAGE_KEY) ?? ''

export const saveTokens = (accessToken: string, refreshToken: string) => {
  localStorage.setItem(JWT_TOKEN_STORAGE_KEY, accessToken)
  localStorage.setItem(REFRESH_TOKEN_STORAGE_KEY, refreshToken)
  scheduleTokenRefresh()
  notifyAuthStateChanged()
}

export const clearTokens = () => {
  localStorage.removeItem(JWT_TOKEN_STORAGE_KEY)
  localStorage.removeItem(REFRESH_TOKEN_STORAGE_KEY)
  localStorage.removeItem(LAST_TOKEN_REFRESH_AT_STORAGE_KEY)
  clearRefreshTimer()
  notifyAuthStateChanged()
}

export const getAccessTokenRemainingSeconds = (): number | null => {
  const accessToken = getAccessToken()
  if (!accessToken) {
    return null
  }

  const payload = getTokenPayload(accessToken)
  const exp = payload?.exp
  if (typeof exp !== 'number') {
    return null
  }

  return Math.max(Math.floor((exp * 1000 - Date.now()) / 1000), 0)
}

const getRefreshDelayMs = (accessToken: string): number => {
  const payload = getTokenPayload(accessToken)
  const exp = payload?.exp
  if (typeof exp !== 'number') {
    return 0
  }

  const expiryMs = exp * 1000
  const refreshAt = expiryMs - REFRESH_BEFORE_EXPIRY_MS
  return Math.max(refreshAt - Date.now(), 0)
}

export const refreshTokens = async (): Promise<string | null> => {
  if (refreshPromise) {
    return refreshPromise
  }

  const accessToken = getAccessToken()
  const refreshToken = getRefreshToken()

  if (!accessToken || !refreshToken) {
    clearTokens()
    return null
  }

  refreshPromise = (async () => {
    try {
      const payload: TokenRequestDto = {
        JwtToken: accessToken,
        RefreshToken: refreshToken,
      }

      const response = await authHttpClient.post<LoginResponseDto>('/Auth/Refresh-Token', payload)
      const newAccessToken =
        response.data.JWTToken ??
        (response.data as LoginResponseDto & { jwtToken?: string }).jwtToken
      const newRefreshToken =
        response.data.RefreshToken ??
        (response.data as LoginResponseDto & { refreshToken?: string }).refreshToken

      if (!newAccessToken || !newRefreshToken) {
        clearTokens()
        return null
      }

      localStorage.setItem(LAST_TOKEN_REFRESH_AT_STORAGE_KEY, Date.now().toString())
      saveTokens(newAccessToken, newRefreshToken)
      return newAccessToken
    } catch {
      clearTokens()
      return null
    } finally {
      refreshPromise = null
    }
  })()

  return refreshPromise
}

export const scheduleTokenRefresh = () => {
  clearRefreshTimer()
  const accessToken = getAccessToken()
  if (!accessToken) {
    return
  }

  const delayMs = getRefreshDelayMs(accessToken)
  refreshTimerId = window.setTimeout(() => {
    void refreshTokens()
  }, delayMs)
}

export const startTokenAutoRefresh = () => {
  scheduleTokenRefresh()
}

export const logoutAndRevokeRefreshToken = async (): Promise<void> => {
  const accessToken = getAccessToken()
  const refreshToken = getRefreshToken()

  if (accessToken && refreshToken) {
    try {
      const payload: TokenRequestDto = {
        JwtToken: accessToken,
        RefreshToken: refreshToken,
      }

      await authHttpClient.post('/Auth/Logout', payload, {
        headers: {
          Authorization: `Bearer ${accessToken}`,
        },
      })
    } catch {
      // Revoke is best-effort; local cleanup still proceeds.
    }
  }

  clearTokens()
}
