import { ChatInbox } from '../features/chats/components/ChatInbox.jsx'

export function ChatsPage({ chats }) {
  return (
    <section className="page-stack">
      <ChatInbox chats={chats} />
    </section>
  )
}
