const FtpDeploy = require('ftp-deploy')
const ftpDeploy = new FtpDeploy()

const config = {
  user: '',
  password: '',
  host: '10.62.20.8',
  port: 21,
  localRoot: __dirname + '/dist/',
  remoteRoot: '/workflow_dev/wwwroot/',
  include: ['*', '**/*'],
  deleteRemote: true,
  forcePasv: true,
}
const description = `FTP: ${config.host}:${config.port} ${config.remoteRoot}`
const successMessage = `Deploy finished! ${description}`
const errorMessage = `Deploy failed! ${description}`

console.log(`Deploy in process...`)
ftpDeploy
  .deploy(config)
  .then((res) => console.log(successMessage))
  .catch((err) => console.log(errorMessage, err))
