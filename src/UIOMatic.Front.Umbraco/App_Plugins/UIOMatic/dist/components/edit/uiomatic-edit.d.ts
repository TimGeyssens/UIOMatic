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
export declare class UIOMaticEditElement extends LitElement {
    typeAlias: string;
    id: string;
    private _item;
    private _fields;
    private _loading;
    private _errors;
    constructor();
    firstUpdated(): Promise<void>;
    private _loadItem;
    private _loadFields;
    private _validateField;
    private _handleInput;
    private _handleSubmit;
    render(): import("lit-html").TemplateResult<1>;
    static styles: import("lit").CSSResult;
}
