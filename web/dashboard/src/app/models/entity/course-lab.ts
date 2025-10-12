import { ICourse } from "./course";
import { IEntityBase } from "./entity-base";

export interface ICourseLab extends IEntityBase {
    name: string;
    courseId: number | undefined;
    gitLabName: string | undefined;

    // parent links
    course: ICourse | undefined;
}