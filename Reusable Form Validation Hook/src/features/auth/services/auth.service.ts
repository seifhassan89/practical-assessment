import type {
  AuthUser,
  LoginCredentials,
  RegisterCredentials,
} from '../types/auth.types';

const AUTH_STORAGE_KEY = 'reusable-form-validation-auth-user';

function wait(milliseconds = 500) {
  return new Promise((resolve) => {
    window.setTimeout(resolve, milliseconds);
  });
}

export const authService = {
  async login(credentials: LoginCredentials): Promise<AuthUser> {
    await wait();

    const user: AuthUser = {
      id: crypto.randomUUID(),
      name: credentials.email.split('@')[0] || 'User',
      email: credentials.email,
    };

    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));

    return user;
  },

  async register(credentials: RegisterCredentials): Promise<AuthUser> {
    await wait();

    const user: AuthUser = {
      id: crypto.randomUUID(),
      name: credentials.name,
      email: credentials.email,
    };

    localStorage.setItem(AUTH_STORAGE_KEY, JSON.stringify(user));

    return user;
  },

  logout(): void {
    localStorage.removeItem(AUTH_STORAGE_KEY);
  },

  getCurrentUser(): AuthUser | null {
    const storedUser = localStorage.getItem(AUTH_STORAGE_KEY);

    if (!storedUser) {
      return null;
    }

    try {
      return JSON.parse(storedUser) as AuthUser;
    } catch {
      localStorage.removeItem(AUTH_STORAGE_KEY);
      return null;
    }
  },
};