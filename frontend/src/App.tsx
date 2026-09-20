import { useState } from "react";
import PdfUpload from "./components/PdfUpload";
import Chat from "./components/Chat";
import "./App.css";

function App() {
  const [isPdfReady, setIsPdfReady] = useState(false);

  function handleUploadSuccess() {
    setIsPdfReady(true);
  }

  return (
    <div className="app-container">
      <h1>AI PDF Chatbot</h1>
      <PdfUpload onUploadSuccess={handleUploadSuccess} />
      <hr />
      <Chat isEnabled={isPdfReady} />
    </div>
  );
}

export default App;