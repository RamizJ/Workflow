<script lang="ts">
import { Component, Prop, Vue } from 'vue-property-decorator'
import { ChartData, ChartOptions } from 'chart.js'
import { Line } from 'vue-chartjs'

@Component({
  extends: Line,
})
export default class ChartPie extends Vue<Line> {
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
