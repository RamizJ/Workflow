<template>
  <el-dropdown placement="bottom" :show-timeout="0">
    <el-button class="actions-toggle" type="text" size="mini">
      <unicon name="ellipsis-h" />
    </el-button>
    <el-dropdown-menu slot="dropdown">
      <el-dropdown-item>
        <el-button type="text" size="mini" @click="$emit('add-goal')">Добавить задачу</el-button>
      </el-dropdown-item>
      <el-dropdown-item>
        <el-button type="text" size="mini" @click="$emit('add-team')">Добавить команду</el-button>
      </el-dropdown-item>
      <el-divider></el-divider>
      <el-dropdown-item>
        <el-button type="text" size="mini" @click="$emit('delete-project')"
          >Удалить проект</el-button
        >
      </el-dropdown-item>
    </el-dropdown-menu>
  </el-dropdown>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'

@Component
export default class ProjectActions extends Vue {}
</script>

<style lang="scss" scoped>
.el-dropdown {
  height: 24px;
}
.actions-toggle {
  padding: 0;
  height: 24px;
  width: 24px;
  svg {
    fill: var(--text-muted);
    padding: 0;
    vertical-align: text-bottom;
  }
}
</style>
