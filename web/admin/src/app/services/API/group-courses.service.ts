import { Injectable } from '@angular/core';
import { ApiServiceBase, cDataConversionOption } from './api-base.service';
import { IGroupCourse } from '../../models/entity/group-course';
import { IGroupCourseLab } from '../../models/entity/group-course-lab';

@Injectable({
  providedIn: 'root'
})
export class GroupCoursesService extends ApiServiceBase {
  private makeEndpoint(courseId: number, endpoint: string): string {
    return `/${courseId}/courses${endpoint}`;
  }

  constructor() {
    super("rest/Groups");
  }

  public async getAll(groupId: number): Promise<IGroupCourse[]> {
    return await this.apiGet<IGroupCourse[]>(this.makeEndpoint(groupId, ""));
  }

  public async getById(groupId: number, courseId: number, dataOption: cDataConversionOption = cDataConversionOption.Default): Promise<IGroupCourse> {
    return await this.apiGet<IGroupCourse>(this.makeEndpoint(groupId, `/${courseId}`), dataOption);
  }

  public async assignCourse(groupId: number, courseId: number): Promise<IGroupCourse> {
    return await this.apiPost<IGroupCourse>(this.makeEndpoint(groupId, ""), {
      courseId: courseId
    });
  }

  public async getAllLabs(groupId: number, courseId: number): Promise<IGroupCourseLab[]> {
    return await this.apiGet<IGroupCourseLab[]>(this.makeEndpoint(groupId, `/${courseId}/labs`));
  }

  public async getLabById(groupId: number, courseId: number, courseLabId: number): Promise<IGroupCourse> {
    return await this.apiGet<IGroupCourseLab>(this.makeEndpoint(groupId, `/${courseId}/labs/${courseLabId}`));
  }

  public async startCourseLab(groupId: number, courseId: number, courseLabId: number) {
    await this.apiPost<boolean>(this.makeEndpoint(groupId, `/${courseId}/labs`), {
      courseLabId: courseLabId
    })
  }

  public async gilabSyncCourses(groupId: number) {
    await this.apiGet<boolean>(this.makeEndpoint(groupId, "/sync"));
  }

  public async gitlabSyncGroupCourse(groupId: number, courseId: number) {
    await this.apiGet<boolean>(this.makeEndpoint(groupId, `/${courseId}/sync`));
  }
}
