import type { ButtonHTMLAttributes } from 'react';
import { cva, type VariantProps } from 'class-variance-authority';

import { cn } from '@/shared/lib/cn';

const buttonVariants = cva(
  'button',
  {
    variants: {
      variant: {
        primary: 'button--primary',
        secondary: 'button--secondary',
        ghost: 'button--ghost',
        danger: 'button--danger',
      },
      size: {
        sm: 'button--sm',
        md: 'button--md',
        lg: 'button--lg',
      },
      fullWidth: {
        true: 'button--full',
        false: '',
      },
    },
    defaultVariants: {
      variant: 'primary',
      size: 'md',
      fullWidth: false,
    },
  }
);

export type ButtonProps = ButtonHTMLAttributes<HTMLButtonElement> &
  VariantProps<typeof buttonVariants>;

export function Button({
  className,
  variant,
  size,
  fullWidth,
  type = 'button',
  ...props
}: ButtonProps) {
  return (
    <button
      type={type}
      className={cn(buttonVariants({ variant, size, fullWidth }), className)}
      {...props}
    />
  );
}
