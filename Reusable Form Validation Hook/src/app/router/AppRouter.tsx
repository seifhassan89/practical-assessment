import { Navigate, Route, Routes } from 'react-router-dom';

import { LoginPage, RegisterPage } from '@/pages/auth';
import { HomePage } from '@/pages/home';

import { NotFoundPage } from '@/pages/system';
import { PrivateRoute } from './PrivateRoute';
import { PublicOnlyRoute } from './PublicOnlyRoute';
import { AppRoute } from '@/shared/config/appRoutes';

export function AppRouter() {
  return (
    <Routes>
      <Route element={<PublicOnlyRoute />}>
        <Route path={AppRoute.Login} element={<LoginPage />} />
        <Route path={AppRoute.Register} element={<RegisterPage />} />
      </Route>

      <Route element={<PrivateRoute />}>
        <Route path={AppRoute.Home} element={<HomePage />} />
      </Route>

      <Route path={AppRoute.Root} element={<Navigate to={AppRoute.Home} replace />} />
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  );
}
