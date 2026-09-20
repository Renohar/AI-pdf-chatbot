Backend (.NET + Semantic Kernel): PDF text extraction (PdfPig), custom chunking with overlap, embedding generation via Ollama's nomic-embed-text, vector storage/search in Qdrant, and grounded chat completion via llama3.2:1b
Frontend (React + TypeScript): a typed API layer, an upload component with proper loading/error states, and a chat interface — all wired together with CORS

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) (v18+)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Ollama](https://ollama.com/download)

### 1. Pull the required Ollama models
```bash
ollama pull nomic-embed-text
ollama pull llama3.2:1b
```

### 2. Start Qdrant (vector database)
```bash
docker run -p 6333:6333 -p 6334:6334 -v qdrant_storage:/qdrant/storage qdrant/qdrant
```
Verify it's running at [http://localhost:6333/dashboard](http://localhost:6333/dashboard)

### 3. Start the backend (.NET API)
```bash
cd backend
dotnet run
```
The API will start at a URL printed in the console (e.g. `http://localhost:5130`). Swagger docs available at `<that-url>/swagger`.

### 4. Start the frontend (React)
```bash
cd frontend
npm install
npm run dev
```
Open [http://localhost:5173](http://localhost:5173)

### 5. Use the app
1. Upload a PDF and wait for processing (larger PDFs take longer — embedding runs locally)
2. Once uploaded, ask questions about the document in the chat box

> **Note:** All three services (Ollama, Qdrant, and the .NET backend) must be running simultaneously for the app to work. The frontend alone will show connection errors if the backend or Qdrant isn't running.