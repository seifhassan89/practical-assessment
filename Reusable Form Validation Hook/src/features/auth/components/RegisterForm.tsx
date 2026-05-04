import { useRef } from 'react';
import { useNavigate } from 'react-router-dom';

import { useAuth } from '@/features/auth/context/useAuth';
import {
  registerInitialValues,
  registerValidationSchema,
} from '@/features/auth/schemas/register.schema';
import type { RegisterFormValues } from '@/features/auth/types/register.types';

import { Button, DebugPanel, ErrorSummary, FormField } from '@/shared/components';
import { AppRoute } from '@/shared/config/appRoutes';
import { useFocusFirstError } from '@/shared/hooks/useFocusFirstError';
import { useFormValidation } from '@/shared/hooks/useFormValidation';

export function RegisterForm() {
  const navigate = useNavigate();
  const { register } = useAuth();

  const form = useFormValidation<RegisterFormValues>({
    initialValues: registerInitialValues,
    validationSchema: registerValidationSchema,
    mode: 'onBlur',
    reValidateMode: 'onChange',
  });

  const handleRegister = async (values: RegisterFormValues) => {
    const credentials = {
      name: values.name,
      email: values.email,
      password: values.password,
    };

    await register(credentials);
    navigate(AppRoute.Home, { replace: true });
  };

  const nameMeta = form.getFieldMeta('name');
  const emailMeta = form.getFieldMeta('email');
  const passwordMeta = form.getFieldMeta('password');
  const confirmPasswordMeta = form.getFieldMeta('confirmPassword');
  const errorSummaryRef = useRef<HTMLDivElement>(null);

  useFocusFirstError<RegisterFormValues>({
    submitCount: form.submitCount,
    errors: form.errors,
    summaryRef: errorSummaryRef,
  });

  return (
    <div className="space-y-4">
      {form.submitCount > 0 && (
        <ErrorSummary
          errors={form.errors}
          fieldLabels={{
            name: 'Name',
            email: 'Email',
            password: 'Password',
            confirmPassword: 'Confirm password',
          }}
          summaryRef={errorSummaryRef}
        />
      )}

      <form
        className="space-y-3"
        onSubmit={form.handleSubmit(handleRegister)}
        noValidate
        aria-busy={form.isSubmitting}
      >
        <FormField
          label="Name"
          type="text"
          placeholder="John Doe"
          autoComplete="name"
          required
          {...form.getFieldProps('name')}
          error={nameMeta.error}
          touched={nameMeta.touched || form.submitCount > 0}
        />

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
          placeholder="At least 8 characters"
          autoComplete="new-password"
          required
          {...form.getFieldProps('password')}
          error={passwordMeta.error}
          touched={passwordMeta.touched || form.submitCount > 0}
        />

        <FormField
          label="Confirm password"
          type="password"
          placeholder="Repeat your password"
          autoComplete="new-password"
          required
          {...form.getFieldProps('confirmPassword')}
          error={confirmPasswordMeta.error}
          touched={confirmPasswordMeta.touched || form.submitCount > 0}
        />

        <div className="flex flex-col gap-3 sm:flex-row">
          <Button type="submit" fullWidth disabled={form.isSubmitting}>
            {form.isSubmitting ? 'Creating account...' : 'Create account'}
          </Button>

          <Button
            type="button"
            variant="secondary"
            onClick={form.resetForm}
            disabled={!form.isDirty || form.isSubmitting}
          >
            Reset
          </Button>
        </div>
        <p className="sr-only" role="status" aria-live="polite">
          {form.isSubmitting ? 'Creating account, please wait.' : ''}
        </p>
      </form>

      <DebugPanel
        title="Register form state"
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
