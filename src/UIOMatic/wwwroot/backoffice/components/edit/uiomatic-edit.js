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
import { UmbNotificationService } from '@umbraco-cms/notification';
let UIOMaticEditElement = class UIOMaticEditElement extends UmbLitElement {
    constructor() {
        super();
        this.typeAlias = '';
        this.id = '0';
        this._loading = false;
        this._saving = false;
        this._properties = [];
        this._errors = [];
        this._serverError = '';
        this._notificationService = new UmbNotificationService(this);
    }
    async firstUpdated() {
        await this._loadData();
    }
    async _loadData() {
        this._loading = true;
        try {
            // Load type info to get properties
            const typeResponse = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetTypeInfo?typeAlias=${this.typeAlias}`);
            if (!typeResponse.ok) {
                throw new Error('Failed to load type information');
            }
            const typeData = await typeResponse.json();
            this._properties = typeData.properties;
            // If editing existing item, load its data
            if (this.id !== '0') {
                const itemResponse = await fetch(`/umbraco/backoffice/UIOMatic/Object/GetById?typeAlias=${this.typeAlias}&id=${this.id}`);
                if (!itemResponse.ok) {
                    throw new Error('Failed to load item data');
                }
                const itemData = await itemResponse.json();
                // Update property values with loaded data
                this._properties = this._properties.map(prop => ({
                    ...prop,
                    value: itemData[prop.alias]
                }));
            }
        }
        catch (error) {
            console.error('Error loading data:', error);
            this._serverError = error instanceof Error ? error.message : 'Failed to load data';
            this._notificationService.peek('negative', {
                headline: 'Error',
                message: this._serverError
            });
        }
        finally {
            this._loading = false;
        }
    }
    _validateProperty(property) {
        if (property.validation?.required && !property.value) {
            return property.validation.message || `${property.name} is required`;
        }
        if (property.validation?.pattern && property.value) {
            const regex = new RegExp(property.validation.pattern);
            if (!regex.test(property.value)) {
                return property.validation.message || `${property.name} has an invalid format`;
            }
        }
        if (property.type === 'number') {
            const num = Number(property.value);
            if (property.validation?.min !== undefined && num < property.validation.min) {
                return property.validation.message || `${property.name} must be at least ${property.validation.min}`;
            }
            if (property.validation?.max !== undefined && num > property.validation.max) {
                return property.validation.message || `${property.name} must be at most ${property.validation.max}`;
            }
        }
        return null;
    }
    _validateForm() {
        this._errors = [];
        let isValid = true;
        this._properties.forEach(property => {
            const error = this._validateProperty(property);
            if (error) {
                this._errors.push({ property: property.alias, message: error });
                isValid = false;
            }
        });
        return isValid;
    }
    _handlePropertyChange(e, property) {
        const input = e.target;
        property.value = input.value;
        // Clear error for this property when it changes
        this._errors = this._errors.filter(err => err.property !== property.alias);
        this.requestUpdate();
    }
    async _handleSave() {
        if (!this._validateForm()) {
            this._notificationService.peek('negative', {
                headline: 'Validation Error',
                message: 'Please fix the errors in the form'
            });
            return;
        }
        this._saving = true;
        try {
            const data = this._properties.reduce((acc, prop) => {
                acc[prop.alias] = prop.value;
                return acc;
            }, {});
            const url = `/umbraco/backoffice/UIOMatic/Object/${this.id !== '0' ? 'Update' : 'Create'}?typeAlias=${this.typeAlias}`;
            const method = this.id !== '0' ? 'PUT' : 'POST';
            const response = await fetch(url, {
                method,
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to save');
            }
            this._notificationService.peek('positive', {
                headline: 'Success',
                message: `Item ${this.id !== '0' ? 'updated' : 'created'} successfully`
            });
            // Redirect to list view
            window.location.href = `/umbraco/#/uiomatic/list/${this.typeAlias}`;
        }
        catch (error) {
            console.error('Error saving:', error);
            this._serverError = error instanceof Error ? error.message : 'Failed to save';
            this._notificationService.peek('negative', {
                headline: 'Error',
                message: this._serverError
            });
        }
        finally {
            this._saving = false;
        }
    }
    _getPropertyError(property) {
        const error = this._errors.find(err => err.property === property.alias);
        return error?.message || null;
    }
    _renderPropertyEditor(property) {
        const error = this._getPropertyError(property);
        switch (property.type.toLowerCase()) {
            case 'string':
                return html `
          <umb-property-editor
            .alias=${property.alias}
            .config=${property.config}
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-property-editor>
        `;
            case 'number':
                return html `
          <umb-input-number
            .value=${property.value}
            .min=${property.validation?.min}
            .max=${property.validation?.max}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-input-number>
        `;
            case 'boolean':
                return html `
          <umb-toggle
            .checked=${property.value}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-toggle>
        `;
            case 'datetime':
                return html `
          <umb-input-date-time
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-input-date-time>
        `;
            case 'richtext':
                return html `
          <umb-rich-text-editor
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-rich-text-editor>
        `;
            case 'file':
                return html `
          <umb-upload-field
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-upload-field>
        `;
            case 'media':
                return html `
          <umb-media-picker
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-media-picker>
        `;
            case 'content':
                return html `
          <umb-content-picker
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-content-picker>
        `;
            case 'member':
                return html `
          <umb-member-picker
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-member-picker>
        `;
            default:
                return html `
          <umb-property-editor
            .alias=${property.alias}
            .config=${property.config}
            .value=${property.value}
            .error=${error}
            @change=${(e) => this._handlePropertyChange(e, property)}
          ></umb-property-editor>
        `;
        }
    }
    render() {
        if (this._loading) {
            return html `
        <umb-body-layout>
          <div class="loading">
            <umb-loader></umb-loader>
          </div>
        </umb-body-layout>
      `;
        }
        return html `
      <umb-body-layout>
        <div slot="header">
          <h1>${this.id !== '0' ? 'Edit' : 'Create'} ${this.typeAlias}</h1>
        </div>

        ${this._serverError ? html `
          <umb-alert
            type="error"
            .message=${this._serverError}
          ></umb-alert>
        ` : ''}

        <div class="form-container">
          ${this._properties.map(property => html `
            <div class="property-editor ${this._getPropertyError(property) ? 'has-error' : ''}">
              <label for=${property.alias}>
                ${property.name}
                ${property.validation?.required ? html `<span class="required">*</span>` : ''}
              </label>
              ${this._renderPropertyEditor(property)}
              ${this._getPropertyError(property) ? html `
                <span class="error-message">${this._getPropertyError(property)}</span>
              ` : ''}
            </div>
          `)}
        </div>

        <div slot="footer">
          <umb-button
            @click=${this._handleSave}
            .state=${this._saving ? 'waiting' : 'default'}
            look="primary"
            label=${this._saving ? 'Saving...' : 'Save'}
          ></umb-button>
          <umb-button
            @click=${() => window.history.back()}
            look="secondary"
            label="Cancel"
          ></umb-button>
        </div>
      </umb-body-layout>
    `;
    }
};
UIOMaticEditElement.styles = [
    UmbTextStyles,
    css `
      :host {
        display: block;
        padding: var(--uui-size-space-5);
      }

      .loading {
        display: flex;
        justify-content: center;
        align-items: center;
        min-height: 200px;
      }

      .form-container {
        display: grid;
        gap: var(--uui-size-space-4);
        padding: var(--uui-size-space-4);
      }

      .property-editor {
        display: grid;
        gap: var(--uui-size-space-2);
      }

      .property-editor.has-error {
        border-left: 3px solid var(--uui-color-danger);
        padding-left: var(--uui-size-space-3);
      }

      label {
        font-weight: bold;
      }

      .required {
        color: var(--uui-color-danger);
        margin-left: var(--uui-size-space-1);
      }

      .error-message {
        color: var(--uui-color-danger);
        font-size: 0.875rem;
      }

      umb-button {
        margin-right: var(--uui-size-space-2);
      }
    `
];
__decorate([
    property({ type: String })
], UIOMaticEditElement.prototype, "typeAlias", void 0);
__decorate([
    property({ type: String })
], UIOMaticEditElement.prototype, "id", void 0);
__decorate([
    state()
], UIOMaticEditElement.prototype, "_loading", void 0);
__decorate([
    state()
], UIOMaticEditElement.prototype, "_saving", void 0);
__decorate([
    state()
], UIOMaticEditElement.prototype, "_properties", void 0);
__decorate([
    state()
], UIOMaticEditElement.prototype, "_errors", void 0);
__decorate([
    state()
], UIOMaticEditElement.prototype, "_serverError", void 0);
UIOMaticEditElement = __decorate([
    customElement('uiomatic-edit')
], UIOMaticEditElement);
export { UIOMaticEditElement };
