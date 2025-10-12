import { Injectable } from '@angular/core';
import { RestApiServiceBase } from './rest-api-base.service';
import { ITest } from '../../models/entity/test';

@Injectable({
  providedIn: 'root'
})
export class TestService extends RestApiServiceBase<ITest> {

  constructor() {
    super("Tests");
  }
  
}
