import React from "react";
import "./forum.css";
import NavBar from "../NavBar/NavBar";

function Forum() {
  return (
    <>
      <NavBar />
      <header className="landing-header">
        Welcome to the <span className="glow">Holos Forum</span>
      </header>
      <div className="landing-body">
        <p>Welcome to the Holos Forum! This is a space for our community to discuss, share, and learn together. Whether you're here to ask questions, share your projects, or simply explore, we're glad to have you with us.</p>
        
        <p>Feel free to start a new discussion thread or contribute to an existing one. Our forum is a place for respectful and constructive conversations aimed at pushing the boundaries of what we can achieve with technology.</p>
        
        <p>Remember to follow our community guidelines to ensure a positive experience for all members. Let's create a vibrant and supportive environment for everyone interested in Holos and its applications.</p>
      </div>
    </>
  );
}

export default Forum;