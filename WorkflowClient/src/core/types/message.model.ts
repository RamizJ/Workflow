export default class Message {
  public id: number
  public text: string

  public date: string
  public ownerFio = 'Аноним'
  public ownerId?: string

  public isRead: boolean

  constructor(data: MessageData) {
    this.id = this.getId()
    this.text = data.text || ''
    this.date = data.date
    this.ownerFio = data.ownerFio
    this.ownerId = data.ownerId
    this.isRead = data.isRead || true
  }

  private getId(): number {
    return Math.floor(Math.random() * Math.floor(10000))
  }

  public get shortDate(): string {
    return new Date(this.date).toLocaleString('ru', {
      timeZone: 'UTC',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  public get fullDate(): string {
    return new Date(this.date).toLocaleString('ru', {
      timeZone: 'UTC',
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }
}

export interface MessageData {
  id?: number
  text?: string

  date: string
  ownerId: string
  ownerFio: string

  isRead?: boolean
}
