<template>
  <BasePage>
    <BasePageHeader>Настройки</BasePageHeader>
    <el-tabs ref="tabs" v-model="activeTab">
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
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import SettingsGeneral from '@/modules/settings/components/settings-general.vue'
import SettingsAccount from '@/modules/settings/components/settings-account.vue'
import SettingsAbout from '@/modules/settings/components/settings-about.vue'
import BasePage from '@/core/components/base-page.vue'
import BasePageHeader from '@/core/components/base-page-header.vue'

@Component({
  components: {
    BasePageHeader,
    BasePage,
    SettingsGeneral,
    SettingsAccount,
    SettingsAbout,
  },
})
export default class SettingsWindow extends Vue {
  private activeTab = 'general'
}
</script>

<style scoped></style>
