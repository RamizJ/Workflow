import authenticationAPI from './authentication.api'
import usersAPI from './users.api'

export default {
  ...authenticationAPI,
  ...usersAPI,
}
