import Folder from "../components/Folder";
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
                <h2 className="text-xl font-bold text-[#222]">Recent Sessions</h2>
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
            </div>
            <div className="p-5">
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
            </div>
            


        </div>
    )
}