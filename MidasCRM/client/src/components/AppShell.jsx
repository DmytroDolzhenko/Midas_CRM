import { useMemo, useState } from 'react'
import {
  AppBar,
  Avatar,
  Badge,
  Box,
  Button,
  Card,
  CardActionArea,
  CardContent,
  CssBaseline,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Divider,
  Drawer,
  FormControl,
  InputLabel,
  List,
  ListItemButton,
  ListItemText,
  Menu,
  MenuItem,
  Select,
  Stack,
  Tab,
  Tabs,
  Toolbar,
  Typography,
  TextField,
  createTheme,
  ThemeProvider,
} from '@mui/material'
import NotificationsOutlinedIcon from '@mui/icons-material/NotificationsOutlined'
import SettingsOutlinedIcon from '@mui/icons-material/SettingsOutlined'
import SearchIcon from '@mui/icons-material/Search'
import AutoAwesomeOutlinedIcon from '@mui/icons-material/AutoAwesomeOutlined'
import { integrations as defaultIntegrations } from '../lib/integrationsApi.js'
import styles from './AppShell.module.css'

const drawerWidth = 240

const navItems = [
  { id: 'dashboard', label: 'Головна' },
  { id: 'products', label: 'Товари' },
  { id: 'orders', label: 'Продажі' },
  { id: 'finances', label: 'Фінанси' },
  { id: 'customers', label: 'Клієнти' },
  { id: 'operations', label: 'Історія' },
  { id: 'company', label: 'Компанія' },
]

function getValue(item, camelKey, pascalKey) {
  return item?.[camelKey] ?? item?.[pascalKey]
}

function buildTheme(mode) {
  return createTheme({
    palette: {
      mode,
      primary: { main: mode === 'dark' ? '#fb923c' : '#f97316' },
      secondary: { main: '#f59e0b' },
      background: {
        default: mode === 'dark' ? '#0d0805' : '#fff7ed',
        paper: mode === 'dark' ? '#1b120d' : '#ffffff',
      },
      text: {
        primary: mode === 'dark' ? '#fff7ed' : '#1f1308',
        secondary: mode === 'dark' ? '#b8a99d' : '#7c5a3a',
      },
      divider: mode === 'dark' ? 'rgba(255, 255, 255, 0.1)' : '#fed7aa',
      action: {
        selected: mode === 'dark' ? 'rgba(251, 146, 60, 0.16)' : '#ffedd5',
        hover: mode === 'dark' ? 'rgba(251, 146, 60, 0.12)' : '#ffedd5',
      },
    },
    shape: { borderRadius: 18 },
    typography: {
      fontFamily: 'Inter, ui-sans-serif, system-ui, "Segoe UI", sans-serif',
      fontWeightRegular: 500,
      fontWeightMedium: 700,
      fontWeightBold: 800,
    },
    components: {
      MuiButton: {
        defaultProps: { disableElevation: true },
        styleOverrides: {
          root: {
            borderRadius: 14,
            fontWeight: 800,
            textTransform: 'none',
          },
        },
      },
      MuiOutlinedInput: {
        styleOverrides: {
          root: {
            borderRadius: 14,
          },
        },
      },
      MuiCard: {
        styleOverrides: {
          root: {
            backgroundImage: 'none',
          },
        },
      },
    },
  })
}

