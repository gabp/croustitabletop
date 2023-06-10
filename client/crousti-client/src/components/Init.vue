<template>
  <div>
    <div v-if="error != null">
      <h2>Could not connect to <i>{{ this.serverUrl }}</i></h2>
      <h3>{{ this.error }}</h3>
    </div>
    <div v-else-if="cards == null">
      <Preloader color="red" />
    </div>
    <div v-else>
      <hand :cards="cards" :playerHubConnection="playerHubConnection"></hand>
    </div>
    
  </div>
</template>

<script>
import { defineComponent } from 'vue'
import Preloader from './Preloader.vue'
import Hand from './Hand.vue'
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr'


export default defineComponent({
  name: 'Home',
  components: {
    Preloader,
    Hand
  },
  data: () => ({
    serverUrl: null,
    playerColor: null,
    error: null,
    cards: null,
    playerHubConnection: null,
  }),
  methods: {

  },
  mounted() {
    this.serverUrl = this.$route.query.serverUrl
    this.playerColor = this.$route.query.playerColor
    this.token = this.$route.query.access_token

    var url = `${this.serverUrl}/player-hub`;
    this.playerHubConnection = new HubConnectionBuilder()
      .withUrl(url, { accessTokenFactory: () => this.token })
      .configureLogging(LogLevel.Error)
      .withAutomaticReconnect()
      .build();

    this.playerHubConnection.on("PlayerHandUpdatedEvent", data => {
      console.log(data)
      this.cards = data
    });

    this.playerHubConnection.start()
      .catch(error => {
          this.error = error
        })
  },
})



</script>

<style scoped>
  
</style>