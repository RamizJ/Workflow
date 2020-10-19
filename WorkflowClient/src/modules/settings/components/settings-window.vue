<template>
  <Window width="70%" top="15vh" @closed="onClosed">
    <h1 slot="title">Настройки</h1>
    <el-tabs slot="body" ref="tabs" v-model="activeTab">
      <el-tab-pane label="Основные" name="general">
        <SettingsGeneral v-if="activeTab === 'general'" />
      </el-tab-pane>
      <el-tab-pane label="Аккаунт" name="account">
        <SettingsAccount v-if="activeTab === 'account'" />
      </el-tab-pane>
      <el-tab-pane label="О приложении" name="about">
        <SettingsAbout v-if="activeTab === 'about'" />
      </el-tab-pane>
    </el-tabs>
  </Window>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Window from '@/core/components/base-window.vue'
import SettingsGeneral from '@/modules/settings/components/settings-general.vue'
import SettingsAccount from '@/modules/settings/components/settings-account.vue'
import SettingsAbout from '@/modules/settings/components/settings-about.vue'

@Component({
  components: {
    Window,
    SettingsGeneral,
    SettingsAccount,
    SettingsAbout,
  },
})
export default class SettingsWindow extends Vue {
  private activeTab = 'general'

  private onClosed(): void {
    this.$emit('closed')
  }
}
</script>

<style scoped></style>
