export interface TreeNode {
    id: number | string;
    mid?: number | string;
    fid?: number | string;
    pids: number[];
    childrenIds: number[];
    name: string;
    gender: string;
    img?: string;
}