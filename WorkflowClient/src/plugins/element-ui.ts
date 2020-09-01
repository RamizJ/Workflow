import Vue from 'vue'
import {
  Dialog,
  Autocomplete,
  Dropdown,
  DropdownMenu,
  DropdownItem,
  Menu,
  Submenu,
  MenuItem,
  MenuItemGroup,
  Input,
  Radio,
  RadioButton,
  Checkbox,
  CheckboxButton,
  Switch,
  Select,
  Option,
  OptionGroup,
  Button,
  ButtonGroup,
  Table,
  TableColumn,
  DatePicker,
  TimeSelect,
  TimePicker,
  Popover,
  Tooltip,
  Form,
  FormItem,
  Tabs,
  TabPane,
  Tag,
  Row,
  Col,
  Upload,
  Progress,
  Badge,
  Card,
  Container,
  Header,
  Main,
  Footer,
  Timeline,
  TimelineItem,
  Link,
  Divider,
  Loading,
  MessageBox,
  Message
} from 'element-ui'
import language from 'element-ui/lib/locale/lang/ru-RU'
import locale from 'element-ui/lib/locale'
import 'element-ui/lib/theme-chalk/index.css'

locale.use(language)

Vue.use(Dialog)
Vue.use(Autocomplete)
Vue.use(Dropdown)
Vue.use(DropdownMenu)
Vue.use(DropdownItem)
Vue.use(Menu)
Vue.use(Submenu)
Vue.use(MenuItem)
Vue.use(MenuItemGroup)
Vue.use(Input)
Vue.use(Radio)
Vue.use(RadioButton)
Vue.use(Checkbox)
Vue.use(CheckboxButton)
Vue.use(Switch)
Vue.use(Select)
Vue.use(Option)
Vue.use(OptionGroup)
Vue.use(Button)
Vue.use(ButtonGroup)
Vue.use(Table)
Vue.use(TableColumn)
Vue.use(DatePicker)
Vue.use(TimeSelect)
Vue.use(TimePicker)
Vue.use(Popover)
Vue.use(Tooltip)
Vue.use(Form)
Vue.use(FormItem)
Vue.use(Tabs)
Vue.use(TabPane)
Vue.use(Tag)
Vue.use(Row)
Vue.use(Col)
Vue.use(Upload)
Vue.use(Progress)
Vue.use(Badge)
Vue.use(Card)
Vue.use(Container)
Vue.use(Header)
Vue.use(Main)
Vue.use(Footer)
Vue.use(Timeline)
Vue.use(TimelineItem)
Vue.use(Link)
Vue.use(Divider)

Vue.use(Loading.directive)

Vue.prototype.$loading = Loading.service
Vue.prototype.$confirm = MessageBox.confirm
Vue.prototype.$message = Message
