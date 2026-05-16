import { ChatInbox } from '../features/chats/components/ChatInbox.jsx'

export function ChatsPage({ chats }) {
  return (
    <section className="page-stack">
      <div className="page-header">
      </div>

      <ChatInbox chats={chats} />
    </section>
  )
}
