import { useState } from 'react'
import { Button } from '../components/Button.jsx'

const categories = ['Техніка', 'Аксесуари', 'Витратні матеріали']

export function CreateProductPage({ onBack, onCreate }) {
  const [name, setName] = useState('')
  const [category, setCategory] = useState(categories[0])
  const [stock, setStock] = useState(1)
  const [price, setPrice] = useState(0)
  const [description, setDescription] = useState('')

  function handleSubmit(event) {
    event.preventDefault()
    onCreate({ name, category, stock, price, description })
  }

  return (
    <section className="page-stack">
      <div className="page-header">
        <div>
          <p className="eyebrow">Create product</p>
          <h1>Новий товар</h1>
        </div>
        <Button variant="secondary" onClick={onBack}>
          До каталогу
        </Button>
      </div>

      <form className="form-grid" onSubmit={handleSubmit}>
        <section className="panel form-section">
          <h2>Основна інформація</h2>
          <label className="field">
            <span>Назва</span>
            <input required value={name} onChange={(event) => setName(event.target.value)} />
          </label>
          <label className="field">
            <span>Категорія</span>
            <select value={category} onChange={(event) => setCategory(event.target.value)}>
              {categories.map((item) => (
                <option key={item}>{item}</option>
              ))}
            </select>
          </label>
          <label className="field">
            <span>Опис</span>
            <textarea
              rows="4"
              value={description}
              onChange={(event) => setDescription(event.target.value)}
            />
          </label>
        </section>

        <aside className="panel summary-panel">
          <h2>Склад</h2>
          <label className="field">
            <span>Ціна</span>
            <input
              min="0"
              type="number"
              value={price}
              onChange={(event) => setPrice(Number(event.target.value))}
            />
          </label>
          <label className="field">
            <span>Кількість</span>
            <input
              min="0"
              type="number"
              value={stock}
              onChange={(event) => setStock(Number(event.target.value))}
            />
          </label>
          <div className="summary-total">
            <span>Вартість залишку</span>
            <strong>{(price * stock).toLocaleString('uk-UA')} грн</strong>
          </div>
          <Button className="full-width" type="submit">
            Створити
          </Button>
        </aside>
      </form>
    </section>
  )
}
