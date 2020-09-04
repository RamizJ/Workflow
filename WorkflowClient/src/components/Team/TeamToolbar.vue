<template>
  <toolbar>
    <toolbar-filters>
      <div class="filter">
        <div class="label">Поиск</div>
        <el-input v-model="search" size="medium" placeholder="Искать..." @change="onSearch">
          <el-button slot="prefix" type="text" size="mini" @click="onSearch(search || '')">
            <feather type="search" size="16"></feather>
          </el-button>
        </el-input>
      </div>
    </toolbar-filters>
    <toolbar-filters-extra v-if="!$route.params.projectId && !$route.params.teamId">
      <el-row :gutter="20">
        <el-col :span="24">
          <el-checkbox v-model="filters.showOnlyDeleted" @change="onFiltersChange"
            >Только удалённые</el-checkbox
          >
        </el-col>
      </el-row>
    </toolbar-filters-extra>
    <toolbar-view
      :sort-fields="sortFields"
      @order="onOrderChange"
      @sort="onSortChange"
    ></toolbar-view>
  </toolbar>
</template>

<script lang="ts">
import { Component } from 'vue-property-decorator'
import { mixins } from 'vue-class-component'

import ToolbarMixin from '@/mixins/toolbar.mixin'
import Toolbar from '@/components/Toolbar/Toolbar.vue'
import ToolbarFilters from '@/components/Toolbar/ToolbarFilters.vue'
import ToolbarFiltersExtra from '@/components/Toolbar/ToolbarFiltersExtra.vue'
import ToolbarView from '@/components/Toolbar/ToolbarView.vue'

@Component({ components: { Toolbar, ToolbarFilters, ToolbarFiltersExtra, ToolbarView } })
export default class TeamToolbar extends mixins(ToolbarMixin) {
  private sortFields = [{ value: 'name', label: 'По названию' }]
}
</script>
