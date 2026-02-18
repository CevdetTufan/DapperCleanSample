import { useState, useEffect, useCallback } from 'react';

interface UseApiState<T> {
    data: T | null;
    loading: boolean;
    error: string | null;
}

export function useApi<T>(fetcher: () => Promise<T>, deps: unknown[] = []) {
    const [state, setState] = useState<UseApiState<T>>({
        data: null,
        loading: true,
        error: null,
    });

    const execute = useCallback(async () => {
        setState({ data: null, loading: true, error: null });
        try {
            const data = await fetcher();
            setState({ data, loading: false, error: null });
        } catch (err: unknown) {
            const message = err instanceof Error ? err.message : 'An error occurred';
            setState({ data: null, loading: false, error: message });
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, deps);

    useEffect(() => {
        execute();
    }, [execute]);

    return { ...state, refetch: execute };
}
