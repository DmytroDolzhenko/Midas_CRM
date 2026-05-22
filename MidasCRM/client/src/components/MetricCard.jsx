import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Typography from '@mui/material/Typography'

export function MetricCard({ label, value, hint }) {
  return (
    <Card elevation={0} sx={{ border: '1px solid', borderColor: 'divider', borderRadius: 3 }}>
      <CardContent>
        <Typography color="text.secondary" variant="body2">{label}</Typography>
        <Typography sx={{ my: 1 }} variant="h5">{value}</Typography>
        <Typography color="text.secondary" variant="caption">{hint}</Typography>
      </CardContent>
    </Card>
  )
}
