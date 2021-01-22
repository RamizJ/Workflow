<template>
  <div class="notification" @mouseenter="hover">
    <div class="notification__title">
      {{ title || 'Новое сообщение' }}
      <span class="notification__date">{{ date }}</span>
    </div>
    <div class="notification__subtitle">
      {{ subtitle }}
    </div>
    <div class="notification__body">{{ content }}</div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'

@Component
export default class Notification extends Vue {
  @Prop() readonly title!: string
  @Prop() readonly content!: string
  @Prop() readonly subtitle?: string
  @Prop() readonly date?: string

  private hover(): void {
    this.$emit('hover')
  }
}
</script>

<style lang="scss" scoped>
.notification {
  padding: 8px 0;
  font-size: 13px;
  &__title {
    font-size: 14px;
    font-weight: 600;
    color: var(--text);
    margin-bottom: 3px;
    display: flex;
    align-items: center;
    justify-content: space-between;
  }
  &__subtitle {
    font-weight: 600;
    color: var(--color-primary);
    margin-bottom: 4px;
  }
  &__date {
    font-size: 12px;
    font-weight: 400;
    color: var(--text-muted);
  }
  &__body {
    color: var(--text);
  }
}
</style>
