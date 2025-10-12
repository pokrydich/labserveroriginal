import { Routes } from '@angular/router';
import { EnterTokenComponent } from './components/enter-token/enter-token.component';
import { MyLabsComponent } from './components/my-labs/my-labs.component';
import { ErrorComponent } from './components/error/error.component';

export const routes: Routes = [
    {
        path: '',
        component: EnterTokenComponent
    },
    {
        path: 'mylabs/:token',
        component: MyLabsComponent
    },
    {
        path: 'error',
        component: ErrorComponent
    }
];
