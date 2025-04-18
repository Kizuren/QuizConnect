import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { Login } from './pages/Login';
import { Dashboard } from './pages/Dashboard';
import { AdminDashboard } from './pages/AdminDashboard';
import { Quiz } from './pages/Quiz';
import { useAuthStore } from './store/auth';

const queryClient = new QueryClient();

function App() {
  const { token, isAdmin } = useAuthStore();

  return (
    <QueryClientProvider client={queryClient}>
      <Router>
        <Routes>
          <Route path="/login" element={!token ? <Login /> : <Navigate to={isAdmin ? "/admin" : "/dashboard"} />} />
          <Route path="/dashboard" element={token && !isAdmin ? <Dashboard /> : <Navigate to={isAdmin ? "/admin" : "/login"} />} />
          <Route path="/admin" element={token && isAdmin ? <AdminDashboard /> : <Navigate to="/login" />} />
          <Route path="/quiz/:questionSetId" element={token && !isAdmin ? <Quiz /> : <Navigate to="/login" />} />
          <Route path="*" element={<Navigate to={token ? (isAdmin ? "/admin" : "/dashboard") : "/login"} />} />
        </Routes>
      </Router>
    </QueryClientProvider>
  );
}

export default App;