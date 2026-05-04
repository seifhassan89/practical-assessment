import { useEffect, useRef } from 'react';
import type { RefObject } from 'react';

import type { FormErrors, FormValues } from '@/shared/hooks/useFormValidation';
import { getFieldInputId } from '@/shared/lib/formFieldIds';

type UseFocusFirstErrorOptions<TValues extends FormValues> = {
  submitCount: number;
  errors: FormErrors<TValues>;
  summaryRef?: RefObject<HTMLElement | null>;
};

export function useFocusFirstError<TValues extends FormValues>({
  submitCount,
  errors,
  summaryRef,
}: UseFocusFirstErrorOptions<TValues>) {
  const previousSubmitCountRef = useRef(submitCount);

  useEffect(() => {
    const didSubmit = submitCount > previousSubmitCountRef.current;
    previousSubmitCountRef.current = submitCount;

    if (!didSubmit) {
      return;
    }

    const errorEntries = Object.entries(errors).filter(([, error]) => Boolean(error));

    if (errorEntries.length === 0) {
      return;
    }

    if (summaryRef?.current) {
      summaryRef.current.focus();
      return;
    }

    const [firstFieldName] = errorEntries[0];
    const firstInvalidField = document.getElementById(getFieldInputId(firstFieldName));

    firstInvalidField?.focus();
  }, [submitCount, errors, summaryRef]);
}
