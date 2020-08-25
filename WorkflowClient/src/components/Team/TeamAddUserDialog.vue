<template lang="pug">
  base-dialog(v-if="visible" @close="exit" ref="dialog")
    h1(slot="title") Добавить участника
    el-form(slot="body" :model="form" ref="form" v-loading="loading" @submit.native.prevent="submit")
      el-row(:gutter="20")
        el-col(:span="24")
          el-form-item
            el-select(
              ref="input"
              v-model="form.userId"
              placeholder="Найти участника..."
              :remote-method="searchUsers"
              filterable remote clearable default-first-option)
              el-option(v-for="item in usersToAdd" :key="item.id" :label="item.value" :value="item.id")
    template(slot="footer")
      div.extra
      div.send
        el-tooltip(content="Сохранить" effect="dark" placement="top" transition="fade" :visible-arrow="false" :open-delay="500")
          el-button(type="text" @click="submit" circle)
            feather(type="arrow-right")
</template>

<script>
import { mapActions, mapGetters } from 'vuex';
import BaseDialog from '@/components/BaseDialog';
import dialogMixin from '@/mixins/dialog.mixin';

export default {
  components: { BaseDialog },
  mixins: [dialogMixin],
  data() {
    return {
      form: {
        userId: ''
      }
    };
  },
  computed: {
    ...mapGetters({
      teamUsers: 'teams/getTeamUsers',
      teamProjects: 'teams/getTeamProjects'
    }),
    usersToAdd() {
      const allUsers = this.userList;
      const existingUsers = this.existingUsers;
      return allUsers.filter(user => {
        return !existingUsers.find(existingUser => existingUser.id === user.id);
      });
    },
    existingUsers() {
      return this.teamUsers.map(user => {
        return {
          value: `${user.lastName} ${user.firstName}`,
          id: user.id
        };
      });
    }
  },
  async mounted() {
    await this.searchUsers();
    await this.fetchTeamUsers({
      teamId: this.$route.params.teamId,
      pageNumber: 0,
      pageSize: 100
    });
    this.$refs.input.focus();
  },
  methods: {
    ...mapActions({
      addUser: 'teams/addUser',
      fetchItem: 'teams/findOneById',
      createItem: 'teams/createOne',
      updateItem: 'teams/updateOne',
      fetchTeamUsers: 'teams/findUsers',
      fetchTeamProjects: 'teams/findProjects'
    }),
    async submit() {
      const teamId = this.$route.params.teamId;
      const userId = this.form.userId.trim();
      if (userId) {
        this.loading = true;
        await this.addUser({ teamId, userId });
        this.$emit('submit');
        this.loading = false;
      }
      this.exit();
    }
  }
};
</script>
