import { Injectable } from '@angular/core';
import { ApiServiceBase, cDataConversionOption } from './api-base.service';
import { IStudentLab } from '../../models/entity/student-lab';

@Injectable({
  providedIn: 'root'
})
export class StudentService extends ApiServiceBase {
  private makeEndpoint(studentId: number, endpoint: string): string {
    return `/${studentId}/labs${endpoint}`;
  }

  constructor() {
    super("rest/Students");
  }

  public async getLabById(studentId: number, studentLabId: number, include: cDataConversionOption = cDataConversionOption.Default): Promise<IStudentLab> {
    return await this.apiGet<IStudentLab>(this.makeEndpoint(studentId, `/${studentLabId}`), include);
  }

  public async updateLabById(studentId: number, studentLabId: number, lab: IStudentLab): Promise<IStudentLab> {
    return await this.apiPut<IStudentLab>(this.makeEndpoint(studentId, `/${studentLabId}`), lab);
  }
}
