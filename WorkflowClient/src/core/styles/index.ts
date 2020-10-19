import './_reset.scss'
import './_variables.scss'
import './fonts.scss'
import './_general.scss'
import './_transitions.scss'
import './vue-context.scss'

document.documentElement.setAttribute('theme', localStorage.getItem('theme') || 'light')
