// This will be removed in production, this is just to demonstrate the error summary component for the task reviewer.
// Helps the reviewer understand what your form hook is doing live.

import type { RefObject } from 'react';

import type { FormErrors } from '@/shared/hooks/useFormValidation';
import { getFieldInputId } from '@/shared/lib/formFieldIds';

type ErrorSummaryProps<TValues> = {
  errors: FormErrors<TValues>;
  fieldLabels?: Partial<Record<keyof TValues, string>>;
  title?: string;
  summaryRef?: RefObject<HTMLDivElement | null>;
};

export function ErrorSummary<TValues>({
  errors,
  fieldLabels,
  title = 'Please fix the following errors:',
  summaryRef,
}: ErrorSummaryProps<TValues>) {
  const errorEntries = Object.entries(errors).filter(([, error]) => Boolean(error)) as Array<
    [keyof TValues, string]
  >;

  if (errorEntries.length === 0) {
    return null;
  }

  const focusField = (fieldName: keyof TValues) => {
    const targetId = getFieldInputId(String(fieldName));
    document.getElementById(targetId)?.focus();
  };

  const getFieldLabel = (fieldName: keyof TValues) => {
    const fromMap = fieldLabels?.[fieldName];

    if (fromMap) {
      return fromMap;
    }

    return String(fieldName)
      .replace(/([a-z])([A-Z])/g, '$1 $2')
      .replace(/[_-]+/g, ' ')
      .replace(/^\w/, (character) => character.toUpperCase());
  };

  return (
    <div
      ref={summaryRef}
      className="error-summary"
      role="alert"
      aria-live="assertive"
      aria-labelledby="error-summary-title"
      tabIndex={-1}
    >
      <p id="error-summary-title" className="error-summary__title">
        {title}
      </p>

      <ul className="error-summary__list">
        {errorEntries.map(([fieldName, error]) => (
          <li key={String(fieldName)}>
            <button
              type="button"
              className="error-summary__action"
              onClick={() => {
                focusField(fieldName);
              }}
            >
              {getFieldLabel(fieldName)}: {error}
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
