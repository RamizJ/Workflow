<template>
  <ElTableColumn v-if="custom" v-bind="$attrs" v-on="$listeners">
    <template slot-scope="scope">
      <slot :row="scope.row"></slot>
    </template>
  </ElTableColumn>
  <ElTableColumn v-else v-bind="$attrs" v-on="$listeners"></ElTableColumn>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class BaseTableColumn extends Vue {
  @Prop() readonly custom?: boolean
}
</script>

<style lang="scss"></style>
