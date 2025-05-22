import { LitElement, html, css } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

declare global {
  interface Window {
    UmbNotificationService: {
      show: (options: { type: string; message: string }) => void;
    };
  }
}

@customElement('uiomatic-list')
export class UIOMaticListElement extends LitElement {
  @property({ type: String })
  typeAlias = '';

  @state()
  private _items: any[] = [];

  @state()
  private _currentPage = 1;

  @state()
  private _itemsPerPage = 10;

  @state()
  private _totalItems = 0;

  @state()
  private _searchTerm = '';

  @state()
  private _sortColumn = '';

  @state()
  private _sortOrder: 'asc' | 'desc' = 'asc';

  constructor() {
    super();
  }

  async firstUpdated() {
    await this._loadItems();
  }

  private async _loadItems() {
    try {
      const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetPaged?typeAlias=${this.typeAlias}&pageNumber=${this._currentPage}&pageSize=${this._itemsPerPage}&sortColumn=${this._sortColumn}&sortOrder=${this._sortOrder}&searchTerm=${this._searchTerm}`);
      const data = await response.json();
      this._items = data.items;
      this._totalItems = data.totalItems;
    } catch (error) {
      console.error('Error loading items:', error);
    }
  }

  private _handleSearch(e: Event) {
    const input = e.target as HTMLInputElement;
    this._searchTerm = input.value;
    this._currentPage = 1;
    this._loadItems();
  }

  private _handleSort(column: string) {
    if (this._sortColumn === column) {
      this._sortOrder = this._sortOrder === 'asc' ? 'desc' : 'asc';
    } else {
      this._sortColumn = column;
      this._sortOrder = 'asc';
    }
    this._loadItems();
  }

  private _handlePageChange(page: number) {
    this._currentPage = page;
    this._loadItems();
  }

  private _handleEdit(id: string) {
    window.location.href = `/umbraco/#/uiomatic/edit/${id}?ta=${this.typeAlias}`;
  }

  private _handleDelete(id: string) {
    if (confirm('Are you sure you want to delete this item?')) {
      fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${id}`, {
        method: 'DELETE'
      }).then(() => {
        this._loadItems();
      });
    }
  }

  render() {
    return html`
      <umb-body-layout>
        <div slot="header">
          <h1>${this.typeAlias}</h1>
          <umb-input-search
            @input=${this._handleSearch}
            placeholder="Search..."
          ></umb-input-search>
        </div>

        <div class="uui-table-container">
          <table class="uui-table">
            <thead>
              <tr>
                <th @click=${() => this._handleSort('id')}>ID</th>
                <th @click=${() => this._handleSort('name')}>Name</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              ${this._items.map(item => html`
                <tr>
                  <td>${item.id}</td>
                  <td>${item.name}</td>
                  <td>
                    <umb-button
                      @click=${() => this._handleEdit(String(item.id))}
                      label="Edit"
                      look="secondary"
                    ></umb-button>
                    <umb-button
                      @click=${() => this._handleDelete(String(item.id))}
                      label="Delete"
                      look="danger"
                    ></umb-button>
                  </td>
                </tr>
              `)}
            </tbody>
          </table>
        </div>

        <umb-pagination
          .total=${this._totalItems}
          .currentPage=${this._currentPage}
          .itemsPerPage=${this._itemsPerPage}
          @change=${(e: any) => this._handlePageChange(e.detail.page)}
        ></umb-pagination>
      </umb-body-layout>
    `;
  }

  static styles = css`
    :host {
      display: block;
      padding: var(--uui-size-space-5);
    }

    .uui-table-container {
      margin: var(--uui-size-space-5) 0;
    }

    umb-button {
      margin-right: var(--uui-size-space-2);
    }
  `;
} 