import "./landingPage.css";
import React from "react";
import NavBar from "../NavBar/NavBar";

function LandingPage() {
  return (
    <>
      <NavBar />
      <header className="landing-header">
        Welcome to the Website for Holos Team 1
      </header>
      <div className="landing-body">
        We're developing a Unity demo for the company
        <a href="https://holos.io/" className="link">
          {" "}
          Holos
        </a>
        . We're using this website to document our progress and share our demo.
        We will be building a Virtual Reality Classroom for medical students to
        help them learn.
      </div>
    </>
  );
}

export default LandingPage;
