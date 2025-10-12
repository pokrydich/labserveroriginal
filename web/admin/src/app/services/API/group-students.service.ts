import { Injectable } from '@angular/core';
import { ApiServiceBase } from './api-base.service';
import { IStudent } from '../../models/entity/student';

@Injectable({
  providedIn: 'root'
})
export class GroupStudentsService extends ApiServiceBase {

  private makeEndpoint(groupId: number, endpoint: string): string {
    return `/${groupId}/students${endpoint}`;
  }

  constructor() {
    super("rest/Groups");
  }

  public async getStudents(groupId: number): Promise<IStudent[]> {
    return await this.apiGet<IStudent>(this.makeEndpoint(groupId, ""));
  }

  public async addStudnet(student: IStudent) {
    return await this.apiPost<IStudent>(this.makeEndpoint(student.groupId, ""), student);
  }

  public async updateStudent(student: IStudent) {
    return await this.apiPut<IStudent>(this.makeEndpoint(student.groupId, `/${student.id}`), student);
  }

  public async gitlabSync(groupId: number) {
    return await this.apiGet<IStudent>(this.makeEndpoint(groupId, "/sync"));
  }

  public async importFromCsv(groupId: number, csvText: string) {
    return await this.apiPost<boolean>(this.makeEndpoint(groupId, "/import"), {
      "text" : csvText
    });
  }

  public async sendCredentialsAll(groupId: number) {
    return await this.apiGet<boolean>(this.makeEndpoint(groupId, "/sendcredentials"));
  }

  public async sendCredentialStudent(groupId: number, studentId: number) {
    return await this.apiGet<boolean>(this.makeEndpoint(groupId, `/${studentId}/sendcredentials`));
  }

  public async openStudentLab(groupId: number, studentId: number, groupCourseLabId: number) {
    await this.apiPost<boolean>(this.makeEndpoint(groupId, `/${studentId}/labs`), {
      groupCourseLabId: groupCourseLabId
    });
  }
}
