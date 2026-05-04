type JsonValue =
  | string
  | number
  | boolean
  | null
  | { [key: string]: JsonValue }
  | JsonValue[];

type DebugPanelProps = {
  title?: string;
  data: JsonValue;
};

export function DebugPanel({ title = 'Debug state', data }: DebugPanelProps) {
  return (
    <details className="rounded-xl bg-slate-950 p-3.5 text-slate-100">
      <summary className="cursor-pointer text-sm font-semibold text-slate-300 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-slate-200">
        {title} (debug panel)
      </summary>
      <pre className="mt-2.5 max-h-72 overflow-auto text-xs leading-relaxed">
        {JSON.stringify(data, null, 2)}
      </pre>
    </details>
  );
}
