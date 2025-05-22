import { LitElement } from 'lit';
declare global {
    interface Window {
        UmbNotificationService: {
            show: (options: {
                type: string;
                message: string;
            }) => void;
        };
    }
}
export declare class UIOMaticDeleteElement extends LitElement {
    typeAlias: string;
    id: string;
    private _handleDelete;
    render(): import("lit-html").TemplateResult<1>;
    static styles: import("lit").CSSResult;
}
