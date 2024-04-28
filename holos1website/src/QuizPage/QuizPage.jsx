import React from 'react';
import { useNavigate } from 'react-router-dom';
import './QuizPage.css';
import Navbar from '../NavBar/NavBar';

const QuestionBank = () => {
  let navigate = useNavigate();

  const navigateToQuizDashboard = () => {
    navigate("/quiz-dashboard");
  };

  return (
    <div className="question-bank">
      <h1>Question Bank</h1>
      <p>This is the question bank where all questions can be stored and managed.</p>
      <button onClick={navigateToQuizDashboard} className="quiz-content-button">View Problem Library</button>
    </div>
  );
};

const BasicQuiz = ({ title }) => {
  return (
    <div className="basic-quiz">
      <div className="quiz-title">{title}</div>
    </div>
  );
};

const BasicQuizzes = () => {
  const quizTitles = ['Bones', 'Skeletal and Muscular System', 'Terminology', 'One More'];
  return (
    <div className="basic-quizzes">
      <h1>Basic Quizzes</h1>
      <div className="basic-quizzes-container">
        {quizTitles.map((title, index) => (
          <BasicQuiz key={index} title={title} />
        ))}
      </div>
    </div>
  );
};

function QuizPage() {
  return (
    <>
      <Navbar />
      <div className="content-container">
        <QuestionBank />
        <BasicQuizzes />
      </div>
    </>
  );
}

export default QuizPage;
