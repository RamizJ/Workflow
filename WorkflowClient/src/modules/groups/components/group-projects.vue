<template>
  <div class="group-projects">
    <ProjectTable />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import ProjectTable from '@/modules/projects/components/project-table.vue'

@Component({ components: { ProjectTable, BasePageHeader } })
export default class GroupProjects extends Vue {}
</script>

<style scoped>
.group-projects {
  height: 100%;
}
</style>
