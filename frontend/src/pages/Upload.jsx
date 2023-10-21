import React, { useState } from 'react';
import Header from '../components/Header';

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
            <Header/>
            <div  className='flex justify-center'>
            <form onSubmit={handleSubmit}>
                <div className='flex h-[175px] w-[300px] justify-center'>
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

                </div>
                <div className='flex gap-10 justify-center'>
                    <button className='bg-[#F0F4F9] px-10 py-2 rounded text-[#22222] border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl'>Flashcards</button>
                    <button className='bg-[#F0F4F9] px-10 py-2 rounded text-[#22222] border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl'>Chat</button>    
                    <button className='bg-[#F0F4F9] px-10 py-2 rounded text-[#22222] border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl'>Quiz</button>
                </div>
            </form>
        </div>
        </div>
    );
}

