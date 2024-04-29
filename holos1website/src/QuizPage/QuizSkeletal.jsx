import React, { useState } from 'react';
import './QuizDashboard.css';
import NavBar from "../NavBar/NavBar";

const QuizSkeletal = () => {
  const [score, setScore] = useState(0);
  const [submitted, setSubmitted] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState({});

  const questions = [
    { question: "What is the primary function of the skeletal system?", options: ["Movement", "Storage of minerals", "Blood cell production", "All of the above"], answer: "All of the above" },
    { question: "Which bone protects the brain?", options: ["Skull", "Spine", "Femur", "Ribcage"], answer: "Skull" },
    { question: "How many bones are in the adult human body?", options: ["206", "305", "412", "126"], answer: "206" },
    { question: "What type of joint is the elbow?", options: ["Hinge", "Saddle", "Pivot", "Ball and socket"], answer: "Hinge" },
    { question: "Which condition is characterized by a decrease in bone density?", options: ["Arthritis", "Osteoporosis", "Scoliosis", "Bursitis"], answer: "Osteoporosis" }
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
      <h1>Quiz Skeletal</h1>
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

export default QuizSkeletal;