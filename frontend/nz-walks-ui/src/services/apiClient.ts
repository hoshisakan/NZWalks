import axios from 'axios'
import { getAccessToken, refreshTokens } from './authService'

type RetryableRequestConfig = {
  _retry?: boolean
  url?: string
  headers?: Record<string, string>
}

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'https://localhost/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = getAccessToken()

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  async (error) => {
    const requestConfig = (error.config ?? {}) as RetryableRequestConfig
    const requestUrl = requestConfig.url ?? ''
    const isAuthEndpoint =
      requestUrl.includes('/Auth/Login') ||
      requestUrl.includes('/Auth/Register') ||
      requestUrl.includes('/Auth/Refresh-Token') ||
      requestUrl.includes('/Auth/Logout')

    if (error?.response?.status === 401 && !requestConfig._retry && !isAuthEndpoint) {
      requestConfig._retry = true

      const newAccessToken = await refreshTokens()
      if (newAccessToken) {
        requestConfig.headers = {
          ...(requestConfig.headers ?? {}),
          Authorization: `Bearer ${newAccessToken}`,
        }
        return apiClient(requestConfig)
      }
    }

    return Promise.reject(error)
  },
)

export default apiClient
