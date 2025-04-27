import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { logout, adminLogout } from '../lib/api';

interface AuthState {
  token: string | null;
  isAdmin: boolean;
  setToken: (token: string, isAdmin: boolean) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      isAdmin: false,
      setToken: (token, isAdmin) => {
        localStorage.setItem('token', token);
        set({ token, isAdmin });
      },
      logout: async () => {
        try {
          const token = get().token;
          if (token) {
            if (get().isAdmin) {
              await adminLogout(token);
            } else {
              await logout(token);
            }
          }
        } catch (error) {
          console.error('Error during logout:', error);
        } finally {
          localStorage.removeItem('token');
          set({ token: null, isAdmin: false });
        }
      },
    }),
    {
      name: 'auth-storage',
    }
  )
);