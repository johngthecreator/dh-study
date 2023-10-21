
import { useEffect, useState } from "react"
import "./flashcard.css"
import { sessionIdAtom } from "../../atoms/sessionIdAtom"
import { flashcardsAtom } from "../../atoms/flashcardsAtom"
import { useAtom } from "jotai"
import { LineWobble } from "@uiball/loaders"
import axios from "axios"

function FlashCard(props){
    return(
        <div id="flip-card">
            <div id="flip-card-inner">
                <div id="flip-card-front">
                    <p><strong>{props.front}</strong></p>
                </div>
                <div id="flip-card-back">
                    <p><strong>{props.back}</strong></p>
                </div>
            </div>
        </div>
    )
}

export default function FlashCards(){
    const [sessionId, ] = useAtom(sessionIdAtom);
    const [flashcards, setFlashcards] = useAtom(flashcardsAtom);
    useEffect(()=>{
        if(sessionId && !flashcards){
            axios.post(`https://purelearnmono.azurewebsites.net/AiTools/createFlashcards?studySessionId=${sessionId}`)
            .then(resp=>{
                setFlashcards(resp.data);
            })
            .catch(e=>console.error(e))
        }

    },[sessionId])
    if(flashcards){
        return(
            <div className="flex flex-col items-center gap-5">
                {Object.keys(flashcards).map(keh => {
                    return(
                        <FlashCard key={`${keh}${flashcards[keh]}`} front={keh} back={flashcards[keh]}/>
                    )
                    })
                }
            </div>
        )
    }

    return(
        <div className="flex flex-col h-screen justify-center items-center gap-5">
            <LineWobble size={175} color="#446DF6" />
        </div>
        
        
    )

}