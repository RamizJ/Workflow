<template lang="pug">
  el-table(
    ref="table"
    height="100%"
    v-loading="loading"
    :data="data"
    highlight-current-row border)
    el-table-column(type="selection" width="38")
    el-table-column(prop="title" label="Задача")
    el-table-column(v-if="!$route.params.projectId" prop="projectName" label="Проект" width="150")
    el-table-column(prop="state" label="Статус" width="120" :formatter="formatStatus")
    el-table-column(prop="priority" label="Приоритет" width="120" :formatter="formatPriority")
    el-table-column(prop="creationDate" label="Дата создания" width="180" :formatter="formatDate")
    infinite-loading(
      slot="append"
      ref="loader"
      spinner="waveDots"
      :distance="300"
      @infinite="loadData"
      force-use-infinite-wrapper=".el-table__body-wrapper")
      div(slot="no-more")
      div(slot="no-results")
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator';
import { FilterField } from '@/types/query.type';

@Component
export default class Table extends Vue {
  @Prop()
  params: {} | undefined;

  private loading = false;
  private data = [];
}
</script>

<style scoped></style>
