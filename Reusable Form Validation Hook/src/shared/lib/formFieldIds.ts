export function getFieldInputId(fieldName: string): string {
  const normalizedFieldName = fieldName
    .trim()
    .toLowerCase()
    .replace(/[^a-z0-9_-]+/g, '-')
    .replace(/^-+|-+$/g, '');

  const safeFieldName = normalizedFieldName || 'input';

  return `field-${safeFieldName}`;
}
