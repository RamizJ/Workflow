<template>
  <BasePage>
    <BasePageHeader>
      Области
      <GroupToolbar slot="toolbar" />
    </BasePageHeader>
    <GroupTable />
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import GroupToolbar from '@/modules/groups/components/group-toolbar.vue'
import GroupTable from '@/modules/groups/components/group-table.vue'

@Component({
  components: {
    BasePage,
    BasePageHeader,
    GroupToolbar,
    GroupTable,
  },
})
export default class GroupsPage extends Vue {}
</script>
