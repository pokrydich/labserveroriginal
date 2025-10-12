import { ICourseLab } from "./course-lab";
import { IEntityBase } from "./entity-base";
import { IStudentLab } from "./student-lab";

export interface IGroupCourseLab extends IEntityBase {
    groupCourseLabId: number;

    courseLabId: number | undefined;
    gitLabGroupId: number | undefined;
    
    // children links
    studentLabs: IStudentLab[] | undefined;

    // parent links
    courseLab: ICourseLab | undefined;
}