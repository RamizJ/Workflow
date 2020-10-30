<template>
  <toolbar
    :sort-fields="sortFields"
    @order="onOrderChange"
    @sort="onSortChange"
    @filters-collapse="onFiltersCollapse"
  >
    <template slot="filters">
      <div class="filter">
        <div class="label">Поиск</div>
        <el-input v-model="search" size="medium" placeholder="Искать..." @change="onSearch">
          <el-button slot="prefix" type="text" size="mini" @click="onSearch(search || '')">
            <feather type="search" size="16"></feather>
          </el-button>
        </el-input>
      </div>
      <div class="filter">
        <div class="label">Архив</div>
        <el-checkbox
          v-model="filters.showOnlyDeleted"
          size="medium"
          @change="onFiltersChange"
          border
          >Только удалённые</el-checkbox
        >
      </div>
    </template>
  </toolbar>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import ToolbarMixin from '@/core/mixins/toolbar.mixin'
import Toolbar from '@/core/components/base-toolbar.vue'

@Component({ components: { Toolbar } })
export default class TeamToolbar extends Mixins(ToolbarMixin) {
  private sortFields = [{ value: 'name', label: 'По названию' }]
}
</script>
