<template lang="pug">
  div.project-overview
    el-row(:gutter="20")
      el-col(:span="10")
        el-card.card(shadow="never")
          div.card__title Описание
          div.card__body
            el-input(
              v-model="project.description"
              :autosize="{ minRows: 2 }"
              type="textarea"
              placeholder="Подробно опишите ваш проект...")
      el-col(:span="6")
        el-card.card(shadow="never")
          div.card__title Задач выполнено
          div.card__body
            div.tasks-stats {{ completedTasksCount }} из {{ totalTasksCount }}
            el-progress(:percentage="progressPercentage")
      el-col(:span="8")
        el-card.card(shadow="never")
          div.card__title Информация
          div.card__body
            div Руководитель проекта: {{ project.ownerFio }}
            //div Сроки проекта: 14.07.2020 – 03.08.2020

</template>

<script>
import { mapActions } from 'vuex';

export default {
  name: 'ProjectOverview',
  props: {
    data: Object
  },
  data() {
    return {
      project: {
        name: '',
        description: '',
        ownerId: null,
        ownerFio: null,
        teamId: null,
        teamName: null,
        groupId: null,
        groupName: null
      },
      totalTasksCount: 0,
      completedTasksCount: 0
    };
  },
  computed: {
    progressPercentage() {
      const total = parseInt(this.totalTasksCount);
      const completed = parseInt(this.completedTasksCount);
      return Math.round((completed * 100) / total);
    }
  },
  async mounted() {
    this.project = { ...this.data };
    this.totalTasksCount = await this.getTasksCount(this.project.id);
    this.completedTasksCount = await this.getTasksCountByStatus({
      projectId: this.project.id,
      status: 'Succeed'
    });
  },
  methods: {
    ...mapActions({
      getTasksCount: 'tasks/getTasksCount',
      getTasksCountByStatus: 'tasks/getTasksCountByStatus'
    })
  }
};
</script>

<style lang="scss" scoped>
.card {
  &__title {
    font-size: 15px;
    font-weight: 600;
    color: var(--text);
  }
  &__body {
    padding-top: 14px;
    font-size: 14px;
    line-height: 18px;
    color: var(--text);
  }
}
.tasks-stats {
  font-size: 22px;
  font-weight: 600;
  color: var(--color-primary);
  margin-bottom: 10px;
}
</style>

<style lang="scss">
.project-overview {
  .el-textarea__inner,
  .el-textarea__inner:hover,
  .el-textarea__inner:focus {
    background-color: transparent;
    padding: 0;
    border: none;
  }
}
</style>
