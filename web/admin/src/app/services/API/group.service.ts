import { Injectable } from '@angular/core';
import { RestApiServiceBase } from './rest-api-base.service';
import { IGroup } from '../../models/entity/group';

@Injectable({
  providedIn: 'root'
})
export class GroupService extends RestApiServiceBase<IGroup> {

  constructor() {
    super("Groups");
  }

  /**
   * request groups sync with gitlab
   * @returns bool (indicates sync success or failure)
   */
  public async gitlabSync(): Promise<boolean> {
    return await this.apiGet<boolean>("/sync");
  }
}
