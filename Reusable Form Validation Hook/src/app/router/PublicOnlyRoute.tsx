import { Navigate, Outlet } from 'react-router-dom';

import { useAuth } from '@/features/auth/context/useAuth';

import { AppRoute } from '@/shared/config/appRoutes';

export function PublicOnlyRoute() {
  const { isAuthenticated } = useAuth();

  if (isAuthenticated) {
    return <Navigate to={AppRoute.Home} replace />;
  }

  return <Outlet />;
}
