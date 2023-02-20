<template>
  <div class="header">
    <Menu />
  </div>
  <div class="container">
    <router-view></router-view>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Menu from "./views/Menu/menu.vue";
export default defineComponent({
  components: { Menu },
  setup() {
    return {};
  },
});
</script>
