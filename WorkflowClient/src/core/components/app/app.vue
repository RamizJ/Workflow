<template>
  <div id="app">
    <vue-ins-progress-bar></vue-ins-progress-bar>
    <component :is="this.$route.meta.layout || 'div'">
      <router-view></router-view>
    </component>
  </div>
</template>
