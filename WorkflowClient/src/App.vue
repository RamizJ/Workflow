<template lang="pug">
  div(id="app")
    component(:is="this.$route.meta.layout || 'div'")
      router-view
</template>
