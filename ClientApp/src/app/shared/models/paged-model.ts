export interface PagedModel<T> {
    pageSize: number;
    currentPage: number;
    totalPages: number;
    totalCount: number;
    items: T[];
}