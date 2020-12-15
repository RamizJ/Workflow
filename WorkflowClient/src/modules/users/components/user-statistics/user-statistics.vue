<template>
  <div class="user-statistics">
    <el-row :gutter="20">
      <el-col :span="8">
        <project-select
          v-model="selectedProjectIds"
          :multiple="true"
          :collapse-tags="true"
          :full-width="true"
          @change="updateProject"
        />
      </el-col>
      <el-col :span="8">
        <el-date-picker
          v-model="selectedRange"
          type="daterange"
          format="dd.MM.yyyy"
          range-separator="-"
          start-placeholder="Начальная дата"
          end-placeholder="Конечная дата"
          @change="updateRange"
        />
      </el-col>
    </el-row>
    <el-row v-loading="loading" :gutter="20">
      <el-col :span="8">
        <overview />
      </el-col>
      <el-col :span="8">
        <goal-completion :data="goalCompletion" />
      </el-col>
      <el-col :span="8">
        <workload-by-projects :data="workloadByProjects" />
      </el-col>
    </el-row>
    <el-row v-loading="loading" :gutter="20">
      <el-col :span="24">
        <workload-by-days :data="workloadByDays" />
      </el-col>
    </el-row>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import moment from 'moment'
import GoalCompletion from '@/modules/users/components/user-statistics/goal-completion.vue'
import WorkloadByProjects from '@/modules/users/components/user-statistics/workload-by-projects.vue'
import WorkloadByDays from '@/modules/users/components/user-statistics/workload-by-days.vue'
import Overview from '@/modules/users/components/user-statistics/overview.vue'
import ProjectSelect from '@/modules/projects/components/project-select.vue'
import statisticsStore from '@/modules/users/store/statistics.store'
import { GoalsCompletion } from '@/modules/users/models/goal-completion-statistics.interface'
import { ProjectHours } from '@/modules/users/models/workload-by-projects-statistics.interface'
import { WorkloadByDaysStatistics } from '@/modules/users/models/workload-by-days-statistics.interface'
import Project from '@/modules/projects/models/project.type'

@Component({
  components: {
    ProjectSelect,
    Overview,
    GoalCompletion,
    WorkloadByProjects,
    WorkloadByDays,
  },
})
export default class UserStatistics extends Vue {
  private loading = false
  private selectedProjectIds: number | null = null
  private selectedProjects: Project[] = []
  private selectedRange: Date[] = [moment().subtract(1, 'month').toDate(), moment().toDate()]

  private get userId(): string {
    return this.$route.params.userId
  }

  private get goalCompletion(): GoalsCompletion | null {
    return (
      statisticsStore.total?.goalCompletion.userGoalsCompletions.find(
        (item) => item.userId === this.userId
      )?.goalsCompletion || null
    )
  }

  private get workloadByProjects(): ProjectHours | null {
    if (!statisticsStore.total) return null
    const totalHours = statisticsStore.total.workloadByProjects.totalHours
    const hoursForProject =
      statisticsStore.total.workloadByProjects.projectHoursForUsers.find(
        (item) => item.userId === this.userId
      )?.hoursForProject || []
    const hoursWithProjectNames = hoursForProject.map((item) => {
      return {
        projectName: this.selectedProjects.find((p) => p.id === item.projectId)?.name || '',
        hours: item.hours,
      }
    })
    return {
      totalHours,
      hoursForProject: hoursWithProjectNames,
    }
  }

  private get workloadByDays(): WorkloadByDaysStatistics | null {
    return statisticsStore.total?.workloadByDays || null
  }

  protected async mounted(): Promise<void> {
    await this.updateStatistics()
  }

  private async updateProject(projects: Project[]): Promise<void> {
    this.selectedProjects = projects
    await this.updateStatistics()
  }

  private async updateRange(range: Date[]): Promise<void> {
    this.selectedRange = range
    await this.updateStatistics()
  }

  private async updateStatistics(): Promise<void> {
    this.loading = true
    await statisticsStore.getTotal({
      dateBegin: moment.utc(this.selectedRange[0]).format(),
      dateEnd: moment.utc(this.selectedRange[1]).format(),
      projectIds: this.selectedProjects.map((project) => project.id || 0),
      userIds: [this.userId],
    })
    this.loading = false
  }
}
</script>

<style lang="scss">
.user-statistics {
  overflow-y: auto;
  overflow-x: hidden;
  &::-webkit-scrollbar {
    display: none;
  }
  .el-row {
    margin-bottom: 20px;
    &:last-child {
      margin-bottom: 0;
    }
  }
  .card {
    min-width: 350px;
    min-height: 350px;
    &__title {
      cursor: default;
      color: var(--text);
      font-size: 12px;
      font-weight: 600;
      letter-spacing: 0.3px;
      text-transform: uppercase;
      margin-bottom: 15px;
    }
    &__content {
      width: 100%;
      display: flex;
      position: relative;
    }
    &__aside {
      width: 100px;
      display: flex;
      align-items: center;
      justify-content: center;
    }
  }
}
</style>
