import { Component, Vue } from 'vue-property-decorator';
import { FilterField, SortType } from '@/types/query.type';
import { View } from '@/types/view.type';

@Component
export default class PageMixin extends Vue {
  public filters: FilterField[] = [];
  public search = this.$route.query.q || '';
  public order: SortType = (this.$route.query.order as SortType) || SortType.Descending;
  public sort: string = (this.$route.query.sort as string) || '';
  public view: View = (this.$route.query.view as View) || View.List;

  onCreate() {
    // this.$refs.items.onItemCreate();
  }

  onFiltersChange(value: FilterField[]) {
    this.filters = value;
  }

  async onSearch(value: string): Promise<void> {
    this.search = value;
    await this.updateUrl('q', value);
  }

  async onOrderChange(value: SortType): Promise<void> {
    this.order = value;
    await this.updateUrl('order', value);
  }

  async onSortChange(value: string): Promise<void> {
    this.sort = value;
    await this.updateUrl('sort', value);
  }

  async onViewChange(value: View): Promise<void> {
    this.view = value;
    await this.updateUrl('view', value);
  }

  async updateUrl(queryLabel: string, queryValue: string): Promise<void> {
    const query: any = { ...this.$route.query };
    if (query[queryLabel] !== queryValue) {
      query[queryLabel] = queryValue || undefined;
      await this.$router.replace({ query });
    }
  }
}
