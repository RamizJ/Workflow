import qs from 'qs'
import http from './http'
import Query from '@/types/query.type'
import Team from '@/types/team.type'
import User from '@/types/user.type'
import Project from '@/types/project.type'
import { AxiosResponse } from 'axios'

export default {
  findAll(query: Query): Promise<AxiosResponse<Team[]>> {
    return http.post(`/api/Teams/GetPage`, query)
  },
  findAllByIds(ids: number[]): Promise<AxiosResponse<Team[]>> {
    return http.get(`/api/Teams/GetRange?${qs.stringify(ids)}`)
  },
  findOneById(id: number): Promise<AxiosResponse<Team>> {
    return http.get(`/api/Teams/Get/${id}`)
  },
  createOne(request: {
    team: Team
    userIds: string[]
    projectIds: number[]
  }): Promise<AxiosResponse<Team>> {
    return http.post(`/api/Teams/CreateByForm`, request)
  },
  updateOne(request: {
    team: Team
    userIds: string[]
    projectIds: number[]
  }): Promise<AxiosResponse<void>> {
    return http.put(`/api/Teams/UpdateByForm`, request)
  },
  updateMany(entities: Team[]): Promise<AxiosResponse<void>> {
    return http.put(`/api/Teams/UpdateByFormRange/${entities}`)
  },
  deleteOne(id: number): Promise<AxiosResponse<void>> {
    return http.delete(`/api/Teams/Delete/${id}`)
  },
  deleteMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/DeleteRange`, ids)
  },
  restoreOne(id: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/Restore/${id}`)
  },
  restoreMany(ids: number[]): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/RestoreRange`, ids)
  },
  findUsers(query: Query): Promise<AxiosResponse<User>> {
    return http.post(`/api/Teams/GetUsersPage?teamId=${query.teamId}`, query)
  },
  addUser(teamId: number, userId: string): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/AddUser/${teamId}`, JSON.stringify(userId))
  },
  removeUser(teamId: number, userId: string): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/RemoveUser/${teamId}`, JSON.stringify(userId))
  },
  findProjects(query: Query): Promise<AxiosResponse<Project[]>> {
    return http.post(`/api/Teams/GetProjectsPage?teamId=${query.teamId}`, query)
  },
  addProject(teamId: number, projectId: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/AddProject/${teamId}`, projectId)
  },
  removeProject(teamId: number, projectId: number): Promise<AxiosResponse<void>> {
    return http.patch(`/api/Teams/RemoveProject/${teamId}/${projectId}`)
  },
}