export function AppShell({
  activePage,
  user,
  theme,
  onThemeChange,
  onNavigate,
  onLogout,
  companies = [],
  activeCompanyId,
  onCompanyChange,
  children,
}) {
  const [accountAnchor, setAccountAnchor] = useState(null)
  const [isSettingsOpen, setIsSettingsOpen] = useState(false)
  const [isNotificationsOpen, setIsNotificationsOpen] = useState(false)
  const [isAiOpen, setIsAiOpen] = useState(false)
  const [settingsTab, setSettingsTab] = useState(0)
  const [integrations, setIntegrations] = useState(defaultIntegrations)

  const muiTheme = useMemo(() => buildTheme(theme === 'dark' ? 'dark' : 'light'), [theme])
  return (
    <ThemeProvider theme={muiTheme}>
      <CssBaseline />
      <Box className={styles['app-shell']} data-theme={theme === 'dark' ? 'dark' : 'light'} sx={{ display: 'block', minHeight: '100vh' }}>
        <Drawer
          variant="permanent"
          sx={{
            display: { xs: 'none', md: 'block' },
            width: drawerWidth,
            flexShrink: 0,
            '& .MuiDrawer-paper': {
              width: drawerWidth,
              boxSizing: 'border-box',
              borderRight: 1,
              borderColor: 'divider',
              backgroundImage: 'none',
            },
          }}
        >
          <Stack spacing={2} sx={{ p: 1.5, height: '100%' }}>
            <Box sx={{ pt: 1 }}>
              <Stack alignItems="center" direction="row" spacing={1.25}>
                <Box sx={{ width: 34, height: 34, borderRadius: 1.5, bgcolor: 'primary.main' }} />
                <Box>
                  <Typography fontWeight={900} variant="h6">Midas CRM</Typography>
                  <Typography color="text.secondary" fontWeight={700} variant="body2">Client workspace</Typography>
                </Box>
              </Stack>
            </Box>

            <FormControl fullWidth size="small">
              <InputLabel id="company-label">Компанія</InputLabel>
              <Select
                label="Компанія"
                labelId="company-label"
                value={activeCompanyId ?? ''}
                onChange={(event) => onCompanyChange?.(event.target.value)}
              >
                {companies.map((company) => {
                  const companyId = String(getValue(company, 'id', 'Id'))
                  return <MenuItem key={companyId} value={companyId}>{getValue(company, 'name', 'Name')}</MenuItem>
                })}
              </Select>
            </FormControl>

            <List disablePadding sx={{ display: 'grid', gap: 0.75, my: 'auto' }}>
              {navItems.map((item) => (
                <ListItemButton
                  key={item.id}
                  selected={item.id === activePage}
                  onClick={() => onNavigate(item.id)}
                  sx={{
                    borderRadius: 3,
                    minHeight: 46,
                    px: 2,
                    '&.Mui-selected': {
                      bgcolor: 'action.selected',
                      color: 'primary.main',
                    },
                    '&.Mui-selected:hover': {
                      bgcolor: 'action.selected',
                    },
                  }}
                >
                  <ListItemText
                    primary={item.label}
                    primaryTypographyProps={{ fontWeight: 800, textAlign: 'center' }}
                  />
                </ListItemButton>
              ))}
            </List>

            <Box sx={{ pb: 1 }}>
              <Button
                fullWidth
                startIcon={<Avatar sx={{ width: 24, height: 24 }}>{(user?.name ?? 'U').slice(0, 1).toUpperCase()}</Avatar>}
                variant="outlined"
                sx={{ justifyContent: 'flex-start', overflow: 'hidden', textTransform: 'none' }}
                onClick={(event) => setAccountAnchor(event.currentTarget)}
              >
                {user?.email ?? 'user'}
              </Button>
            </Box>
          </Stack>
        </Drawer>

        <Box sx={{ ml: { md: `${drawerWidth}px` }, minWidth: 0 }}>
          <AppBar
            color="inherit"
            elevation={0}
            position="sticky"
            sx={{
              borderBottom: 1,
              borderColor: 'divider',
            }}
          >
          <Toolbar sx={{ gap: 1.5, justifyContent: 'space-between', flexWrap: { xs: 'wrap', md: 'nowrap' }, py: { xs: 1, md: 0 } }}>

          <TextField
            size="small"
            placeholder="Пошук..."
            variant="outlined"
            // Сюди передаєш свій стейт для пошуку, наприклад value={searchQuery}
            // onChange={(e) => setSearchQuery(e.target.value)}
            InputProps={{
              startAdornment: (
                <SearchIcon fontSize="small" sx={{ color: 'text.secondary', mr: 1 }} />
              ),
            }}
            sx={{
              flexGrow: 1,
              maxWidth: { xs: '100%', md: '400px' },
              mx: { xs: 0, md: 2 },
              order: { xs: 3, md: 2 },
              
              '& .MuiOutlinedInput-root': {
                borderRadius: '20px',
                backgroundColor: 'background.paper',
                '&:hover fieldset': {
                  borderColor: 'primary.main',
                },
              },
            }}
          />

  <Stack 
    direction="row" 
    spacing={1} 
    sx={{ 
      flexWrap: 'wrap', 
      rowGap: 1, 
      justifyContent: 'flex-end',
      flexGrow: { xs: 1, md: 0 },
      order: { xs: 2, md: 3 }
    }}
  >
    <Button size="small" startIcon={<AutoAwesomeOutlinedIcon />} variant="outlined" onClick={() => setIsAiOpen(true)}>AI</Button>
    <Button size="small" variant="outlined" onClick={() => setIsSettingsOpen(true)}><SettingsOutlinedIcon fontSize="small" /></Button>
    <Button size="small" variant="outlined" onClick={() => setIsNotificationsOpen(true)}>
      <Badge color="error" variant="dot"><NotificationsOutlinedIcon fontSize="small" /></Badge>
    </Button>
  </Stack>
</Toolbar>
            <Box
              sx={{
                display: { xs: 'flex', md: 'none' },
                gap: 1,
                overflowX: 'auto',
                px: 2,
                pb: 1.25,
              }}
            >
              {navItems.map((item) => (
                <Button
                  key={item.id}
                  size="small"
                  variant={item.id === activePage ? 'contained' : 'outlined'}
                  onClick={() => onNavigate(item.id)}
                  sx={{ flex: '0 0 auto' }}
                >
                  {item.label}
                </Button>
              ))}
            </Box>
          </AppBar>

          <Box component="main" sx={{ p: { xs: 2, md: 3 }, maxWidth: 1240, mx: 'auto', width: '100%' }}>
            {children}
          </Box>
        </Box>
      </Box>

      <Menu anchorEl={accountAnchor} open={Boolean(accountAnchor)} onClose={() => setAccountAnchor(null)}>
        <MenuItem onClick={() => { setIsSettingsOpen(true); setAccountAnchor(null) }}>Налаштування</MenuItem>
        <MenuItem onClick={() => { setSettingsTab(1); setIsSettingsOpen(true); setAccountAnchor(null) }}>Інтеграції</MenuItem>
        <Divider />
        <MenuItem onClick={onLogout}>Вийти</MenuItem>
      </Menu>

      <Dialog fullWidth maxWidth="md" open={isSettingsOpen} onClose={() => setIsSettingsOpen(false)}>
        <DialogTitle>Налаштування</DialogTitle>
        <DialogContent>
          <Tabs sx={{ mb: 2 }} value={settingsTab} onChange={(_, value) => setSettingsTab(value)}>
            <Tab label="Профіль" />
            <Tab label="Інтеграції" />
          </Tabs>

          {settingsTab === 0 && (
            <Stack spacing={2}>
              <TextField label="Імʼя профілю" size="small" value={user?.name ?? ''} InputProps={{ readOnly: true }} />
              <TextField label="Email" size="small" value={user?.email ?? ''} InputProps={{ readOnly: true }} />
              <FormControl size="small" sx={{ maxWidth: 240 }}>
                <InputLabel id="theme-label">Тема</InputLabel>
                <Select label="Тема" labelId="theme-label" value={theme} onChange={(event) => onThemeChange(event.target.value)}>
                  <MenuItem value="light">Світла</MenuItem>
                  <MenuItem value="dark">Темна</MenuItem>
                </Select>
              </FormControl>
            </Stack>
          )}

          {settingsTab === 1 && (
            <Box sx={{ display: 'grid', gap: 1.5, gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' } }}>
              {integrations.map((integration) => (
                <Card
                  key={integration.id}
                  sx={{
                    border: 1,
                    borderColor: integration.enabled ? 'primary.main' : 'divider',
                    bgcolor: integration.enabled
                      ? (theme === 'dark' ? 'rgba(251, 146, 60, 0.18)' : 'rgba(249, 115, 22, 0.1)')
                      : 'background.paper',
                    color: 'text.primary',
                  }}
                  variant="outlined"
                >
                  <CardActionArea
                    onClick={() => {
                      setIntegrations((currentIntegrations) =>
                        currentIntegrations.map((item) =>
                          item.id === integration.id ? { ...item, enabled: !item.enabled } : item,
                        ),
                      )
                    }}
                  >
                    <CardContent>
                      <Typography fontWeight={700}>{integration.name}</Typography>
                      <Typography color="text.secondary" sx={{ mb: 1 }} variant="body2">{integration.description}</Typography>
                      <Button size="small" variant={integration.enabled ? 'contained' : 'outlined'}>
                        {integration.enabled ? 'Підключено' : 'Підключити'}
                      </Button>
                    </CardContent>
                  </CardActionArea>
                </Card>
              ))}
            </Box>
          )}
        </DialogContent>
        <DialogActions>
          <Button variant="outlined" onClick={() => setIsSettingsOpen(false)}>Закрити</Button>
        </DialogActions>
      </Dialog>

      <Dialog fullWidth maxWidth="xs" open={isNotificationsOpen} onClose={() => setIsNotificationsOpen(false)}>
        <DialogTitle>Сповіщення</DialogTitle>
        <DialogContent>
          <Typography color="text.secondary">Нових сповіщень немає</Typography>
        </DialogContent>
      </Dialog>

      <Dialog fullWidth maxWidth="sm" open={isAiOpen} onClose={() => setIsAiOpen(false)}>
        <DialogTitle>AI рекомендації</DialogTitle>
        <DialogContent>
          <Typography color="text.secondary" variant="body2">
            Тут буде модуль аналітики продажів, витрат, маржі та складських залишків з порадами для прийняття рішень.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button variant="contained" onClick={() => setIsAiOpen(false)}>Зрозуміло</Button>
        </DialogActions>
      </Dialog>
    </ThemeProvider>
  )
}
