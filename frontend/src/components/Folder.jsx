import { useNavigate } from "react-router-dom"
import { sessionIdAtom } from "../atoms/sessionIdAtom";
import { useAtom } from "jotai";

export default function Folder(props){
    const navigate = useNavigate();
    const [ , setSessionId] = useAtom(sessionIdAtom);

    const removeItem = (sid) => {
        setSessionId(sid);
        localStorage.removeItem("flashcards");
        localStorage.removeItem("quiz");
        navigate("/study")
    }
    return(
        <div onClick={()=>removeItem(props.sessionId)} className="h-[175px] w-[300px] flex justify-center items-center border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl">
            <h1 className="font-bold text-lg">{props.name}</h1>
        </div>
    )
}