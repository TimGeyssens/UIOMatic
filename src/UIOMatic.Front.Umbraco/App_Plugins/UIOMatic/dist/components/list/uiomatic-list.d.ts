import { LitElement } from 'lit';
declare global {
    interface Window {
        UmbNotificationService: {
            show: (options: {
                type: string;
                message: string;
            }) => void;
        };
    }
}
export declare class UIOMaticListElement extends LitElement {
    typeAlias: string;
    private _items;
    private _currentPage;
    private _itemsPerPage;
    private _totalItems;
    private _searchTerm;
    private _sortColumn;
    private _sortOrder;
    constructor();
    firstUpdated(): Promise<void>;
    private _loadItems;
    private _handleSearch;
    private _handleSort;
    private _handlePageChange;
    private _handleEdit;
    private _handleDelete;
    render(): import("lit-html").TemplateResult<1>;
    static styles: import("lit").CSSResult;
}
