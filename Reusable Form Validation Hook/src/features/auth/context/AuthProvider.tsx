import { useMemo, useState } from 'react';

import { authService } from '@/features/auth/services/auth.service';
import type {
  AuthContextValue,
  AuthUser,
  LoginCredentials,
  RegisterCredentials,
} from '@/features/auth/types/auth.types';

import { AuthContext } from './AuthContext';

type AuthProviderProps = {
  children: React.ReactNode;
};

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<AuthUser | null>(() => {
    return authService.getCurrentUser();
  });

  const login = async (credentials: LoginCredentials) => {
    const loggedInUser = await authService.login(credentials);
    setUser(loggedInUser);

    return loggedInUser;
  };

  const register = async (credentials: RegisterCredentials) => {
    const registeredUser = await authService.register(credentials);
    setUser(registeredUser);

    return registeredUser;
  };

  const logout = () => {
    authService.logout();
    setUser(null);
  };

  const value = useMemo<AuthContextValue>(
    () => ({
      user,
      isAuthenticated: Boolean(user),
      login,
      register,
      logout,
    }),
    [user]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}
