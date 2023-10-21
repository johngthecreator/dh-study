import { useEffect, useState } from "react";
import Folder from "../components/Folder";
import Header from "../components/Header";
import Upload from "../components/Upload";
import UploadModal from "../components/UploadModal";
import axios from "axios";
import { uuidAtom } from "../atoms/uuidAtom";
import { useAtom } from "jotai";

export default function Home(){
    const [recentSessions, setRecentSessions] = useState([]);
    const [uuid, ] = useAtom(uuidAtom);
    // const uuid = "matthew_dev"
    useEffect(()=>{
        axios.post("https://purelearnmono.azurewebsites.net/StudySession/getsessions",{},{})
        .then(resp=>setRecentSessions(resp.data))
        .catch(e=>console.error(e));
    })
    return(
        <div className="h-screen w-full">
            <Header/>
            <div className="bg-[#BBCDE5] flex flex-col md:flex-row items-center text-[40px] font-bold gap-10 p-5">
                <img src="./files.png" />
                <div className="flex flex-col gap-5 text-[50px]">
                    <div>
                        <h2>Create a session</h2>
                        <h2>to get started!</h2>
                        <Upload />
                    </div>
                </div>
            </div>
            <div className="p-5">
                <h2 className="text-xl font-bold text-[#222]">Recent Sessions</h2>
                <div className="flex flex-row overflow-x-scroll py-5 gap-5">
                    {recentSessions.map((session)=>{
                        if(session.userId == uuid){
                            return(
                                <Folder key={session.name} name={session.name} />
                            )
                        }
                    })}
                </div>
            </div>
            <div className="p-5">
                <h2 className="text-xl font-bold text-[#222]">Subjects</h2>
                <div className="flex flex-row overflow-x-scroll py-5 gap-5">
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                    <Folder />
                </div>
            </div>
            


        </div>
    )
}