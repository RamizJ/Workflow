import * as SignalR from '@microsoft/signalr'
import { baseUrl } from '@/core/api/config'
import tokenStore from '@/modules/users/store/token.store'

export const HUB_METHOD = 'EntityStateChanged'

export default class GlobalHub {
  private connection!: SignalR.HubConnection
  public active = false

  private initialize(): void {
    console.log('hub: initialize')
    const url = `${baseUrl}/entity-state-observer`
    const token = tokenStore.token || ''
    console.log(url)
    console.log(token)
    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(SignalR.LogLevel.Warning)
      .build()
  }

  public async start(): Promise<void> {
    console.log('hub: open')
    this.initialize()
    this.connection.on(HUB_METHOD, (message) => this.onMessageReceived(message))
    await this.connection.start()
    this.active = true
  }

  public async stop(): Promise<void> {
    await this.connection.stop()
  }

  public async sendMessage(message: HubMessage): Promise<void> {
    console.log('hub: sending message')
    console.log(message)
    await this.connection.invoke(HUB_METHOD, message)
  }

  private onMessageReceived(message: HubMessage) {
    console.log('hub: message received')
    console.log(message)
  }
}

export class HubMessage {
  public senderId: string
  public entityIds: (string | number)[]
  public entityType: EntityType
  public operation: OperationType

  constructor(
    senderId: string,
    entitiesId: (string | number)[],
    entityType: EntityType,
    operation: OperationType
  ) {
    this.senderId = senderId
    this.entityIds = entitiesId
    this.entityType = entityType
    this.operation = operation
  }
}

export enum OperationType {
  Create,
  Update,
  Delete,
}

export enum EntityType {
  Goal,
  Project,
  Group,
  Team,
  User,
  Message,
}
