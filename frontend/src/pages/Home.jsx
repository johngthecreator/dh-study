import Header from "../components/Header";

export default function Home(){
    return(
        <div className="h-screen w-full">
            <Header/>
            <div className="bg-[#BBCDE5] flex flex-col md:flex-row items-center text-[40px] font-bold gap-10 p-5">
                <img src="./uploadcard.png" />
                <div className="flex flex-col">
                    <h2>Upload a file</h2>
                    <h2>to get started!</h2>
                </div>
            </div>
            <div className="p-5">
                <h2>Recent Sessions</h2>

            </div>
            


        </div>
    )
}