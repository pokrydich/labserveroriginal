import { IEntityBase } from "./entity-base";
import { IGitLabMergeRequest } from "./gitlab/gitlab-merge-request";

export enum StudentLabSubmissionStatus {
    cActive = 0, // current lates lab submission (test will be executed for such submissions)
    cRevoked, // user revoked merge request
    cDeprecated // newer submission is available
}

export interface IStudentLabSubmission extends IEntityBase {
    studentLabId: number;
    status: StudentLabSubmissionStatus;
    submittedDate: string;
    
    gitLabMergeRequestId: number | undefined;
    gitLabMergeRequest: IGitLabMergeRequest | undefined;
}

export class StudentLabSubmission implements IStudentLabSubmission {
    id: number | undefined;
    
    studentLabId: number;
    status: StudentLabSubmissionStatus;
    submittedDate: string;
    parsedSubmittedDate: number;

    gitLabMergeRequestId: number | undefined;
    gitLabMergeRequest: IGitLabMergeRequest | undefined;
    
    constructor(raw: IStudentLabSubmission) {
        this.id = raw.id;
        
        this.studentLabId = raw.studentLabId;
        this.status = raw.status;
        this.submittedDate = raw.submittedDate;
        this.parsedSubmittedDate = Date.parse(this.submittedDate);

        this.gitLabMergeRequestId = raw.gitLabMergeRequestId;
        this.gitLabMergeRequest = raw.gitLabMergeRequest;
    }
}