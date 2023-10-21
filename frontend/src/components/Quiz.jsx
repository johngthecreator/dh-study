import { useEffect, useState } from 'react';
import { data } from './question';

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

    return(
        <div>
            {data.map((resp,i)=>{
                return(
                    <MultipleChoice index={i} question={resp.question} choices={resp.options} answer={resp.answer}/>
                )
            })}
        </div>
    )
}