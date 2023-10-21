import { useEffect, useState } from "react";
import Folder from "../components/Folder";
import Header from "../components/Header";
import Upload from "../components/Upload";
import axios from "axios";
import { uuidAtom } from "../atoms/uuidAtom";
import { useAtom } from "jotai";
import { useNavigate } from "react-router-dom";

export default function Home(){
    const [recentSessions, setRecentSessions] = useState([]);
    const [uuid, ] = useAtom(uuidAtom);
    const navigate = useNavigate();

    useEffect(()=>{
        if(!uuid){
            navigate("/login");
        }else{
            navigate("/");
        }

        if(uuid){
            axios.post("https://purelearnmono.azurewebsites.net/StudySession/getsessions",{},{
                headers:{
                    'Authorization': `Bearer ${uuid}`
                }
            })
            .then(resp=>{
                let uuidSessions = []
                for(let i in resp.data){
                    if(resp.data[i].userId == uuid){
                        uuidSessions.push(resp.data[i]);
                    }
                }
                setRecentSessions(uuidSessions);
            })
            .catch(e=>console.error(e));
        }
    }, [])
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
                {
                    (recentSessions.length > 0) ? (
                        <div className="flex flex-row overflow-x-scroll py-5 gap-5">
                            {recentSessions.map((session, index)=>{
                                if(session.userId == uuid){
                                    return(
                                        <Folder key={`${session.name}${index}`} sessionId={session.id} name={session.name} />
                                    )
                                }
                            })}
                        </div>
                    ):(
                        <div className="flex flex-col lg:flex-row items-center p-5">
                            <div className="text-3xl font-bold">
                                <h1>Looks like you don't have</h1>
                                <h1>any recent sessions.</h1>
                            </div>
                            <img src="./octo_pose_clear.png" className="h-[300px]"/>

                        </div>
                    )

                }
            </div>
            {/* <div className="p-5">
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
            </div> */}
            


        </div>
    )
}