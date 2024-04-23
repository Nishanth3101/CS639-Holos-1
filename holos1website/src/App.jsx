import { BrowserRouter, Route, Routes } from "react-router-dom";
import LandingPage from "./Holos Landing Page/LandingPage";
import AboutUs from "./About Us Page/AboutUs";
import DemoPage from "./Demo Page/DemoPage";
import NotFoundPage from "./NotFoundPage/NotFoundPage";
import QuizPage from "./QuizPage/QuizPage"; 
import "./App.css";

function App() {

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/aboutus" element={<AboutUs />} />
        <Route path="/demopage" element={<DemoPage />} />
        <Route path="/quiz" element={<QuizPage />} /> 
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;