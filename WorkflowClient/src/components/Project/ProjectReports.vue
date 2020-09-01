<template>
  <div class="project-reports">
    <el-row :gutter="20">
      <el-col :span="8"><report-tasks-overview /></el-col>
      <el-col :span="16"><report-daily-statistics /></el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import ReportTasksOverview from '@/components/Report/ReportTasksOverview.vue'
import ReportDailyStatistics from '@/components/Report/ReportDailyStatistics.vue'

@Component({ components: { ReportDailyStatistics, ReportTasksOverview } })
export default class ProjectReports extends Vue {}
</script>

<style lang="scss" scoped>
.el-row {
  margin-right: 0 !important;
}
</style>
