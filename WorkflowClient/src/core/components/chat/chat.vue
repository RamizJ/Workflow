<template>
  <div class="chat">
    <div class="chat__messages" ref="chat-messages">
      <div v-if="isListEmpty" class="no-messages">Нет сообщений</div>
      <transition-group name="message" tag="p">
        <chat-message v-for="item in list" :key="item.id" :message="item" />
      </transition-group>
    </div>
    <div class="chat__input">
      <chat-input v-model="newMessage" @keydown.enter.native="emitMessage" />
    </div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Ref, Vue, Watch } from 'vue-property-decorator'
import ChatMessage from '@/core/components/chat/chat-message.vue'
import ChatInput from '@/core/components/chat/chat-input.vue'
import Message from '@/core/types/message.model'

@Component({
  components: { ChatMessage, ChatInput },
})
export default class Chat extends Vue {
  @Prop() readonly messages!: Message[]
  @Ref('chat-messages') readonly chatMessages!: HTMLDivElement

  private list: Message[] = []
  private newMessage = ''

  protected mounted(): void {
    this.onMessagesChange(this.messages)
  }

  private get isListEmpty(): boolean {
    return this.list.length === 0
  }

  @Watch('messages')
  onMessagesChange(newMessagesList: Message[]) {
    this.list = newMessagesList
    setTimeout(() => this.scrollToBottom(), 1)
  }

  private emitMessage(): void {
    this.$emit('new-message', this.newMessage)
    this.newMessage = ''
  }

  private scrollToBottom(): void {
    this.chatMessages.scrollTop = this.chatMessages.scrollHeight
  }
}
</script>

<style lang="scss" scoped>
.chat {
  &__messages {
    height: 30vh;
    padding-right: 10px;
    overflow: auto;
    scroll-behavior: smooth;
    &::-webkit-scrollbar {
      width: 6px;
      background-color: var(--sidebar-background);
      border-radius: 6px;
    }
    &::-webkit-scrollbar-thumb {
      background-color: var(--text-muted);
      border-radius: 6px;
    }
  }
  .no-messages {
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
  }
}
</style>
