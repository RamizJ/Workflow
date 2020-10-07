<template>
  <base-dialog class="settings" v-if="visible" @close="exit" ref="dialog">
    <h1 slot="title">Настройки</h1>
    <el-tabs slot="body" ref="tabs" v-model="activeTab">
      <el-tab-pane label="Основные" name="general">
        <settings-dialog-general />
      </el-tab-pane>
      <el-tab-pane label="Аккаунт" name="account">
        <settings-dialog-account />
      </el-tab-pane>
      <el-tab-pane label="О приложении" name="updates">
        <settings-dialog-about />
      </el-tab-pane>
    </el-tabs>
  </base-dialog>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import settingsModule from '@/store/modules/settings.module'
import BaseDialog from '@/components/BaseDialog.vue'
import SettingsDialogGeneral from '@/components/Settings/SettingsDialogGeneral.vue'
import SettingsDialogAccount from '@/components/Settings/SettingsDialogAccount.vue'
import SettingsDialogAbout from '@/components/Settings/SettingsDialogAbout.vue'

@Component({
  components: {
    BaseDialog,
    SettingsDialogAbout,
    SettingsDialogAccount,
    SettingsDialogGeneral,
  },
})
export default class SettingsDialog extends Vue {
  private visible = false
  private activeTab = 'general'

  protected mounted(): void {
    this.visible = true
  }

  private exit(): void {
    settingsModule.closeSettings()
  }
}
</script>

<style lang="scss">
.settings {
  .el-dialog {
    width: 60%;
    margin-top: 10vh !important;
  }
  .section {
    color: var(--text);
    margin-bottom: 30px;
    h2 {
      font-size: 14px;
      font-weight: 500;
      margin-top: 10px;
      margin-bottom: 15px;
    }
    h1 {
      font-size: 28px;
      font-weight: 600;
      margin-top: 15px;
    }
  }
}
</style>
