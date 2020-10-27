<template>
  <div class="sidebar-header">
    <AppSidebarAdd />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import AppSidebarAdd from '@/core/components/app-sidebar/app-sidebar-add.vue'

@Component({
  components: { AppSidebarAdd },
})
export default class AppSidebarHeader extends Vue {}
</script>

<style lang="scss" scoped>
.sidebar-header {
  padding-bottom: 27px;
}
</style>
