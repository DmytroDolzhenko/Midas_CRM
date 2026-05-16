export const initialCustomers = [
  { id: 1, name: 'Анна Коваль', email: 'anna@midas.test', phone: '+380671112233' },
  { id: 2, name: 'Олег Мельник', email: 'oleh@midas.test', phone: '+380502224455' },
  { id: 3, name: 'Марія Шевченко', email: 'maria@midas.test', phone: '+380933336677' },
]

export const initialProducts = [
  { id: 11, sku: 'PRD-11', name: 'Кавомашина Midas Pro', category: 'Техніка', stock: 18, price: 4820 },
  { id: 12, sku: 'PRD-12', name: 'Фільтр для води', category: 'Витратні матеріали', stock: 124, price: 690 },
  { id: 13, sku: 'PRD-13', name: 'Набір чашок', category: 'Аксесуари', stock: 41, price: 420 },
]

export const initialOrders = [
  { id: 1, code: 'MD-1007', customer: 'Анна Коваль', product: 'Кавомашина Midas Pro', quantity: 1, total: 4820, status: 'processing' },
  { id: 2, code: 'MD-1008', customer: 'Олег Мельник', product: 'Фільтр для води', quantity: 2, total: 1380, status: 'draft' },
  { id: 3, code: 'MD-1009', customer: 'Марія Шевченко', product: 'Набір чашок', quantity: 4, total: 1680, status: 'completed' },
]

export const initialChats = [
  {
    id: 1,
    customer: 'Анна Коваль',
    channel: 'Instagram',
    unread: 2,
    lastMessage: 'Чи є в наявності Midas Pro?',
    time: '12:45',
  },
  {
    id: 2,
    customer: 'Олег Мельник',
    channel: 'OLX',
    unread: 0,
    lastMessage: 'Дякую, очікую накладну.',
    time: '11:20',
  },
  {
    id: 3,
    customer: 'Марія Шевченко',
    channel: 'Instagram',
    unread: 1,
    lastMessage: 'Можна оформити доставку на завтра?',
    time: '09:05',
  },
]
