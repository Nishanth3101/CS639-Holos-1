import React, { useEffect, useState, useRef } from "react";
import { Container, Row, Col, Pagination, Form, Button } from "react-bootstrap";
import ForumMessage from "./ForumMessage";
import NavBar from "../NavBar/NavBar"; // 引入NavBar组件
import "./Forum.css";

export default function Forum() {
    const [posts, setPosts] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const titleRef = useRef();
    const contentRef = useRef();

    const loadPosts = () => {
        const handleData = (data, index) => {
            if (index + 1 === currentPage) {
                setPosts(data.posts);
            }
        };

        for (let i = 1; i <= 4; i++) {
            fetch(`https://wm0sx9jx6f.execute-api.us-east-1.amazonaws.com/dep/forum?page=${i}`)
                .then((res) => res.json())
                .then((data) => handleData(data, i - 1))
                .catch((error) => {
                    console.error("Error loading posts:", error);
                });
        }
    };

    const handlePost = () => {
        const title = titleRef.current.value;
        const content = contentRef.current.value;

        if (!title || !content) {
            alert("Title and content are required.");
            return;
        }

        fetch(`https://wm0sx9jx6f.execute-api.us-east-1.amazonaws.com/dep/forum`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({ title, content }),
        })
            .then((res) => res.json())
            .then((data) => {
                if (data.success) {
                    alert("Post created successfully!");
                    loadPosts();
                } else {
                    alert("Failed to create post.");
                }
            })
            .catch((error) => {
                console.error("Error creating post:", error);
                alert("Failed to create post.");
            });
    };

    useEffect(() => {
        loadPosts();
    }, [currentPage]);

    return (
        <>
            <NavBar /> {/* 使用NavBar组件 */}
            <Container>
                <h1 className="forum-header">Forum</h1>
                <Form className="forum-content">
                    <Form.Group>
                        <Form.Label htmlFor="title">Title</Form.Label>
                        <Form.Control id="title" ref={titleRef} />
                    </Form.Group>
                    <Form.Group>
                        <Form.Label htmlFor="content">Content</Form.Label>
                        <Form.Control as="textarea" id="content" ref={contentRef} />
                    </Form.Group>
                    <Button onClick={handlePost} className="submit-post-button">Post</Button>
                </Form>
                <hr />
                {posts.length > 0 ? (
                    <Row>
                        {posts.map((post, i) => (
                            <Col key={i} xs={6} md={4} lg={3} xl={2.4}> {/* 更新以支持5列布局 */}
                                <ForumMessage
                                    id={post.id}
                                    title={post.title}
                                    content={post.content}
                                />
                            </Col>
                        ))}
                    </Row>
                ) : (
                    <p>There are no posts on this page yet!</p>
                )}
             <Pagination className="justify-content-center mt-5">
    {[1, 2, 3, 4].map((page) => (
        <Pagination.Item
            key={page}
            className="pagination-item"  // 确保这里有正确的类名
            active={page === currentPage}
            onClick={() => setCurrentPage(page)}
        >
            {page}
        </Pagination.Item>
    ))}
</Pagination>
            </Container>
        </>
    );
}