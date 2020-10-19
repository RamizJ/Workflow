<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import { Doughnut } from 'vue-chartjs'

@Component({
  extends: Doughnut,
})
export default class ChartDoughnut extends Vue<Doughnut> {
  constructor(props: any) {
    super(props)
  }
  @Prop() readonly data!: ChartData
  @Prop() readonly options!: ChartOptions

  protected mounted(): void {
    this.renderChart(this.data, this.options)
  }
}
</script>
