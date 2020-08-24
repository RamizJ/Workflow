import { Component, Vue } from 'vue-property-decorator';
import { FilterField, SortType } from '@/types/query.type';
import { Priority, Status } from '@/types/task.type';
import { View } from '@/types/view.type';

import projectsModule from '@/store/modules/projects.module';
import usersModule from '@/store/modules/users.module';

@Component
export default class ToolbarMixin extends Vue {
  public search = this.$route.query.q || '';
  public order = this.$route.query.order || 'Descending';
  public sort = this.$route.query.sort || '';
  public view = this.$route.query.view || 'list';
  public filters = {
    statuses: [],
    priorities: [],
    projects: [],
    performers: [],
    owners: [],
    deadlineRange: '',
    showOnlyDeleted: false
  };
  public statuses = [
    { value: Status.New, label: 'Новое' },
    { value: Status.Perform, label: 'Выполняется' },
    { value: Status.Testing, label: 'Тестируется' },
    { value: Status.Delay, label: 'Отложено' },
    { value: Status.Succeed, label: 'Выполнено' },
    { value: Status.Rejected, label: 'Отклонено' }
  ];
  public priorities = [
    { value: Priority.Low, label: 'Низкий' },
    { value: Priority.Normal, label: 'Средний' },
    { value: Priority.High, label: 'Высокий' }
  ];

  public get filterFields() {
    const filterFields: FilterField[] = [];
    if (this.filters.statuses.length)
      filterFields.push({
        fieldName: 'state',
        values: this.filters.statuses
      });
    if (this.filters.priorities.length)
      filterFields.push({
        fieldName: 'priority',
        values: this.filters.priorities
      });
    if (this.filters.projects.length)
      filterFields.push({
        fieldName: 'projectId',
        values: this.filters.projects
      });
    if (this.filters.performers.length)
      filterFields.push({
        fieldName: 'performerId',
        values: this.filters.performers
      });
    if (this.filters.owners.length)
      filterFields.push({
        fieldName: 'ownerId',
        values: this.filters.owners
      });
    if (this.filters.showOnlyDeleted) {
      filterFields.push({
        fieldName: 'isRemoved',
        values: [true]
      });
    }
    return filterFields;
  }

  public get projects() {
    return projectsModule.projects.map(project => {
      return {
        value: project.name,
        id: project.id
      };
    });
  }

  public get users() {
    return usersModule.users.map(user => {
      return {
        value: `${user.lastName} ${user.firstName}`,
        id: user.id
      };
    });
  }

  private mounted() {
    this.fixSearchableDropdown();
  }

  public onSearch(value: string) {
    this.$emit('search', value);
  }

  public onFiltersChange() {
    this.$emit('filters', this.filterFields);
  }

  public onOrderChange(value: SortType) {
    this.order = value;
    this.$emit('order', value);
  }

  public onSortChange(value: string) {
    this.sort = value;
    this.$emit('sort', value);
  }

  public onViewChange(value: View) {
    this.view = value;
    this.$emit('view', value);
  }

  public async onProjectsFocus(): Promise<void> {
    await this.searchProjects();
  }

  public async searchProjects(query = ''): Promise<void> {
    await projectsModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    });
  }

  public async onUsersFocus(): Promise<void> {
    await this.searchUsers();
  }

  public async searchUsers(query = ''): Promise<void> {
    await usersModule.findAll({
      filter: query,
      pageNumber: 0,
      pageSize: 10
    });
  }

  private fixSearchableDropdown() {
    document.querySelectorAll('.el-select.remote').forEach(element => {
      const dropdownElement = element as HTMLElement;
      const arrow = element.children[1].children[1];
      arrow.addEventListener('click', function() {
        // this.click();
        dropdownElement.click();
      });
    });
  }
}
