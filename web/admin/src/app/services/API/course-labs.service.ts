import { Injectable } from '@angular/core';
import { ApiServiceBase } from './api-base.service';
import { ICourse } from '../../models/entity/course';
import { ICourseLab } from '../../models/entity/course-lab';

@Injectable({
  providedIn: 'root'
})
export class CourseLabsService extends ApiServiceBase {

  private makeEndpoint(courseId: number, endpoint: string): string {
    return `/${courseId}/labs${endpoint}`;
  }

  constructor() {
    super("rest/Courses");
  }

  public async getAll(courseId: number): Promise<ICourseLab[]> {
    return await this.apiGet<ICourseLab[]>(this.makeEndpoint(courseId, ""));
  }

  public async createLab(courseId: number, lab: ICourseLab): Promise<ICourseLab> {
    return await this.apiPost<ICourseLab>(this.makeEndpoint(courseId, ""), lab);
  }
}
