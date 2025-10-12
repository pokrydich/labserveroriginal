import { ICourse } from "./course";
import { IEntityBase } from "./entity-base";
import { IGroup } from "./group";
import { IGroupCourseLab } from "./group-course-lab";

export interface IGroupCourse extends IEntityBase {
    groupId: number | undefined;
    courseId: number;
    courseName: string | undefined;
    gitLabGroupId: number | undefined;

    group: IGroup | undefined;
    course: ICourse | undefined;

    groupLabs: IGroupCourseLab[] | undefined;
}