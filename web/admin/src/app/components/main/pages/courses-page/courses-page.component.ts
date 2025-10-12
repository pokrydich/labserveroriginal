import { Component, OnDestroy, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { UpdatableMap } from '../../../../utils/utils';
import { ICourse } from '../../../../models/entity/course';
import { CoursesService } from '../../../../services/API/courses.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { CreateCourseModalComponent } from './create-course-modal/create-course-modal.component';
import { Subscription } from 'rxjs';
import { HubClientService } from '../../../../services/API/hub-client.service';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-courses-page',
  standalone: true,
  imports: [TranslateModule, RouterLink],
  templateUrl: './courses-page.component.html',
  styleUrl: './courses-page.component.css'
})
export class CoursesPageComponent implements OnInit, OnDestroy {
  protected courses: UpdatableMap<ICourse> = new UpdatableMap(c => String(c.id));

  protected createCourseModalRef?: BsModalRef;

  protected courseUpdateSubscription?: Subscription;

  constructor (private coursesService: CoursesService,
    private modalService: BsModalService,
    private hubClient: HubClientService) {}

  async ngOnInit() {
    this.courses.setElems(await this.coursesService.getAll());

    this.courseUpdateSubscription = this.hubClient.courseDataUpdates.subscribe(course => {
      this.courses.onUpdate(course);
    })
  }

  async ngOnDestroy() {
    this.courseUpdateSubscription?.unsubscribe();
  }

  protected openCreateCourseModal() {
    this.createCourseModalRef = this.modalService.show(CreateCourseModalComponent);
  }
}
