export default interface Query {
  pageNumber?: number;
  pageSize?: number;
  filter?: string;
  filterFields?: FilterField[];
  sortFields?: SortField[];
  withRemoved?: boolean;

  projectId?: number;
  teamId?: number;
}

export interface FilterField {
  fieldName: string;
  values: string[] | boolean[];
}

export interface SortField {
  fieldName: string;
  sortType: SortType;
}

export enum SortType {
  Ascending = 'Ascending',
  Descending = 'Descending'
}
