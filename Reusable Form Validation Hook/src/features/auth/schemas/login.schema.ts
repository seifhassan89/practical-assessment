import type { ValidationSchema } from '@/shared/hooks/useFormValidation';
import {
  emailFormat,
  passwordMinLength,
  required,
} from '@/shared/hooks/useFormValidation';

import type { LoginFormValues } from '@/features/auth/types/login.types';

export const loginInitialValues: LoginFormValues = {
  email: '',
  password: '',
};

export const loginValidationSchema: ValidationSchema<LoginFormValues> = {
  email: [
    required('Email is required'),
    emailFormat('Please enter a valid email address'),
  ],
  password: [
    required('Password is required'),
    passwordMinLength(),
  ],
};