import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Typography from '@mui/material/Typography'

export function Pagination({ page, pageSize, total, onPageChange }) {
  const pageCount = Math.max(1, Math.ceil(total / pageSize))
  const firstItem = total === 0 ? 0 : (page - 1) * pageSize + 1
  const lastItem = Math.min(page * pageSize, total)

  if (total <= pageSize) {
    return null
  }

  return (
    <Box sx={{ alignItems: 'center', borderTop: 1, borderColor: 'divider', display: 'flex', justifyContent: 'space-between', mt: 2, pt: 2 }}>
      <Typography color="text.secondary" variant="body2">
        {firstItem}-{lastItem} з {total}
      </Typography>
      <Box sx={{ alignItems: 'center', display: 'flex', gap: 1.5 }}>
        <Button disabled={page === 1} size="small" variant="outlined" onClick={() => onPageChange(page - 1)}>
          Назад
        </Button>
        <Typography variant="body2">{page} / {pageCount}</Typography>
        <Button disabled={page === pageCount} size="small" variant="outlined" onClick={() => onPageChange(page + 1)}>
          Далі
        </Button>
      </Box>
    </Box>
  )
}

