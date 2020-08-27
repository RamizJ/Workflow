<template>
  <page>
    <base-header>
      <template slot="title">Отчёты</template>
      <template slot="action">
        <el-button type="text" size="mini">Создать</el-button>
      </template>
    </base-header>
  </page>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

import Page from '@/components/Page.vue'
import BaseHeader from '@/components/BaseHeader.vue'

@Component({ components: { Page, BaseHeader } })
export default class ReportsPage extends Vue {}
</script>

<style scoped></style>
