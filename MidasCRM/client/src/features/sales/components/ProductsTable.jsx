
import sharedStyles from '../../../styles/Shared.module.css'

const cx = (...classes) => classes.map((className) => sharedStyles[className] ?? className).join(' ')

export function ProductsTable({ products }) {
  return (
    <>
      <div className={cx('table-header', 'product-table')}>
        <span>Артикул</span>
        <span>Назва</span>
        <span>Бренд</span>
        <span>Склад</span>
        <span>Ціна</span>
      </div>
      {products.map((product) => (
        <div className={cx('table-row', 'product-table')} key={product.id}>
          <strong>{product.sku}</strong>
          <span>{product.name}</span>
          <span>{product.brand}</span>
          <span>{product.stock} {product.unit}</span>
          <span>{product.price.toLocaleString('uk-UA')} грн</span>
        </div>
      ))}
    </>
  )
}
