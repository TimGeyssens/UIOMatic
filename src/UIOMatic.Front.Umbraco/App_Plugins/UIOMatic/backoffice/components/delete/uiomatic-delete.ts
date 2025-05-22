import { LitElement, html, css } from 'lit';
import { customElement, property } from 'lit/decorators.js';

declare global {
  interface Window {
    UmbNotificationService: {
      show: (options: { type: string; message: string }) => void;
    };
  }
}

@customElement('uiomatic-delete')
export class UIOMaticDeleteElement extends LitElement {
  @property({ type: String })
  typeAlias = '';

  @property({ type: String })
  id = '';

  private async _handleDelete() {
    if (!confirm('Are you sure you want to delete this item?')) {
      return;
    }

    try {
      const response = await fetch(`/umbraco/backoffice/UIOMatic/Object/DeleteByIds?typeAlias=${this.typeAlias}&ids=${this.id}`, {
        method: 'DELETE'
      });

      if (response.ok) {
        window.UmbNotificationService.show({
          type: 'success',
          message: 'Item deleted successfully'
        });
        window.location.href = `/umbraco/#/uiomatic/list?ta=${this.typeAlias}`;
      } else {
        throw new Error('Failed to delete item');
      }
    } catch (error) {
      console.error('Error deleting item:', error);
      window.UmbNotificationService.show({
        type: 'error',
        message: 'Failed to delete item'
      });
    }
  }

  render() {
    return html`
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

  static styles = css`
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
} 