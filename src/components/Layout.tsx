import React, { useEffect, useRef } from 'react';
import { useNavigate } from 'react-router-dom';
import { LogOut } from 'lucide-react';
import { Button } from './Button';
import { useAuthStore } from '../store/auth';
import { checkUserResetState } from '../lib/api';

interface LayoutProps {
  children: React.ReactNode;
}

export const Layout: React.FC<LayoutProps> = ({ children }) => {
  const navigate = useNavigate();
  const { logout, isAdmin } = useAuthStore();
  const pollingIntervalRef = useRef<number | null>(null);

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  // Check if the user has been reset by an admin
  useEffect(() => {
    if (!isAdmin) {
      const checkResetState = async () => {
        try {
          const resetState = await checkUserResetState();
          if (resetState === true) {
            console.log('User has been reset by admin. Logging out...');
            logout();
            navigate('/login', { 
              state: { 
                message: 'Your account has been reset by an administrator. Please log in again.' 
              } 
            });
          }
        } catch (error) {
          console.error('Error checking reset state:', error);
        }
      };

      checkResetState();
      pollingIntervalRef.current = window.setInterval(checkResetState, 5000);

      return () => {
        if (pollingIntervalRef.current !== null) {
          clearInterval(pollingIntervalRef.current);
        }
      };
    }
  }, [isAdmin, logout, navigate]);

  return (
    <div className="min-h-screen bg-gray-900">
      <header className="bg-gray-800 border-b border-gray-700">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 h-16 flex items-center justify-between">
          <h1 className="text-xl font-bold text-white">
            QuizConnect {isAdmin && <span className="text-blue-500">(Admin)</span>}
          </h1>
          <Button
            variant="secondary"
            size="sm"
            onClick={handleLogout}
            className="flex items-center gap-2"
          >
            <LogOut className="w-4 h-4" />
            Logout
          </Button>
        </div>
      </header>
      <main className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        {children}
      </main>
    </div>
  );
};