import Vue from 'vue';
import {
  Button,
  Input,
  Select,
  Option,
  DatePicker,
  Upload,
  Tooltip,
  Table,
  TableColumn,
  Menu,
  MenuItem,
  Tabs,
  TabPane,
  Collapse,
  CollapseItem,
  Card,
  Popover,
  Avatar,
  Dialog,
  Form,
  FormItem,
  Row,
  Col,
  Divider,
  Checkbox,
  Dropdown,
  DropdownMenu,
  DropdownItem,
  Switch,
  Progress,
  Loading
} from 'element-ui';
import language from 'element-ui/lib/locale/lang/ru-RU';
import locale from 'element-ui/lib/locale';
import 'element-ui/lib/theme-chalk/index.css';

locale.use(language);

Vue.component(Button.name, Button);
Vue.component(Input.name, Input);
Vue.component(Select.name, Select);
Vue.component(Option.name, Option);
Vue.component(DatePicker.name, DatePicker);
Vue.component(Upload.name, Upload);
Vue.component(Tooltip.name, Tooltip);
Vue.component(Table.name, Table);
Vue.component(TableColumn.name, TableColumn);
Vue.component(Menu.name, Menu);
Vue.component(MenuItem.name, MenuItem);
Vue.component(Tabs.name, Tabs);
Vue.component(TabPane.name, TabPane);
Vue.component(Collapse.name, Collapse);
Vue.component(CollapseItem.name, CollapseItem);
Vue.component(Card.name, Card);
Vue.component(Popover.name, Popover);
Vue.component(Avatar.name, Avatar);
Vue.component(Dialog.name, Dialog);
Vue.component(Form.name, Form);
Vue.component(FormItem.name, FormItem);
Vue.component(Row.name, Row);
Vue.component(Col.name, Col);
Vue.component(Divider.name, Divider);
Vue.component(Checkbox.name, Checkbox);
Vue.component(Dropdown.name, Dropdown);
Vue.component(DropdownMenu.name, DropdownMenu);
Vue.component(DropdownItem.name, DropdownItem);
Vue.component(Switch.name, Switch);
Vue.component(Progress.name, Progress);
Loading.install(Vue);
