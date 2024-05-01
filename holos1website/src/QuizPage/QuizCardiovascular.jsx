import React, { useState } from 'react';
import './QuizDashboard.css';
import NavBar from "../NavBar/NavBar";

const QuizCardiovascular = () => {
  const [score, setScore] = useState(0);
  const [submitted, setSubmitted] = useState(false);
  const [selectedOptions, setSelectedOptions] = useState({});

  const questions = [
    { question: "What is the main function of the cardiovascular system?", options: ["To transport nutrients", "To remove wastes", "To carry oxygen", "All of the above"], answer: "All of the above" },
    { question: "Which organ is responsible for pumping blood throughout the body?", options: ["Liver", "Heart", "Lung", "Kidney"], answer: "Heart" },
    { question: "What type of blood vessel carries blood away from the heart?", options: ["Vein", "Artery", "Capillary", "Nerve"], answer: "Artery" },
    { question: "What is the largest artery in the human body?", options: ["Aorta", "Coronary", "Carotid", "Jugular"], answer: "Aorta" },
    { question: "What is a common sign of a heart attack?", options: ["Headache", "Chest pain", "Arm numbness", "All of the above"], answer: "All of the above" },
    { question: "Which condition is characterized by narrowed or blocked blood vessels?", options: ["Hypertension", "Cardiac arrest", "Atherosclerosis", "Arrhythmia"], answer: "Atherosclerosis" },
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
      <h1>Cardiovascular System Quiz</h1>
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

export default QuizCardiovascular;