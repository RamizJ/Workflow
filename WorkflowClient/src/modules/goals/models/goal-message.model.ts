import Message, { MessageData } from '@/core/types/message.model'

export default class GoalMessage extends Message {
  public goalId?: number
  public goalTitle?: string

  constructor(data: GoalMessageData) {
    super(data)
    this.goalId = data.goalId
    this.goalTitle = data.goalTitle
  }
}

export interface GoalMessageData extends MessageData {
  goalId?: number
  goalTitle?: string
}
