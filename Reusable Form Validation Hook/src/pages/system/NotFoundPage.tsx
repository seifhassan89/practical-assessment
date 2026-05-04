import { Link } from 'react-router-dom';

import { AppRoute } from '@/shared/config/appRoutes';
import { useDocumentTitle } from '@/shared/hooks/useDocumentTitle';

export function NotFoundPage() {
  useDocumentTitle('Page Not Found | Reusable Form Validation Hook');

  return (
    <main className="grid min-h-screen place-items-center bg-slate-50 px-4">
      <section className="text-center">
        <p className="text-sm font-semibold uppercase tracking-wide text-slate-500">404</p>

        <h1 className="mt-2 text-3xl font-bold text-slate-900">Page not found</h1>

        <p className="mt-3 text-slate-600">The page you are looking for does not exist.</p>

        <Link
          className="mt-6 inline-flex rounded-xl bg-slate-900 px-4 py-2 text-sm font-semibold text-white hover:bg-slate-800"
          to={AppRoute.Login}
        >
          Go to login
        </Link>
      </section>
    </main>
  );
}
