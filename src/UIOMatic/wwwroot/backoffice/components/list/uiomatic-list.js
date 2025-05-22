var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
import { customElement, html, property, state } from 'lit/decorators.js';
import { UmbLitElement } from '@umbraco-cms/element';
import { UmbTextStyles } from '@umbraco-cms/styles';
import { css } from 'lit';
let UIOMaticListElement = class UIOMaticListElement extends UmbLitElement {
    constructor() {
        super();
        this.typeAlias = '';
        this._items = [];
        this._currentPage = 1;
        this._itemsPerPage = 10;
        this._totalItems = 0;
        this._searchTerm = '';
        this._sortColumn = '';
        this._sortOrder = 'asc';
    }
    async firstUpdated() {
        await this._loadItems();
    }
    async _loadItems() {
        try {
            const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetPaged?typeAlias=${this.typeAlias}&pageNumber=${this._currentPage}&pageSize=${this._itemsPerPage}&sortColumn=${this._sortColumn}&sortOrder=${this._sortOrder}&searchTerm=${this._searchTerm}`);
            const data = await response.json();
            this._items = data.items;
            this._totalItems = data.totalItems;
        }
        catch (error) {
            console.error('Error loading items:', error);
        }
    }
    _handleSearch(e) {
        const input = e.target;
        this._searchTerm = input.value;
        this._currentPage = 1;
        this._loadItems();
    }
    _handleSort(column) {
        if (this._sortColumn === column) {
            this._sortOrder = this._sortOrder === 'asc' ? 'desc' : 'asc';
        }
        else {
            this._sortColumn = column;
            this._sortOrder = 'asc';
        }
        this._loadItems();
    }
    _handlePageChange(page) {
        this._currentPage = page;
        this._loadItems();
    }
    _handleEdit(id) {
        window.location.href = `/umbraco/#/uiomatic/edit/${id}?ta=${this.typeAlias}`;
    }
    _handleDelete(id) {
        if (confirm('Are you sure you want to delete this item?')) {
            fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${id}`, {
                method: 'DELETE'
            }).then(() => {
                this._loadItems();
            });
        }
    }
    render() {
        return html `
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
              ${this._items.map(item => html `
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
          @change=${(e) => this._handlePageChange(e.detail.page)}
        ></umb-pagination>
      </umb-body-layout>
    `;
    }
};
UIOMaticListElement.styles = [
    UmbTextStyles,
    css `
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
    `
];
__decorate([
    property({ type: String })
], UIOMaticListElement.prototype, "typeAlias", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_items", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_currentPage", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_itemsPerPage", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_totalItems", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_searchTerm", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_sortColumn", void 0);
__decorate([
    state()
], UIOMaticListElement.prototype, "_sortOrder", void 0);
UIOMaticListElement = __decorate([
    customElement('uiomatic-list')
], UIOMaticListElement);
export { UIOMaticListElement };
