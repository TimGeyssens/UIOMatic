import { css as d, LitElement as m, html as c } from "lit";
import { n as u, t as _ } from "../../property-C20eUtZx.js";
import { r as i } from "../../state-BSTHeoLO.js";
var p = Object.defineProperty, b = Object.getOwnPropertyDescriptor, r = (t, s, h, o) => {
  for (var a = o > 1 ? void 0 : o ? b(s, h) : s, l = t.length - 1, n; l >= 0; l--)
    (n = t[l]) && (a = (o ? n(s, h, a) : n(a)) || a);
  return o && a && p(s, h, a), a;
};
let e = class extends m {
  constructor() {
    super(), this.typeAlias = "", this._items = [], this._currentPage = 1, this._itemsPerPage = 10, this._totalItems = 0, this._searchTerm = "", this._sortColumn = "", this._sortOrder = "asc";
  }
  async firstUpdated() {
    await this._loadItems();
  }
  async _loadItems() {
    try {
      const s = await (await fetch(`/umbraco/backoffice/UIOMatic/Object/GetPaged?typeAlias=${this.typeAlias}&pageNumber=${this._currentPage}&pageSize=${this._itemsPerPage}&sortColumn=${this._sortColumn}&sortOrder=${this._sortOrder}&searchTerm=${this._searchTerm}`)).json();
      this._items = s.items, this._totalItems = s.totalItems;
    } catch (t) {
      console.error("Error loading items:", t);
    }
  }
  _handleSearch(t) {
    const s = t.target;
    this._searchTerm = s.value, this._currentPage = 1, this._loadItems();
  }
  _handleSort(t) {
    this._sortColumn === t ? this._sortOrder = this._sortOrder === "asc" ? "desc" : "asc" : (this._sortColumn = t, this._sortOrder = "asc"), this._loadItems();
  }
  _handlePageChange(t) {
    this._currentPage = t, this._loadItems();
  }
  _handleEdit(t) {
    window.location.href = `/umbraco/#/uiomatic/edit/${t}?ta=${this.typeAlias}`;
  }
  _handleDelete(t) {
    confirm("Are you sure you want to delete this item?") && fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${t}`, {
      method: "DELETE"
    }).then(() => {
      this._loadItems();
    });
  }
  render() {
    return c`
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
                <th @click=${() => this._handleSort("id")}>ID</th>
                <th @click=${() => this._handleSort("name")}>Name</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              ${this._items.map((t) => c`
                <tr>
                  <td>${t.id}</td>
                  <td>${t.name}</td>
                  <td>
                    <umb-button
                      @click=${() => this._handleEdit(String(t.id))}
                      label="Edit"
                      look="secondary"
                    ></umb-button>
                    <umb-button
                      @click=${() => this._handleDelete(String(t.id))}
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
          @change=${(t) => this._handlePageChange(t.detail.page)}
        ></umb-pagination>
      </umb-body-layout>
    `;
  }
};
e.styles = d`
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
r([
  u({ type: String })
], e.prototype, "typeAlias", 2);
r([
  i()
], e.prototype, "_items", 2);
r([
  i()
], e.prototype, "_currentPage", 2);
r([
  i()
], e.prototype, "_itemsPerPage", 2);
r([
  i()
], e.prototype, "_totalItems", 2);
r([
  i()
], e.prototype, "_searchTerm", 2);
r([
  i()
], e.prototype, "_sortColumn", 2);
r([
  i()
], e.prototype, "_sortOrder", 2);
e = r([
  _("uiomatic-list")
], e);
export {
  e as UIOMaticListElement
};
