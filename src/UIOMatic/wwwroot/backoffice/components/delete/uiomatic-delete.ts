import { customElement, html, property, state } from 'lit/decorators.js';
import { UmbLitElement } from '@umbraco-cms/element';
import { UmbTextStyles } from '@umbraco-cms/styles';
import { css } from 'lit';
import { UmbNotificationService } from '@umbraco-cms/notification';

@customElement('uiomatic-delete')
export class UIOMaticDeleteElement extends UmbLitElement {
  @property({ type: String })
  typeAlias = '';

  @property({ type: String })
  id = '0';

  @state()
  private _deleting = false;

  @state()
  private _error = '';

  private _notificationService: UmbNotificationService;

  constructor() {
    super();
    this._notificationService = new UmbNotificationService(this);
  }

  private async _handleDelete() {
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
    } catch (error) {
      console.error('Error deleting:', error);
      this._error = error instanceof Error ? error.message : 'Failed to delete';
      this._notificationService.peek('negative', {
        headline: 'Error',
        message: this._error
      });
    } finally {
      this._deleting = false;
    }
  }

  render() {
    return html`
      <umb-body-layout>
        <div slot="header">
          <h1>Delete ${this.typeAlias}</h1>
        </div>

        ${this._error ? html`
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

  static styles = [
    UmbTextStyles,
    css`
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
} 