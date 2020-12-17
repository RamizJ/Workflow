<template>
  <div class="goal-approval">
    <chat :messages="messages" @new-message="sendNewMessage" />
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import Chat from '@/core/components/chat/chat.vue'
import User from '@/modules/users/models/user.type'
import GoalMessage from '@/modules/goals/models/goal-message.model'
import authStore from '@/modules/users/store/auth.store'
import goalMessagesStore from '@/modules/goals/store/goal-messages.store'

@Component({
  components: { Chat },
})
export default class GoalApproval extends Vue {
  @Prop() readonly goalId?: number

  private get messages(): GoalMessage[] {
    return goalMessagesStore.messages
  }

  private get me(): User | null {
    return authStore.me
  }

  private get ownerFio(): string {
    return `${this.me?.lastName} ${this.me?.firstName} ${this.me?.middleName}`
  }

  private get ownerId(): string {
    return this.me?.id || ''
  }

  protected async mounted(): Promise<void> {
    if (this.goalId) await goalMessagesStore.getMessages(this.goalId)
  }

  private async sendNewMessage(text: string): Promise<void> {
    const message = new GoalMessage({
      text,
      date: new Date().toUTCString(),
      ownerFio: this.ownerFio,
      ownerId: this.ownerId,
    })
    await goalMessagesStore.sendMessage(message)
  }
}
</script>

<style scoped></style>
