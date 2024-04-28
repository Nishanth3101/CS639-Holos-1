import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import QuizPage from './QuizPage';
import QuizDashboard from './QuizDashboard'; 

const App = () => {
  return (
    <Router>
      <Routes>
        <Route exact path="/" element={<QuizPage />} />
        <Route path="/quiz-dashboard" element={<QuizDashboard />} />
      </Routes>
    </Router>
  );
};

export default App;
