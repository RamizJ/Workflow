<template>
  <BasePage>
    <BasePageHeader>Настройки</BasePageHeader>
    <BaseTabs v-model="currentTab" :tabs="tabs" />
  </BasePage>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import BasePage from '@/core/components/base-page/base-page.vue'
import BasePageHeader from '@/core/components/base-page/base-page-header.vue'
import BaseTabs from '@/core/components/base-tabs/base-tabs.vue'
import SettingsGeneral from '@/modules/settings/components/settings-general.vue'
import SettingsAccount from '@/modules/settings/components/settings-account.vue'
import SettingsAbout from '@/modules/settings/components/settings-about.vue'

@Component({
  components: {
    BasePageHeader,
    BasePage,
    BaseTabs,
    SettingsGeneral,
    SettingsAccount,
    SettingsAbout,
  },
})
export default class SettingsWindow extends Vue {
  private currentTab = 'general'
  private tabs: Array<{ label: string; name: string; component: any }> = [
    { label: 'Основные', name: 'general', component: SettingsGeneral },
    { label: 'Аккаунт', name: 'account', component: SettingsAccount },
    { label: 'О приложении', name: 'about', component: SettingsAbout },
  ]
}
</script>

<style lang="scss"></style>
