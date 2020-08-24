import qs from 'qs';
import httpClient from './httpClient';
import Query from '@/types/query.type';
import Project from '@/types/project.type';

export default {
  findOneById: (id: number) => httpClient.get(`/api/Projects/Get/${id}`),
  findAll: (query: Query) => httpClient.post(`/api/Projects/GetPage`, query),
  findAllByIds: (ids: number[]) =>
    httpClient.get(`/api/Projects/GetRange?${qs.stringify(ids)}`),
  createOne: (entity: Project) =>
    httpClient.post(`/api/Projects/CreateByForm`, entity),
  updateOne: (entity: Project) =>
    httpClient.put(`/api/Projects/UpdateByForm`, entity),
  updateMany: (entities: Project[]) =>
    httpClient.put(`/api/Goals/UpdateRange`, entities),
  deleteOne: (id: number) => httpClient.delete(`/api/Projects/Delete/${id}`),
  deleteMany: (ids: number[]) =>
    httpClient.patch(`/api/Projects/DeleteRange`, ids),
  restoreOne: (id: number) => httpClient.patch(`/api/Projects/Restore/${id}`),
  restoreMany: (ids: number[]) =>
    httpClient.patch(`/api/Projects/RestoreRange`, ids),
  findTeams: (query: Query) =>
    httpClient.post(
      `/api/Projects/GetTeamsPage?projectId=${query.projectId}`,
      query
    ),
  addTeam: (projectId: number, teamId: number) =>
    httpClient.patch(`/api/Projects/AddTeam/${teamId}?projectId=${projectId}`),
  removeTeam: (projectId: number, teamId: number) =>
    httpClient.patch(`/api/Projects/RemoveTeam/${teamId}/${projectId}`),
  getTasksCount: (projectId: number) =>
    httpClient.get(`/api/Goals/GetTotalProjectGoalsCount/${projectId}`),
  getTasksCountByStatus: (projectId: number, status: string) =>
    httpClient.get(
      `/api/Goals/GetProjectGoalsByStateCount/${projectId}?goalState=${status}`
    )
};
