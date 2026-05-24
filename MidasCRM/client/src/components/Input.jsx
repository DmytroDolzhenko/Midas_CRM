import TextField from '@mui/material/TextField'

export function Input({ label, ...props }) {
  return (
    <TextField 
      fullWidth 
      label={label} 
      size="small" 
      variant="outlined" 
      {...props} 
      sx={{
        '& .MuiInputLabel-root': {
          color: 'var(--text-muted, rgba(0, 0, 0, 0.6))', 
        },
        '& .MuiInputLabel-root.Mui-focused': {
          color: 'var(--primary, #fb923c)',
        },
        '& .MuiInputBase-input': {
          color: 'var(--text, #ffffff)', 
        }
      }}
    />
  )
}