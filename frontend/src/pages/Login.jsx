import {signInWithPopup, GoogleAuthProvider } from "firebase/auth";
import { auth } from "../firebase";
import axios from "axios";
import { useAtom } from 'jotai';
import { uuidAtom } from "../atoms/uuidAtom";
import { useNavigate } from "react-router-dom";

export default function Login(){
    const [, setUuid] = useAtom(uuidAtom);
    const navigate = useNavigate();
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
            if (uidToken) {
                axios.post("https://purelearnmono.azurewebsites.net/test",{}, {headers:{
                    'Authorization': `Bearer ${uidToken}`
                }})
                .then((resp)=>{
                    console.log(resp);
                    setUuid(uidToken);
                    navigate("/");
                })
                .catch((e)=>{console.error(e)})
            }
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
            <div className="h-2/3 w-full lg:h-screen lg:w-1/2 flex flex-col justify-center items-start gap-5 p-20">
                <div className="flex flex-row items-center">
                    <div>
                        <h2 className="text-[50px] font-bold">Meet the best AI</h2>
                        <h2 className="text-[50px] font-bold"><span className="text-blue-600">Octo-Learning</span> Platform.</h2>
                    </div>
                </div>
                <button className="p-3 w-[250px] bg-blue-600 text-2xl font-bold text-white rounded-lg" onClick={GoogleSignIn}>Sign in with Google</button>
            </div>
            <div className="flex overflow-hidden h-1/3 w-full lg:h-screen lg:w-1/2">
                <img src="./octo_study2.png" className="w-full" />
            </div>

        </div>
    )

}