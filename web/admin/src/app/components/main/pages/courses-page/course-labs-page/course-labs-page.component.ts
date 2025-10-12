import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { UpdatableMap } from '../../../../../utils/utils';
import { ICourseLab } from '../../../../../models/entity/course-lab';
import { CourseLabsService } from '../../../../../services/API/course-labs.service';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { CoursesService } from '../../../../../services/API/courses.service';
import { ICourse } from '../../../../../models/entity/course';
import { CreateCourseLabModalComponent } from './create-course-lab-modal/create-course-lab-modal.component';
import { HubClientService } from '../../../../../services/API/hub-client.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-course-labs-page',
  standalone: true,
  imports: [TranslateModule, RouterLink],
  templateUrl: './course-labs-page.component.html',
  styleUrl: './course-labs-page.component.css'
})
export class CourseLabsPageComponent implements OnInit, OnDestroy {
  protected courseId?: number;
  protected course?: ICourse;

  protected labs: UpdatableMap<ICourseLab> = new UpdatableMap<ICourseLab>(l => String(l.id));

  protected createCourseLabModalRef?: BsModalRef;

  protected courseLabUpdateSubscription?: Subscription;

  constructor(private route: ActivatedRoute,
    private labsService: CourseLabsService,
    private coursesService: CoursesService,
    private modalService: BsModalService,
    private hubClient: HubClientService) {}
  
  async ngOnInit() {
    this.route.paramMap.subscribe(async params => {
      const paramId = params.get("id");
      if (paramId) {
        this.courseId = +paramId;

        this.course = await this.coursesService.getById(this.courseId);

        this.labs.setElems(await this.labsService.getAll(this.courseId));
      }
    });

    this.courseLabUpdateSubscription = this.hubClient.courseLabDataUpdates.subscribe(lab => {
      this.labs.onUpdate(lab);
    })
  }

  async ngOnDestroy() {
    this.courseLabUpdateSubscription?.unsubscribe();
  }

  protected openCreateLabModal() {
    if (this.course) {
      const initialState: ModalOptions = {
        initialState: {
          courseId: this.course.id,
          courseName: this.course.name
        }
      };

      this.createCourseLabModalRef = this.modalService.show(CreateCourseLabModalComponent, initialState);
    }
  }
}
