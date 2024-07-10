<template>
  <div class="main-body">
    <div class="header">
      <Header />
    </div>
    <div class="container">
      <router-view></router-view>
    </div>
    <div class="footer">
      <Footer />
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Header from "./views/Menu/header.vue";
import Footer from "./views/Menu/footer.vue";
export default defineComponent({
  components: { Header, Footer },
  setup() {
    return {};
  },
});
</script>
