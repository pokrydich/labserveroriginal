import { IEntityBase } from "./entity-base";
import { IStudent } from "./student";


export interface IGroup extends IEntityBase {
    name: string;
    gitLabName: string | undefined;
    gitLabGroupId: number | undefined;

    students: IStudent[] | undefined;
}