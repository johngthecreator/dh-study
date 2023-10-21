import { CloseButton } from '@chakra-ui/react'

export default function FileDisplay(props) {
    return (
        <div className="flex flex-row justify-between items-center p-3 border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl">
            <div className='flex flex-row items-center'>
                <img className="w-[40px]" src="./fileIcon.png" alt="fileIcon" />
                <h2 className='truncate w-[100px]'>{props.fileName}</h2>
            </div>
            <CloseButton onClick={props.func}></CloseButton>
        </div>
    )
}   