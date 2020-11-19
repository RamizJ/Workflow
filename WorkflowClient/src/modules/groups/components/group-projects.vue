<template>
  <div class="group-projects">
    <!--    <BasePageHeader size="small" height="45" :no-border="true"></BasePageHeader>-->
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

<style scoped></style>
