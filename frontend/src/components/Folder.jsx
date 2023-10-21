export default function Folder(props){
    return(
        <div className="h-[175px] w-[300px] flex justify-center items-center border-solid border-2 border-[#3E69A3] shrink-0 grow-0 rounded-xl">
            <h1 className="font-bold text-lg">{props.name}</h1>
        </div>
    )
}