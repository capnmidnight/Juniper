export interface MenuItemData {
    id: number;
    label: string;
    order: number;
    enabled: boolean;
    visible: boolean;

    parentID?: number;

    fileID?: number;
    filePath?: string;
    fileName?: string;
    fileType?: string;
    fileSize?: number;

    actionApp?: string;
    actionParam?: string;
}