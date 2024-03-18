import "./landingPage.css";
import React from "react";
import NavBar from "../NavBar/NavBar";

function LandingPage() {
  return (
    <>
      <NavBar />
      <header className="landing-header">
  Welcome to the Website for <span className="glow">Holos</span> Team 1
</header>
      <div className="landing-body">
        We're developing a Unity demo for the company
        <a href="https://holos.io/" className="link">
           Holos
        </a>. We're using this website to document our progress and share our demo.
        We will be building a Virtual Reality Classroom for medical students to
        help them learn.
        <p>
          This project is a VR program integrated with GPT AI technology, aimed at medical students. Our goal is to enable medical students to learn medical knowledge more attentively and conveniently, such as having a clearer view of the skeletal system. Through this program, medical students can experience and learn in a virtual reality environment, gaining a more intuitive and profound learning experience.
        </p>
        <p>
          Our main scenarios include interactive anatomy lessons where students can explore human anatomy in 3D, virtual surgery simulations to practice procedures, and immersive case studies for diagnosing and treating virtual patients. These scenarios are designed to enhance the learning experience by providing practical, hands-on experience in a safe, controlled environment.
        </p>
      </div>
    </>
  );
}

export default LandingPage;