import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  isAdmin: boolean;
  setToken: (token: string, isAdmin: boolean) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      isAdmin: false,
      setToken: (token, isAdmin) => {
        localStorage.setItem('token', token);
        set({ token, isAdmin });
      },
      logout: () => {
        localStorage.removeItem('token');
        set({ token: null, isAdmin: false });
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);