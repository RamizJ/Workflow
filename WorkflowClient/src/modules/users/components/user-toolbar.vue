<template>
  <toolbar
    :sort-fields="sortFields"
    @order="onOrderChange"
    @sort="onSortChange"
    @filters-collapse="onFiltersCollapse"
  >
    <el-collapse slot="filters" v-model="activeCollapseItems">
      <el-collapse-item name="filters">
        <div class="toolbar__filters">
          <div class="toolbar__filters-wrapper">
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
          </div>
        </div>
      </el-collapse-item>
    </el-collapse>
  </toolbar>
</template>

<script lang="ts">
import { Component, Mixins } from 'vue-property-decorator'
import ToolbarMixin from '@/core/mixins/toolbar.mixin'
import Toolbar from '@/core/components/base-toolbar.vue'

@Component({ components: { Toolbar } })
export default class UserToolbar extends Mixins(ToolbarMixin) {
  private sortFields = [
    { value: 'lastName', label: 'По фамилии' },
    { value: 'firstName', label: 'По имени' },
    { value: 'middleName', label: 'По отчеству' },
    { value: 'userName', label: 'По логину' },
    { value: 'email', label: 'По почте' },
    { value: 'position', label: 'По должности' },
  ]
}
</script>
