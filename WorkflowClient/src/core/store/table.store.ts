import { getModule, Module, Mutation, VuexModule } from 'vuex-module-decorators'
import store from '@/core/store/index'
import Query, { FilterField, SortType } from '@/core/types/query.type'
import Entity from '@/core/types/entity.type'

@Module({
  dynamic: true,
  namespaced: true,
  name: 'tableStore',
  store,
})
class TableStore extends VuexModule {
  _data: Entity[] = []
  _isReloadRequired = false

  _query: Query = new Query()

  _selectedRow: Entity | null = null
  _selectedRows: Entity[] = []

  _filters: FilterField[] | undefined
  _search: string | undefined
  _order: SortType | undefined
  _sort: string | undefined

  public get data(): Entity[] {
    return this._data
  }
  public get query(): Query {
    return this._query
  }
  public get isReloadRequired(): boolean {
    return this._isReloadRequired
  }

  public get selectedRow(): Entity | null {
    return this._selectedRow
  }
  public get selectedRows(): Entity[] {
    return this._selectedRows
  }
  public get isMultiselect(): boolean {
    return this._selectedRows.length > 1
  }

  public get filters(): FilterField[] | undefined {
    return this._filters
  }
  public get search(): string | undefined {
    return this._search
  }
  public get order(): SortType | undefined {
    return this._order
  }
  public get sort(): string | undefined {
    return this._sort
  }

  @Mutation
  setData(value: Entity[]) {
    this._data = value
  }

  @Mutation
  appendData(value: Entity[]) {
    this._data = this._data.concat(value)
  }

  @Mutation
  setQuery(value: Query) {
    this._query = value
  }

  @Mutation
  extendQuery(
    property: Record<string, string | number | boolean | Array<FilterField> | undefined>
  ) {
    this._query = { ...this._query, ...property }
  }

  @Mutation
  requireReload() {
    this._isReloadRequired = true
  }

  @Mutation
  completeReload() {
    this._isReloadRequired = false
  }

  @Mutation
  setPage(value: number) {
    this._query.pageNumber = value
  }

  @Mutation
  increasePage() {
    if (this._query.pageNumber !== undefined) this._query.pageNumber++
  }

  @Mutation
  setSelectedRow(value: Entity | null) {
    this._selectedRow = value
  }

  @Mutation
  setSelectedRows(value: Entity[]) {
    this._selectedRows = value
  }

  @Mutation
  setFilters(value: FilterField[]) {
    this._filters = value
    this._query.filterFields = value
    this._query.withRemoved = !!value.find(
      (filter) => filter.fieldName === 'isRemoved' && filter.values[0] === true
    )
    this._isReloadRequired = true
  }

  @Mutation
  setSearch(value: string) {
    this._search = value
    this._query.filter = value
    this._isReloadRequired = true
  }

  @Mutation
  setOrder(value: SortType) {
    this._order = value
    if (this._query.sortFields) this._query.sortFields[0].sortType = value
    else this._query.sortFields = [{ fieldName: this._sort || '', sortType: value }]
    this._isReloadRequired = true
  }

  @Mutation
  setSort(value: string) {
    this._sort = value
    if (this._query.sortFields) this._query.sortFields[0].fieldName = value
    else
      this._query.sortFields = [{ fieldName: value, sortType: this._order || SortType.Descending }]
    this._isReloadRequired = true
  }
}

export default getModule(TableStore)
