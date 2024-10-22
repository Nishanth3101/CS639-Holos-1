import { BrowserRouter, Route, Routes } from "react-router-dom";
import LandingPage from "./Holos Landing Page/LandingPage";
import AboutUs from "./About Us Page/AboutUs";
import DemoPage from "./Demo Page/DemoPage";
import NotFoundPage from "./NotFoundPage/NotFoundPage";
import QuizPage from "./QuizPage/QuizPage";
import Forum from "./Forum/Forum"; 
import QuizDashboard from "./QuizPage/QuizDashboard";
import QuizBones from "./QuizPage/QuizBones";
import QuizSkeletal from "./QuizPage/QuizSkeletal";
import QuizTerminology from "./QuizPage/QuizTerminology";
import QuizCardiovascular from "./QuizPage/QuizCardiovascular"; // 新导入的组件
import "./App.css";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<LandingPage />} />
        <Route path="/aboutus" element={<AboutUs />} />
        <Route path="/demopage" element={<DemoPage />} />
        <Route path="/quiz" element={<QuizPage />} />
        <Route path="/quiz-bones" element={<QuizBones />} />  
        <Route path="/quiz-skeletal" element={<QuizSkeletal />} /> 
        <Route path="/quiz-terminology" element={<QuizTerminology />} />  
        <Route path="/quiz-cardiovascular" element={<QuizCardiovascular />} /> 
        <Route path="/quiz-dashboard" element={<QuizDashboard />} />
        <Route path="/forum" element={<Forum />} />
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;