import { IEntityBase } from "./entity-base";
import { IGitLabProject } from "./gitlab/gitlab-project";
import { IGroupCourseLab } from "./group-course-lab";
import { IStudent } from "./student";
import { IStudentLabSubmission, StudentLabSubmission, StudentLabSubmissionStatus } from "./student-lab-submission";

export enum cStudentLabStatus {
    cInProgress = 0,
    cOverdue = 1,
    cCompleted = 2
}

export interface IStudentLab extends IEntityBase {
    studentId: number;
    groupCourseLabId: number;
    gitLabProjectId: number | undefined;
    openedDate: string | undefined;
    status: cStudentLabStatus;
    notes: string;

    labSubmissions: IStudentLabSubmission[] | undefined;

    gitLabProject: IGitLabProject | undefined;

    // parent links
    student: IStudent | undefined;
    groupCourseLab: IGroupCourseLab | undefined;
}

export class StudentLab implements IStudentLab {
    id: number | undefined;
    
    studentId: number;
    groupCourseLabId: number;
    gitLabProjectId: number | undefined;
    openedDate: string | undefined;
    parsedData: number | undefined;
    status: cStudentLabStatus;
    notes: string;

    labSubmissions: StudentLabSubmission[] | undefined;

    gitLabProject: IGitLabProject | undefined;

    student: IStudent | undefined;
    groupCourseLab: IGroupCourseLab | undefined;

    constructor(raw: IStudentLab) {
        this.id = raw.id;

        this.studentId = raw.studentId;
        this.groupCourseLabId = raw.groupCourseLabId;
        this.gitLabProjectId = raw.gitLabProjectId;
        this.openedDate = raw.openedDate;
        if (this.openedDate) {
            this.parsedData = Date.parse(raw.openedDate!);
        }
        this.status = raw.status;
        this.notes = raw.notes;

        this.labSubmissions = raw.labSubmissions?.map(rawLs => new StudentLabSubmission(rawLs)) ?? [];
        
        this.gitLabProject = raw.gitLabProject;

        this.student = raw.student;
        this.groupCourseLab = raw.groupCourseLab;
    }
}