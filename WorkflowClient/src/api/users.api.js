import httpClient from './httpClient';
import qs from 'qs';

export default {
  get: id => httpClient.get(`/api/Users/Get/${id}`),
  create: user => httpClient.post(`/api/Users/Create`, user),
  update: user => httpClient.put(`/api/Users/Update`, user),
  delete: userId => httpClient.delete(`/api/Users/Delete?id=${userId}`),
  deleteRange: userIds => httpClient.patch(`/api/Users/DeleteRange`, userIds),
  resetPassword: userId =>
    httpClient.patch(`/api/Users/ResetPassword/${userId}`),
  isEmailExist: email => httpClient.get(`/api/Users/IsEmailExist/${email}`),
  isUserNameExist: userName =>
    httpClient.get(`/api/Users/IsUserNameExist/${userName}`),
  getPage: query => httpClient.post(`/api/Users/GetPage`, query),
  getRange: query =>
    httpClient.get(
      `/api/Users/GetRange${qs.stringify(query, { addQueryPrefix: true })}`
    )
};
