import httpClient from './httpClient';
import qs from 'qs';

export default {
  findOneById: id => httpClient.get(`/api/Users/Get/${id}`),
  findAll: query => httpClient.post(`/api/Users/GetPage`, query),
  findAllByIds: query =>
    httpClient.get(
      `/api/Users/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    ),
  createOne: user => httpClient.post(`/api/Users/Create`, user),
  updateOne: user => httpClient.put(`/api/Users/Update`, user),
  deleteOne: userId => httpClient.delete(`/api/Users/Delete?id=${userId}`),
  deleteMany: userIds => httpClient.patch(`/api/Users/DeleteRange`, userIds),
  restoreOne: userId => httpClient.patch(`/api/Users/Restore/${userId}`),
  restoreMany: userIds => httpClient.patch(`/api/Users/RestoreRange`, userIds),
  resetPassword: userId =>
    httpClient.patch(`/api/Users/ResetPassword/${userId}`),
  isEmailExist: email =>
    httpClient.get(`/api/Users/IsEmailExist/${email}?email=${email}`),
  isUserNameExist: userName =>
    httpClient.get(`/api/Users/IsUserNameExist/${userName}`)
};
