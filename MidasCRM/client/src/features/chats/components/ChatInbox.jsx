import { useState } from 'react'

export function ChatInbox({ chats = [] }) {
  const [selectedChannel] = useState('all')
  const [activeChatId, setActiveChatId] = useState(null)
  const [messageText, setMessageText] = useState('')
  const [sentMessages, setSentMessages] = useState([])
  const [notice, setNotice] = useState('')

  const filteredChats = selectedChannel === 'all' 
    ? chats 
    : chats.filter(chat => chat.channelId === selectedChannel)

  const activeChat = chats.find(chat => chat.id === activeChatId)

  function createSaleFromChat() {
    setNotice('Чернетку продажу підготовлено з поточного діалогу')
  }

  function sendMessage() {
    if (!messageText.trim()) {
      return
    }

    setSentMessages((messages) => [
      ...messages,
      {
        sender: 'manager',
        text: messageText,
        time: new Date().toLocaleTimeString('uk-UA', { hour: '2-digit', minute: '2-digit' }),
      },
    ])
    setMessageText('')
  }

  return (
    <div className="chat-layout">
      <section className="panel chat-threads-panel">
        <h2>Діалоги</h2>
        <div className="threads-list">
          {filteredChats.length === 0 ? (
            <p className="empty-state">Немає активних діалогів</p>
          ) : (
            filteredChats.map((chat) => (
              <button
                key={chat.id}
                className={`thread-item ${activeChatId === chat.id ? 'active' : ''}`}
                type="button"
                onClick={() => setActiveChatId(chat.id)}
              >
                <div className="thread-header">
                  <span className={`platform-badge ${chat.channelId}`}>
                    {chat.channelName}
                  </span>
                  <span className="thread-time">{chat.time}</span>
                </div>
                
                <div className="thread-body">
                  <strong className="customer-name">{chat.customer}</strong>
                  <p className="last-message-preview">{chat.lastMessage}</p>
                </div>

                {chat.marketplaceListing && (
                  <div className="marketplace-context">
                    <small>Оголошення:</small>
                    <span className="listing-title">{chat.marketplaceListing.title}</span>
                  </div>
                )}
              </button>
            ))
          )}
        </div>
      </section>

      <section className="panel chat-window">
        {activeChat ? (
          <>
            <div className="chat-window-header">
              <div>
                <h2>{activeChat.customer}</h2>
                <span className={`platform-info ${activeChat.channelId}`}>
                  {activeChat.channelName} 
                  {activeChat.marketplaceListing && ` • До товару: ${activeChat.marketplaceListing.title}`}
                </span>
              </div>
              <button className="secondary-button" type="button" onClick={createSaleFromChat}>
                Створити продаж
              </button>
            </div>

            {notice && <p className="settings-message">{notice}</p>}

            <div className="message-list">
              {activeChat.messages?.map((msg, index) => (
                <div key={index} className={`message-wrapper ${msg.sender}`}>
                  <p className="message">{msg.text}</p>
                  <small className="message-time">{msg.time}</small>
                </div>
              ))}
              {sentMessages.map((msg, index) => (
                <div key={`${msg.time}-${index}`} className={`message-wrapper ${msg.sender}`}>
                  <p className="message">{msg.text}</p>
                  <small className="message-time">{msg.time}</small>
                </div>
              ))}
            </div>

            <div className="message-composer">
              <input
                placeholder={`Написати відповідь у ${activeChat.channelName}...`}
                value={messageText}
                onChange={(event) => setMessageText(event.target.value)}
              />
              <button className="primary-button" type="button" onClick={sendMessage}>
                Надіслати
              </button>
            </div>
          </>
        ) : (
          <div className="chat-empty-state">
            <p>Оберіть діалог зі списку для початку спілкування</p>
          </div>
        )}
      </section>
    </div>
  )
}
