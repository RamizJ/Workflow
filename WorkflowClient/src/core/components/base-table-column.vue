<template>
  <ElTableColumn v-bind="$attrs" v-on="$listeners">
    <template slot-scope="scope">
      <slot :row="scope.row"></slot>
    </template>
  </ElTableColumn>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class BaseTableColumn extends Vue {}
</script>

<style lang="scss"></style>
