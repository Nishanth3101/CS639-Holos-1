import React, { useEffect } from "react";
import "./landingPage.css";
import NavBar from "../NavBar/NavBar";
import AOS from 'aos';
import 'aos/dist/aos.css';

function LandingPage() {
  useEffect(() => {
    AOS.init({
      duration: 1200,
    });
  }, []);

  const handleImageLoaded = (e) => {
    e.target.classList.add('loaded');
  };

  return (
    <>
      <NavBar />
      <header className="landing-header">
        Welcome to the Website for <span className="glow">Holos</span> Team 1
      </header>
      <div className="landing-body">
        <p1>We're excited to share our journey in developing a Unity demo for <a href="https://holos.io/" className="link">Holos</a>. This website serves as a platform to document our progress and showcase our demo.</p1>
        
        <p>Our project is focused on creating a Virtual Reality Classroom for medical students, aiming to revolutionize the way medical education is delivered. By integrating GPT AI technology, we're making learning more interactive and accessible.</p>
        
        <p data-aos="fade-right">
          <img src="/demo1.jpeg" alt="Demo 1" className="landing-image right" onLoad={handleImageLoaded}/>
          The VR program is designed to provide medical students with a clearer understanding of complex medical concepts, such as the skeletal system, through an immersive virtual reality environment.
        </p>
        
        <p data-aos="fade-left">
          <img src="/demo2.png" alt="Demo 2" className="landing-image left" onLoad={handleImageLoaded}/>
          We're developing interactive anatomy lessons, virtual surgery simulations, and immersive case studies for diagnosing and treating virtual patients. These scenarios aim to enhance learning by offering practical experience in a safe, controlled setting.
        </p>
        
        <p>Looking ahead, we plan to expand our VR curriculum to cover more medical fields and incorporate advanced AI features to simulate real-life patient interactions. Stay tuned for more updates as we continue to push the boundaries of medical education.</p>
      </div>
    </>
  );
}

export default LandingPage;