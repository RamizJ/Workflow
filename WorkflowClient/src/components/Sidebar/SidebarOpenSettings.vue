<template>
  <div class="sidebar-open-settings">
    <el-button size="small" class="sidebar-button" plain @click="openSettings">
      <unicon name="sliders-v-alt" />
    </el-button>
    <settings-dialog v-if="isSettingsOpened" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import SettingsDialog from '@/components/Settings/SettingsDialog.vue'
import settingsModule from '@/store/modules/settings.module'

@Component({
  components: { SettingsDialog },
})
export default class SidebarButtonSettings extends Vue {
  private get isSettingsOpened(): boolean {
    return settingsModule.isSettingsOpened
  }

  private openSettings(): void {
    settingsModule.openSettings()
  }
}
</script>

<style lang="scss">
//.sidebar-button.el-button.is-plain {
//  padding: 7px 6px 6px;
//  &:hover {
//    border-color: var(--text-muted);
//    background-color: transparent;
//  }
//  &:focus {
//    border-color: var(--text-muted);
//    background-color: transparent;
//  }
//  svg {
//    height: 20px;
//  }
//}
</style>
