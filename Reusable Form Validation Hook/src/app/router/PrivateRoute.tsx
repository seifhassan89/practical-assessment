import { Navigate, Outlet, useLocation } from 'react-router-dom';

import { useAuth } from '@/features/auth/context/useAuth';

import { AppRoute } from '@/shared/config/appRoutes';

export function PrivateRoute() {
  const { isAuthenticated } = useAuth();
  const location = useLocation();

  if (!isAuthenticated) {
    return <Navigate to={AppRoute.Login} replace state={{ from: location }} />;
  }

  return <Outlet />;
}
