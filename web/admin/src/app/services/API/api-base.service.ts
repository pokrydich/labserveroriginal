import { HttpClient } from '@angular/common/http';
import { inject } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { IApiResponse } from '../../models/API/api-response';
import { PopupNotificationsService } from '../../popup-notifications.service';

export enum cDataConversionOption {
  Default = 0,
  Parent,
  Children,
  Mapping,
  Full,
  GitLab
}

export class ApiServiceBase {
  private httpClient: HttpClient = inject(HttpClient);
  protected notificationService: PopupNotificationsService = inject(PopupNotificationsService);

  //public static endpointStart: string = "https://admin.ooplabs.ru";
  public static endpointStart: string = "http://admin.labserver.local"; // DEBUG
  //public static endpointStart: string = "http://localhost:5288"; // DEBUG

  private endpoint: string;

  constructor(endpoint: string) {
    this.endpoint = `${ApiServiceBase.endpointStart}/api/${endpoint}`; // production
  }

  protected async apiGet<TResp>(endpoint: string, dataOption: cDataConversionOption = cDataConversionOption.Default) : Promise<TResp | any> {
    let url = new URL(`${this.endpoint}${endpoint}`);
    if (dataOption !== cDataConversionOption.Default) {
      url.searchParams.append('include', String(dataOption));
    }

    const resp = await firstValueFrom(this.httpClient.get<IApiResponse<TResp>>(url.toString()));
    if (!resp.successful) {
      this.logError(resp);
      return null;
    } else {
      if (resp.warnings.length > 0) {
        for (let warn of resp.warnings) {
          this.notificationService.warning(warn);
        }
      }

      if (resp.result === undefined) {
        return true;
      }
      return resp.result;
    }
  }

  protected async apiPost<TResp>(endpoint: string, request: any): Promise<TResp | any> {
    const resp = await firstValueFrom(this.httpClient.post<IApiResponse<TResp>>(`${this.endpoint}${endpoint}`, request));
    if (!resp.successful) {
      this.logError(resp);
      return null;
    } else {
      this.notificationService.success('apiResponseNotificationDataCreateSuccess');

      if (resp.result === undefined) {
        return true;
      }
      return resp.result;
    }
  }

  protected async apiPut<TResp>(endpoint: string, request: any): Promise<TResp | any> {
    const resp = await firstValueFrom(this.httpClient.put<IApiResponse<TResp>>(`${this.endpoint}${endpoint}`, request));
    if (!resp.successful) {
      this.logError(resp);
      return null;
    } else {
      this.notificationService.success('apiResponseNotificationDataUpdateSuccess');

      if (resp.result === undefined) {
        return true;
      }
      return resp.result;
    }
  }

  protected async apiDelete<TResp>(endpoint: string): Promise<TResp | any> {
    const resp = await firstValueFrom(this.httpClient.delete<IApiResponse<TResp>>(`${this.endpoint}${endpoint}`));
    if (!resp.successful) {
      this.logError(resp);
      return null;
    } else {
      if (resp.result === undefined) {
        return true;
      }
      return resp.result;
    }
  }

  private logError(resp: IApiResponse<any>) {
    console.log(`[!] API Error: ${resp.error}`);
    if (resp.error !== null) {
      this.notificationService.error(resp.error);
    }
  }
}
