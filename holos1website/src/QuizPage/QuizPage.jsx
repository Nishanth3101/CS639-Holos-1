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

const BasicQuiz = ({ title, onClick }) => {
  return (
    <button onClick={onClick} className="basic-quiz">
      <div className="quiz-title">{title}</div>
    </button>
  );
};

const BasicQuizzes = () => {
  const navigate = useNavigate();
  const quizTitles = [
    { title: 'Bones', path: '/quiz-bones' },
    { title: 'Skeletal and Muscular System', path: '/quiz-skeletal' },
    { title: 'Terminology', path: '/quiz-terminology' },
    { title: 'One More', path: '/quiz-one-more' }  // 假设你有一个额外的路径
  ];

  const handleNavigate = (path) => {
    navigate(path);
  };

  return (
    <div className="basic-quizzes">
      <h1>Basic Quizzes</h1>
      <div className="basic-quizzes-container">
        {quizTitles.map((quiz, index) => (
          <BasicQuiz key={index} title={quiz.title} onClick={() => handleNavigate(quiz.path)} />
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