export type AuthUser = {
  id: string;
  name: string;
  email: string;
};

export type LoginCredentials = {
  email: string;
  password: string;
};

export type RegisterCredentials = {
  name: string;
  email: string;
  password: string;
};

export type AuthState = {
  user: AuthUser | null;
  isAuthenticated: boolean;
};

export type AuthContextValue = AuthState & {
  login: (credentials: LoginCredentials) => Promise<AuthUser>;
  register: (credentials: RegisterCredentials) => Promise<AuthUser>;
  logout: () => void;
};