import { useState } from 'react'
import type { FormEvent } from 'react'
import axios from 'axios'
import { Link } from 'react-router-dom'
import RegionList from '../components/RegionList'
import DifficultyList from '../components/DifficultyList'
import WalkList from '../components/WalkList'
import { useToast } from '../components/ui/ToastProvider'
import { JWT_TOKEN_STORAGE_KEY } from '../constants/auth'
import apiClient from '../services/apiClient'
import type { LoginResponseDto } from '../types'

const Dashboard = () => {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [token, setToken] = useState(localStorage.getItem(JWT_TOKEN_STORAGE_KEY) ?? '')
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [error, setError] = useState('')
  const { pushToast } = useToast()

  const handleLogin = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      const response = await apiClient.post<LoginResponseDto>('/Auth/Login', {
        Email: email.trim(),
        Password: password,
      })

      const tokenFromResponse =
        response.data.JWTToken ??
        (response.data as LoginResponseDto & { jwtToken?: string }).jwtToken

      if (!tokenFromResponse) {
        throw new Error('JWT token is missing in login response.')
      }

      localStorage.setItem(JWT_TOKEN_STORAGE_KEY, tokenFromResponse)
      setToken(tokenFromResponse)
      pushToast('Login successful.', 'success')
    } catch (err) {
      const message = axios.isAxiosError(err)
        ? err.response?.data?.title ||
          err.response?.data?.message ||
          err.response?.data ||
          err.message
        : err instanceof Error
          ? err.message
          : 'Login failed.'
      setError(message)
      pushToast(message, 'error')
    } finally {
      setIsSubmitting(false)
    }
  }

  const handleLogout = () => {
    localStorage.removeItem(JWT_TOKEN_STORAGE_KEY)
    setToken('')
    pushToast('Logged out.', 'info')
  }

  return (
    <main className="min-h-screen bg-slate-50 p-6 md:p-10">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-6">
        <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
          <h1 className="text-2xl font-bold text-slate-800">NZWalks Management Dashboard</h1>
          <p className="mt-1 text-sm text-slate-500">
            Login API: <code className="rounded bg-slate-100 px-1 py-0.5">POST https://localhost/api/Auth/Login</code>
          </p>
          <p className="text-sm text-slate-500">
            Regions API: <code className="rounded bg-slate-100 px-1 py-0.5">https://localhost/api/Regions</code>
          </p>
          <p className="text-sm text-slate-500">
            Difficulties API:{' '}
            <code className="rounded bg-slate-100 px-1 py-0.5">https://localhost/api/Difficulties</code>
          </p>
          <p className="text-sm text-slate-500">
            Walks API: <code className="rounded bg-slate-100 px-1 py-0.5">https://localhost/api/Walks</code>
          </p>

          <form className="mt-4 grid gap-3 md:max-w-md" onSubmit={handleLogin}>
            <input
              className="rounded-md border border-slate-300 px-3 py-2 text-sm"
              type="email"
              value={email}
              onChange={(event) => setEmail(event.target.value)}
              placeholder="Email"
              required
            />
            <input
              className="rounded-md border border-slate-300 px-3 py-2 text-sm"
              type="password"
              value={password}
              onChange={(event) => setPassword(event.target.value)}
              placeholder="Password"
              required
            />
            <div className="flex gap-2">
              <button
                type="submit"
                className="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-500"
                disabled={isSubmitting}
              >
                {isSubmitting ? 'Logging in...' : 'Login'}
              </button>
              <button
                type="button"
                className="rounded-md bg-slate-700 px-3 py-2 text-sm font-semibold text-white hover:bg-slate-600"
                onClick={handleLogout}
              >
                Logout
              </button>
            </div>
          </form>

          <p className="mt-3 text-sm text-slate-600">
            Need an account?{' '}
            <Link className="font-semibold text-blue-600 hover:text-blue-500" to="/register">
              Go to register page
            </Link>
          </p>

          {error && (
            <p className="mt-3 rounded-md border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
              {error}
            </p>
          )}

          <p className="mt-3 text-sm font-medium text-slate-700">
            Token status: {token ? 'Ready' : 'Not found'}
          </p>
        </section>

        <RegionList />
        <DifficultyList />
        <WalkList authToken={token} />
      </div>
    </main>
  )
}

export default Dashboard
