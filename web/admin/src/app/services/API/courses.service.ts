import { Injectable } from '@angular/core';
import { RestApiServiceBase } from './rest-api-base.service';
import { ICourse } from '../../models/entity/course';

@Injectable({
  providedIn: 'root'
})
export class CoursesService extends RestApiServiceBase<ICourse> {

  constructor() {
    super("Courses");
  }
}
