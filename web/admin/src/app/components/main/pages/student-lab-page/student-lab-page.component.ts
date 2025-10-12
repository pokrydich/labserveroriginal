import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { StudentService } from '../../../../services/API/student.service';
import { cStudentLabStatus, StudentLab } from '../../../../models/entity/student-lab';
import { cDataConversionOption } from '../../../../services/API/api-base.service';
import { StudentLabSubmission } from '../../../../models/entity/student-lab-submission';
import { DatePipe, NgClass } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-student-lab-page',
  standalone: true,
  imports: [TranslateModule, DatePipe, NgClass, FormsModule, RouterLink],
  templateUrl: './student-lab-page.component.html',
  styleUrl: './student-lab-page.component.css'
})
export class StudentLabPageComponent implements OnInit, OnDestroy {
  protected studentId?: number;
  protected studentLabId?: number;

  protected studentLab?: StudentLab;

  constructor(private route: ActivatedRoute,
    private studentService: StudentService) {}
  
  async ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const labId = params.get("labId");
      const studentId = params.get("studentId");

      if (labId && studentId) {
        this.studentLabId = +labId;
        this.studentId = +studentId;

        const rawLab = await this.studentService.getLabById(this.studentId, this.studentLabId, cDataConversionOption.Full);
        this.studentLab = new StudentLab(rawLab);
      }
    })  
  }

  async ngOnDestroy() {
  }

  protected getLabSubmissions(): StudentLabSubmission[] {
    if (this.studentLab && this.studentLab.labSubmissions) {
      return this.studentLab.labSubmissions.sort((f, s) => f.parsedSubmittedDate < s.parsedSubmittedDate ? 1 : -1);
    }
    return [];
  }

  protected getLabStatuses(): cStudentLabStatus[] {
    return [cStudentLabStatus.cInProgress, cStudentLabStatus.cOverdue, cStudentLabStatus.cCompleted];
  }

  protected getStatusTranslationSuffix(status: cStudentLabStatus): string {
    switch (status) {
      case cStudentLabStatus.cInProgress:
        return 'InProgress';
      case cStudentLabStatus.cOverdue:
        return 'Overdue';
      case cStudentLabStatus.cCompleted:
        return 'Completed';
      default:
        return 'UnkownStatus';
    }
  }

  protected getStatusBgColor(status: cStudentLabStatus): string {
    if (this.studentLab && this.studentLab.status === status) {
      switch (status) {
        case cStudentLabStatus.cInProgress:
        return 'bg-primary text-white fw-bold';
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

  protected setStatus(status: cStudentLabStatus) {
    if (this.studentLab) {
      this.studentLab.status = status;
    }
  }

  protected async saveLabData() {
    if (this.studentId && this.studentLabId && this.studentLab) {
      await this.studentService.updateLabById(this.studentId, this.studentLabId, this.studentLab)
    }
  }
}
