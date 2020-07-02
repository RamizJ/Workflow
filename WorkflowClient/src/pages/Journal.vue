<template lang="pug">
  page
    page-content
      page-header
        template(slot="title") Журнал
</template>

<script>
import Page from '~/components/Page';
import PageHeader from '~/components/PageHeader';
import PageContent from '~/components/PageContent';

export default {
  name: 'Journal',
  components: { PageContent, PageHeader, Page }
};
</script>

<style lang="scss" scoped></style>
