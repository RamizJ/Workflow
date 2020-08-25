import { Component, Prop, Vue, Watch } from 'vue-property-decorator'
import InfiniteLoading from 'vue-infinite-loading'
import moment from 'moment'

import { Priority, Status } from '@/types/task.type'
import Query, { FilterField, SortType } from '@/types/query.type'

@Component
export default class TableMixin extends Vue {
  @Prop()
  filters: FilterField[] | undefined
  @Prop()
  search: string | undefined
  @Prop()
  order: string | undefined
  @Prop()
  sort: string | undefined

  @Watch('filters', { deep: true })
  onFiltersChange(value: FilterField[]) {
    this.query.filterFields = value
    this.query.withRemoved = !!value.find(
      filter => filter.fieldName === 'isRemoved' && filter.values[0] === true
    )
    this.reloadData()
  }

  @Watch('search')
  onSearchChange(value: string) {
    this.query.filter = value
    this.reloadData()
  }

  @Watch('order')
  onOrderChange(value: SortType) {
    if (this.query.sortFields) {
      this.query.sortFields[0].sortType = value
      this.reloadData()
    }
  }

  @Watch('sort')
  onSortChange(value: string) {
    if (this.query.sortFields) {
      this.query.sortFields[0].fieldName = value
      this.reloadData()
    }
  }

  public query: Query = {
    projectId: parseInt(this.$route.params.projectId) || undefined,
    teamId: parseInt(this.$route.params.teamId) || undefined,
    filter: '',
    pageNumber: 0,
    pageSize: 20,
    filterFields: [],
    sortFields: [
      {
        fieldName: (this.$route.query.sort as string) || 'creationDate',
        sortType: (this.$route.query.order as SortType) || SortType.Descending
      }
    ]
  }
  private statuses = [
    { value: Status.New, label: 'Новое' },
    { value: Status.Perform, label: 'Выполняется' },
    { value: Status.Testing, label: 'Тестируется' },
    { value: Status.Delay, label: 'Отложено' },
    { value: Status.Succeed, label: 'Выполнено' },
    { value: Status.Rejected, label: 'Отклонено' }
  ]
  private priorities = [
    { value: Priority.Low, label: 'Низкий' },
    { value: Priority.Normal, label: 'Средний' },
    { value: Priority.High, label: 'Высокий' }
  ]

  public data: any[] = []
  public selectedRow: any | undefined
  public modalData: any | undefined
  public modalVisible = false
  public isShiftPressed = false

  public get isRowEditable() {
    return !this.selectedRow?.isRemoved
  }

  public get isMultipleSelected() {
    return this.table?.selection?.length > 1
  }

  public get table(): any {
    if (Array.isArray(this.$refs.table)) return this.$refs.table[0] as any
    else return this.$refs.table as any
  }

  public get infiniteLoader(): InfiniteLoading {
    if (Array.isArray(this.$refs.loader)) return this.$refs.loader[0] as InfiniteLoading
    else return this.$refs.loader as InfiniteLoading
  }

  public get contextMenu(): any {
    if (Array.isArray(this.$refs.contextMenu)) return this.$refs.contextMenu[0] as any
    else return this.$refs.contextMenu as any
  }

  private created() {
    document.onkeydown = this.onKeyDown
    document.onkeyup = this.onKeyUp
  }

  public reloadData() {
    this.data = []
    this.query.pageNumber = 0
    this.infiniteLoader.stateChanger.reset()
    this.modalVisible = false
  }

  public setIndex({ row, rowIndex }: { row: any; rowIndex: number }) {
    row.index = rowIndex
  }

  public onRowSelect(selection: any[], entity: any) {
    this.selectedRow = entity
  }

  public onRowSingleClick(row: any) {
    this.table.clearSelection()
    if (this.isShiftPressed) {
      const previousIndex = row.index
      const currentIndex = this.selectedRow?.index
      if (!previousIndex || !currentIndex) return
      let from: number
      let to: number
      if (currentIndex < previousIndex) {
        from = currentIndex
        to = previousIndex
      } else {
        from = previousIndex
        to = currentIndex
      }
      this.data.some((entity: any) => {
        if (!entity.index) return
        if (entity.index >= from && entity.index <= to) this.table.toggleRowSelection(entity)
      })
    } else {
      this.table.toggleRowSelection(row)
    }
    this.selectedRow = row
  }

  public onRowDoubleClick(row: any) {
    if (!row.isRemoved) {
      this.modalData = row.id
      this.modalVisible = true
    }
  }

  public onRowRightClick(row: any, column: any, event: Event) {
    if (!this.isMultipleSelected) {
      this.table.clearSelection()
      this.table.toggleRowSelection(row)
    }
    this.table.setCurrentRow(row)
    this.selectedRow = row
    this.contextMenu.open(event, { row, column })
    event.preventDefault()
  }

  public formatPriority(row: any, column: any, value: string): string {
    return this.priorities.find(priority => priority.value == (value as Priority))?.label || ''
  }

  public formatStatus(row: any, column: any, value: string): string {
    return this.statuses.find(status => status.value === (value as Status))?.label || ''
  }

  public formatDate(row: any, column: any, value: string): string {
    const dateUtc = moment.utc(value)
    return dateUtc.format('DD.MM.YYYY HH:mm')
  }

  public formatFio(value: string): string {
    if (!value) return value
    const fioArray = value.split(' ')
    const lastName = fioArray[0]
    const firstNameInitial = fioArray[1][0] ? `${fioArray[1][0]}.` : ''
    const middleNameInitial = fioArray[2][0] ? `${fioArray[2][0]}.` : ''
    return `${lastName} ${firstNameInitial} ${middleNameInitial}`
  }

  private onKeyDown(): void {
    const key = (window.event as KeyboardEvent).keyCode
    if (key === 16) this.isShiftPressed = true
  }

  private onKeyUp(): void {
    this.isShiftPressed = false
  }
}
