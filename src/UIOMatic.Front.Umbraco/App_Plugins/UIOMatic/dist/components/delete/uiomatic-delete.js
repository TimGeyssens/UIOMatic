import { css as u, LitElement as d, html as p } from "lit";
import { n as c, t as m } from "../../property-C20eUtZx.js";
var y = Object.defineProperty, b = Object.getOwnPropertyDescriptor, n = (t, o, r, s) => {
  for (var e = s > 1 ? void 0 : s ? b(o, r) : o, a = t.length - 1, l; a >= 0; a--)
    (l = t[a]) && (e = (s ? l(o, r, e) : l(e)) || e);
  return s && e && y(o, r, e), e;
};
let i = class extends d {
  constructor() {
    super(...arguments), this.typeAlias = "", this.id = "";
  }
  async _handleDelete() {
    if (confirm("Are you sure you want to delete this item?"))
      try {
        if ((await fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${this.id}`, {
          method: "DELETE"
        })).ok)
          window.UmbNotificationService.show({
            type: "success",
            message: "Item deleted successfully"
          }), window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`;
        else
          throw new Error("Failed to delete item");
      } catch (t) {
        console.error("Error deleting item:", t), window.UmbNotificationService.show({
          type: "error",
          message: "Failed to delete item"
        });
      }
  }
  render() {
    return p`
      <umb-body-layout>
        <div slot="header">
          <h1>Delete ${this.typeAlias}</h1>
        </div>

        <div class="delete-confirmation">
          <p>Are you sure you want to delete this item?</p>
          <p>This action cannot be undone.</p>

          <div class="button-group">
            <umb-button
              @click=${this._handleDelete}
              label="Delete"
              look="danger"
            ></umb-button>
            <umb-button
              @click=${() => window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`}
              label="Cancel"
              look="secondary"
            ></umb-button>
          </div>
        </div>
      </umb-body-layout>
    `;
  }
};
i.styles = u`
    :host {
      display: block;
      padding: var(--uui-size-space-5);
    }

    .delete-confirmation {
      text-align: center;
      padding: var(--uui-size-space-5);
    }

    .button-group {
      margin-top: var(--uui-size-space-5);
      display: flex;
      gap: var(--uui-size-space-3);
      justify-content: center;
    }
  `;
n([
  c({ type: String })
], i.prototype, "typeAlias", 2);
n([
  c({ type: String })
], i.prototype, "id", 2);
i = n([
  m("uiomatic-delete")
], i);
export {
  i as UIOMaticDeleteElement
};
