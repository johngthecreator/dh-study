import { CloseButton } from '@chakra-ui/react'

export default function FileDisplay(props) {
    return (
        <div className="flex flex-col justify-center items-center h-[150px] w-[100px] border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl">
            <img className="w-[40px]" src="./fileIcon.png" alt="fileIcon" />
            <h2>{props.fileName}</h2>
            <CloseButton onClick={props.func}></CloseButton>
        </div>
    )
}   