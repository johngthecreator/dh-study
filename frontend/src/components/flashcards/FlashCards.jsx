
import "./flashcard.css"
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
    return(
        <div className="flex flex-col items-center gap-5">
            {}
            <FlashCard front="Term" back="definition"/>
        </div>
    )
}