import MuiButton from '@mui/material/Button'

export function Button({ children, variant = 'primary', className = '', ...props }) {
  return (
    <MuiButton
      className={className}
      variant={variant === 'secondary' ? 'outlined' : 'contained'}
      type="button"
      {...props}
    >
      {children}
    </MuiButton>
  )
}
