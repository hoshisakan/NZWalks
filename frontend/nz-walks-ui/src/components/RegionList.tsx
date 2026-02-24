import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import axios from 'axios'
import { regionService } from '../services/regionService'
import TableSkeletonRows from './ui/TableSkeletonRows'
import { useToast } from './ui/ToastProvider'
import type { RegionDto } from '../types'

const RegionList = () => {
  const [regions, setRegions] = useState<RegionDto[]>([])
  const [loading, setLoading] = useState(true)
  const [deletingId, setDeletingId] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [formData, setFormData] = useState({
    Code: '',
    Name: '',
    RegionImageUrl: '',
  })
  const [error, setError] = useState('')
  const { pushToast } = useToast()

  const getErrorMessage = (err: unknown, fallback: string): string => {
    if (axios.isAxiosError(err)) {
      return err.response?.data?.title || err.response?.data?.message || err.message || fallback
    }
    return err instanceof Error ? err.message : fallback
  }

  const loadRegions = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await regionService.getAll()
      setRegions(data)
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to load regions.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    void loadRegions()
  }, [])

  const handleDelete = async (id: string, name: string) => {
    const isConfirmed = window.confirm(`Delete region "${name}"?`)
    if (!isConfirmed) return

    setDeletingId(id)
    setError('')

    try {
      await regionService.delete(id)
      setRegions((prev) => prev.filter((region) => region.Id !== id))
      pushToast('Region deleted.', 'success')
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to delete region.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setDeletingId(null)
    }
  }

  const startEdit = (region: RegionDto) => {
    setEditingId(region.Id)
    setFormData({
      Code: region.Code,
      Name: region.Name,
      RegionImageUrl: region.RegionImageUrl ?? '',
    })
  }

  const cancelEdit = () => {
    setEditingId(null)
    setFormData({
      Code: '',
      Name: '',
      RegionImageUrl: '',
    })
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setError('')
    setSaving(true)

    const payload = {
      Code: formData.Code.trim().toUpperCase(),
      Name: formData.Name.trim(),
      RegionImageUrl: formData.RegionImageUrl.trim() || undefined,
    }

    try {
      if (editingId) {
        const updated = await regionService.update(editingId, payload)
        setRegions((prev) => prev.map((item) => (item.Id === editingId ? updated : item)))
        cancelEdit()
        pushToast('Region updated.', 'success')
      } else {
        const created = await regionService.create(payload)
        setRegions((prev) => [created, ...prev])
        setFormData({
          Code: '',
          Name: '',
          RegionImageUrl: '',
        })
        pushToast('Region created.', 'success')
      }
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to save region.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setSaving(false)
    }
  }

  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <form className="mb-4 grid gap-3 md:grid-cols-3" onSubmit={handleSubmit}>
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.Code}
          onChange={(event) => setFormData((prev) => ({ ...prev, Code: event.target.value }))}
          placeholder="Region code (3 chars, e.g. AKL)"
          required
        />
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.Name}
          onChange={(event) => setFormData((prev) => ({ ...prev, Name: event.target.value }))}
          placeholder="Region name"
          required
        />
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.RegionImageUrl}
          onChange={(event) => setFormData((prev) => ({ ...prev, RegionImageUrl: event.target.value }))}
          placeholder="Region image URL (optional)"
        />
        <div className="flex gap-2 md:col-span-3">
          <button
            type="submit"
            className="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:opacity-70"
            disabled={saving}
          >
            {saving ? 'Saving...' : editingId ? 'Update Region' : 'Add Region'}
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
        </div>
      </form>

      <div className="mb-4 flex items-center justify-between">
        <div>
          <h2 className="text-xl font-semibold text-slate-800">Region List</h2>
          <p className="text-sm text-slate-500">Data source: GET /api/Regions</p>
        </div>
        <button
          type="button"
          className="rounded-md bg-slate-800 px-3 py-2 text-sm font-medium text-white hover:bg-slate-700"
          onClick={() => void loadRegions()}
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
              <th className="px-4 py-3 font-semibold text-slate-700">Code</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Name</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Image URL</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 bg-white">
            {!loading &&
              regions.map((region) => (
                <tr key={region.Id}>
                  <td className="px-4 py-3 font-medium text-slate-700">{region.Code}</td>
                  <td className="px-4 py-3 text-slate-700">{region.Name}</td>
                  <td className="px-4 py-3 text-slate-500">{region.RegionImageUrl ?? '-'}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        type="button"
                        className="rounded-md bg-amber-600 px-3 py-2 text-xs font-semibold text-white hover:bg-amber-500"
                        onClick={() => startEdit(region)}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className="rounded-md bg-rose-600 px-3 py-2 text-xs font-semibold text-white hover:bg-rose-500 disabled:cursor-not-allowed disabled:opacity-70"
                        onClick={() => void handleDelete(region.Id, region.Name)}
                        disabled={deletingId === region.Id}
                      >
                        {deletingId === region.Id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

            {!loading && regions.length === 0 && (
              <tr>
                <td colSpan={4} className="px-4 py-6 text-center text-slate-500">
                  No regions found.
                </td>
              </tr>
            )}

            {loading && <TableSkeletonRows columns={4} rows={4} />}
          </tbody>
        </table>
      </div>
    </section>
  )
}

export default RegionList
