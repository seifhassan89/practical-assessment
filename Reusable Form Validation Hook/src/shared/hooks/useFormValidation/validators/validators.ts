import type { Validator } from '../useFormValidation.types';

export function required<TValues>(
  message = 'This field is required'
): Validator<string, TValues> {
  return (value) => {
    return value.trim() ? null : message;
  };
}

export function minLength<TValues>(
  min: number,
  message = `Must be at least ${min} characters`
): Validator<string, TValues> {
  return (value) => {
    return value.length >= min ? null : message;
  };
}

export function maxLength<TValues>(
  max: number,
  message = `Must be no more than ${max} characters`
): Validator<string, TValues> {
  return (value) => {
    return value.length <= max ? null : message;
  };
}

export function emailFormat<TValues>(
  message = 'Please enter a valid email address'
): Validator<string, TValues> {
  return (value) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    return emailRegex.test(value) ? null : message;
  };
}

export function pattern<TValues>(
  regex: RegExp,
  message = 'Invalid format'
): Validator<string, TValues> {
  return (value) => {
    return regex.test(value) ? null : message;
  };
}

export function matchesField<
  TValues,
  FieldName extends keyof TValues
>(
  fieldName: FieldName,
  message = 'Fields do not match'
): Validator<string, TValues> {
  return (value, values) => {
    return value === values[fieldName] ? null : message;
  };
}