import { useEffect, useState } from 'react'
import type { FormEvent } from 'react'
import axios from 'axios'
import { walkService } from '../services/walkService'
import { regionService } from '../services/regionService'
import { difficultyService } from '../services/difficultyService'
import TableSkeletonRows from './ui/TableSkeletonRows'
import { useToast } from './ui/ToastProvider'
import type { DifficultyDto, RegionDto, WalkDto } from '../types'

const defaultFormData = {
  Name: '',
  Description: '',
  LengthInKm: '',
  WalkImageUrl: '',
  DifficultyId: '',
  RegionId: '',
}

type WalkListProps = {
  authToken: string
}

const WalkList = ({ authToken }: WalkListProps) => {
  const [walks, setWalks] = useState<WalkDto[]>([])
  const [regions, setRegions] = useState<RegionDto[]>([])
  const [difficulties, setDifficulties] = useState<DifficultyDto[]>([])
  const [loading, setLoading] = useState(true)
  const [optionsLoading, setOptionsLoading] = useState(false)
  const [deletingId, setDeletingId] = useState<string | null>(null)
  const [saving, setSaving] = useState(false)
  const [editingId, setEditingId] = useState<string | null>(null)
  const [error, setError] = useState('')
  const [formData, setFormData] = useState(defaultFormData)
  const { pushToast } = useToast()

  const getErrorMessage = (err: unknown, fallback: string): string => {
    if (axios.isAxiosError(err)) {
      return err.response?.data?.title || err.response?.data?.message || err.message || fallback
    }
    return err instanceof Error ? err.message : fallback
  }

  const hydrateWalkDisplayFields = (walk: WalkDto): WalkDto => {
    const matchedDifficulty = difficulties.find((item) => item.Id === walk.DifficultyId)
    const matchedRegion = regions.find((item) => item.Id === walk.RegionId)

    return {
      ...walk,
      Difficulty:
        walk.Difficulty?.Name
          ? walk.Difficulty
          : {
              Id: walk.DifficultyId,
              Name: matchedDifficulty?.Name ?? '',
            },
      Region:
        walk.Region?.Name
          ? walk.Region
          : {
              Id: walk.RegionId,
              Code: matchedRegion?.Code ?? '',
              Name: matchedRegion?.Name ?? '',
              RegionImageUrl: matchedRegion?.RegionImageUrl,
            },
    }
  }

  const loadWalks = async () => {
    setLoading(true)
    setError('')

    try {
      const data = await walkService.getAll()
      setWalks(data)
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to load walks.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setLoading(false)
    }
  }

  const loadDependencies = async () => {
    setOptionsLoading(true)
    try {
      const [regionsData, difficultiesData] = await Promise.all([
        regionService.getAll(),
        difficultyService.getAll(),
      ])
      setRegions(regionsData)
      setDifficulties(difficultiesData)
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to load form dependencies.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setOptionsLoading(false)
    }
  }

  useEffect(() => {
    void loadWalks()
  }, [])

  useEffect(() => {
    if (!authToken) {
      setRegions([])
      setDifficulties([])
      setFormData((prev) => ({ ...prev, DifficultyId: '', RegionId: '' }))
      return
    }

    void loadDependencies()
  }, [authToken])

  const startEdit = (walk: WalkDto) => {
    setEditingId(walk.Id)
    setFormData({
      Name: walk.Name,
      Description: walk.Description,
      LengthInKm: String(walk.LengthInKm),
      WalkImageUrl: walk.WalkImageUrl ?? '',
      DifficultyId: walk.DifficultyId,
      RegionId: walk.RegionId,
    })
  }

  const cancelEdit = () => {
    setEditingId(null)
    setFormData(defaultFormData)
  }

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setSaving(true)
    setError('')

    const payload = {
      Name: formData.Name.trim(),
      Description: formData.Description.trim(),
      LengthInKm: Number(formData.LengthInKm),
      WalkImageUrl: formData.WalkImageUrl.trim() || undefined,
      DifficultyId: formData.DifficultyId,
      RegionId: formData.RegionId,
    }

    try {
      if (editingId) {
        const updated = await walkService.update(editingId, payload)
        const displayWalk = hydrateWalkDisplayFields(updated)
        setWalks((prev) => prev.map((item) => (item.Id === editingId ? displayWalk : item)))
        cancelEdit()
        pushToast('Walk updated.', 'success')
      } else {
        const created = await walkService.create(payload)
        const displayWalk = hydrateWalkDisplayFields(created)
        setWalks((prev) => [displayWalk, ...prev])
        setFormData(defaultFormData)
        pushToast('Walk created.', 'success')
      }
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to save walk.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setSaving(false)
    }
  }

  const handleDelete = async (id: string, name: string) => {
    const isConfirmed = window.confirm(`Delete walk "${name}"?`)
    if (!isConfirmed) return

    setDeletingId(id)
    setError('')

    try {
      await walkService.delete(id)
      setWalks((prev) => prev.filter((walk) => walk.Id !== id))
      pushToast('Walk deleted.', 'success')
    } catch (err) {
      const message = getErrorMessage(err, 'Failed to delete walk.')
      setError(message)
      pushToast(message, 'error')
    } finally {
      setDeletingId(null)
    }
  }

  return (
    <section className="rounded-xl border border-slate-200 bg-white p-6 shadow-sm">
      <form className="mb-4 grid gap-3 md:grid-cols-2" onSubmit={handleSubmit}>
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.Name}
          onChange={(event) => setFormData((prev) => ({ ...prev, Name: event.target.value }))}
          placeholder="Walk name"
          required
        />
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          type="number"
          min="0"
          step="0.1"
          value={formData.LengthInKm}
          onChange={(event) => setFormData((prev) => ({ ...prev, LengthInKm: event.target.value }))}
          placeholder="Length in km"
          required
        />
        <textarea
          className="rounded-md border border-slate-300 px-3 py-2 text-sm md:col-span-2"
          value={formData.Description}
          onChange={(event) => setFormData((prev) => ({ ...prev, Description: event.target.value }))}
          placeholder="Description"
          rows={3}
          required
        />
        <input
          className="rounded-md border border-slate-300 px-3 py-2 text-sm md:col-span-2"
          value={formData.WalkImageUrl}
          onChange={(event) => setFormData((prev) => ({ ...prev, WalkImageUrl: event.target.value }))}
          placeholder="Walk image URL (optional)"
        />
        <select
          aria-label="Select difficulty"
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.DifficultyId}
          onChange={(event) => setFormData((prev) => ({ ...prev, DifficultyId: event.target.value }))}
          required
        >
          <option value="">Select difficulty</option>
          {difficulties.map((difficulty) => (
            <option key={difficulty.Id} value={difficulty.Id}>
              {difficulty.Name}
            </option>
          ))}
        </select>
        <select
          aria-label="Select region"
          className="rounded-md border border-slate-300 px-3 py-2 text-sm"
          value={formData.RegionId}
          onChange={(event) => setFormData((prev) => ({ ...prev, RegionId: event.target.value }))}
          required
        >
          <option value="">Select region</option>
          {regions.map((region) => (
            <option key={region.Id} value={region.Id}>
              {region.Name}
            </option>
          ))}
        </select>
        <div className="flex gap-2 md:col-span-2">
          <button
            type="submit"
            className="rounded-md bg-blue-600 px-3 py-2 text-sm font-semibold text-white hover:bg-blue-500 disabled:opacity-70"
            disabled={saving}
          >
            {saving ? 'Saving...' : editingId ? 'Update Walk' : 'Add Walk'}
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
          <h2 className="text-xl font-semibold text-slate-800">Walk List</h2>
          <p className="text-sm text-slate-500">Data source: GET /api/Walks</p>
          {!authToken && (
            <p className="text-xs text-amber-700">Login required to load region/difficulty options.</p>
          )}
        </div>
        <div className="flex gap-2">
          <button
            type="button"
            className="rounded-md bg-slate-800 px-3 py-2 text-sm font-medium text-white hover:bg-slate-700"
            onClick={() => void loadWalks()}
            disabled={loading}
          >
            Refresh walks
          </button>
          <button
            type="button"
            className="rounded-md bg-indigo-700 px-3 py-2 text-sm font-medium text-white hover:bg-indigo-600 disabled:opacity-70"
            onClick={() => void loadDependencies()}
            disabled={optionsLoading || !authToken}
          >
            {optionsLoading ? 'Loading options...' : 'Refresh options'}
          </button>
        </div>
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
              <th className="px-4 py-3 font-semibold text-slate-700">Length (km)</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Difficulty</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Region</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Description</th>
              <th className="px-4 py-3 font-semibold text-slate-700">Actions</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-slate-100 bg-white">
            {!loading &&
              walks.map((walk) => (
                <tr key={walk.Id}>
                  <td className="px-4 py-3 font-medium text-slate-700">{walk.Name}</td>
                  <td className="px-4 py-3 text-slate-700">{walk.LengthInKm}</td>
                  <td className="px-4 py-3 text-slate-700">{walk.Difficulty?.Name ?? '-'}</td>
                  <td className="px-4 py-3 text-slate-700">{walk.Region?.Name ?? '-'}</td>
                  <td className="max-w-xs truncate px-4 py-3 text-slate-500">{walk.Description}</td>
                  <td className="px-4 py-3">
                    <div className="flex gap-2">
                      <button
                        type="button"
                        className="rounded-md bg-amber-600 px-3 py-2 text-xs font-semibold text-white hover:bg-amber-500"
                        onClick={() => startEdit(walk)}
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        className="rounded-md bg-rose-600 px-3 py-2 text-xs font-semibold text-white hover:bg-rose-500 disabled:cursor-not-allowed disabled:opacity-70"
                        onClick={() => void handleDelete(walk.Id, walk.Name)}
                        disabled={deletingId === walk.Id}
                      >
                        {deletingId === walk.Id ? 'Deleting...' : 'Delete'}
                      </button>
                    </div>
                  </td>
                </tr>
              ))}

            {!loading && walks.length === 0 && (
              <tr>
                <td colSpan={6} className="px-4 py-6 text-center text-slate-500">
                  No walks found.
                </td>
              </tr>
            )}

            {loading && <TableSkeletonRows columns={6} rows={4} />}
          </tbody>
        </table>
      </div>
    </section>
  )
}

export default WalkList
