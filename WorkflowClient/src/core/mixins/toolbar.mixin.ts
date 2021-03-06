import { Component, Vue } from 'vue-property-decorator'
import { FilterField, SortType } from '@/core/types/query.type'
import { Priority, Status } from '@/modules/goals/models/goal.type'
import { View } from '@/core/types/view.type'

import projectsModule from '@/modules/projects/store/projects.store'
import usersModule from '@/modules/users/store/users.store'
import tableStore from '@/core/store/table.store'

@Component
export default class ToolbarMixin extends Vue {
  public search = this.$route.query.q || ''
  public order = this.$route.query.order || 'Descending'
  public sort = this.$route.query.sort || ''
  public view = this.$route.query.view || 'list'
  public activeCollapseItems: string[] = []
  public filters = {
    statuses: [],
    priorities: [],
    projects: [],
    performers: [],
    owners: [],
    deadlineRange: '',
    showOnlyDeleted: false,
  }
  public statuses = [
    { value: Status.New, label: 'Новое' },
    { value: Status.Perform, label: 'Выполняется' },
    { value: Status.Testing, label: 'Проверяется' },
    { value: Status.Delay, label: 'Отложено' },
    { value: Status.Succeed, label: 'Выполнено' },
    { value: Status.Rejected, label: 'Отклонено' },
  ]
  public priorities = [
    { value: Priority.Low, label: 'Низкий' },
    { value: Priority.Normal, label: 'Средний' },
    { value: Priority.High, label: 'Высокий' },
  ]

  public get filterFields(): FilterField[] {
    const filterFields: FilterField[] = []
    if (this.filters.statuses.length)
      filterFields.push({
        fieldName: 'state',
        values: this.filters.statuses,
      })
    if (this.filters.priorities.length)
      filterFields.push({
        fieldName: 'priority',
        values: this.filters.priorities,
      })
    if (this.filters.projects.length)
      filterFields.push({
        fieldName: 'projectId',
        values: this.filters.projects,
      })
    if (this.filters.performers.length)
      filterFields.push({
        fieldName: 'performerId',
        values: this.filters.performers,
      })
    if (this.filters.owners.length)
      filterFields.push({
        fieldName: 'ownerId',
        values: this.filters.owners,
      })
    if (this.filters.showOnlyDeleted) {
      filterFields.push({
        fieldName: 'isRemoved',
        values: [true],
      })
    }
    return filterFields
  }

  public get projects(): { value: string | undefined; id: number | undefined }[] {
    return projectsModule.projects.map((project) => {
      return {
        value: project.name,
        id: project.id,
      }
    })
  }

  public get users(): { value: string; id: string | undefined }[] {
    return usersModule.users.map((user) => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id,
      }
    })
  }

  protected mounted(): void {
    this.fixSearchableDropdown()
  }

  public onSearch(value: string): void {
    tableStore.setSearch(value)
    this.$emit('search', value || this.search)
  }

  public onFiltersChange(): void {
    tableStore.setFilters(this.filterFields)
    this.$emit('filters', this.filterFields)
  }

  public onOrderChange(value: SortType): void {
    this.order = value
    tableStore.setOrder(value)
    this.$emit('order', value)
  }

  public onSortChange(value: string): void {
    this.sort = value
    tableStore.setSort(value)
    this.$emit('sort', value)
  }

  public onViewChange(value: View): void {
    this.view = value
    this.$emit('view', value)
  }

  public onFiltersCollapse(value: boolean): void {
    this.activeCollapseItems = value ? [] : ['filters']
  }

  public async onProjectsFocus(): Promise<void> {
    await this.searchProjects()
  }

  public async searchProjects(query = ''): Promise<void> {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  public async onUsersFocus(): Promise<void> {
    await this.searchUsers()
  }

  public async searchUsers(query = ''): Promise<void> {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10,
    })
  }

  private fixSearchableDropdown(): void {
    document.querySelectorAll('.el-select.remote').forEach((element) => {
      const dropdownElement = element as HTMLElement
      const arrow = element.children[1].children[1] as HTMLSpanElement
      arrow.addEventListener('click', function () {
        arrow.click()
        dropdownElement.click()
      })
    })
  }
}
