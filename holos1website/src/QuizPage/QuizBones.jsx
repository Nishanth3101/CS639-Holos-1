import React, { useState } from 'react';
import './QuizDashboard.css';
import NavBar from "../NavBar/NavBar";

const QuizBones = () => {
  const [score, setScore] = useState(0);
  const [submitted, setSubmitted] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState({});

  const questions = [
    { question: "What is the innermost part of the bone called?", options: ["periosteum", "compact bone", "cancellous bone", "bone marrow"], answer: "bone marrow" },
    { question: "Which of the following is NOT a type of muscle?", options: ["smooth muscle", "rough muscle", "cardiac muscle", "skeletal muscle"], answer: "rough muscle" },
    { question: "Which of the following cushions and protects the bones where they meet?", options: ["ligaments", "tendons", "cartilage", "muscle"], answer: "cartilage" },
    { question: "True or false: The bones of your skeleton are alive.", options: ["True", "False"], answer: "True" },
    { question: "What can osteoporosis cause?", options: ["Fragile bones", "Strong bones", "Soft bones"], answer: "Fragile bones" },
    { question: "Which vitamin has a major role in bone health?", options: ["Vitamin C", "Vitamin D", "Vitamin B"], answer: "Vitamin D" },
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
      <h1>Quiz Bones</h1>
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

export default QuizBones;