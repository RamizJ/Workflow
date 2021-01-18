<template>
  <div class="sidebar-footer">
    <Popover>
      <div class="notifications">
        <div class="notifications__title">
          Уведомления
          <span v-if="this.notifications.length" @click="readAll">Отметить все прочитанными</span>
        </div>
        <div class="notifications__list">
          <div v-if="isEmpty" class="empty-text">Нет уведомлений</div>
          <transition-group name="message" tag="p">
            <notification
              v-for="item in notifications"
              :key="item.id"
              :title="item.goalTitle"
              :subtitle="item.ownerFullName"
              :content="item.text"
              :date="item.fullDate"
            />
          </transition-group>
        </div>
      </div>
      <IconButton
        slot="reference"
        icon="bell"
        :badge="notificationsCount"
        @click="updateNotifications"
      />
    </Popover>
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Popover from '@/core/components/base-popover/base-popover.vue'
import IconButton from '@/core/components/base-icon-button.vue'
import goalMessagesStore from '@/modules/goals/store/goal-messages.store'
import GoalMessage from '@/modules/goals/models/goal-message.model'
import Notification from '@/core/components/notification/notification.vue'

@Component({
  components: {
    Notification,
    Popover,
    IconButton,
  },
})
export default class SidebarFooter extends Vue {
  private get notifications(): GoalMessage[] {
    return goalMessagesStore.unreadMessages
  }

  private get notificationsCount(): number {
    return goalMessagesStore.unreadMessagesCount
  }

  private get isEmpty(): boolean {
    return this.notifications.length === 0
  }

  protected async mounted(): Promise<void> {
    await this.updateNotifications()
  }

  private async updateNotifications(): Promise<void> {
    await goalMessagesStore.getUnreadMessages()
    await goalMessagesStore.getUnreadMessagesCount()
  }

  private async readAll(): Promise<void> {
    const ids = this.notifications.map((item) => item.id)
    await goalMessagesStore.markAsRead(ids)
    await this.updateNotifications()
  }
}
</script>

<style lang="scss" scoped>
.sidebar-footer {
  display: flex;
  justify-content: space-between;
}
.notifications {
  height: 350px;
  width: 350px;
  &__title {
    font-size: 18px;
    font-weight: 600;
    color: var(--text);
    margin-bottom: 6px;
    display: flex;
    justify-content: space-between;
    align-items: baseline;
    span {
      color: var(--color-primary);
      font-weight: 400;
      font-size: 12px;
      cursor: pointer;
      transition: 0.3s;
      &:hover {
        filter: brightness(80%);
      }
    }
  }
  &__list {
    height: 90%;
    overflow: auto;
    scroll-behavior: smooth;
  }
  &::-webkit-scrollbar {
    width: 6px;
    background-color: var(--sidebar-background);
    border-radius: 6px;
  }
  &::-webkit-scrollbar-thumb {
    background-color: var(--text-muted);
    border-radius: 6px;
  }
  .empty-text {
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }
}
</style>
