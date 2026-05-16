export function ProductsTable({ products }) {
  return (
    <>
      <div className="table-header product-table">
        <span>SKU</span>
        <span>Назва</span>
        <span>Категорія</span>
        <span>Залишок</span>
      </div>
      {products.map((product) => (
        <div className="table-row product-table" key={product.id}>
          <strong>{product.sku}</strong>
          <span>{product.name}</span>
          <span>{product.category}</span>
          <span>{product.stock} шт</span>
        </div>
      ))}
    </>
  )
}
