<template lang="pug">
  page
    base-header
      template(slot="title") Команды
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        team-toolbar(
          v-if="activeTab === tab.value"
          @search="onSearch"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange")
        team-table(
          v-if="activeTab === tab.value && view === 'list'"
          ref="items"
          :search="search"
          :order="order"
          :sort="sort")

</template>

<script>
import Page from '@/components/Page';
import BaseHeader from '@/components/BaseHeader';
import TeamToolbar from '@/components/Teams/TeamToolbar';
import TeamTable from '@/components/Teams/TeamTable';
import pageMixin from '@/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    TeamToolbar,
    TeamTable
  },
  mixins: [pageMixin]
};
</script>
