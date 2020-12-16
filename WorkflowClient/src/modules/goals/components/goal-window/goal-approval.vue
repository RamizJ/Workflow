<template>
  <div class="goal-approval">
    <chat :messages="messages" @new-message="sendNewMessage" />
  </div>
</template>

<script lang="ts">
import { Component, Vue } from 'vue-property-decorator'
import Chat from '@/core/components/chat/chat.vue'
import Message from '@/core/types/message.model'
import authStore from '@/modules/users/store/auth.store'
import User from '@/modules/users/models/user.type'

@Component({
  components: { Chat },
})
export default class GoalApproval extends Vue {
  private messages: Message[] = []

  private get me(): User | null {
    return authStore.me
  }

  private get senderName(): string {
    return `${this.me?.lastName} ${this.me?.firstName} ${this.me?.middleName}`
  }

  private get senderId(): string {
    return this.me?.id || ''
  }

  protected async mounted(): Promise<void> {
    // TODO: Загрузить существующие сообщения
  }

  private async sendNewMessage(text: string): Promise<void> {
    const message = new Message(text, new Date(), this.senderName, this.senderId)
    // TODO: Отправка сообщения на сервер
    this.messages.push(message)
  }
}
</script>

<style scoped></style>
