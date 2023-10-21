import {signInWithPopup, GoogleAuthProvider } from "firebase/auth";
import { auth } from "../firebase";
import axios from "axios";
import { useAtom } from 'jotai';
import { uuidAtom } from "../atoms/uuidAtom";

export default function Login(){
    const [, setUuid] = useAtom(uuidAtom);
    const GoogleSignIn = () => {
        const provider = new GoogleAuthProvider();
        signInWithPopup(auth, provider)
        .then(async (result) => {
            // This gives you a Google Access Token. You can use it to access the Google API.
            const credential = GoogleAuthProvider.credentialFromResult(result);
            const token = credential.idToken;
            // The signed-in user info.
            const user = result.user;

            // const uid = user.uid
            // const name = user.displayName
            const uidToken = await user.getIdToken();
            if (token) {
                axios.post("https://purelearnmono.azurewebsites.net/test",{}, {headers:{
                    'Authorization': `Bearer ${uidToken}`
                }})
                .then((resp)=>{
                    const uuidDirty = resp.data.id
                    const uuid = uuidDirty.split(":")[1]
                    setUuid(uuid);
                })
                .catch((e)=>{console.log(e)})
            }
            // if (!users.includes(uid)){
            //     addUser(name, uid, email)
            //     console.log("new user added")
            // }
            // localStorage.setItem("user",JSON.stringify({"uid":uid, "name":name, "email":email}))
            // navigate("/listings");
            // ...
        }).catch((error) => {
            // Handle Errors here.
            const errorCode = error.code;
            const errorMessage = error.message;
            // The email of the user's account used.
            // const email = error.customData.email;
            // The AuthCredential type that was used.
            const credential = GoogleAuthProvider.credentialFromError(error);
            console.log(errorMessage)
            // ...
        });
    }
    return(
        <div className="flex flex-col md:flex-row overflow-hidden">
            <div className="h-[300px] w-full lg:h-screen lg:w-1/2 flex flex-col justify-center items-center gap-5">
                <h2 className="text-[40px] font-bold">Log into <span className="text-[#3E69A3]">PureLearn</span></h2>
                <button className="px-3 py-2 bg-blue-300 text-2xl font-bold rounded-lg" onClick={GoogleSignIn}>Login with Google</button>
            </div>
            <div className="overflow-hidden h-1/2 w-full lg:h-screen lg:w-1/2">
                <img src="https://images.unsplash.com/photo-1593698054469-2bb6fdf4b512?auto=format&fit=crop&q=80&w=1974&ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D" />
            </div>

        </div>
    )

}