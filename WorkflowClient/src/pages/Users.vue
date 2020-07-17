<template lang="pug">
  page
    base-header
      template(slot="title") Пользователи
      template(slot="action")
        el-button(type="text" size="mini" @click="onCreate") Создать

    el-tabs(ref="tabs" v-model="activeTab" @tab-click="onTabClick")
      el-tab-pane(v-for="(tab, index) in tabs" :key="index" :label="tab.label" :name="tab.value")
        base-toolbar(
          v-if="activeTab === tab.value"
          :sort-fields="sortFields"
          @search="onSearch"
          @order="onOrderChange"
          @sort="onSortChange"
          @view="onViewChange")
        user-list(
          v-if="activeTab === tab.value && view === 'list'"
          ref="items"
          :search="search"
          :order="order"
          :sort="sort")

</template>

<script>
import Page from '~/components/Page';
import BaseHeader from '~/components/BaseHeader';
import BaseToolbar from '~/components/BaseToolbar';
import UserList from '~/components/UserList';
import pageMixin from '~/mixins/page.mixin';

export default {
  components: {
    Page,
    BaseHeader,
    BaseToolbar,
    UserList
  },
  mixins: [pageMixin],
  data() {
    return {
      sortFields: [{ value: 'firstName', label: 'По имени' }]
    };
  }
};
</script>
