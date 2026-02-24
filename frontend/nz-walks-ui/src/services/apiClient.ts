import axios from 'axios'
import { JWT_TOKEN_STORAGE_KEY } from '../constants/auth'

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL ?? 'https://localhost/api',
  headers: {
    'Content-Type': 'application/json',
  },
})

apiClient.interceptors.request.use((config) => {
  const token = localStorage.getItem(JWT_TOKEN_STORAGE_KEY)

  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }

  return config
})

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error?.response?.status === 401) {
      localStorage.removeItem(JWT_TOKEN_STORAGE_KEY)
    }

    return Promise.reject(error)
  },
)

export default apiClient
