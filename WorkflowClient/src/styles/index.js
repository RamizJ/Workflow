import '~/styles/main.scss';

document.documentElement.setAttribute(
  'theme',
  localStorage.getItem('theme') || 'light'
);
