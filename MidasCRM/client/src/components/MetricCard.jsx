import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Typography from '@mui/material/Typography'

export function MetricCard({ label, value, hint }) {
  return (
    <Card className="metric-card" elevation={0}>
      <CardContent sx={{ p: 2.5, '&:last-child': { pb: 2.5 } }}>
        <Typography className="metric-card-label" variant="body2">{label}</Typography>
        <Typography className="metric-card-value" sx={{ my: 1 }} variant="h5">{value}</Typography>
        <Typography className="metric-card-hint" variant="caption">{hint}</Typography>
      </CardContent>
    </Card>
  )
}
