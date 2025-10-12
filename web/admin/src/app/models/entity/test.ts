import { IEntityBase } from "./entity-base";


export interface ITest extends IEntityBase {
    name: string;
    testServerUrl: string;
}