import { useEffect, useState } from 'react';
import { data } from './question';
import { sessionIdAtom } from '../atoms/sessionIdAtom';
import { useAtom } from 'jotai';
import { quizAtom } from '../atoms/quizAtom';
import { LineWobble } from "@uiball/loaders"
import axios from 'axios';
import { uuidAtom } from '../atoms/uuidAtom';

function MultipleChoice(props){
    const [userAnswer, setUserAnswer] = useState(null);
    const [isCorrect, setIsCorrect] = useState(null)
    useEffect(()=>{
        if(userAnswer){
            if(userAnswer == props.answer){
                setIsCorrect(true);
            }
        }

    },[userAnswer])

    return(
        <div className="flex items-center justify-center p-20">
            <div className="flex flex-col items-center max-w-xl w-full">
                <h2 className="text-l mb-2">Question {props.index + 1}</h2>
                <h2 className="text-2xl mb-6">{props.question}</h2>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 w-full bg-white">
                    {props.choices.map((choice, i)=>{
                        return(
                            <button
                            key={i}
                            className={`${isCorrect ? 'bg-green-200' : 'bg-[#F0F4F9]'} text-[#222222] border border-transparent hover:border-[#3E69A3] py-2 px-4 rounded shadow-md`}
                            onClick={()=>setUserAnswer(choice)}
                            >
                                {choice}
                            </button>
                        )
                    })}
                </div>
            </div>
        </div>
    )
}

export default function Quiz(){
    const [sessionId, ] = useAtom(sessionIdAtom);
    const [quiz, setQuiz] = useAtom(quizAtom);
    const [uuid, ] = useAtom(uuidAtom);
    useEffect(()=>{
        if(sessionId && !quiz){
            axios.post(`https://purelearnmono.azurewebsites.net/AiTools/createMultipleChioce?studySessionId=${sessionId}`,{},{
                headers:{
                    'Authorization': `Bearer ${uuid}`
                }
            })
            .then(resp=>{
                setQuiz(resp.data);
            })
            .catch(e=>console.error(e))
        }

    },[sessionId])
    if(quiz){
        return(
            <div>
                {quiz.questions.map((resp,i)=>{
                    return(
                        <MultipleChoice key={i} index={i} question={resp.question} choices={resp.options} answer={resp.answer}/>
                    )
                })}
            </div>
        )
    }
    return(
        <div className="flex flex-col h-screen justify-center items-center gap-5">
            <LineWobble size={175} color="#446DF6" />
        </div>
    )

}