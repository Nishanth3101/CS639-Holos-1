import React, { useState, useEffect, useCallback } from 'react';
import "./QuizPage.css";
import Navbar from "../NavBar/NavBar";

function QuizPage() {
    
  const [score, setScore] = useState(0);
  const [questions, setQuestions] = useState([]);
  const [selectedAnswers, setSelectedAnswers] = useState({});
  const [showScore, setShowScore] = useState(false);

  const fetchQuestions = useCallback((retryCount = 0) => {
    fetch('https://96g4r1viz7.execute-api.us-east-1.amazonaws.com/hh/questions', {
      method: 'GET',
      headers: {
        'x-api-key': '1JYUsgVf0s1vmCEiITxKK9tmsO1yIEQTGKiWdEof'
      },
    })
    .then(response => response.json())
    .then(data => {
      setQuestions(data.questions);
    })
    .catch(error => {
      console.error('Error fetching questions:', error);
      if (retryCount < 5) {
        setTimeout(() => {
          fetchQuestions(retryCount + 1);
        }, Math.pow(2, retryCount) * 1000);
      }
    });
  }, []);

  useEffect(() => {
    fetchQuestions();
  }, [fetchQuestions]);

  const handleAnswer = (questionIndex, answerIndex, isCorrect) => {
    setSelectedAnswers(prev => ({...prev, [questionIndex]: {answerIndex, isCorrect}}));
  };

  const handleSubmit = () => {
    const newScore = Object.values(selectedAnswers).reduce((acc, curr) => acc + (curr.isCorrect ? 1 : 0), 0);
    setScore(newScore);
    setShowScore(true);

    // 设置5秒后自动隐藏分数弹窗
    setTimeout(() => {
      setShowScore(false);
    }, 5000);
  };

  return (
    <>
      <Navbar />
      <header className="quiz-header">Tasks</header>
      <div className="quiz-content">
        {questions.length > 0 ? (
          questions.map((question, index) => (
            <div key={index} className="question-section">
              <h2>{question.question}</h2>
              {question.answers.map((answer, answerIndex) => (
                <button
                  key={answerIndex}
                  onClick={() => handleAnswer(index, answerIndex, answer.correct)}
                  className={`answer-button ${selectedAnswers[index]?.answerIndex === answerIndex ? 'selected' : ''}`}
                >
                  {answer.text}
                </button>
              ))}
            </div>
          ))
        ) : (
          <p>Loading questions...</p>
        )}
        <button onClick={handleSubmit} className="submit-button">Submit</button>
        {showScore && (
          <div className="score-modal">
            Your score: {score}
            <button onClick={() => setShowScore(false)} className="close-button">Close</button>
          </div>
        )}
      </div>
    </>
  );
}

export default QuizPage;