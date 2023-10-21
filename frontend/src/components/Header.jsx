import {
    Menu,
    MenuButton,
    MenuList,
    MenuItem,
    IconButton
  } from '@chakra-ui/react'
import { HamburgerIcon } from "@chakra-ui/icons";
import {useNavigate} from 'react-router-dom';


export default function Header(){
    const navigate = useNavigate();
    return(
        <header className="flex flex-row bg-[#3E69A3] p-5 items-center">
            <Menu>
                <MenuButton
                    as={IconButton}
                    aria-label='Options'
                    icon={<HamburgerIcon boxSize={8}/>}
                    variant='none'
                    className='text-white font-bold'
                />
                <MenuList>
                    <MenuItem onClick={()=>navigate("/study")}>
                        Study
                    </MenuItem>
                </MenuList>
            </Menu>
            <a href='/'>
                <h1 className='text-3xl text-white font-extrabold mx-3'>PureLearn</h1>
            </a>
        </header>
    )
}