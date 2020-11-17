import { Metadata } from '@/core/types/metadata.model'

export default class Query {
  pageNumber?: number
  pageSize?: number
  filter?: string
  filterFields?: FilterField[]
  sortFields?: SortField[]
  withRemoved?: boolean

  projectId?: number
  teamId?: number

  constructor(pageSize = 25) {
    this.pageNumber = 0
    this.pageSize = pageSize
  }
}

export interface FilterField {
  fieldName: string
  values: Array<string> | Array<number> | Array<boolean> | Array<Metadata[]>
}

export interface SortField {
  fieldName: string
  sortType: SortType
}

export enum SortType {
  Ascending = 'Ascending',
  Descending = 'Descending',
}
