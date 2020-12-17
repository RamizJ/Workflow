<template>
  <div class="sidebar-footer">
    <Popover>
      <div class="notifications">
        <div class="notifications__title">Уведомления</div>
        <div v-if="isEmpty" class="empty-text">Нет уведомлений</div>
        <transition-group name="message" tag="p">
          <notification
            v-for="item in notifications"
            :key="item.id"
            :title="item.goalTitle"
            :subtitle="item.ownerFio"
            :content="item.text"
            :date="item.fullDate"
          />
        </transition-group>
      </div>
      <IconButton
        slot="reference"
        icon="bell"
        :badge="notifications.length"
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
    return goalMessagesStore.messages
  }

  private get isEmpty(): boolean {
    return this.notifications.length === 0
  }

  private async updateNotifications(): Promise<void> {
    await goalMessagesStore.getUnreadMessages()
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
  width: 250px;
  overflow: auto;
  scroll-behavior: smooth;
  .empty-text {
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }
  &__title {
    font-size: 18px;
    font-weight: 600;
    color: var(--text);
    margin-bottom: 6px;
  }
}
</style>
