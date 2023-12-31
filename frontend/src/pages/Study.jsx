import FlashCards from "../components/flashcards/FlashCards";
import Quiz from "../components/Quiz";
import Header from "../components/Header";
import { Tabs, TabList, TabPanels, Tab, TabPanel } from '@chakra-ui/react'
import Chat from "../components/Chat";
import { useEffect } from "react";
import { useAtom } from 'jotai';
import { idAtom } from "../atoms/idAtom";
import { useNavigate } from "react-router-dom";

export default function Study(){
    const navigate = useNavigate();
    const [id, ] = useAtom(idAtom);
    useEffect(()=>{
        if(!id.length > 1){
            navigate("/login")
        }
    })
    return(
        <div>
            <Header />
            <Tabs variant='soft-rounded' className="flex m-5">
                <TabList className="self-center">
                    <Tab>Flashcards</Tab>
                    <Tab>Quizzes</Tab>
                    <Tab>Chat</Tab>
                </TabList>
                <TabPanels>
                    <TabPanel>
                        <FlashCards />
                    </TabPanel>
                    <TabPanel>
                        <Quiz />
                    </TabPanel>
                    <TabPanel>
                        <Chat />
                    </TabPanel>
                </TabPanels>
            </Tabs>

        </div>
    )
}