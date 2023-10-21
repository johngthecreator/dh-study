import FlashCards from "../components/flashcards/FlashCards";
import Quiz from "../components/Quiz";
import Header from "../components/Header";
import { Tabs, TabList, TabPanels, Tab, TabPanel } from '@chakra-ui/react'
import Upload from "../components/Upload";
import Quiz from "../components/Quiz";

export default function Study(){
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
                    <p>Threeee</p>
                    </TabPanel>
                </TabPanels>
            </Tabs>

        </div>
    )
}