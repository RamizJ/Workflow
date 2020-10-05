<template>
  <div class="settings-general">
    <div class="section">
      <h2>Оформление</h2>
      <el-tooltip content="Светлое" placement="top">
        <el-button class="theme-preview light" type="text" @click="switchTheme('light')"
          >А</el-button
        >
      </el-tooltip>
      <el-tooltip content="Тёмное" placement="top">
        <el-button class="theme-preview dark" type="text" @click="switchTheme('dark')">А</el-button>
      </el-tooltip>
      <el-tooltip content="Системное" placement="top">
        <el-button class="theme-preview system" type="text" @click="switchTheme('system')">
          <span class="text">A</span>
        </el-button>
      </el-tooltip>
    </div>
    <div class="section">
      <h2>Диалоговые окна</h2>
      <el-checkbox
        v-model="confirmDialogClose"
        label="Подтверждать закрытие диалоговых окон"
        @change="switchConfirmDialogClose"
      ></el-checkbox>
    </div>
    <div class="section">
      <h2>Отладка</h2>
      <el-checkbox
        v-model="debugMode"
        label="Отображать детали ошибок"
        @change="switchDebugMode"
      ></el-checkbox>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import settingsModule from '@/store/modules/settings.module'
import { Theme } from '@/types/theme.type'

@Component
export default class SettingsDialogGeneral extends Vue {
  private confirmDialogClose = settingsModule.confirmDialogClose
  private debugMode = settingsModule.debugMode

  private switchTheme(theme: string): void {
    settingsModule.setTheme(theme as Theme)
  }

  private switchConfirmDialogClose(): void {
    settingsModule.setConfirmDialogClose(this.confirmDialogClose)
  }

  private switchDebugMode(): void {
    settingsModule.setDebugMode(this.debugMode)
  }
}
</script>

<style lang="scss" scoped>
.section {
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
.theme-preview {
  border-radius: 3px;
  border: none;
  box-shadow: inset 0 0 0 3px var(--color-primary);
  cursor: pointer;
  display: inline-block;
  font-weight: 600;
  height: 40px;
  margin-right: 10px;
  padding-left: 9px;
  padding-top: 15px;
  width: 40px;
  font-size: 18px;
  text-align: left;
  &.dark {
    background: #1b1b1b;
    color: #eeeeee;
  }
  &.light {
    background: #f6f6f6;
    color: #303030;
  }
  &.system {
    background: linear-gradient(
      135deg,
      rgba(27, 27, 27, 1) 0%,
      rgba(27, 27, 27, 1) 50%,
      rgba(246, 246, 246, 1) 50%,
      rgba(246, 246, 246, 1) 100%
    );
    .text {
      color: #f6f6f6;
      text-shadow: 1.5px 1px 0 #1b1b1b;
    }
  }
}
</style>
