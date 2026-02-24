import { createContext, useCallback, useContext, useMemo, useState } from 'react'
import type { ReactNode } from 'react'

type ToastType = 'success' | 'error' | 'info'

type ToastItem = {
  id: number
  message: string
  type: ToastType
}

type ToastContextValue = {
  pushToast: (message: string, type?: ToastType) => void
}

const ToastContext = createContext<ToastContextValue | undefined>(undefined)

const toastTypeClassMap: Record<ToastType, string> = {
  success: 'border-emerald-200 bg-emerald-50 text-emerald-700',
  error: 'border-rose-200 bg-rose-50 text-rose-700',
  info: 'border-slate-200 bg-white text-slate-700',
}

export const ToastProvider = ({ children }: { children: ReactNode }) => {
  const [toasts, setToasts] = useState<ToastItem[]>([])

  const pushToast = useCallback((message: string, type: ToastType = 'info') => {
    const toastId = Date.now() + Math.floor(Math.random() * 1000)
    setToasts((prev) => [...prev, { id: toastId, message, type }])

    window.setTimeout(() => {
      setToasts((prev) => prev.filter((item) => item.id !== toastId))
    }, 2600)
  }, [])

  const contextValue = useMemo(() => ({ pushToast }), [pushToast])

  return (
    <ToastContext.Provider value={contextValue}>
      {children}
      <div className="pointer-events-none fixed right-4 top-4 z-50 flex w-full max-w-sm flex-col gap-2">
        {toasts.map((toast) => (
          <div
            key={toast.id}
            className={`rounded-md border px-3 py-2 text-sm shadow-sm ${toastTypeClassMap[toast.type]}`}
          >
            {toast.message}
          </div>
        ))}
      </div>
    </ToastContext.Provider>
  )
}

export const useToast = (): ToastContextValue => {
  const context = useContext(ToastContext)
  if (!context) {
    throw new Error('useToast must be used within ToastProvider.')
  }
  return context
}
