import React, { useState } from 'react';

export default function Upload() {
    const [files, setFiles] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');

    const validateFiles = () => {
        if (files.length === 0) {
            setErrorMessage('Please select at least one file.');
            return false;
        }

        const validExtensions = ['text/plain', 'application/msword', 'application/pdf'];

        for (let i = 0; i < files.length; i++) {
            if (!validExtensions.includes(files[i].type)) {
                setErrorMessage(`Invalid file format for ${files[i].name}. Please upload TXT, DOC, or PDF files.`);
                return false;
            }
        }

        setErrorMessage('');
        return true;
    };

    const handleFileChange = (event) => {
        setFiles([...event.target.files]);
    };

    const handleSubmit = (event) => {
        event.preventDefault();
        if (validateFiles()) {
            console.log('Files are valid:', files);
            // Handle the valid files here (e.g., upload them to a server)
        }
    };

    return (
        <div>
            <form onSubmit={handleSubmit}>
                <div>
                    <label htmlFor="fileInput">Upload files:</label>
                    <input
                        type="file"
                        id="fileInput"
                        onChange={handleFileChange}
                        accept=".txt, .doc, .pdf"
                        multiple
                    />
                </div>
                <div>
                    {errorMessage && <p style={{ color: 'red' }}>{errorMessage}</p>}
                </div>
                <div>
                    <button type="submit">Submit</button>
                </div>
            </form>
        </div>
    );
}

