import React, { useState } from 'react';

export default function Chat() {
    const [messages, setMessages] = useState([]);
  
    const handleSendMessage = async (text) => {
      setMessages([...messages, { sender: 'user', text }]);
  
      // Here, you can add logic to send the user's message to a backend and update the state with the bot's reply.
      // For brevity, I've omitted that part. Add it if needed.
  
    }
  
    return (
      <ChatContainer>
        <div className="overflow-y-auto h-80">
          {messages.map((msg, idx) => <Message key={idx} text={msg.text} sender={msg.sender} />)}
        </div>
        <InputArea onSend={handleSendMessage} />
      </ChatContainer>
    );
  }
  
// ChatContainer component
const ChatContainer = ({ children }) => {
  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gray-200">
      <div className="w-11/12 md:w-2/3 lg:w-1/2 xl:w-1/3 bg-white rounded-xl shadow-md p-4">
        {children}
      </div>
    </div>
  );
}

// Message component
const Message = ({ text, sender }) => {
  return (
    <div className={`my-2 w-full ${sender === 'user' ? 'text-right' : ''}`}>
      <div className={`inline-block px-4 py-2 rounded-xl ${sender === 'user' ? 'bg-[#3E69A3] text-white' : 'bg-gray-300 text-gray-900'}`}>
        {text}
      </div>
    </div>
  );
}

// InputArea component
const InputArea = ({ onSend }) => {
  const [input, setInput] = useState('');

  const handleSend = () => {
    if (input.trim()) {
      onSend(input.trim());
      setInput('');
    }
  }

  return (
    <div className="mt-4 border-t pt-2">
      <div className="relative">
        <input 
          type="text" 
          value={input}
          onChange={e => setInput(e.target.value)}
          className="w-full px-4 py-2 border rounded-lg"
          placeholder="Type your message..."
        />
        <button 
          onClick={handleSend}
          className="absolute right-2 top-1/2 transform -translate-y-1/2 bg-[#3E69A3] text-white px-4 py-2 rounded-lg"
        >
          Send
        </button>
      </div>
    </div>
  );
}
