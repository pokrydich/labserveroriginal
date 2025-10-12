import { Component, Input, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { cStudentLabStatus, IStudentLab, StudentLab } from '../../../../../../../models/entity/student-lab';
import { DatePipe, NgClass } from '@angular/common';
import { GitLabHelperService } from '../../../../../../../services/git-lab-helper.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-student-lab-preview',
  standalone: true,
  imports: [TranslateModule, DatePipe, RouterLink, NgClass],
  templateUrl: './student-lab-preview.component.html',
  styleUrl: './student-lab-preview.component.css'
})
export class StudentLabPreviewComponent implements OnInit {
  protected lab?: StudentLab;

  @Input() studentLab!: IStudentLab;

  async ngOnInit() {
    this.lab = new StudentLab(this.studentLab);
  }

  protected makeGitLabLink(): string {
    const rawUrl = this.lab?.gitLabProject?.web_url;
    if (rawUrl) {
      return GitLabHelperService.fromUrl(rawUrl);
    }
    return "";
  }

  protected latestSubmissionDate(): number | undefined {
    if (this.lab && this.lab.labSubmissions && this.lab.labSubmissions.length > 0) {
      return this.lab.labSubmissions!.sort((f, s) => f.parsedSubmittedDate < s.parsedSubmittedDate ? 1 : -1 )[0].parsedSubmittedDate;
    } else {
      return undefined;
    }
  }

  protected getStatusBgColor(status: cStudentLabStatus): string {
    if (this.studentLab && this.studentLab.status === status) {
      switch (status) {
        case cStudentLabStatus.cInProgress:
        return '';
      case cStudentLabStatus.cOverdue:
        return 'bg-danger text-white fw-bold';
      case cStudentLabStatus.cCompleted:
        return 'bg-success text-white fw-bold';
      default:
        return '';
      }
    } else {
      return "";
    }
  }
}
