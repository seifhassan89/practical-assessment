import { useCallback, useMemo, useState, type SubmitEventHandler } from 'react';

import {
  getDirtyFields,
  getDirtyValues as getDirtyValuesFromEngine,
  hasErrors,
  markAllFieldsTouched,
  validateField as validateFieldFromEngine,
  validateForm as validateFormFromEngine,
} from './engine/validationEngine';

import type {
  DirtyFields,
  FormValues,
  FormErrors,
  FormTouched,
  UseFormValidationOptions,
  UseFormValidationReturn,
} from './useFormValidation.types';

export function useFormValidation<TValues extends FormValues>({
  initialValues,
  validationSchema,
  mode = 'onSubmit',
  reValidateMode = 'onChange',
}: UseFormValidationOptions<TValues>): UseFormValidationReturn<TValues> {
  const [values, setValues] = useState<TValues>(initialValues);
  const [errors, setErrors] = useState<FormErrors<TValues>>({});
  const [touched, setTouched] = useState<FormTouched<TValues>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [submitCount, setSubmitCount] = useState(0);

  const dirtyFields = useMemo<DirtyFields<TValues>>(() => {
    return getDirtyFields(values, initialValues);
  }, [values, initialValues]);

  const isDirty = useMemo(() => {
    return Object.values(dirtyFields).some(Boolean);
  }, [dirtyFields]);

  const isValid = useMemo(() => {
    return !hasErrors(errors);
  }, [errors]);

  const updateFieldError = useCallback(
    <FieldName extends keyof TValues>(
      fieldName: FieldName,
      error: string | null
    ) => {
      setErrors((previousErrors) => {
        const nextErrors = { ...previousErrors };

        if (error) {
          nextErrors[fieldName] = error;
        } else {
          delete nextErrors[fieldName];
        }

        return nextErrors;
      });
    },
    []
  );

  const validateField = useCallback(
    <FieldName extends keyof TValues>(fieldName: FieldName): string | null => {
      const error = validateFieldFromEngine(fieldName, values, validationSchema);

      updateFieldError(fieldName, error);

      return error;
    },
    [values, validationSchema, updateFieldError]
  );

  const validateForm = useCallback((): boolean => {
    const nextErrors = validateFormFromEngine(values, validationSchema);

    setErrors(nextErrors);
    setTouched(markAllFieldsTouched(validationSchema));

    return !hasErrors(nextErrors);
  }, [values, validationSchema]);

  const setFieldValue = useCallback(
    <FieldName extends keyof TValues>(
      fieldName: FieldName,
      value: TValues[FieldName]
    ) => {
      setValues((previousValues) => {
        const nextValues = {
          ...previousValues,
          [fieldName]: value,
        };

        const fieldHasError = Boolean(errors[fieldName]);
        const shouldValidateOnChange =
          mode === 'onChange' || (fieldHasError && reValidateMode === 'onChange');

        if (shouldValidateOnChange) {
          const error = validateFieldFromEngine(
            fieldName,
            nextValues,
            validationSchema
          );

          setErrors((previousErrors) => {
            const nextErrors = { ...previousErrors };

            if (error) {
              nextErrors[fieldName] = error;
            } else {
              delete nextErrors[fieldName];
            }

            return nextErrors;
          });
        }

        return nextValues;
      });
    },
    [errors, mode, reValidateMode, validationSchema]
  );

  const setFieldTouched = useCallback(
    <FieldName extends keyof TValues>(
      fieldName: FieldName,
      isTouched: boolean
    ) => {
      setTouched((previousTouched) => ({
        ...previousTouched,
        [fieldName]: isTouched,
      }));
    },
    []
  );

  const setFieldError = useCallback(
    <FieldName extends keyof TValues>(
      fieldName: FieldName,
      error?: string
    ) => {
      setErrors((previousErrors) => {
        const nextErrors = { ...previousErrors };

        if (error) {
          nextErrors[fieldName] = error;
        } else {
          delete nextErrors[fieldName];
        }

        return nextErrors;
      });
    },
    []
  );

  const handleChange = useCallback(
    (
      event: React.ChangeEvent<
        HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
      >
    ) => {
      const { name, value } = event.target;

      setFieldValue(
        name as keyof TValues,
        value as TValues[keyof TValues]
      );
    },
    [setFieldValue]
  );

  const handleBlur = useCallback(
    (
      event: React.FocusEvent<
        HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement
      >
    ) => {
      const fieldName = event.target.name as keyof TValues;

      setFieldTouched(fieldName, true);

      const fieldHasError = Boolean(errors[fieldName]);
      const shouldValidateOnBlur =
        mode === 'onBlur' || (fieldHasError && reValidateMode === 'onBlur');

      if (shouldValidateOnBlur) {
        validateField(fieldName);
      }
    },
    [errors, mode, reValidateMode, setFieldTouched, validateField]
  );

  const getFieldProps = useCallback(
    <FieldName extends keyof TValues>(fieldName: FieldName) => {
      return {
        name: String(fieldName),
        value: values[fieldName],
        onChange: handleChange,
        onBlur: handleBlur,
      };
    },
    [values, handleChange, handleBlur]
  );

  const getFieldMeta = useCallback(
    <FieldName extends keyof TValues>(fieldName: FieldName) => {
      const error = errors[fieldName];
      const isTouched = Boolean(touched[fieldName]);
      const isFieldDirty = Boolean(dirtyFields[fieldName]);

      return {
        error,
        touched: isTouched,
        dirty: isFieldDirty,
        invalid: Boolean(error),
      };
    },
    [errors, touched, dirtyFields]
  );

  const resetForm = useCallback(() => {
    setValues(initialValues);
    setErrors({});
    setTouched({});
    setIsSubmitting(false);
    setSubmitCount(0);
  }, [initialValues]);

  const getDirtyValues = useCallback(() => {
    return getDirtyValuesFromEngine(values, dirtyFields);
  }, [values, dirtyFields]);

  const handleSubmit = useCallback(
    (
      onValidSubmit: (values: TValues) => void | Promise<void>
    ): SubmitEventHandler<HTMLFormElement> => {
      return async (event) => {
        event.preventDefault();

        setSubmitCount((previousCount) => previousCount + 1);

        const isFormValid = validateForm();

        if (!isFormValid) {
          return;
        }

        setIsSubmitting(true);

        try {
          await onValidSubmit(values);
        } finally {
          setIsSubmitting(false);
        }
      };
    },
    [validateForm, values]
  );

  return {
    values,
    errors,
    touched,
    dirtyFields,

    isValid,
    isDirty,
    isSubmitting,
    submitCount,

    getFieldProps,
    getFieldMeta,
    setFieldValue,
    setFieldTouched,
    setFieldError,
    setErrors,
    validateField,
    validateForm,
    handleSubmit,
    resetForm,
    getDirtyValues,
  };
}