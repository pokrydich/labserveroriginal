

export interface IApiResponse<TData> {
    result: TData | null;
    successful: boolean;
    warnings: string[];
    error: string | null;
}