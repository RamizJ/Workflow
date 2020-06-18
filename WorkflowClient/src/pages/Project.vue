<template lang="pug">
  div.container
    base-header
      template(slot="title") {{ projectItem.name }}

</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseHeader from '~/components/BaseHeader';

export default {
  name: 'Project',
  components: { BaseHeader },
  data() {
    return {
      projectItem: {
        name: '',
        description: '',
        tags: [],
        ownerId: null,
        ownerFio: null,
        teamId: null,
        teamName: null,
        groupId: null,
        groupName: null,
        creationDate: new Date()
      }
    };
  },
  computed: {
    ...mapGetters({ project: 'projects/getProject' })
  },
  async mounted() {
    await this.fetchProject(this.$route.params.id);
    this.projectItem = { ...this.project };
  },
  methods: {
    ...mapActions({ fetchProject: 'projects/fetchProject' })
  }
};
</script>

<style scoped></style>
