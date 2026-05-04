import { Link } from 'react-router-dom';

import { AppRoute } from '@/shared/config/appRoutes';
import { AuthLayout } from '@/features/auth/components/AuthLayout';
import { LoginForm } from '@/features/auth/components/LoginForm';
import { useDocumentTitle } from '@/shared/hooks/useDocumentTitle';

export function LoginPage() {
  useDocumentTitle('Login | Reusable Form Validation Hook');

  return (
    <AuthLayout title="Welcome back" subtitle="Login to access your protected workspace">
      <div className="space-y-4">
        <LoginForm />

        <p className="text-center text-sm text-slate-600">
          Do not have an account?{' '}
          <Link className="font-semibold text-slate-900 hover:underline" to={AppRoute.Register}>
            Create account
          </Link>
        </p>
      </div>
    </AuthLayout>
  );
}
