import TextField from '@mui/material/TextField'

export function Input({ label, ...props }) {
  return <TextField fullWidth label={label} size="small" variant="outlined" {...props} />
}
