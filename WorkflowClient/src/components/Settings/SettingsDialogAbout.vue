<template>
  <div class="settings-about">
    <div class="section">
      <h2>Текущая версия</h2>
      <h1>{{ currentVersion }}</h1>
    </div>
    <div class="section changelog">
      <h2>История версий</h2>
      <el-timeline>
        <el-timeline-item
          v-for="(release, index) in changelog"
          :key="index"
          :timestamp="release.date"
        >
          <div class="release">{{ release.version }}</div>
          <ul>
            <li v-for="(change, index) in release.changes" :key="index">– {{ change }}</li>
          </ul>
        </el-timeline-item>
      </el-timeline>
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class SettingsDialogAbout extends Vue {
  private changelog = require('./changelog.json')
  private currentVersion = require('../../../package.json').version
}
</script>

<style lang="scss" scoped>
.changelog {
  height: 30vh;
}
.el-timeline {
  padding: 10px 2px;
}
.release {
  color: var(--text) !important;
  font-size: 14px;
  font-weight: 600;
  margin-bottom: 8px;
  line-height: normal;
}
li {
  color: var(--text) !important;
  font-size: 13px;
  line-height: 1.8;
}
</style>

<style>
.el-timeline-item__timestamp {
  font-size: 12px;
}
.el-timeline-item__timestamp.is-bottom {
  margin-top: 10px;
  margin-bottom: 5px;
}
</style>
