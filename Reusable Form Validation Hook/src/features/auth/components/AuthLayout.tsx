import type { ReactNode } from 'react';

type AuthLayoutProps = {
  title: string;
  subtitle: string;
  children: ReactNode;
};

export function AuthLayout({ title, subtitle, children }: AuthLayoutProps) {
  return (
    <main className="min-h-screen bg-slate-50 px-4 py-6 sm:py-8">
      <section className="auth-layout">
        <div className="w-full max-w-md rounded-2xl bg-white p-6 shadow-sm ring-1 ring-slate-200 sm:p-7">
          <div className="mb-5 text-center sm:mb-6">
            <h1 className="text-3xl font-bold tracking-tight text-slate-900">{title}</h1>
            <p className="mt-1.5 text-sm text-slate-600">{subtitle}</p>
          </div>

          {children}
        </div>
      </section>
    </main>
  );
}
