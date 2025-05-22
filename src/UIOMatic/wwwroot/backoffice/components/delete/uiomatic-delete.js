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
let UIOMaticDeleteElement = class UIOMaticDeleteElement extends UmbLitElement {
    constructor() {
        super();
        this.typeAlias = '';
        this.id = '0';
        this._deleting = false;
        this._error = '';
        this._notificationService = new UmbNotificationService(this);
    }
    async _handleDelete() {
        if (!confirm('Are you sure you want to delete this item?')) {
            return;
        }
        this._deleting = true;
        try {
            const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${this.id}`, {
                method: 'DELETE'
            });
            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to delete');
            }
            this._notificationService.peek('positive', {
                headline: 'Success',
                message: 'Item deleted successfully'
            });
            // Redirect to list view
            window.location.href = `/umbraco/#/uiomatic/list/${this.typeAlias}`;
        }
        catch (error) {
            console.error('Error deleting:', error);
            this._error = error instanceof Error ? error.message : 'Failed to delete';
            this._notificationService.peek('negative', {
                headline: 'Error',
                message: this._error
            });
        }
        finally {
            this._deleting = false;
        }
    }
    render() {
        return html `
      <umb-body-layout>
        <div slot="header">
          <h1>Delete ${this.typeAlias}</h1>
        </div>

        ${this._error ? html `
          <umb-alert
            type="error"
            .message=${this._error}
          ></umb-alert>
        ` : ''}

        <div class="delete-container">
          <p>Are you sure you want to delete this item? This action cannot be undone.</p>
          
          <div class="button-container">
            <umb-button
              @click=${this._handleDelete}
              .state=${this._deleting ? 'waiting' : 'default'}
              look="danger"
              label=${this._deleting ? 'Deleting...' : 'Delete'}
            ></umb-button>
            <umb-button
              @click=${() => window.history.back()}
              look="secondary"
              label="Cancel"
            ></umb-button>
          </div>
        </div>
      </umb-body-layout>
    `;
    }
};
UIOMaticDeleteElement.styles = [
    UmbTextStyles,
    css `
      :host {
        display: block;
        padding: var(--uui-size-space-5);
      }

      .delete-container {
        padding: var(--uui-size-space-4);
        text-align: center;
      }

      .button-container {
        margin-top: var(--uui-size-space-4);
      }

      umb-button {
        margin: 0 var(--uui-size-space-2);
      }
    `
];
__decorate([
    property({ type: String })
], UIOMaticDeleteElement.prototype, "typeAlias", void 0);
__decorate([
    property({ type: String })
], UIOMaticDeleteElement.prototype, "id", void 0);
__decorate([
    state()
], UIOMaticDeleteElement.prototype, "_deleting", void 0);
__decorate([
    state()
], UIOMaticDeleteElement.prototype, "_error", void 0);
UIOMaticDeleteElement = __decorate([
    customElement('uiomatic-delete')
], UIOMaticDeleteElement);
export { UIOMaticDeleteElement };
