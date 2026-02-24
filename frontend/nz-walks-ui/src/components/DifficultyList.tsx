import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import { difficultyService } from '../services/difficultyService'
import axios from 'axios'
import TableSkeletonRows from './ui/TableSkeletonRows'
import { useToast } from './ui/ToastProvider'
import type { DifficultyDto } from '../types'

const DifficultyList = () => {
  const [difficulties, setDifficulties] = useState<DifficultyDto[]>([])
  const [loading, setLoading] = useState(true)
  const [deletingId, setDeletingId] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [nameInput, setNameInput] = useState('')
  const [error, setError] = useState('')
  const { pushToast } = useToast()

  const getErrorMessage = (err: unknown, fallback: string): string => {
    if (axios.isAxiosError(err)) {
      return err.response?.data?.title || err.response?.data?.message || err.message || fallback
    }
    return err instanceof Error ? err.message : fallback
  }

  const loadDifficulties = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await difficultyService.getAll()
      setDifficulties(data)
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to load difficulties.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadDifficulties()
  }, [])

  const handleDelete = async (id: string, name: string) => {
    const isConfirmed = window.confirm(`Delete difficulty "${name}"?`)
    if (!isConfirmed) return

    setDeletingId(id)
    setError('')

    try {
      await difficultyService.delete(id)
      setDifficulties((prev) => prev.filter((difficulty) => difficulty.Id !== id))
      pushToast('Difficulty deleted.', 'success')
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to delete difficulty.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setDeletingId(null)
    }
  }

  const startEdit = (difficulty: DifficultyDto) => {
    setEditingId(difficulty.Id)
    setNameInput(difficulty.Name)
  }

  const cancelEdit = () => {
    setEditingId(null)
    setNameInput('')
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError('')
    setSaving(true)

    try {
      if (editingId) {
        const updated = await difficultyService.update(editingId, {
          Name: nameInput.trim(),
        })
        setDifficulties((prev) => prev.map((item) => (item.Id === editingId ? updated : item)))
        cancelEdit()
        pushToast('Difficulty updated.', 'success')
      } else {
        const created = await difficultyService.create({
          Name: nameInput.trim(),
        })
        setDifficulties((prev) => [created, ...prev])
        setNameInput('')
        pushToast('Difficulty created.', 'success')
      }
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to save difficulty.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setSaving(false)
    }
  }

  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <form className="mb-4 grid gap-3 sm:grid-cols-[1fr_auto_auto]" onSubmit={handleSubmit}>
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={nameInput}
          onChange={(event) => setNameInput(event.target.value)}
          placeholder="Difficulty name (e.g. Easy)"
          required
        />
        <button
          type="submit"
          className="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:opacity-70"
          disabled={saving}
        >
          {saving ? 'Saving...' : editingId ? 'Update Difficulty' : 'Add Difficulty'}
        </button>
        {editingId && (
          <button
            type="button"
            className="rounded-md bg-slate-600 px-3 py-2 text-sm font-semibold text-white hover:bg-slate-500"
            onClick={cancelEdit}
          >
            Cancel
          </button>
        )}
      </form>

      <div className="mb-4 flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-slate-800">Difficulty List</h2>
          <p className="text-sm text-slate-500">Data source: GET /api/Difficulties</p>
        </div>
        <button
          type="button"
          className="rounded-md bg-slate-800 px-3 py-2 text-sm font-medium text-white hover:bg-slate-700"
          onClick={() => void loadDifficulties()}
          disabled={loading}
        >
          Refresh
        </button>
      </div>

      {error && (
        <div className="mb-4 rounded-md border border-rose-200 bg-rose-50 px-3 py-2 text-sm text-rose-700">
          {error}
        </div>
      )}

      <div className="overflow-x-auto">
        <table className="min-w-full divide-y divide-slate-200 text-left text-sm">
          <thead className="bg-slate-50">
            <tr>
              <th className="px-4 py-3 font-semibold text-slate-700">Name</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 bg-white">
            {!loading &&
              difficulties.map((difficulty) => (
                <tr key={difficulty.Id}>
                  <td className="px-4 py-3 font-medium text-slate-700">{difficulty.Name}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        type="button"
                        className="rounded-md bg-amber-600 px-3 py-2 text-xs font-semibold text-white hover:bg-amber-500"
                        onClick={() => startEdit(difficulty)}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className="rounded-md bg-rose-600 px-3 py-2 text-xs font-semibold text-white hover:bg-rose-500 disabled:cursor-not-allowed disabled:opacity-70"
                        onClick={() => void handleDelete(difficulty.Id, difficulty.Name)}
                        disabled={deletingId === difficulty.Id}
                      >
                        {deletingId === difficulty.Id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

            {!loading && difficulties.length === 0 && (
              <tr>
                <td colSpan={2} className="px-4 py-6 text-center text-slate-500">
                  No difficulties found.
                </td>
              </tr>
            )}

            {loading && <TableSkeletonRows columns={2} rows={4} />}
          </tbody>
        </table>
      </div>
    </section>
  )
}

export default DifficultyList
