import React, { useContext } from "react";
import { Card, Button } from "react-bootstrap";

function ForumMessage(props) {
    const loggedInUser = sessionStorage.getItem("loggedInUser"); // 假设用户登录状态存储在sessionStorage中
    const dt = new Date(props.created); // 假设props.created是帖子创建的时间戳

    return (
        <Card style={{ margin: "0.5rem", padding: "0.5rem" }}>
            <Card.Body>
                <Card.Title>{props.title}</Card.Title>
                <Card.Subtitle className="mb-2 text-muted">
                    Posted by <i>{props.poster}</i> on {dt.toLocaleDateString()} at {dt.toLocaleTimeString()}
                </Card.Subtitle>
                <Card.Text>{props.content}</Card.Text>
                {loggedInUser === props.poster && (
                    <Button variant="danger" onClick={() => props.toDelete(props.id)}>
                        Delete
                    </Button>
                )}
            </Card.Body>
        </Card>
    );
}

export default ForumMessage;