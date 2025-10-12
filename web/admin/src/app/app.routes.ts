import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { MainComponent } from './components/main/main.component';
import { RegisterComponent } from './components/register/register.component';
import { UserPreferencesMenuComponent } from './components/main/user-preferences-menu/user-preferences-menu.component';
import { UsersPageComponent } from './components/main/pages/users-page/users-page.component';
import { CoursesPageComponent } from './components/main/pages/courses-page/courses-page.component';
import { GroupsPageComponent } from './components/main/pages/groups-page/groups-page.component';
import { TestsPageComponent } from './components/main/pages/tests-page/tests-page.component';
import { GroupStudentsPageComponent } from './components/main/pages/groups-page/group-students-page/group-students-page.component';
import { CourseLabsPageComponent } from './components/main/pages/courses-page/course-labs-page/course-labs-page.component';
import { GroupCoursesPageComponent } from './components/main/pages/groups-page/group-courses-page/group-courses-page.component';
import { GroupCourseDetailsPageComponent } from './components/main/pages/groups-page/group-courses-page/group-course-details-page/group-course-details-page.component';
import { StudentLabPageComponent } from './components/main/pages/student-lab-page/student-lab-page.component';

export const routes: Routes = [
    {
        path: '',
        component: MainComponent,
        children: [
            {
                path: 'account-settings',
                component: UserPreferencesMenuComponent
            },
            {
                path: 'courses',
                component: CoursesPageComponent
            },
            {
                path: 'course/:id/labs',
                component: CourseLabsPageComponent
            },
            {
                path: 'groups',
                component: GroupsPageComponent
            },
            {
                path: 'group/:id/students',
                component: GroupStudentsPageComponent
            },
            {
                path: 'group/:id/courses',
                component: GroupCoursesPageComponent
            },
            {
                path: 'group/:groupId/course/:courseId',
                component: GroupCourseDetailsPageComponent
            },
            {
                path: 'student/:studentId/lab/:labId',
                component: StudentLabPageComponent
            },
            {
                path: 'tests',
                component: TestsPageComponent
            },
            {
                path: 'users',
                component: UsersPageComponent
            }
        ]
    },
    {
        path: 'login',
        component: LoginComponent
    },
    {
        path: 'register',
        component: RegisterComponent
    }
];
