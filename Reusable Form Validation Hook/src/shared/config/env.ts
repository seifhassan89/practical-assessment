const DEFAULT_PASSWORD_MIN_LENGTH = 8;

function parsePositiveInteger(value: string | undefined, fallback: number): number {
  const parsed = Number(value);

  if (!Number.isInteger(parsed) || parsed <= 0) {
    return fallback;
  }

  return parsed;
}

export const env = {
  passwordMinLength: parsePositiveInteger(
    import.meta.env.VITE_PASSWORD_MIN_LENGTH,
    DEFAULT_PASSWORD_MIN_LENGTH
  ),
} as const;
