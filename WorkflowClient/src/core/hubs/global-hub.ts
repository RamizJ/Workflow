import * as SignalR from '@microsoft/signalr'
import { baseUrl } from '@/core/api/config'
import tokenStore from '@/modules/users/store/token.store'
import authStore from '@/modules/users/store/auth.store'
import goalMessagesStore from '@/modules/goals/store/goal-messages.store'

export const HUB_METHOD = 'EntityStateChanged'

export default class GlobalHub {
  private connection!: SignalR.HubConnection
  public active = false

  private initialize(): void {
    const url = `${baseUrl}/entity-state-observer`
    const token = tokenStore.token || ''
    this.connection = new SignalR.HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .configureLogging(SignalR.LogLevel.Warning)
      .build()
  }

  public async start(): Promise<void> {
    if (this.active) return
    this.initialize()
    this.connection.on(HUB_METHOD, (message) => this.onMessageReceived(message))
    try {
      await this.connection.start()
      this.active = true
    } catch (e) {
      console.error('Failed to start SignalR global hub')
    }
  }

  public async stop(): Promise<void> {
    await this.connection.stop()
  }

  public async sendMessage(message: HubMessage): Promise<void> {
    console.log('hub: sending message')
    console.log(message)
    await this.connection.invoke(HUB_METHOD, message)
  }

  private async onMessageReceived(message: HubMessage): Promise<void> {
    console.log('hub: message received')
    console.log(message)
    switch (message.entityType) {
      case EntityType.GoalMessage:
        if (message.operation === OperationType.Create) {
          if (message.senderId === authStore.me?.id) return
          for (const id of message.entityIds) {
            const goalMessage = await goalMessagesStore.getMessage(id as number)
            if (goalMessage) goalMessagesStore.pushUnreadMessage(goalMessage)
          }
        }
        break
      default:
        break
    }
  }
}

export class HubMessage {
  public entityIds: (string | number)[]
  public entityType: EntityType
  public operation: OperationType
  public senderId: string

  constructor(
    entitiesId: (string | number)[],
    entityType: EntityType,
    operation: OperationType,
    senderId: string
  ) {
    this.entityIds = entitiesId
    this.entityType = entityType
    this.operation = operation
    this.senderId = senderId
  }
}

export enum OperationType {
  Create,
  Update,
  Delete,
}

export enum EntityType {
  Goal = 'Goal',
  Project = 'Project',
  Group = 'Group',
  Team = 'Team',
  User = 'User',
  GoalMessage = 'GoalMessage',
}
