import Dashboard from './pages/Dashboard.tsx'
import RegisterPage from './pages/RegisterPage.tsx'
import { Navigate, Route, Routes } from 'react-router-dom'

function App() {
  return (
    <Routes>
      <Route path="/" element={<Dashboard />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App
