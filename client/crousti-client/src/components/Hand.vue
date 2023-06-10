<template>
  <Carousel class="carousel_width" :wrapAround="false" :mouseDrag="true" :touchDrag="true" :breakpoints="breakpoints" v-model="currentSlide" ref="carousel">
    <Slide v-for="(card, i) in cards" :key="card['guid']" @pointerup="slideTo(i, card['guid'])">
      <div :style="`background-image: url('${card['imageURL']}'); background-repeat: no-repeat; background-size: 173px 200px;`" class="carousel__item" :class="{ 'selected_carousel_item': isActive(i) }"></div>
    </Slide>
  </Carousel>
</template>

<script>
import { defineComponent } from 'vue'
import { Carousel, Pagination, Slide } from 'vue3-carousel'

import 'vue3-carousel/dist/carousel.css'

export default defineComponent({
  name: 'CardHand',
  components: {
    Carousel,
    Slide,
    Pagination,
  },
  props: ['cards', 'playerHubConnection'],
  data: () => ({
    serverUrl: null,
    playerColor: null,
    currentSlide: 0,
    activeSlide: null,
    //cards: null,
    // carousel settings
    settings: {
      itemsToShow: 1,
      snapAlign: 'start',
    },
    // breakpoints are mobile first
    // any settings not specified will fallback to the carousel settings
    breakpoints: {
      // 500px and up
      500: {
        itemsToShow: 4,
        snapAlign: 'start',
      },
      // 1024px and up
      1024: {
        itemsToShow: 7,
        snapAlign: 'start',
      },
    },
  }),
  methods: {
    isActive(slide) {
      return this.activeSlide == (slide)
    },
    slideTo(slideIndex, cardGuid) {
      this.currentSlide = slideIndex

      if (slideIndex != this.activeSlide) {
        this.activeSlide = slideIndex
        this.toggleHighlight(cardGuid, true)
      }
      else {
        this.activeSlide = null
        this.toggleHighlight(cardGuid, false)
      }
    },
    toggleHighlight(cardGuid, shouldHighlight) {
      this.playerHubConnection.invoke("HighlightCard", cardGuid, shouldHighlight)
    }
  },
  mounted() {
    this.serverUrl = this.$route.query.serverUrl
    this.playerColor = this.$route.query.playerColor
  }
})



</script>

<style scoped>

@media (min-width: 500px) {
  .carousel_width {
    width: 730px;
  }
}

@media (min-width: 1224px) {
  .carousel_width {
    min-width: 1280px;
  }
}

.carousel__slide {
  padding: 5px;
}

.carousel__viewport {
  perspective: 200px;
}

.carousel__track {
  transform-style: preserve-3d;
}

.carousel__slide--sliding {
  transition: 0.5s;
}

/*.carousel__slide {
  opacity: 0.9;
  transform: rotateY(-20deg) scale(0.9);
}

.carousel__slide--active ~ .carousel__slide {
  transform: rotateY(20deg) scale(0.9);
}

.carousel__slide--prev {
  opacity: 1;
  transform: rotateY(-10deg) scale(0.95);
}

.carousel__slide--next {
  opacity: 1;
  transform: rotateY(10deg) scale(0.95);
}

.carousel__slide--active {
  opacity: 1;
  transform: rotateY(0) scale(1.05);
}*/

/*.carousel__item {
  min-height: 200px;
  width: 100%;
  background-color: var(--vc-clr-primary);
  color: var(--vc-clr-white);
  font-size: 20px;
  border-radius: 8px;
  display: flex;
  justify-content: center;
  align-items: center;
}*/

.carousel__item {
  min-height: 200px;
  width: 100%;
  background-color: var(--vc-clr-primary);
  color: var(--vc-clr-white);
  font-size: 20px;
  border-radius: 8px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.selected_carousel_item {
  outline: 5px solid red;
}

</style>