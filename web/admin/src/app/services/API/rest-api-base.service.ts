import { IEntityBase } from '../../models/entity/entity-base';
import { ApiServiceBase } from './api-base.service';

export class RestApiServiceBase<TEntity> extends ApiServiceBase {
  constructor(endpoint: string) {
    super(`rest/${endpoint}`);
  }

  public async getAll() {
    return await this.apiGet<TEntity[]>("");
  }

  public async getById(id: number) {
    return await this.apiGet<TEntity>(`/${id}`);
  }

  public async create(elem: TEntity): Promise<TEntity> {
    return await this.apiPost<TEntity>("", elem);
  }

  public async update(id: number, elem: TEntity): Promise<TEntity>  {
    return await this.apiPut<TEntity>(`/${id}`, elem);
  }
}
