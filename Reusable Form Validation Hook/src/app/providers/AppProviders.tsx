import { BrowserRouter } from 'react-router-dom';

import { AuthProvider } from '@/features/auth/context/AuthProvider';

type AppProvidersProps = {
  children: React.ReactNode;
};

export function AppProviders({ children }: AppProvidersProps) {
  return (
    <BrowserRouter>
      <AuthProvider>{children}</AuthProvider>
    </BrowserRouter>
  );
}
