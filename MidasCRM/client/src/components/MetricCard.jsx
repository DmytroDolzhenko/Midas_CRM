import Card from '@mui/material/Card'
import CardContent from '@mui/material/CardContent'
import Typography from '@mui/material/Typography'
import sharedStyles from '../styles/Shared.module.css'
import pageStyles from '../styles/pages/Dashboard.module.css'


const cx = (...classes) => classes.flatMap((className) => {
  const resolved = [sharedStyles[className], pageStyles[className]].filter(Boolean)
  return resolved.length ? resolved : className
}).join(' ')



export function MetricCard({ label, value, hint }) {
  return (
    <Card className={cx('metric-card')} elevation={0}>
      <CardContent sx={{ p: 2.5, '&:last-child': { pb: 2.5 } }}>
        <Typography className={cx('metric-card-label')} variant="body2">{label}</Typography>
        <Typography className={cx('metric-card-value')} sx={{ my: 1 }} variant="h5">{value}</Typography>
        <Typography className={cx('metric-card-hint')} variant="caption">{hint}</Typography>
      </CardContent>
    </Card>
  )
}
