// Import the functions you need from the SDKs you need
import { initializeApp } from "firebase/app";
import { getAnalytics } from "firebase/analytics";
import {getAuth} from "firebase/auth";
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
// For Firebase JS SDK v7.20.0 and later, measurementId is optional
const firebaseConfig = {
  apiKey: "AIzaSyBcm1VDAnOKsgPP-qT9_k2HMU1V9FjG8DQ",
  authDomain: "purelearn-76a09.firebaseapp.com",
  projectId: "purelearn-76a09",
  storageBucket: "purelearn-76a09.appspot.com",
  messagingSenderId: "111102107696",
  appId: "1:111102107696:web:2b83c080e84d6b30d48df5",
  measurementId: "G-1CDG4WH7BX"
};

// Initialize Firebase
const app = initializeApp(firebaseConfig);
const analytics = getAnalytics(app);
const auth = getAuth(app)
export {auth};