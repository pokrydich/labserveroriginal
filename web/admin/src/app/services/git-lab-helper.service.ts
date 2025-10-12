import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GitLabHelperService {
  public static gitLabUrlBase: string = "https://git.ooplabs.ru"

  public static makeGroupLink(groupName: string): string {
    return `${GitLabHelperService.gitLabUrlBase}/${groupName}`;
  }

  public static makeUserLink(username: string): string {
    return `${GitLabHelperService.gitLabUrlBase}/${username}`;
  }

  public static fromUrl(rawUrl: string) {
    const url = new URL(rawUrl);
    return `${GitLabHelperService.gitLabUrlBase}${url.pathname}`;
  }

  constructor() { }
}
