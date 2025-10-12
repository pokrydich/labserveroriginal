import { Injectable } from '@angular/core';
import { HttpTransportType, HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { AuthService } from '../auth.service';
import { IUser } from '../../models/entity/user';
import { Observable, Subject } from 'rxjs';
import { ApiServiceBase } from './api-base.service';
import { IGroup } from '../../models/entity/group';
import { IStudent } from '../../models/entity/student';
import { ICourse } from '../../models/entity/course';
import { ICourseLab } from '../../models/entity/course-lab';
import { IGroupCourse } from '../../models/entity/group-course';
import { IGroupCourseLab } from '../../models/entity/group-course-lab';
import { IStudentLab } from '../../models/entity/student-lab';
import { ITest } from '../../models/entity/test';

@Injectable({
  providedIn: 'root'
})
export class HubClientService {
  private hubConnection: HubConnection;

  private pendingUpdatedUserData = new Subject<IUser>();

  private pendingUpdatedGroupData = new Subject<IGroup>();
  private pendingUpdatedGroupCourseData = new Subject<IGroupCourse>();
  private pendingUpdatedGroupCourseLabData = new Subject<IGroupCourseLab>();

  private pendingUpdatedStudentData = new Subject<IStudent>();
  private pendingUpdatedStudentLabData = new Subject<IStudentLab>();

  private pendingUpdatedCourseData = new Subject<ICourse>();
  private pendingUpdatedCourseLabData = new Subject<ICourseLab>();

  private pendingUpdatedTestData = new Subject<ITest>(); 


  public userDataUpdates: Observable<IUser> = this.pendingUpdatedUserData.asObservable();

  public groupDataUpdates: Observable<IGroup> = this.pendingUpdatedGroupData.asObservable();
  public groupCourseDataUpdates: Observable<IGroupCourse> = this.pendingUpdatedGroupCourseData.asObservable();
  public groupCourseLabDataUpdates: Observable<IGroupCourseLab> = this.pendingUpdatedGroupCourseLabData.asObservable();

  public studentDataUpdates: Observable<IStudent> = this.pendingUpdatedStudentData.asObservable();
  public studentLabDataUpdates: Observable<IStudentLab> = this.pendingUpdatedStudentLabData.asObservable();

  public courseDataUpdates: Observable<ICourse> = this.pendingUpdatedCourseData.asObservable();
  public courseLabDataUpdates: Observable<ICourseLab> = this.pendingUpdatedCourseLabData.asObservable();

  public testDataUpdates: Observable<ITest> = this.pendingUpdatedTestData.asObservable();

  constructor(private authService: AuthService) {
    const hubUrl = `${ApiServiceBase.endpointStart}/hub/data`; // production

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(hubUrl, {
        withCredentials: this.authService.getAuthToken() != null,
        accessTokenFactory: () => this.authService.getAuthToken() ?? '',
        skipNegotiation: true,
        transport: HttpTransportType.WebSockets
      })
      .build();

    this.hubConnection
      .start()
      .then(() => console.log("[*] Connected to SignalR hub"))
      .catch(error => console.error("[!] Error connecting to SignalR hub", error));

    this.hubConnection.on('UserData', (user: IUser) => {
      this.pendingUpdatedUserData.next(user);
    });

    this.hubConnection.on('GroupData', (group: IGroup) => {
      this.pendingUpdatedGroupData.next(group);
    });
    this.hubConnection.on('GroupCourseData', (groupCourse: IGroupCourse) => {
      this.pendingUpdatedGroupCourseData.next(groupCourse);
    });
    this.hubConnection.on('GroupCourseLabData', (groupCourseLab: IGroupCourseLab) => {
      this.pendingUpdatedGroupCourseLabData.next(groupCourseLab);
    });

    this.hubConnection.on('StudentData', (student: IStudent) => {
      this.pendingUpdatedStudentData.next(student);
    });
    this.hubConnection.on('StudentLabData', (studentLab: IStudentLab) => {
      this.pendingUpdatedStudentLabData.next(studentLab);
    });

    this.hubConnection.on('CourseData', (course: ICourse) => {
      this.pendingUpdatedCourseData.next(course);
    });
    this.hubConnection.on('CourseLabData', (courseLab: ICourseLab) => {
      this.pendingUpdatedCourseLabData.next(courseLab);
    });
    
    this.hubConnection.on('TestData', (test: ITest) => {
      this.pendingUpdatedTestData.next(test);
    });
  }
}
