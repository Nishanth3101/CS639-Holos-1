import React from "react";
import "./card.css";

function Card({ image, name, description }) {
  return (
    <div className="card">
      <img src={image} alt={name} />
      <h2 className="card-title">{name}</h2>
      <p className="card-description">{description}</p>
    </div>
  );
}

export default Card;
