import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/context/useAuth';
import { loginInitialValues, loginValidationSchema } from '@/features/auth/schemas/login.schema';
import type { LoginFormValues } from '@/features/auth/types/login.types';

import { Button, DebugPanel, ErrorSummary, FormField } from '@/shared/components';
import { AppRoute } from '@/shared/config/appRoutes';
import { useFocusFirstError } from '@/shared/hooks/useFocusFirstError';
import { useFormValidation } from '@/shared/hooks/useFormValidation';

export function LoginForm() {
  const navigate = useNavigate();
  const { login } = useAuth();

  const form = useFormValidation<LoginFormValues>({
    initialValues: loginInitialValues,
    validationSchema: loginValidationSchema,
    mode: 'onBlur',
    reValidateMode: 'onChange',
  });

  const handleLogin = async (values: LoginFormValues) => {
    await login(values);
    navigate(AppRoute.Home, { replace: true });
  };

  const emailMeta = form.getFieldMeta('email');
  const passwordMeta = form.getFieldMeta('password');
  const errorSummaryRef = useRef<HTMLDivElement>(null);

  useFocusFirstError<LoginFormValues>({
    submitCount: form.submitCount,
    errors: form.errors,
    summaryRef: errorSummaryRef,
  });

  return (
    <div className="space-y-4">
      <div className="space-y-4">
        {form.submitCount > 0 && (
          <ErrorSummary
            errors={form.errors}
            fieldLabels={{
              email: 'Email',
              password: 'Password',
            }}
            summaryRef={errorSummaryRef}
          />
        )}

        <form
          className="space-y-3"
          onSubmit={form.handleSubmit(handleLogin)}
          noValidate
          aria-busy={form.isSubmitting}
        >
          <FormField
            label="Email"
            type="email"
            placeholder="john@example.com"
            autoComplete="email"
            required
            {...form.getFieldProps('email')}
            error={emailMeta.error}
            touched={emailMeta.touched || form.submitCount > 0}
          />

          <FormField
            label="Password"
            type="password"
            placeholder="Enter your password"
            autoComplete="current-password"
            required
            {...form.getFieldProps('password')}
            error={passwordMeta.error}
            touched={passwordMeta.touched || form.submitCount > 0}
          />

          <Button type="submit" fullWidth disabled={form.isSubmitting}>
            {form.isSubmitting ? 'Logging in...' : 'Login'}
          </Button>
          <p className="sr-only" role="status" aria-live="polite">
            {form.isSubmitting ? 'Logging in, please wait.' : ''}
          </p>
        </form>
      </div>
      <DebugPanel
        title="Login form state"
        data={{
          values: form.values,
          errors: form.errors,
          touched: form.touched,
          dirtyFields: form.dirtyFields,
          isValid: form.isValid,
          isDirty: form.isDirty,
          submitCount: form.submitCount,
        }}
      />
    </div>
  );
}
