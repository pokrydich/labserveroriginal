import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { IStudent } from '../models/entity/student';
import { firstValueFrom } from 'rxjs';
import { IApiResponse } from '../models/API/api-response';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private httpClient: HttpClient = inject(HttpClient);
  
  public static endpointStart: string = "https://my.ooplabs.ru";

  private endpoint: string;

  constructor(private router: Router) {
    this.endpoint = `${ApiService.endpointStart}/api/my`;
  }

  public async getData(token: string) : Promise<IStudent | any> {
    let url = new URL(`${this.endpoint}/${token}`);

    const resp = await firstValueFrom(this.httpClient.get<IApiResponse<IStudent>>(url.toString()));
    if (!resp.successful) {
      this.router.navigate(['/error']);
      return null;
    } else {

      if (resp.result === undefined) {
        return true;
      }
      return resp.result;
    }
  }
}
