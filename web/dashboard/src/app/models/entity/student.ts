import { IEntityBase } from "./entity-base";
import { IGroup } from "./group";
import { IStudentLab } from "./student-lab";

export interface IStudent extends IEntityBase {
    groupId: number;
    name: string;
    email: string;
    username: string | undefined;
    gitLabUserId: number | undefined;
    initialPassword: string | undefined;

    group: IGroup | undefined;

    labsData: IStudentLab[];
}