import { useState } from "react";
import { askQuestion } from "../api/api";

interface Message {
  role: "user" | "bot";
  text: string;
}

interface ChatProps {
  isEnabled: boolean;
}

function Chat({ isEnabled }: ChatProps) {
  const [question, setQuestion] = useState("");
  const [messages, setMessages] = useState<Message[]>([]);
  const [isAsking, setIsAsking] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");

  function handleQuestionChange(event: React.ChangeEvent<HTMLInputElement>) {
    setQuestion(event.target.value);
  }

  async function handleAsk() {
    if (question.trim() === "") {
      return;
    }

    const userMessage: Message = { role: "user", text: question };
    setMessages((previousMessages) => [...previousMessages, userMessage]);
    setQuestion("");
    setIsAsking(true);
    setErrorMessage("");

    try {
      const result = await askQuestion(question);
      const botMessage: Message = { role: "bot", text: result.answer };
      setMessages((previousMessages) => [...previousMessages, botMessage]);
    } catch (error) {
      setErrorMessage("Failed to get an answer. Is the backend running?");
    } finally {
      setIsAsking(false);
    }
  }

  function handleKeyDown(event: React.KeyboardEvent<HTMLInputElement>) {
    if (event.key === "Enter") {
      handleAsk();
    }
  }

  return (
    <div>
      <h2>Ask about your PDF</h2>

      {isEnabled === false && <p>Upload a PDF first to start chatting.</p>}

      <div>
        {messages.map((message, index) => (
          <p key={index}>
            <strong>{message.role === "user" ? "You: " : "Bot: "}</strong>
            {message.text}
          </p>
        ))}
      </div>

      {isAsking && <p>Thinking... (this can take up to a minute on local models)</p>}
      {errorMessage !== "" && <p style={{ color: "red" }}>{errorMessage}</p>}

      <input
        type="text"
        value={question}
        onChange={handleQuestionChange}
        onKeyDown={handleKeyDown}
        placeholder="Ask a question about the PDF..."
        disabled={isEnabled === false || isAsking}
      />
      <button onClick={handleAsk} disabled={isEnabled === false || isAsking}>
        Ask
      </button>
    </div>
  );
}

export default Chat;