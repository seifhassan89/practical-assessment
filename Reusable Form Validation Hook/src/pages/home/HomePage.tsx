import { useNavigate } from 'react-router-dom';

import { AppRoute } from '@/shared/config/appRoutes';
import { useAuth } from '@/features/auth/context/useAuth';
import { Button, Card, CardContent, CardHeader } from '@/shared/components';
import { useDocumentTitle } from '@/shared/hooks/useDocumentTitle';

export function HomePage() {
  useDocumentTitle('Home | Reusable Form Validation Hook');

  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate(AppRoute.Login, { replace: true });
  };

  return (
    <main className="min-h-screen bg-slate-50 px-4 py-10">
      <section className="w-full space-y-6">
        <Card>
          <CardHeader>
            <p className="text-sm font-semibold uppercase tracking-wide text-slate-500">
              Protected Route
            </p>

            <h1 className="mt-2 text-3xl font-bold tracking-tight text-slate-900">
              Welcome, {user?.name}
            </h1>

            <p className="mt-3 text-slate-600">You are logged in as {user?.email}.</p>
          </CardHeader>

          <CardContent className="space-y-4">
            <div className="rounded-xl bg-slate-50 p-4 text-sm text-slate-700">
              This page is protected using <strong>PrivateRoute</strong>. Login and Register forms
              will use the reusable validation hook.
            </div>

            <Button variant="secondary" onClick={handleLogout}>
              Logout
            </Button>
          </CardContent>
        </Card>
      </section>
    </main>
  );
}
