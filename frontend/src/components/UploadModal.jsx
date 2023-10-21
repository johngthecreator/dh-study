import {
    Modal,
    ModalOverlay,
    ModalContent,
    ModalHeader,
    ModalFooter,
    ModalBody,
    ModalCloseButton,
    useDisclosure,
    Button

  } from '@chakra-ui/react'

export default function UploadModal(props){
    const { isOpen, onOpen, onClose } = useDisclosure()
    return (
    <>
        <button className="bg-[#3E69A3] p-3 rounded-xl text-lg text-white font-bold" onClick={onOpen}>Create a Session</button>
        <Modal isOpen={isOpen} onClose={onClose}>
        <ModalOverlay />
        <ModalContent>
            <ModalHeader>Create a New Session</ModalHeader>
            <ModalCloseButton />
            <ModalBody>
            {props.children}
            </ModalBody>
            <ModalFooter>
            <Button colorScheme='red' mr={3} onClick={onClose}>
                Close
            </Button>
            <Button colorScheme='green'>Submit</Button>
            </ModalFooter>
        </ModalContent>
        </Modal>
    </>
    )
}