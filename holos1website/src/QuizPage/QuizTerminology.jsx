import React, { useState } from 'react';
import './QuizDashboard.css';
import NavBar from "../NavBar/NavBar";

const QuizTerminology = () => {
  const [score, setScore] = useState(0);
  const [submitted, setSubmitted] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState({});

  const questions = [
    { question: "What term is used to describe the study of bones?", options: ["Cardiology", "Osteology", "Dermatology", "Neurology"], answer: "Osteology" },
    { question: "Which term refers to the end of a long bone?", options: ["Diaphysis", "Epiphysis", "Periosteum", "Metaphysis"], answer: "Epiphysis" },
    { question: "What is the term for new bone formation?", options: ["Osteoclasty", "Osteoplasty", "Osteogenesis", "Osteopathy"], answer: "Osteogenesis" },
    { question: "What is the medical term for bone pain?", options: ["Arthralgia", "Myalgia", "Ostealgia", "Neuralgia"], answer: "Ostealgia" },
    { question: "What term describes the surgical repair or replacement of a joint?", options: ["Arthroscopy", "Arthroplasty", "Arthrotomy", "Arthrodesis"], answer: "Arthroplasty" }
  ];

  const handleOptionChange = (questionIndex, option) => {
    setSelectedOptions({
      ...selectedOptions,
      [questionIndex]: option
    });
  };

  const handleSubmit = (event) => {
    event.preventDefault();
    let points = 0;
    questions.forEach((question, index) => {
      if (selectedOptions[index] === question.answer) {
        points += 1;
      }
    });
    setScore(points);
    setSubmitted(true);
  };

  return (
    <div className="quiz-dashboard">
      <NavBar />
      <h1>Quiz Terminology</h1>
      <form onSubmit={handleSubmit}>
        {questions.map((question, index) => (
          <div key={index} className="question-card">
            <p>{question.question}</p>
            {question.options.map((option, idx) => (
              <label key={idx} className={`option-card ${selectedOptions[index] === option ? 'selected' : ''}`}>
                <input
                  type="radio"
                  name={`question${index}`}
                  value={option}
                  onChange={() => handleOptionChange(index, option)}
                  checked={selectedOptions[index] === option}
                />
                {option}
              </label>
            ))}
          </div>
        ))}
        <button type="submit">Submit</button>
      </form>
      {submitted && <p>Your score is: {score}/{questions.length}</p>}
    </div>
  );
};

export default QuizTerminology;