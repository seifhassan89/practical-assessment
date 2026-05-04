import type { ValidationSchema } from '@/shared/hooks/useFormValidation';
import {
  emailFormat,
  matchesField,
  minLength,
  required,
} from '@/shared/hooks/useFormValidation';

import type { RegisterFormValues } from '@/features/auth/types/register.types';

export const registerInitialValues: RegisterFormValues = {
  name: '',
  email: '',
  password: '',
  confirmPassword: '',
};

export const registerValidationSchema: ValidationSchema<RegisterFormValues> = {
  name: [
    required('Name is required'),
    minLength(3, 'Name must be at least 3 characters'),
  ],
  email: [
    required('Email is required'),
    emailFormat('Please enter a valid email address'),
  ],
  password: [
    required('Password is required'),
    minLength(8, 'Password must be at least 8 characters'),
  ],
  confirmPassword: [
    required('Confirm password is required'),
    matchesField('password', 'Passwords do not match'),
  ],
};