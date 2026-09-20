import { useState } from "react";
import { uploadPdf, type UploadResponse } from "../api/api";

interface PdfUploadProps {
  onUploadSuccess: () => void;
}

function PdfUpload({ onUploadSuccess }: PdfUploadProps) {
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadResult, setUploadResult] = useState<UploadResponse | null>(null);
  const [errorMessage, setErrorMessage] = useState("");

  function handleFileChange(event: React.ChangeEvent<HTMLInputElement>) {
    const file = event.target.files ? event.target.files[0] : null;
    setSelectedFile(file);
    setErrorMessage("");
    setUploadResult(null);
  }

  async function handleUpload() {
    if (selectedFile === null) {
      setErrorMessage("Please select a PDF file first.");
      return;
    }

    setIsUploading(true);
    setErrorMessage("");

    try {
      const result = await uploadPdf(selectedFile);
      setUploadResult(result);
      onUploadSuccess();
    } catch (error) {
      setErrorMessage("Upload failed. Is the backend running?");
    } finally {
      setIsUploading(false);
    }
  }

  return (
    <div>
      <h2>Upload a PDF</h2>
      <input type="file" accept="application/pdf" onChange={handleFileChange} />
      <button onClick={handleUpload} disabled={isUploading}>
        {isUploading ? "Processing... (this can take a few minutes)" : "Upload"}
      </button>

      {errorMessage !== "" && <p style={{ color: "red" }}>{errorMessage}</p>}

      {uploadResult !== null && (
        <p>
          Uploaded <strong>{uploadResult.fileName}</strong> — {uploadResult.pageCount} pages,{" "}
          {uploadResult.totalChunks} chunks stored.
        </p>
      )}
    </div>
  );
}

export default PdfUpload;