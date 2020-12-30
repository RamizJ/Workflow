export default class Message {
  public id: number
  public text: string

  public creationDate?: string
  public lastEditDate?: string
  public ownerFullName = 'Аноним'
  public ownerId?: string

  public messageSubscribers: MessageSubscriber[]

  constructor(data: MessageData) {
    this.id = this.getId()
    this.text = data.text || ''
    this.creationDate = data.creationDate
    this.lastEditDate = data.lastEditDate
    this.ownerFullName = data.ownerFullName
    this.ownerId = data.ownerId
    this.messageSubscribers = data.messageSubscribers || []
  }

  private getId(): number {
    return Math.floor(Math.random() * Math.floor(10000))
  }

  public get shortDate(): string {
    if (this.creationDate)
      return new Date(this.creationDate).toLocaleString('ru', {
        timeZone: 'UTC',
        hour: '2-digit',
        minute: '2-digit',
      })
    else return '--:--'
  }

  public get fullDate(): string {
    if (this.creationDate)
      return new Date(this.creationDate).toLocaleString('ru', {
        timeZone: 'UTC',
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
      })
    else return '--.--.-- --:--'
  }
}

export interface MessageData {
  id?: number
  text?: string

  creationDate?: string
  lastEditDate?: string
  ownerId: string
  ownerFullName: string

  messageSubscribers?: MessageSubscriber[]
}

export interface MessageSubscriber {
  userId: string
  userFullName: string
  lastReadingDate: string
}
