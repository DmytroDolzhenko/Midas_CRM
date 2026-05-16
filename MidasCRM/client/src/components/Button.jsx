export function Button({ children, variant = 'primary', className = '', ...props }) {
  const buttonClass = variant === 'secondary' ? 'secondary-button' : 'primary-button'

  return (
    <button className={`${buttonClass} ${className}`.trim()} type="button" {...props}>
      {children}
    </button>
  )
}
