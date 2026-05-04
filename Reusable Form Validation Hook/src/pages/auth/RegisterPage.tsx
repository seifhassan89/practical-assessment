import { Link } from 'react-router-dom';

import { AppRoute } from '@/shared/config/appRoutes';
import { AuthLayout } from '@/features/auth/components/AuthLayout';
import { RegisterForm } from '@/features/auth/components/RegisterForm';
import { useDocumentTitle } from '@/shared/hooks/useDocumentTitle';

export function RegisterPage() {
  useDocumentTitle('Register | Reusable Form Validation Hook');

  return (
    <AuthLayout title="Create account" subtitle="Register to test the reusable validation hook">
      <div className="space-y-4">
        <RegisterForm />

        <p className="text-center text-sm text-slate-600">
          Already have an account?{' '}
          <Link className="font-semibold text-slate-900 hover:underline" to={AppRoute.Login}>
            Login
          </Link>
        </p>
      </div>
    </AuthLayout>
  );
}
