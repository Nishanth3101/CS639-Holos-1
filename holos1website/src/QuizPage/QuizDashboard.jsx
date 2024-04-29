import React from 'react';
import './QuizDashboard.css';
import NavBar from "../NavBar/NavBar";

const DashboardItem = ({ title, description, onClick }) => (
  <div className="dashboard-item" onClick={onClick}>
    <h2>{title}</h2>
    <p>{description}</p>
  </div>
);

const QuizDashboard = () => {
  return (
    <div className="quiz-dashboard">
      <NavBar />
      <h1>Quiz Launchpad 🚀</h1>
      <div className="dashboard-container">
        <DashboardItem
          title="Question packs"
          description="Start a quiz using one of our collections of questions"
          onClick={() => console.log("Navigate to Question Packs")}
        />
        <DashboardItem
          title="Mock exams"
          description="Sit a timed mock exam"
          onClick={() => console.log("Navigate to Mock Exams")}
        />
        <DashboardItem
          title="Create a quiz"
          description="Create and share your own quizzes"
          onClick={() => console.log("Navigate to Create a Quiz")}
        />
        <DashboardItem
          title="Quizzes taken"
          description="See a list of all quizzes you've taken"
          onClick={() => console.log("Navigate to Quizzes Taken")}
        />
      </div>
    </div>
  );
};

export default QuizDashboard;
