import FlashCards from "../components/flashcards/FlashCards";
import Header from "../components/Header";
import { Tabs, TabList, TabPanels, Tab, TabPanel } from '@chakra-ui/react'

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
                    <p>two!</p>
                    </TabPanel>
                    <TabPanel>
                    <p>Threeee</p>
                    </TabPanel>
                </TabPanels>
            </Tabs>

        </div>
    )
}