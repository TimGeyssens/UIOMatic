import { css as p, LitElement as h, html as l } from "lit";
import { n as c, t as m } from "../../property-C20eUtZx.js";
import { r as n } from "../../state-BSTHeoLO.js";
var b = Object.defineProperty, y = Object.getOwnPropertyDescriptor, o = (i, e, t, r) => {
  for (var s = r > 1 ? void 0 : r ? y(e, t) : e, d = i.length - 1, u; d >= 0; d--)
    (u = i[d]) && (s = (r ? u(e, t, s) : u(s)) || s);
  return r && s && b(e, t, s), s;
};
let a = class extends h {
  constructor() {
    super(), this.typeAlias = "", this.id = "", this._item = {}, this._fields = [], this._loading = !1, this._errors = {};
  }
  async firstUpdated() {
    await this._loadItem(), await this._loadFields();
  }
  async _loadItem() {
    if (this.id) {
      this._loading = !0;
      try {
        const i = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetById?typeAlias=${this.typeAlias}&id=${this.id}`);
        this._item = await i.json();
      } catch (i) {
        console.error("Error loading item:", i), window.UmbNotificationService.show({
          type: "error",
          message: "Failed to load item"
        });
      } finally {
        this._loading = !1;
      }
    }
  }
  async _loadFields() {
    try {
      const i = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetFields?typeAlias=${this.typeAlias}`);
      this._fields = await i.json();
    } catch (i) {
      console.error("Error loading fields:", i), window.UmbNotificationService.show({
        type: "error",
        message: "Failed to load fields"
      });
    }
  }
  _validateField(i, e) {
    return i.required && !e ? "This field is required" : null;
  }
  _handleInput(i, e) {
    const r = i.target.value;
    this._item[e.alias] = r;
    const s = this._validateField(e, r);
    s ? this._errors[e.alias] = s : delete this._errors[e.alias], this.requestUpdate();
  }
  async _handleSubmit(i) {
    i.preventDefault();
    let e = !1;
    if (this._fields.forEach((t) => {
      const r = this._validateField(t, this._item[t.alias]);
      r && (this._errors[t.alias] = r, e = !0);
    }), e) {
      window.UmbNotificationService.show({
        type: "error",
        message: "Please fix the validation errors"
      });
      return;
    }
    this._loading = !0;
    try {
      if ((await fetch(`/umbraco/backoffice/UIOMatic/Object/Save?typeAlias=${this.typeAlias}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify(this._item)
      })).ok)
        window.UmbNotificationService.show({
          type: "success",
          message: "Item saved successfully"
        }), window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`;
      else
        throw new Error("Failed to save item");
    } catch (t) {
      console.error("Error saving item:", t), window.UmbNotificationService.show({
        type: "error",
        message: "Failed to save item"
      });
    } finally {
      this._loading = !1;
    }
  }
  render() {
    return l`
      <umb-body-layout>
        <div slot="header">
          <h1>${this.id ? "Edit" : "Create"} ${this.typeAlias}</h1>
        </div>

        ${this._loading ? l`
          <umb-loader></umb-loader>
        ` : l`
          <form @submit=${this._handleSubmit}>
            ${this._fields.map((i) => l`
              <div class="form-group">
                <label for=${i.alias}>${i.name}</label>
                <input
                  type="text"
                  id=${i.alias}
                  name=${i.alias}
                  .value=${this._item[i.alias] || ""}
                  @input=${(e) => this._handleInput(e, i)}
                  ?required=${i.required}
                />
                ${this._errors[i.alias] ? l`
                  <div class="error">${this._errors[i.alias]}</div>
                ` : ""}
              </div>
            `)}

            <div class="button-group">
              <umb-button
                type="submit"
                label="Save"
                look="primary"
                ?disabled=${this._loading}
              ></umb-button>
              <umb-button
                @click=${() => window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`}
                label="Cancel"
                look="secondary"
              ></umb-button>
            </div>
          </form>
        `}
      </umb-body-layout>
    `;
  }
};
a.styles = p`
    :host {
      display: block;
      padding: var(--uui-size-space-5);
    }

    .form-group {
      margin-bottom: var(--uui-size-space-4);
    }

    label {
      display: block;
      margin-bottom: var(--uui-size-space-2);
    }

    input {
      width: 100%;
      padding: var(--uui-size-space-2);
      border: 1px solid var(--uui-color-border);
      border-radius: var(--uui-border-radius);
    }

    .error {
      color: var(--uui-color-danger);
      font-size: var(--uui-type-size-sm);
      margin-top: var(--uui-size-space-1);
    }

    .button-group {
      margin-top: var(--uui-size-space-5);
      display: flex;
      gap: var(--uui-size-space-3);
    }
  `;
o([
  c({ type: String })
], a.prototype, "typeAlias", 2);
o([
  c({ type: String })
], a.prototype, "id", 2);
o([
  n()
], a.prototype, "_item", 2);
o([
  n()
], a.prototype, "_fields", 2);
o([
  n()
], a.prototype, "_loading", 2);
o([
  n()
], a.prototype, "_errors", 2);
a = o([
  m("uiomatic-edit")
], a);
export {
  a as UIOMaticEditElement
};
