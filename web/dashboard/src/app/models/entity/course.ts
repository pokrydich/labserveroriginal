import { ICourseLab } from "./course-lab";
import { IEntityBase } from "./entity-base";

export interface ICourse extends IEntityBase {
    name: string;
    gitLabName: string | undefined;

    courseLabs: ICourseLab[] | undefined;
}