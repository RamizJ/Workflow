<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import { Line, mixins } from 'vue-chartjs'

@Component({ extends: Line, mixins: [mixins.reactiveProp] })
export default class BaseChartPie extends Vue<Line> {
  @Prop() readonly chartData!: ChartData
  @Prop() readonly chartOptions!: ChartOptions

  constructor(props: any) {
    super(props)
  }

  protected mounted(): void {
    this.renderChart(this.chartData, this.chartOptions)
  }
}
</script>
