declare module '@umbraco-cms/element' {
  export class UmbLitElement extends HTMLElement {
    requestUpdate(): void;
  }
}

declare module '@umbraco-cms/styles' {
  export const UmbTextStyles: any;
}

declare module '@umbraco-cms/notification' {
  export class UmbNotificationService {
    constructor(host: HTMLElement);
    peek(type: 'positive' | 'negative', options: { headline: string; message: string }): void;
  }
}

declare module '@umbraco-cms/modal' {
  export const UMB_MODAL_MANAGER_CONTEXT: string;
}

declare module '@umbraco-cms/context-api' {
  export const UmbContextConsumerMixin: any;
}

declare module '@umbraco-cms/entity-action' {
  export class UmbEntityActionBase {
    constructor();
  }
}

declare module '@umbraco-cms/repository' {
  export class UmbRepositoryItemsManager {
    constructor();
  }
}

declare module 'lit/decorators.js' {
  export const customElement: (tagName: string) => ClassDecorator;
  export const property: (options?: PropertyDeclaration) => PropertyDecorator;
  export const state: () => PropertyDecorator;
  export const html: (strings: TemplateStringsArray, ...values: any[]) => TemplateResult;
}

declare module 'lit' {
  export const css: (strings: TemplateStringsArray, ...values: any[]) => CSSResult;
} 