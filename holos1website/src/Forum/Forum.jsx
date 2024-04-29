import React, { useEffect, useState, useRef, useCallback } from "react";
import { Container, Row, Col, Pagination, Form, Button, Card, Spinner } from "react-bootstrap";
import NavBar from "../NavBar/NavBar";
import "./Forum.css";

export default function Forum() {
    const [posts, setPosts] = useState([]);
    const [currentPage, setCurrentPage] = useState(1);
    const [isLoading, setIsLoading] = useState(false);
    const titleRef = useRef();
    const contentRef = useRef();

    const loadPosts = useCallback(() => {
        setIsLoading(true);
        for (let i = 1; i <= 4; i++) {
            fetch(`https://wm0sx9jx6f.execute-api.us-east-1.amazonaws.com/dep/forum?page=${i}`)
                .then(res => res.json())
                .then(data => {
                    if (i === currentPage) {
                        const postsWithReply = data.posts.map(post => ({
                            ...post,
                            isReplying: false,
                            replyContent: "",
                            showReplies: false,
                            replies: post.replies || [],
                            replyCount: post.replies ? post.replies.length : 0
                        }));
                        setPosts(postsWithReply);
                    }
                })
                .catch(error => console.error("Error loading posts:", error))
                .finally(() => {
                    if (i === 4) setIsLoading(false);
                });
        }
    }, [currentPage]);

    const handlePost = () => {
        const title = titleRef.current.value;
        const content = contentRef.current.value;
        if (!title || !content) {
            alert("Title and content are required.");
            return;
        }

        fetch(`https://wm0sx9jx6f.execute-api.us-east-1.amazonaws.com/dep/forum`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ title, content }),
        })
            .then(res => res.json())
            .then(data => {
                if (data.Operation === "SAVE" && data.Message === "Success") {
                    alert("Post created successfully!");
                    loadPosts();
                } else {
                    alert("Failed to create post.");
                }
            })
            .catch(error => {
                console.error("Error creating post:", error);
                alert("Failed to create post.");
            });
    };

    const toggleReply = (post) => {
        const newPosts = posts.map(p =>
            p.messageid === post.messageid ? {...p, isReplying: !p.isReplying} : p
        );
        setPosts(newPosts);
    };

    const toggleReplies = (post) => {
        const newPosts = posts.map(p =>
            p.messageid === post.messageid ? {...p, showReplies: !p.showReplies} : p
        );
        setPosts(newPosts);
    };

    const handleReply = (post) => {
        fetch(`https://wm0sx9jx6f.execute-api.us-east-1.amazonaws.com/dep/forum`, {
            method: "PATCH",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({
                messageid: post.messageid,
                replyContent: post.replyContent
            }),
        })
        .then(res => res.json())
        .then(data => {
            if (data.Operation === "UPDATE" && data.Message === "Success") {
                alert("Reply posted successfully!");
                toggleReply(post);  // 关闭回复框
                const updatedPosts = posts.map(p => {
                    if (p.messageid === post.messageid) {
                        return {
                            ...p,
                            replyCount: p.replyCount + 1,  // 更新回复数量
                            replies: [...p.replies, { content: post.replyContent }]  // 假设你可以这样更新回复列表
                        };
                    }
                    return p;
                });
                setPosts(updatedPosts);  // 更新帖子列表
            } else {
                alert("Failed to post reply.");
            }
        })
        .catch(error => {
            console.error("Error posting reply:", error);
            alert("Failed to post reply.");
        });
    };

    useEffect(() => {
        loadPosts();
    }, [loadPosts]);

    return (
        <>
            <NavBar />
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
                {isLoading ? (
                    <div className="text-center">
                        <Spinner animation="border" role="status">
                            <span className="sr-only">Loading...</span>
                        </Spinner>
                    </div>
                ) : (
                    <Row className="justify-content-center">
                        {posts.map((post, i) => (
                            <Col key={i} md={6} className="mb-4">
                                <Card className="post-card">
                                    <Card.Body>
                                        <Card.Title>{post.title}</Card.Title>
                                        <Card.Text>{post.content}</Card.Text>
                                        <Button variant="link" onClick={() => toggleReplies(post)}>
                                            Replies ({post.replyCount || 0})
                                        </Button>
                                        {post.showReplies && post.replies.map((reply, index) => (
                                            <Card key={index} className="mt-2">
                                                <Card.Body>
                                                    <Card.Text>{reply.content}</Card.Text>
                                                </Card.Body>
                                            </Card>
                                        ))}
                                        {post.isReplying && (
                                            <Form.Group>
                                                <Form.Control
                                                    as="textarea"
                                                    value={post.replyContent}
                                                    onChange={(e) => {
                                                        const newPosts = posts.map(p =>
                                                            p.messageid === post.messageid ? {...p, replyContent: e.target.value} : p
                                                        );
                                                        setPosts(newPosts);
                                                    }}
                                                />
                                                <Button onClick={() => handleReply(post)}>Submit Reply</Button>
                                            </Form.Group>
                                        )}
                                        <Button onClick={() => toggleReply(post)}>Reply</Button>
                                    </Card.Body>
                                </Card>
                            </Col>
                        ))}
                    </Row>
                )}
                <Pagination className="justify-content-center mt-5">
                    {[1, 2, 3, 4].map((page) => (
                        <Pagination.Item
                            key={page}
                            className="pagination-item"
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