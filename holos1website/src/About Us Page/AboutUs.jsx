import NavBar from "../NavBar/NavBar";
import "./aboutUs.css";
import Card from "../Card/Card";
import NishanthImage from "../assets/Nishanth.jpg";
import SakshamImage from "../assets/Saksham.jpg";
import WeibingImage from "../assets/Weibing.jpg";
import YuyangImage from "../assets/Yuyang.JPG";
import JordanImage from "../assets/Jordan.jpg";

function AboutUs() {
  return (
    <>
      <NavBar />
      <header className="about-header">We're more than just a student team. We've got goals, hopes, and dreams, just like you...</header>
      <div className="team-container">
        <Card name="Nishanth Naik" description="Hi! My name is Nishanth Naik and I am a senior at UW-Madison. I enjoy gaming and I find VR games quite interesting so a VR project was very cool to work with as it was my first time using the Unity software." image={NishanthImage} />
        <Card name="Saksham Jain" description="Hello! I'm Saksham Jain, also a senior at UW-Madison. My passion for software development has led me to exciting VR projects where I can apply my coding skills in innovative ways." image={SakshamImage} />
        <Card name="Weibing Wang" description="Greetings! I am Weibing Wang, a senior at UW-Madison. My academic focus is on computer science, and working on VR projects has opened a new dimension of interactive technology for me." image={WeibingImage} />
        <Card name="Yuyang Liu" description="I'm a senior at the University of Wisconsin-Madison, and through VR, I explore new possibilities, and it's a good idea to make learning and entertainment come alive!" image={YuyangImage} />
        <Card name="Jordan Alper" description="Hi! My name’s Jordan Alper and I’m a senior at UW-Madison! I enjoy developing video games, so a VR project was a really cool experience for me, as it uses Unity as well!" image={JordanImage} />
      </div>
    </>
  );
}

export default AboutUs;
