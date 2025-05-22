import { LitElement, html, css } from 'lit';
import { customElement, property, state } from 'lit/decorators.js';

declare global {
  interface Window {
    UmbNotificationService: {
      show: (options: { type: string; message: string }) => void;
    };
  }
}

@customElement('uiomatic-edit')
export class UIOMaticEditElement extends LitElement {
  @property({ type: String })
  typeAlias = '';

  @property({ type: String })
  id = '';

  @state()
  private _item: any = {};

  @state()
  private _fields: any[] = [];

  @state()
  private _loading = false;

  @state()
  private _errors: { [key: string]: string } = {};

  constructor() {
    super();
  }

  async firstUpdated() {
    await this._loadItem();
    await this._loadFields();
  }

  private async _loadItem() {
    if (!this.id) return;
    
    this._loading = true;
    try {
      const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetById?typeAlias=${this.typeAlias}&id=${this.id}`);
      this._item = await response.json();
    } catch (error) {
      console.error('Error loading item:', error);
      window.UmbNotificationService.show({
        type: 'error',
        message: 'Failed to load item'
      });
    } finally {
      this._loading = false;
    }
  }

  private async _loadFields() {
    try {
      const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetFields?typeAlias=${this.typeAlias}`);
      this._fields = await response.json();
    } catch (error) {
      console.error('Error loading fields:', error);
      window.UmbNotificationService.show({
        type: 'error',
        message: 'Failed to load fields'
      });
    }
  }

  private _validateField(field: any, value: any): string | null {
    if (field.required && !value) {
      return 'This field is required';
    }
    return null;
  }

  private _handleInput(e: Event, field: any) {
    const input = e.target as HTMLInputElement;
    const value = input.value;
    this._item[field.alias] = value;
    
    const error = this._validateField(field, value);
    if (error) {
      this._errors[field.alias] = error;
    } else {
      delete this._errors[field.alias];
    }
    
    this.requestUpdate();
  }

  private async _handleSubmit(e: Event) {
    e.preventDefault();
    
    // Validate all fields
    let hasErrors = false;
    this._fields.forEach(field => {
      const error = this._validateField(field, this._item[field.alias]);
      if (error) {
        this._errors[field.alias] = error;
        hasErrors = true;
      }
    });

    if (hasErrors) {
      window.UmbNotificationService.show({
        type: 'error',
        message: 'Please fix the validation errors'
      });
      return;
    }

    this._loading = true;
    try {
      const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/Save?typeAlias=${this.typeAlias}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(this._item)
      });

      if (response.ok) {
        window.UmbNotificationService.show({
          type: 'success',
          message: 'Item saved successfully'
        });
        window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`;
      } else {
        throw new Error('Failed to save item');
      }
    } catch (error) {
      console.error('Error saving item:', error);
      window.UmbNotificationService.show({
        type: 'error',
        message: 'Failed to save item'
      });
    } finally {
      this._loading = false;
    }
  }

  render() {
    return html`
      <umb-body-layout>
        <div slot="header">
          <h1>${this.id ? 'Edit' : 'Create'} ${this.typeAlias}</h1>
        </div>

        ${this._loading ? html`
          <umb-loader></umb-loader>
        ` : html`
          <form @submit=${this._handleSubmit}>
            ${this._fields.map(field => html`
              <div class="form-group">
                <label for=${field.alias}>${field.name}</label>
                <input
                  type="text"
                  id=${field.alias}
                  name=${field.alias}
                  .value=${this._item[field.alias] || ''}
                  @input=${(e: Event) => this._handleInput(e, field)}
                  ?required=${field.required}
                />
                ${this._errors[field.alias] ? html`
                  <div class="error">${this._errors[field.alias]}</div>
                ` : ''}
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

  static styles = css`
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
} 