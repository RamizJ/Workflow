<template>
  <div class="chat-message">
    <div class="chat-message__caption">
      {{ message.ownerFullName }}
      <span class="chat-message__date" :title="message.fullDate">{{ message.shortDate }}</span>
    </div>
    <div class="chat-message__body">{{ message.text }}</div>
  </div>
</template>

<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import Message from '@/core/types/message.model'

@Component
export default class ChatMessage extends Vue {
  @Prop() readonly message!: Message
}
</script>

<style lang="scss" scoped>
.chat-message {
  padding: 8px 0;
  font-size: 13px;
  &__caption {
    font-weight: 600;
    color: var(--color-primary);
    margin-bottom: 6px;
  }
  &__date {
    float: right;
    font-size: 12px;
    font-weight: 400;
    color: var(--text-muted);
  }
  &__body {
    color: var(--text);
  }
}
</style>
