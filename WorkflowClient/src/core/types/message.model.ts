export default class Message {
  public id: number
  public avatar?: string
  public text = ''
  public date: Date
  public senderName = 'Аноним'
  public senderId?: string

  constructor(text: string, date: Date, senderName: string, senderId?: string) {
    this.id = this.getId()
    this.text = text
    this.date = date
    this.senderName = senderName
    this.senderId = senderId
  }

  private getId(): number {
    return Math.floor(Math.random() * Math.floor(10000))
  }

  public get shortDate(): string {
    return this.date.toLocaleString('ru', {
      timeZone: 'UTC',
      hour: '2-digit',
      minute: '2-digit',
    })
  }

  public get fullDate(): string {
    return this.date.toLocaleString('ru', {
      timeZone: 'UTC',
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    })
  }
}
