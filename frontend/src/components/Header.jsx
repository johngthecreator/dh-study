import { Drawer, DrawerBody, DrawerOverlay, DrawerContent, useDisclosure, IconButton } from '@chakra-ui/react'
import { HamburgerIcon } from "@chakra-ui/icons";
import {useNavigate} from 'react-router-dom';
import { useRef } from 'react';


export default function Header(){
    const navigate = useNavigate();
    const btnRef = useRef();
    const { isOpen, onOpen, onClose } = useDisclosure();
    return(
        // <header className="flex flex-row bg-[#3E69A3] p-5 items-center">
        <header className="flex flex-row bg-blue-600 p-5 items-center">
            <div className='flex flex-row gap-5 items-center'>
            {/* <IconButton
            colorScheme="dark"
            aria-label='menu button'
            icon={<HamburgerIcon boxSize={8}/>}
            ref={btnRef}
            onClick={onOpen}
            /> */}
            </div>
            <Drawer
            isOpen={isOpen}
            placement='left'
            onClose={onClose}
            finalFocusRef={btnRef}
            >
            <DrawerOverlay />
            <DrawerContent
            maxW="200px"
            backgroundColor="white">
                <DrawerBody>
                <div className="grid grid-columns-1 w-full">
                    <button className="text-center font-extrabold text-[#3E69A3] duration-200 text-xl px-5 py-2" onClick={()=>{navigate("/")}} >Home</button>
                    <button className="text-center font-extrabold text-[#3E69A3] duration-200 text-xl px-5 py-2" onClick={()=>{navigate("/study")}} >Study</button>
                </div>
                </DrawerBody>
            </DrawerContent>
            </Drawer>
            <a href='/'>
                <div className='flex flex-row items-center'>
                    <h1 className='text-3xl text-white font-extrabold ml-3'>OctoLearn</h1>
                    <img src='./octo_logo_clear.png' className='h-[50px]'/>
                </div>
            </a>
        </header>
    )
}