import React from "react";
import { Card } from "react-bootstrap";

function ForumMessage(props) {


    return (
        <Card style={{ margin: "0.5rem", padding: "0.5rem" }}>
            <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Text>{props.content}</Card.Text>
            </Card.Body>
        </Card>
    );
}

export default ForumMessage;