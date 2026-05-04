import type {
  DirtyFields,
  FormValues,
  FormErrors,
  ValidationSchema,
} from '../useFormValidation.types';

export function validateField<TValues>(
  fieldName: keyof TValues,
  values: TValues,
  validationSchema: ValidationSchema<TValues>
): string | null {
  const validators = validationSchema[fieldName] ?? [];

  for (const validator of validators) {
    const error = validator(values[fieldName], values);

    if (error) {
      return error;
    }
  }

  return null;
}

export function validateForm<TValues>(
  values: TValues,
  validationSchema: ValidationSchema<TValues>
): FormErrors<TValues> {
  const errors: FormErrors<TValues> = {};

  Object.keys(validationSchema).forEach((fieldName) => {
    const typedFieldName = fieldName as keyof TValues;

    const error = validateField(typedFieldName, values, validationSchema);

    if (error) {
      errors[typedFieldName] = error;
    }
  });

  return errors;
}

export function hasErrors<TValues>(errors: FormErrors<TValues>): boolean {
  return Object.keys(errors).length > 0;
}

export function getDirtyFields<TValues extends FormValues>(
  values: TValues,
  initialValues: TValues
): DirtyFields<TValues> {
  const dirtyFields: DirtyFields<TValues> = {};

  Object.keys(values).forEach((fieldName) => {
    const typedFieldName = fieldName as keyof TValues;

    dirtyFields[typedFieldName] =
      values[typedFieldName] !== initialValues[typedFieldName];
  });

  return dirtyFields;
}

export function getDirtyValues<TValues extends FormValues>(
  values: TValues,
  dirtyFields: DirtyFields<TValues>
): Partial<TValues> {
  const dirtyValues: Partial<TValues> = {};

  Object.keys(dirtyFields).forEach((fieldName) => {
    const typedFieldName = fieldName as keyof TValues;

    if (dirtyFields[typedFieldName]) {
      dirtyValues[typedFieldName] = values[typedFieldName];
    }
  });

  return dirtyValues;
}

export function markAllFieldsTouched<TValues>(
  validationSchema: ValidationSchema<TValues>
): Partial<Record<keyof TValues, boolean>> {
  const touchedFields: Partial<Record<keyof TValues, boolean>> = {};

  Object.keys(validationSchema).forEach((fieldName) => {
    touchedFields[fieldName as keyof TValues] = true;
  });

  return touchedFields;
}