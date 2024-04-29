import React from "react";
import { Card } from "react-bootstrap";
import "./forumMessage.css";

function ForumMessage(props) {
  return (
    <Card className="card-forum">
      <Card.Body className="card-body-forum">
        <Card.Title className="card-title">{props.title}</Card.Title>
        <Card.Text className="card-text">{props.content}</Card.Text>
      </Card.Body>
    </Card>
  );
}

export default ForumMessage;
