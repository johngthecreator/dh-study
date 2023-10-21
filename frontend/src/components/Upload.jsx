import React, { useState } from 'react';
import Header from './Header';
import FileDisplay from './FileDisplay';
import UploadModal from './UploadModal';
import axios from 'axios';
import { useAtom } from 'jotai';
import { uuidAtom } from '../atoms/uuidAtom';
import { sessionIdAtom } from '../atoms/sessionIdAtom';
import { useNavigate } from 'react-router-dom';

export default function Upload() {
    const [fileNames, setFileNames] = useState([]);
    const [files, setFiles] = useState([]);
    const [errorMessage, setErrorMessage] = useState('');
    const [sessionName, setSessionName] = useState('');
    const [uuid, ] = useAtom(uuidAtom);
    const [ , setSessionId] = useAtom(sessionIdAtom);
    const navigate = useNavigate();

    const removeFile = (i) => {
        const newItems = fileNames.filter((file, idx) => idx !== i);
        setFileNames(newItems);
    }

    const handleFileChange = (event) => {
        const selectedFiles = [...event.target.files];
        setFiles(selectedFiles);
        
        const names = selectedFiles.map(file => file.name);
        setFileNames(names);
    };

    const validateFiles = () => {
        if (files.length === 0) {
            setErrorMessage('Please select at least one file.');
            return false;
        }

        // const validExtensions = ['text/plain', 'application/msword', 'application/pdf'];
        const validExtensions = ['.pdf', '.docx', '.txt'];

        for (let i = 0; i < files.length; i++) {
            const ext = '.' + files[i].name.split('.').pop()
            if (!validExtensions.includes(ext)) {
                setErrorMessage(`Invalid file format for ${files[i].name}. Please upload TXT, DOC, or PDF files.`);
                return false;
            }
        }

        setErrorMessage('');
        return true;
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        const formData = new FormData();
        formData.append('sessionName', sessionName);
        for (let i = 0; i < files.length; i++) {
            formData.append('files', files[i]);
        }

        if (validateFiles()) {
            axios.post("https://purelearnmono.azurewebsites.net/StudySession/makesession", formData, {
                headers:{
                    'Content-Type': 'multipart/form-data',
                    'Authorization': `Bearer ${uuid}`

            }})
            .then(resp=>{
                localStorage.removeItem('flashcards');
                setSessionId(resp.data);
                navigate("/study");
            })
            .catch(e=>console.error(e));
            // Handle the valid files here (e.g., upload them to a server)
        }
    };

    return (
        <UploadModal>
        <div className='p-5'>
            <div  className='flex justify-center'>
            <form onSubmit={handleSubmit} className='flex flex-col gap-5'>
                <input type='text' onChange={e=>setSessionName(e.target.value)} className='border-solid border-2 border-black p-2 rounded-lg' placeholder='Session Name'/>
                <div className='flex justify-center'>
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
                { (fileNames.length > 0) ? (
                    <div className="flex flex-col  overflow-y-scroll h-[200px] py-5 gap-5">
                        {fileNames.map((name, index) => (
                            <FileDisplay key={index} fileName={name} func={()=>removeFile(index)} />
                        ))}
                    </div>

                ):(
                    <div className='flex justify-center py-3'>
                        <h2 className='font-bold text-2xl'>Upload some files!</h2>
                    </div>
                )
                }
                <div className='flex gap-10 justify-center mt-5'>
                    <button onClick={handleSubmit} className='bg-[#F0F4F9] px-10 py-2 rounded text-[#22222] border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl'>Let's Study!</button>
                </div>
            </form>
        </div>
        </div>
    </UploadModal>
    );
}

