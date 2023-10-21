import {
    Menu,
    MenuButton,
    MenuList,
    MenuItem,
    MenuItemOption,
    MenuGroup,
    MenuOptionGroup,
    MenuDivider,
    IconButton
  } from '@chakra-ui/react'
import { AddIcon, HamburgerIcon, RepeatIcon, EditIcon, ExternalLinkIcon } from "@chakra-ui/icons";

export default function Header(){
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
                    <MenuItem>
                    New Tab
                    </MenuItem>
                    <MenuItem icon={<ExternalLinkIcon />} command='⌘N'>
                    New Window
                    </MenuItem>
                    <MenuItem icon={<RepeatIcon />} command='⌘⇧N'>
                    Open Closed Tab
                    </MenuItem>
                    <MenuItem icon={<EditIcon />} command='⌘O'>
                    Open File...
                    </MenuItem>
                </MenuList>
            </Menu>
            <h1 className='text-3xl text-white font-extrabold mx-3'>PureLearn</h1>
        </header>
    )
}