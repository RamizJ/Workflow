import httpClient from './httpClient';
import qs from 'qs';

export default {
  findOneById: id => httpClient.get(`/api/Projects/Get/${id}`),
  findAll: params => httpClient.post(`/api/Projects/GetPage`, params),
  findAllByIds: ids =>
    httpClient.get(`/api/Projects/GetRange?${qs.stringify(ids)}`),
  createOne: item => httpClient.post(`/api/Projects/CreateByForm`, item),
  updateOne: item => httpClient.put(`/api/Projects/UpdateByForm`, item),
  updateMany: items => httpClient.put(`/api/Goals/UpdateRange`, items),
  deleteOne: id => httpClient.delete(`/api/Projects/Delete/${id}`),
  deleteMany: ids => httpClient.patch(`/api/Projects/DeleteRange`, ids),
  restoreOne: id => httpClient.patch(`/api/Projects/Restore/${id}`),
  restoreMany: ids => httpClient.patch(`/api/Projects/RestoreRange`, ids),
  findTeams: params =>
    httpClient.post(
      `/api/Projects/GetTeamsPage?projectId=${params.projectId}`,
      params
    ),
  addTeam: (projectId, teamId) =>
    httpClient.patch(`/api/Projects/AddTeam/${teamId}?projectId=${projectId}`),
  removeTeam: (projectId, teamId) =>
    httpClient.patch(`/api/Projects/RemoveTeam/${teamId}/${projectId}`),
  getTasksCount: projectId =>
    httpClient.get(`/api/Goals/GetTotalProjectGoalsCount/${projectId}`),
  getTasksCountByStatus: (projectId, status) =>
    httpClient.get(
      `/api/Goals/GetProjectGoalsByStateCount/${projectId}?goalState=${status}`
    )
};
