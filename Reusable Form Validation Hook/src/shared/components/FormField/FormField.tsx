import { useId } from 'react';
import type { InputHTMLAttributes } from 'react';

import { cn } from '@/shared/lib/cn';
import { getFieldInputId } from '@/shared/lib/formFieldIds';

export type FormFieldProps = {
  label: string;
  error?: string;
  hint?: string;
  touched?: boolean;
  required?: boolean;
} & InputHTMLAttributes<HTMLInputElement>;

export function FormField({
  id,
  label,
  error,
  hint,
  touched,
  required,
  className,
  ...props
}: FormFieldProps) {
  const generatedId = useId();
  const inputId =
    id ??
    (props.name
      ? getFieldInputId(String(props.name))
      : `field-${generatedId.replace(/[^a-zA-Z0-9_-]/g, '')}`);
  const shouldShowError = Boolean(touched && error);

  const hintId = hint && inputId ? `${inputId}-hint` : undefined;
  const errorId = shouldShowError && inputId ? `${inputId}-error` : undefined;
  const describedBy = [hintId, errorId].filter(Boolean).join(' ') || undefined;

  return (
    <div className={cn('form-field', className)}>
      <label className="form-field__label" htmlFor={inputId}>
        {label}
        {required && (
          <span className="form-field__required" aria-hidden="true">
            *
          </span>
        )}
      </label>

      <input
        id={inputId}
        className="form-field__input"
        aria-invalid={shouldShowError ? 'true' : undefined}
        aria-describedby={describedBy}
        {...props}
      />

      {hint && !shouldShowError && (
        <p id={hintId} className="form-field__hint">
          {hint}
        </p>
      )}

      {shouldShowError && (
        <p id={errorId} className="form-field__error" aria-live="assertive">
          {error}
        </p>
      )}
    </div>
  );
}
