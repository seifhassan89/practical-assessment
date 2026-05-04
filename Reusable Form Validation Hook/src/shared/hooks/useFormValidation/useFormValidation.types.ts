import type {
  ChangeEventHandler,
  Dispatch,
  FocusEventHandler,
  SetStateAction,
  SubmitEventHandler,
} from 'react';

export type ValidationMode = 'onSubmit' | 'onBlur' | 'onChange';

export type ValidationResult = string | null;

export type FormPrimitive = string | number | boolean | null | undefined;

export type FormValues = Record<string, FormPrimitive>;

export type Validator<TValue, TValues> = (
  value: TValue,
  values: TValues
) => ValidationResult;

export type ValidationSchema<TValues> = {
  [FieldName in keyof TValues]?: Validator<TValues[FieldName], TValues>[];
};

export type FormErrors<TValues> = Partial<Record<keyof TValues, string>>;

export type FormTouched<TValues> = Partial<Record<keyof TValues, boolean>>;

export type DirtyFields<TValues> = Partial<Record<keyof TValues, boolean>>;

export type FieldMeta = {
  error?: string;
  touched: boolean;
  dirty: boolean;
  invalid: boolean;
};

export type UseFormValidationOptions<TValues> = {
  initialValues: TValues;
  validationSchema: ValidationSchema<TValues>;
  mode?: ValidationMode;
  reValidateMode?: Extract<ValidationMode, 'onBlur' | 'onChange'>;
};

export type FieldProps<TValue> = {
  name: string;
  value: TValue;
  onChange: ChangeEventHandler<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>;
  onBlur: FocusEventHandler<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>;
};

export type UseFormValidationReturn<TValues> = {
  values: TValues;
  errors: FormErrors<TValues>;
  touched: FormTouched<TValues>;
  dirtyFields: DirtyFields<TValues>;

  isValid: boolean;
  isDirty: boolean;
  isSubmitting: boolean;
  submitCount: number;

  getFieldProps: <FieldName extends keyof TValues>(
    fieldName: FieldName
  ) => FieldProps<TValues[FieldName]>;

  getFieldMeta: <FieldName extends keyof TValues>(
    fieldName: FieldName
  ) => FieldMeta;

  setFieldValue: <FieldName extends keyof TValues>(
    fieldName: FieldName,
    value: TValues[FieldName]
  ) => void;

  setFieldTouched: <FieldName extends keyof TValues>(
    fieldName: FieldName,
    isTouched: boolean
  ) => void;

  setFieldError: <FieldName extends keyof TValues>(
    fieldName: FieldName,
    error?: string
  ) => void;

  setErrors: Dispatch<SetStateAction<FormErrors<TValues>>>;

  validateField: <FieldName extends keyof TValues>(
    fieldName: FieldName
  ) => string | null;

  validateForm: () => boolean;

  handleSubmit: (
    onValidSubmit: (values: TValues) => void | Promise<void>
  ) => SubmitEventHandler<HTMLFormElement>;

  resetForm: () => void;

  getDirtyValues: () => Partial<TValues>;
};